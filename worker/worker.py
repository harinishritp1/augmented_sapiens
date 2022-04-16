import configparser
import json
import os
import platform
import sys
import time

import pika
from sqlalchemy import create_engine, update
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import scoped_session, sessionmaker
from flask import Flask
import threading
from worker_function import analyze_priority

app = Flask(__name__)
app.config.from_pyfile('../config/app.conf')

hostname = platform.node()

db_config = {}

parser = configparser.ConfigParser()
parser.read('worker/config.ini')
for sect in parser.sections():
    if sect == "Database":
        for k, v in parser.items(sect):
            db_config[k] = v
print(db_config)
# url = 'postgresql://' + db_config['user'] + ':' + db_config['password'] + '@' + db_config['host'] + ":" + db_config['port'] + '/' + db_config['db_name']
url = 'postgresql://nqhhsndosqitvj:4f39a3506fdfb516f035fcd5bb21d77fdeca238b853abad2011999fc6b328fb5@ec2-34-194-73-236.compute-1.amazonaws.com:5432/d90r6plpb25oio'
# url = db_config['database_url']

engine = create_engine(url, convert_unicode=True, echo=False)
Base = declarative_base()
Base.metadata.reflect(engine)


class Ticket(Base):
    __table__ = Base.metadata.tables['tickets']


def getMQ():
    # Access the CLODUAMQP_URL environment variable and parse it (fallback to localhost)
    # url = db_config['cloudamqp_url']
    url = 'amqps://kjnpzwnf:qgkK0d67EqbfhmHc8ujyX2sOSn_T2Q3t@woodpecker.rmq.cloudamqp.com/kjnpzwnf'
    print("Connecting to cloudAMQP({})".format(url))
    params = pika.URLParameters(url)
    connection = pika.BlockingConnection(params)
    channel = connection.channel()  # start a channel
    channel.queue_declare(queue='toWorker')  # Declare a queue
    channel.exchange_declare(exchange='logs', exchange_type='topic')
    # channel.connection.process_data_events(time_limit=1)
    return channel


#
def callback(ch, method, properties, body):
    db_session = scoped_session(sessionmaker(bind=engine))
    body = json.loads((body.decode("utf-8")))

    description = body["description"]
    color = body['color']
    ticket_id = body['ticket_id']
    priority = analyze_priority(color, description)
    conn = engine.connect()
    query = update(Ticket).where(Ticket.ticket_id == ticket_id).values(priority=priority)
    conn.execute(query)
    db_session.commit()

    ch.basic_ack(delivery_tag=method.delivery_tag)


# def analyze_priority(color, description):
#     colorMarker = {"red": 1, "yellow": 2, "blue": 3, "green": 4}
#     keywords = ["urgent", "important", "priority", "critical"]
# 
#     priority = 0
# 
#     for word in keywords:
#         if word in description.lower():
#             priority = 1
#     # color = color.split()
#     priority += colorMarker.get(color.lower())
#     return priority


with getMQ() as mq:
    mq.basic_consume(queue='toWorker', on_message_callback=callback, auto_ack=False)
    mq.start_consuming()