from rest.rest import app1
from worker.worker import app2

if __name__ == '__main__':
    app1.run(debug=True)
    app2.run(debug=True)