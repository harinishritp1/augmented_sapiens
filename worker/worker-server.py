import configparser
import json
import os
import platform
import sys

import pika
from sqlalchemy import create_engine, update
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import scoped_session, sessionmaker

hostname = platform.node()

rabbitMQHost = os.getenv("RABBITMQ_HOST") or "localhost"
print(f"Connecting to rabbitmq({rabbitMQHost})")

db_config = {}

parser = configparser.ConfigParser()
parser.read('config.ini')
for sect in parser.sections():
    if sect == "Database":
        for k, v in parser.items(sect):
            db_config[k] = v

url = 'postgresql://' + db_config['user'] + ':' + db_config['password'] + '@' + db_config['host'] + ":" + db_config['port'] + '/' + db_config['db_name']

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


def callback(ch, method, properties, body):
    body = json.loads((body.decode("utf-8")))
    ticket_id = body['ticket_id']
    description = body['description']
    color = body['color']

    priority = analyze_priority(color, description)    

    db_session = scoped_session(sessionmaker(bind=engine))
    conn = engine.connect()
    query = update(Ticket).where(Ticket.ticket_id==ticket_id).values(priority=priority)
    conn.execute(query)
    db_session.commit()

    ch.basic_ack(delivery_tag=method.delivery_tag)

def analyze_priority(color, description):
    types = { "Blue" : 1,
              "Red" : 4,
              "White" : 1,
              "Yellow" : 3}
    signs = { "leaking" : 2 }
    priority = 0
    priority += types.get(color)
    desc = description.split(" ")
    for word in desc:
        if word in list(signs.keys()):
            priority += signs.get(word)
    if priority > 5:
        priority = 5
    return priority


with getMQ() as mq:
    mq.basic_consume(queue='toWorker', on_message_callback=callback, auto_ack=False)
    log_info("Worker running")
    mq.start_consuming()

