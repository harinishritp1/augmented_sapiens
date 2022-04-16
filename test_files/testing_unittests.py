import unittest
from unittest import TestCase
import sys
import os

myDir = os.getcwd()
sys.path.append(myDir)
from pathlib import Path

path = Path(myDir)
a = str(path.parent.absolute())
sys.path.append(a)
from worker.worker_function import analyze_priority
#import worker.worker
# from worker import worker #import analyze_priority

class AnalyzePriorityFunction(TestCase):
    def test_analyze_null_condition(self):
        result = analyze_priority(color="red", description="important")
        self.assertIsNotNone(result);

    def test_analyze_priority(self):
        actual = analyze_priority(color="red", description="urgent");
        expected = 2
        self.assertEqual(actual, expected)


if __name__ == '__main__':
    unittest.main()

# To run : python3 -m unittest testing_unittests.AnalyzePriorityFunction
