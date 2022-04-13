import json
from unittest import TestCase
from unittest.mock import patch
import requests
import sys
import os

from worker import worker

myDir = os.getcwd()
sys.path.append(myDir)

from pathlib import Path
path = Path(myDir)
a=str(path.parent.absolute())
sys.path.append(a)
from worker.worker import analyze_priority


class MockResponseTesting(TestCase):
    @patch('rest.getactiveticket')
    def test_request_response_with_decorator_getactiveticket(self, mock_get):
        info = {"ticket_id": 1, "status": "In progress"}
        mock_get.return_value.status_code = 200
        response = requests.post("{REST}/getactiveticket", data=json.dumps(info),
                                 headers={'Content-Type': 'application/json'})
        self.assertEqual(response.status_code, 200)

    def test_analyze_function(self):
        with patch("worker.worker.analyze_priority") as analyzeprioritymock:
            analyzeprioritymock.return_value = 2
            assert analyze_priority(color="red", description="urgent")

