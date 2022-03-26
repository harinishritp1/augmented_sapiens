import json
import os
import platform
import sys

import pika
from flask import Flask, Response, request
from sqlalchemy import create_engine, func, or_, update
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import scoped_session, sessionmaker
from sqlalchemy.sql.functions import coalesce

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

## Configure test vs. production
##
rabbitMQHost = os.getenv("RABBITMQ_HOST") or "localhost"
print("Connecting to rabbitmq({})".format(rabbitMQHost))

def getMQ():
    parameters = (
    pika.ConnectionParameters(host=rabbitMQHost, port=5672)
    )
    rabbitMQ = pika.BlockingConnection(parameters)
    rabbitMQChannel = rabbitMQ.channel()
    rabbitMQChannel.exchange_declare(exchange='logs', exchange_type='topic')
    rabbitMQChannel.queue_declare(queue='toWorker')
    return rabbitMQChannel

infoKey = f"{platform.node()}.worker.info"
debugKey = f"{platform.node()}.worker.debug"

def log_debug(message, key=debugKey):
    print("DEBUG:", message, file=sys.stdout)
    with getMQ() as mq:
        mq.basic_publish(
        exchange='logs', routing_key=key, body=message)

def log_info(message, key=infoKey):
    print("INFO:", message, file=sys.stdout)
    with getMQ() as mq:
        mq.basic_publish(
        exchange='logs', routing_key=key, body=message)


class Ticket(Base):
    __table__ = Base.metadata.tables['tickets']


@app.route('/createticket', methods=['POST'])
def create_ticket():
    json_data = request.get_json()
    db_session = scoped_session(sessionmaker(bind=engine))
    ticket_id = coalesce(db_session.query(func.max(Ticket.ticket_id))[0][0], 0) + 1

    new_ticket = Ticket(
        ticket_id=ticket_id,
        image=json_data['image'],
        latitude=json_data['latitude'],
        longitude=json_data['longitude'],
        color=json_data['color'],
        priority=json_data['priority'],
        description=json_data['description'],
        status=json_data['status']
    )
    db_session.add(new_ticket)

    db_session.commit()
    response = {'Action': 'Ticket created'}

    return Response(json.dumps(response), status=200, mimetype="application/json")


@app.route('/updateticket', methods=['POST'])
def update_ticket():
    json_data = request.get_json()
    db_session = scoped_session(sessionmaker(bind=engine))
    ticket_id = json_data['ticket_id']
    status = json_data['status']

    conn = engine.connect()
    query = update(Ticket).where(Ticket.ticket_id == ticket_id).values(status=status)
    conn.execute(query)
    db_session.commit()
    response = {'Action': 'Ticket updated'}

    return Response(json.dumps(response), status=200, mimetype="application/json")


@app.route('/getactiveticket', methods=['GET'])
def get_active_ticket():
    db_session = scoped_session(sessionmaker(bind=engine))
    query = db_session.query(Ticket).filter(or_(Ticket.status == "Open", Ticket.status == "In progress"))
    response_list = {}

    results = query.all()
    for result in results:
        dictionary = {"Image": result.image, "Latitude": result.latitude, "Longitude": result.longitude,
                      "Priority": result.priority, "Description": result.description, "Status": result.status}
        response_list[result.ticket_id] = dictionary

    return Response(json.dumps(response_list), status=200, mimetype="application/json")


app.run(host="0.0.0.0", port=5001, debug=True)
