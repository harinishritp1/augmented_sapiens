web: gunicorn rest.rest:app
worker: rq worker -u $REDIS_URL high default low