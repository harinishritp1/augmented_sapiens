web: gunicorn rest.rest:app
worker: rq worker.worker -u $REDIS_URL high default low