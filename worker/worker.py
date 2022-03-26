#
# Worker server
#
import configparser
import hashlib
import io
import json
import math
import os
import pickle
import platform
import sys

import pika
import requests
import sqlalchemy
from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import Query, scoped_session, sessionmaker

hostname = platform.node()

##
## Configure test vs. production
##
rabbitMQHost = os.getenv("RABBITMQ_HOST") or "localhost"

print(f"Connecting to rabbitmq({rabbitMQHost})")

db_config = {}

parser = configparser.ConfigParser()
parser.read('config.ini')
for sect in parser.sections():
    if sect == "Database":
        for k, v in parser.items(sect):
            db_config[k] = v

url = 'postgresql://' + db_config['user'] + ':' + db_config['password'] + '@' + db_config['host'] + ":" + db_config[
    'port'] + '/' + db_config['db_name']

engine = create_engine(url, convert_unicode=True, echo=False)
Base = declarative_base()
Base.metadata.reflect(engine)


class Ticket(Base):
    __table__ = Base.metadata.tables['tickets']


##
## Set up rabbitmq connection
##
def getMQ():
    parameters = (
        pika.ConnectionParameters(host=rabbitMQHost)
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


##
## Your code goes here...
##
def callback(ch, method, properties, body):
    body = json.loads((body.decode("utf-8")))
    color = body['color']
    desc = body['description']
    id = body['ticket_id']
 
    priority = priority(color, desc)    
 

    # Write to database
    db_session = scoped_session(sessionmaker(bind=engine))
    
    query = update(Ticket).where(Ticket.ticket_id==ticket_id).values(priority=priority)

    db_session.commit()

    ch.basic_ack(delivery_tag=method.delivery_tag)



def priority(color, description):
    
    types = { "Blue" : 1,
              "Red" : 4,
              "White" : 1,
              "Yellow" : 3}

    signs = { "leaking" : 2 }

    prio = 0
    prio += types.get(color)

    desc = description.split(" ")
    
    for word in desc:
        if word in list(signs.keys()):
            prio += signs.get(word)


    if prio > 5:
        prio = 5

    return prio


with getMQ() as mq:
    mq.basic_consume(queue='toWorker', on_message_callback=callback, auto_ack=False)
    log_info("Worker running")
    mq.start_consuming()

