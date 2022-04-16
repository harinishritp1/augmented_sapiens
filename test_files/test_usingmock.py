import json
from unittest import TestCase
from unittest.mock import patch
import requests
import sys
import os

# from worker.worker import analyze_priority

myDir = os.getcwd()
sys.path.append(myDir)

from pathlib import Path

path = Path(myDir)
a = str(path.parent.absolute())
sys.path.append(a)
from worker.worker_function import analyze_priority


def test_analyze_function():
        with patch("worker.worker.analyze_priority") as analyzeprioritymock:
            analyzeprioritymock.return_value = 2
            assert analyze_priority(color="red", description="urgent")


#test_analyze_function()
# pytest .\test_usingmock.py
