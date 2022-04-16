import json
import os
import platform
import sys

import pika
from flask import Flask, jsonify, make_response, render_template, request
from sqlalchemy import create_engine, func, or_, update
from sqlalchemy.ext.declarative import DeclarativeMeta, declarative_base
from sqlalchemy.orm import scoped_session, sessionmaker
from sqlalchemy.sql.functions import coalesce

app = Flask(__name__)
app.config.from_pyfile('../config/app.conf')

url = app.config.get("DATABASE_URL")
engine = create_engine(url, convert_unicode=True, echo=False)
Base = declarative_base()
Base.metadata.reflect(engine)


def getMQ():
    # Access the CLODUAMQP_URL environment variable and parse it (fallback to localhost)
    url = app.config.get("CLOUDAMQP_URL")
    print("Connecting to cloudAMQP({})".format(url))
    params = pika.URLParameters(url)
    connection = pika.BlockingConnection(params)
    channel = connection.channel()  # start a channel
    channel.queue_declare(queue='toWorker')  # Declare a queue
    channel.exchange_declare(exchange='logs', exchange_type='topic')
    return channel


class Ticket(Base):
    __table__ = Base.metadata.tables['tickets']


class AlchemyEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj.__class__, DeclarativeMeta):
            # an SQLAlchemy class
            fields = {}
            for field in [x for x in dir(obj) if not x.startswith('_') and x != 'metadata']:
                data = obj.__getattribute__(field)
                try:
                    json.dumps(data)  # this will fail on non-encodable values, like other classes
                    fields[field] = data
                except TypeError:
                    fields[field] = None
            # a json-encodable dict
            return fields
        return json.JSONEncoder.default(self, obj)


@app.route('/')
def home():
    return render_template('homepage.html')


@app.route('/createticket', methods=['POST', 'GET'])
def createticket(data=None):
    db_session = scoped_session(sessionmaker(bind=engine))  # starts new session
    
    if request.method == 'POST':

        ticket_id = coalesce(db_session.query(func.max(Ticket.ticket_id))[0][0], 0) + 1
        image = request.form.get("image")
        latitude = request.form.get("latitude")
        longitude = request.form.get("longitude")
        color = request.form.get("color")
        priority = None
        description = request.form.get("description")
        status = "Open"

        response = {}

        if not image:
            response["ERROR"] = "Please enter an image"
        elif type(image) != str:
            response["ERROR"] = "Please enter an image string"
        elif not latitude:
            response["ERROR"] = "Please enter latitude"
        elif not float(latitude):
            response["ERROR"] = "Please enter a float value"
        elif not longitude:
            response["ERROR"] = "Please enter longitude"
        elif not float(longitude):
            response["ERROR"] = "Please enter a float value"
        elif not color:
            response["ERROR"] = "Please enter color marker"
        elif type(color) != str:
            response["ERROR"] = "Please enter an color string"
        elif not description:
            response["ERROR"] = "Please enter description"
        elif type(description) != str:
            response["ERROR"] = "Please enter a string value"
        else:

            new_ticket = Ticket(
                ticket_id=ticket_id,
                image=image,
                latitude=latitude,
                longitude=longitude,
                color=color,
                description=description,
                priority=priority,
                status=status
            )

            db_session.add(new_ticket)
            db_session.commit()

            formattedJson = json.dumps(new_ticket, cls=AlchemyEncoder)

            with getMQ() as mq:
                mq.basic_publish(exchange='', routing_key='toWorker', body=formattedJson)

            response["MESSAGE"] = "Ticket created successfully!"

        return jsonify(response)

    else:
        response = make_response(render_template('createticket.html'), 200)
        return response

    # json_data = request.get_json()
    # db_session = scoped_session(sessionmaker(bind=engine))

    # ticket_id = coalesce(db_session.query(func.max(Ticket.ticket_id))[0][0], 0) + 1
    # new_ticket = Ticket(
    #     ticket_id=ticket_id,
    #     image=json_data['image'],
    #     latitude=json_data['latitude'],
    #     longitude=json_data['longitude'],
    #     color=json_data['color'],
    #     description=json_data['description'],
    #     priority = None,
    #     status=json_data['status']
    # )
    # db_session.add(new_ticket)
    # db_session.commit()

    # response = {'Action': 'Ticket created'}

    # formattedJson = json.dumps(json_data)

    # with getMQ() as mq:
    #     mq.basic_publish(exchange='', routing_key='toWorker', body=formattedJson)

    # return Response(json.dumps(response), status=200, mimetype="application/json")


@app.route('/updateticket', methods=['POST', 'GET'])
def updateticket():
    db_session = scoped_session(sessionmaker(bind=engine))

    if request.method == 'POST':

        ticket_id = int(request.form.get("ticket_id"))
        status = request.form.get("status")

        response = {}

        if not ticket_id:
            response["ERROR"] = "Please enter a ticket id"
        elif type(ticket_id) == str:
            response["ERROR"] = "Please enter a numeric value"
        elif status == "null":
            response["ERROR"] = "Please select a valid status"
        else:
            conn = engine.connect()
            query = update(Ticket).where(Ticket.ticket_id == ticket_id).values(status=status)
            conn.execute(query)
            db_session.commit()

            response["MESSAGE"] = "Ticket updated successfully!"

        return jsonify(response)

    else:
        response = make_response(render_template('updateticket.html'), 200)
        return response

    # json_data = request.get_json()
    # db_session = scoped_session(sessionmaker(bind=engine))
    # ticket_id = json_data['ticket_id']
    # status = json_data['status']

    # conn = engine.connect()
    # query = update(Ticket).where(Ticket.ticket_id == ticket_id).values(status=status)
    # conn.execute(query)
    # db_session.commit()

    # response = {'Action': 'Ticket updated'}

    # #return Response(json.dumps(response), status=200, mimetype="application/json")
    # response = make_response(render_template('updateticket.html'), 200)
    # return response


@app.route('/getactiveticket')
def getactiveticket():
    db_session = scoped_session(sessionmaker(bind=engine))
    query = db_session.query(Ticket).filter(
        or_(func.lower(Ticket.status) == "open", func.lower(Ticket.status) == "in progress")).order_by(Ticket.priority,
                                                                                                       Ticket.status)
    results = query.all()
    tickets = []
    response = {}
    if results is not None:
        count = 0
        for result in results:
            ticket = {"Ticket Id": result.ticket_id, "Image": result.image, "Latitude": result.latitude,
                      "Longitude": result.longitude,
                      "Color": result.color, "Description": result.description, "Priority": result.priority,
                      "Status": result.status}
            tickets.append(ticket)
            count += 1
        response = {"count" : count, "data": tickets}
    else:
        response = {"count" : 0, "data" : None}

    return jsonify(response)
    #return Response(json.dumps(response_list), status=200, mimetype="application/json")
