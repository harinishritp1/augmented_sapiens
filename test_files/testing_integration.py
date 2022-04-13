import requests
import json
import sys
from unittest import TestCase, main, mock
from rest.rest import createticket


class IntegrationTesting(TestCase):
    @mock.patch("rest.rest.createticket")
    def testcreateticket(self, mock_post):
        my_mock_response = mock.Mock(status_code=200)
        my_mock_response.json.return_value = {
            "result": [
                {
                    "MESSAGE": "Ticket created successfully"
                }
            ]
        }
        mock_post.return_value = my_mock_response
        # response = requests.post(url="http://127.0.0.1:5000/createticket", data={
        #     "image": "xyz",
        #     "latitude": 80.5,
        #     "longitude": 120.5,
        #     "color": "Yellow",
        #     "description": "Short circuit",
        #     "ticket_ID": 1,
        #     "status": "in progress"
        # } )

        response = createticket(data={
            "image": "xyz",
            "latitude": 80.5,
            "longitude": 120.5,
            "color": "Yellow",
            "description": "Short circuit",
            "ticket_ID": 1,
            "status": "in progress"
        })
        self.assertEqual(response.status_code, 200)

        agent_data = response["result"][0]
        self.assertEqual(agent_data["result"][0], "Ticket created successfully")

    def testexample(self):
        response = requests.get("http://127.0.0.1:5000/createticket")
        assert response.headers["Content-Type"] == "application/json"

    def test_post_headers_body_json():
        url = 'http://127.0.0.1:5000/createticket'

        # Additional headers.
        headers = {'Content-Type': 'application/json'}

        # Body
        #payload = {'key1': 1, 'key2': 'value2'}
        payload = {
            "image": "xyz",
            "latitude": 80.5,
            "longitude": 120.5,
            "color": "Yellow",
            "description": "Short circuit",
            "ticket_ID": 1,
            "status": "in progress"
        }

        # convert dict to json string by json.dumps() for body data.
        resp = requests.post(url, headers=headers, data=json.dumps(payload, indent=4))

        # Validate response headers and body contents, e.g. status code.
        assert resp.status_code == 200
        resp_body = resp.json()
        assert resp_body['url'] == url

        # print response full body as text
        print(resp.text)


if __name__ == "__main__":
    main()
