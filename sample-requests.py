import requests
import json
import os
import sys
import base64

REST = os.getenv("REST") or "localhost:5000"
img_byte = b'[10, 20]'
b = img_byte.decode('utf-8')

# d = img_byte.decode('utf8').replace("'", '"')
binar = json.loads(b)
binar1 = json.dumps(binar)
print("dsds")
print(type(binar1))
print("b;eh")


def mkReq(reqmethod, endpoint, data):
    print(f"Response to http://{REST}/{endpoint} request is")
    jsonData = json.dumps(data)
    response = reqmethod(f"http://{REST}/{endpoint}", data=jsonData,
                         headers={'Content-type': 'application/json'})
    if response.status_code == 200:
        jsonResponse = json.dumps(response.json(), indent=4, sort_keys=True)
        print(jsonResponse)
        return
    else:
        print(
            f"response code is {response.status_code}, raw response is {response.text}")
        return response.text


mkReq(requests.post, "createticket",
      data={
          "ticket_id": 1,
          "image": binar1,
          "latitude": 70.5,
          "longitude": 110.5,
          "color": "Red",
          "priority": "Fire",
          "description": "Gas stove is leaking",
          "status": "Open"
      }
      )

sys.exit(0)
