import requests
import json
import os
import sys

REST = os.getenv("REST") or "localhost:5001"


def mkReq(reqmethod, endpoint, data):
    print(f"Response to http://{REST}/{endpoint} request is")
    jsonData = json.dumps(data)
    response = reqmethod(f"http://{REST}/{endpoint}", data=jsonData,
                         headers={'Content-type': 'application/json'})
    print(f"http://{REST}/{endpoint}")
    if response.status_code == 200:
        jsonResponse = json.dumps(response.json(), indent=4, sort_keys=True)
        print(jsonResponse)
        return
    else:
        print(
            f"response code is {response.status_code}, raw response is {response.text}")
        return response.text


# mkReq(requests.post, "createticket",
#       data={
#             "image" : "abc",
#             "latitude" : 70.5,
#             "longitude" : 110.5,
#             "color" : "Red",
#             "priority" : 1,
#             "description" : "Gas stove is leaking",
#             "status" : "Open"
#         }
#     )

# mkReq(requests.post, "createticket",
#       data={
#             "image" : "xyz",
#             "latitude" : 80.5,
#             "longitude" : 120.5,
#             "color" : "Yellow",
#             "priority" : 4,
#             "description" : "Short circuit",
#             "status" : "In progress"
#         }
#     )

# mkReq(requests.post, "createticket",
#       data={
#             "image" : "xyz",
#             "latitude" : 80.5,
#             "longitude" : 120.5,
#             "color" : "Blue",
#             "priority" : 2,
#             "description" : "Pipe broken",
#             "status" : "In progress"
#         }
#     )

mkReq(requests.post, "updateticket",
      data={
          "ticket_id": 2,
          "status": "Closed"
      }
      )

mkReq(requests.get, "getactiveticket",
      data={
      }
      )

sys.exit(0)
