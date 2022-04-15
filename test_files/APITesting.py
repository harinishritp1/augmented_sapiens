import json
import os
import unittest
from unittest import TestCase
from unittest.mock import patch

import requests

from worker.worker import analyze_priority

REST = os.getenv("REST") or "localhost:5001"


# class ApiTest(TestCase):
#     @patch('requests.post')
#     def test_update_ticket(self):
#         info = {"ticket_id": 1, "status": "In progress"}
#         resp = requests.post("{REST}/updateticket", data=json.dumps(info), headers={'Content-Type': 'application/json'})
#         self.assertEqual(resp.status_code, 200)


class AnalyzePriorityFunction(TestCase):
    def test_analyze_null_condition(self):
        print("23232")
        result = analyze_priority(color="red", description="important")
        print("frrrr")
        self.assertIsNotNone(result);

    def test_analyze_priority(self):
        actual = analyze_priority(color="red", description="urgent");
        expected = 1
        self.assertEqual(actual, expected)


class MockResponseTesting(TestCase):
    @patch('rest.requests.get')
    def test_request_response_with_decorator_getactiveticket(self, mock_get):
        mock_get.return_value.status_code = 200
        response = requests.post("{REST}/getactiveticket", data=json.dumps(info),
                                 headers={'Content-Type': 'application/json'})
        self.assertEqual(response.status_code, 200)


# if __name__ == '__main__':
#     unittest.main()
