import json
import os
import platform
import sys

import pika
from flask import Flask, jsonify, make_response, render_template, request
from sqlalchemy import create_engine, desc, func, or_, update
from sqlalchemy.ext.declarative import DeclarativeMeta, declarative_base
from sqlalchemy.orm import scoped_session, sessionmaker
from sqlalchemy.sql.functions import coalesce


class rest:

    app = Flask(__name__)
    app.config.from_pyfile('../config/app.conf')
    user = app.config.get("USER")
    password = app.config.get("PASSWORD")
    host = app.config.get("HOST")
    port = app.config.get("PORT")
    db_name = app.config.get("DB_NAME")

    url = 'postgresql://' + user + ':' + password + '@' + host + ":" + port + '/' + db_name
    engine = create_engine(url, convert_unicode=True, echo=False)
    Base = declarative_base()
    Base.metadata.reflect(engine)

    rabbitMQHost = os.getenv("RABBITMQ_HOST") or "localhost"
    print("Connecting to rabbitmq({})".format(rabbitMQHost))

    infoKey = f"{platform.node()}.worker.info"
    debugKey = f"{platform.node()}.worker.debug"

    #def __init__(self):

        # self.app = Flask(__name__)
        # self.app.config.from_pyfile('../config/app.conf')
        # self.user = self.app.config.get("USER")
        # self.password = self.app.config.get("PASSWORD")
        # self.host = self.app.config.get("HOST")
        # self.port = self.app.config.get("PORT")
        # self.db_name = self.app.config.get("DB_NAME")
        #
        # self.url = 'postgresql://' + self.user + ':' + self.password + '@' + self.host + ":" + self.port + '/' + self.db_name
        # self.engine = create_engine(self.url, convert_unicode=True, echo=False)

    ## Configure test vs. production

    def getMQ(self):
        parameters = (
            pika.ConnectionParameters(host= rest.rabbitMQHost, port=5672)
        )
        rabbitMQ = pika.BlockingConnection(parameters)
        rabbitMQChannel = rabbitMQ.channel()
        rabbitMQChannel.exchange_declare(exchange='logs', exchange_type='topic')
        rabbitMQChannel.queue_declare(queue='toWorker')
        return rabbitMQChannel



    def log_debug(self, message, key=debugKey):
        print("DEBUG:", message, file=sys.stdout)
        with self.getMQ() as mq:
            mq.basic_publish(
                exchange='logs', routing_key=key, body=message)


    def log_info(self, message, key=infoKey):
        print("INFO:", message, file=sys.stdout)
        with self.getMQ() as mq:
            mq.basic_publish(
                exchange='logs', routing_key=key, body=message)

    def get_engine(self):
        return self.engine

    class Ticket(Base):
        __tablename__ = "tickets"
        def __init__(self,Base):
            self.__table__ = Base.metadata.tables['tickets']

    class AlchemyEncoder(json.JSONEncoder):
        def default(self, obj):
            if isinstance(obj.__class__, DeclarativeMeta):
                # an SQLAlchemy class
                fields = {}
                for field in [x for x in dir(obj) if not x.startswith('_') and x != 'metadata']:
                    data = obj.__getattribute__(field)
                    try:
                        json.dumps(data) # this will fail on non-encodable values, like other classes
                        fields[field] = data
                    except TypeError:
                        fields[field] = None
                # a json-encodable dict
                return fields
            return json.JSONEncoder.default(self, obj)

    @app.route('/')
    def home(self):
        return render_template('homepage.html')

    @app.route('/createticket', methods=['POST','GET'])
    def createticket(self):

        db_session = scoped_session(sessionmaker(bind=rest.engine))

        if request.method == 'POST':

            ticket_id = coalesce(db_session.query(func.max(rest.Ticket.ticket_id))[0][0], 0) + 1
            image = request.form.get("image")
            latitude = request.form.get("latitude")
            longitude = request.form.get("longitude")
            color = request.form.get("color")
            priority = None
            description = request.form.get("description")
            status = "Open"

            response={}

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

                new_ticket = rest.Ticket(
                ticket_id=ticket_id,
                image=image,
                latitude=latitude,
                longitude=longitude,
                color=color,
                priority=priority,
                description=description,
                status=status
                )

                db_session.add(new_ticket)
                db_session.commit()


                formattedJson = json.dumps(new_ticket, cls= rest.AlchemyEncoder)

                with rest.getMQ() as mq:
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
        #     priority = None,
        #     description=json_data['description'],
        #     status=json_data['status']
        # )
        # db_session.add(new_ticket)
        # db_session.commit()

        # response = {'Action': 'Ticket created'}

        # formattedJson = json.dumps(json_data)

        # with getMQ() as mq:
        #     mq.basic_publish(exchange='', routing_key='toWorker', body=formattedJson)

        # return Response(json.dumps(response), status=200, mimetype="application/json")

    @app.route('/updateticket', methods=['POST','GET'])
    def updateticket(self):

        db_session = scoped_session(sessionmaker(bind=engine))

        if request.method == 'POST':

            ticket_id = int(request.form.get("ticket_id"))
            status = request.form.get("status")

            response={}

            if not ticket_id:
                response["ERROR"] = "Please enter a ticket id"
            elif type(ticket_id) == str:
                response["ERROR"] = "Please enter a numeric value"
            elif status == "null":
                response["ERROR"] = "Please select a valid status"
            else:
                conn = rest.engine.connect()
                query = update(rest.Ticket).where(rest.Ticket.ticket_id == ticket_id).values(status=status)
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
    def getactiveticket(self):
        db_session = scoped_session(sessionmaker(bind=rest.engine))
        query = db_session.query(rest.Ticket).filter(or_(func.lower(rest.Ticket.status) == "open", func.lower(rest.Ticket.status) == "in progress")).order_by(desc(rest.Ticket.ticket_id))
        results = query.all()
        tickets = []
        response = {}
        if results is not None:
            id = 1
            for result in results:
                ticket = {"Id": id, "Priority": result.priority, "Image": result.image, "Latitude": result.latitude, "Longitude": result.longitude,
                            "Color": result. color, "Description": result.description, "Status": result.status}
                tickets.append(ticket)
                id += 1
            response = make_response(render_template('getactiveticket.html', tickets = tickets), 200)
        else:
            response["MESSAGE"] = "No active ticket found!"

        return response
        #return Response(json.dumps(response_list), status=200, mimetype="application/json")


if __name__ == "__main__":
    rest()