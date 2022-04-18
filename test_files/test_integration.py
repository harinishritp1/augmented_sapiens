import json
import os
import sys

from flask import Flask, jsonify, make_response, render_template, request
import pytest
from sqlalchemy import create_engine, false, func, or_, update
import sqlalchemy
from sqlalchemy.ext.declarative import DeclarativeMeta, declarative_base
from sqlalchemy.orm import scoped_session, sessionmaker
from sqlalchemy.sql.functions import coalesce

import requests
from rest import rest
#from worker import worker, worker_function

@pytest.fixture
def app_context():
    with rest.app.app_context():
        yield

def test_send_to_mq(monkeypatch, app_context):

    MQ = fake_MQ()

    def create_fake_db(String):
        return fake_db()

    def create_fake_form():
        return fake_form()

    def fake_getMQ():
        return MQ

    #Set up the fake objects to mock
    monkeypatch.setattr(rest, "scoped_session", create_fake_db)
    monkeypatch.setattr(rest, "request", create_fake_form())
    monkeypatch.setattr(rest, "getMQ", fake_getMQ)

    rest.createticket()

    assert MQ.touched


# def test_get_from_mq(monkeypatch):

#     db = fake_db()

#     def create_fake_db(String):
#         return db

#     def create_fake_json():
#         return fake_json()

#     def create_fake_engine():
#         return fake_engine()
    
#     def create_fake_update():
#         return fake_update()

#     def create_fake_body():
#         return fake_body

#     monkeypatch.setattr(rest, "scoped_session", create_fake_db)
#     monkeypatch.setattr(rest, "json", create_fake_json)
#     monkeypatch.setattr(rest, "create_engine", create_fake_engine)
#     monkeypatch.setattr(rest, "update", create_fake_update)

#     worker.callback(fake_ch(), fake_method(), "one", create_fake_body())

#     assert db.touched


class fake_db:

        def __init__(self) -> None:
            self.touched = False
        
        def query(self, list: list):
            return ("string","othger")

        def add(self, ticket):
            return
        
        def commit(self):
            self.touched = True
            return

class fake_form:


    def __init__(self) -> None:
        self.method = "POST"

    def method(self):
        pass

    class form:

        def get(string):
            
            if string == "latitude" or string == "longitude":
                return 9999
            else:
                return "TEST"

class fake_MQ:
    
    def __init__(self) -> None:
        self.touched = False
    
    def basic_publish(self, exchange, routing_key, body):
        self.touched = True

    def __enter__(self):
        return self

    def __exit__(self, one, two, three):
        return

class fake_engine:

    def connect(self):
        return fake_conn()

class fake_conn:

    def execute(self, query):
        return

class fake_json:

    def loads(self, string):
       return {"description" : "TEST",
        "color" : "Blue", 
        "ticket_id": 9999}

class fake_update:

    def where(self):
        return

    def values(self,priority):
        return

class fake_ch:

    def basic_ack(self, delivery_tag):
        return

class fake_method:

    def __init__(self) -> None:
        self.delivery_tag = "TEST"

class fake_body:

    def decode(string):
        return '''{"description" : "TEST",
        "color" : "Blue", 
        "ticket_id": 9999}'''

