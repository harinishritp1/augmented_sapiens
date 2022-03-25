import json
import os
import sys

from flask import Flask, Response, request
from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import scoped_session, sessionmaker

app = Flask(__name__)

# load_config()

# app = Flask(__name__)
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


class Ticket(Base):
    __table__ = Base.metadata.tables['tickets']


@app.route('/createticket', methods=['POST'])
def create_listing():
    json_data = request.get_json()
    db_session = scoped_session(sessionmaker(bind=engine))

    new_ticket = Ticket(
        ticket_id=json_data['ticket_id'],
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
    response = {'Action': 'Listing created'}

    return Response(json.dumps(response), status=200, mimetype="application/json")


app.run(host="0.0.0.0", port=5000, debug=True)
