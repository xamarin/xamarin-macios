# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

import contextlib
import io
import json
import tempfile
import unittest
from pathlib import Path

from check_devskim import BANNED_FUNCTION_TAG, banned_findings, main


def report(results):
    return {
        "version": "2.1.0",
        "runs": [{"tool": {"driver": {"name": "devskim"}}, "results": results}],
    }


def finding(tags=None, level="warning"):
    return {
        "ruleId": "DS154189",
        "level": level,
        "properties": {"tags": [BANNED_FUNCTION_TAG] if tags is None else tags},
    }


class DevSkimTests(unittest.TestCase):
    def test_clean_report(self):
        self.assertEqual([], banned_findings(report([])))

    def test_all_severities_are_gated(self):
        for level in ["error", "warning", "note", "none"]:
            with self.subTest(level=level):
                result = finding(level=level)
                self.assertEqual([result], banned_findings(report([result])))

    def test_other_findings_are_not_banned_functions(self):
        self.assertEqual([], banned_findings(report([finding(["Other.Tag"])])))

    def test_multiple_runs(self):
        document = report([])
        document["runs"] += report([finding()])["runs"]
        self.assertEqual(1, len(banned_findings(document)))

    def test_suppression_does_not_bypass_gate(self):
        result = finding()
        result["suppressions"] = [{"kind": "inSource", "status": "accepted"}]
        self.assertEqual([result], banned_findings(report([result])))

    def test_malformed_reports_fail(self):
        documents = [
            {},
            {"version": "2.1.0", "runs": []},
            {"version": "2.1.0", "runs": [{}]},
            {"version": "2.1.0", "runs": [{"tool": None}]},
            report(None),
            report([{}]),
            report([{"ruleId": "DS154189", "properties": {}}]),
            report([{"ruleId": "DS154189", "properties": None}]),
            report([finding(tags="not-a-list")]),
        ]
        for document in documents:
            with self.subTest(document=document), self.assertRaises(ValueError):
                banned_findings(document)

    def test_execution_errors_fail(self):
        for invocation in [
            {"executionSuccessful": False},
            {"executionSuccessful": True, "toolExecutionNotifications": [{"level": "error"}]},
        ]:
            document = report([])
            document["runs"][0]["invocations"] = [invocation]
            with self.subTest(invocation=invocation), self.assertRaises(ValueError):
                banned_findings(document)

    def test_exit_codes_and_diagnostics(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "report.sarif"
            for document, expected in [(report([]), 0), (report([finding()]), 1), ({}, 2)]:
                path.write_text(json.dumps(document), encoding="utf-8")
                with contextlib.redirect_stdout(io.StringIO()), contextlib.redirect_stderr(io.StringIO()):
                    self.assertEqual(expected, main([str(path)]))
            for contents in ["", "{not json"]:
                path.write_text(contents, encoding="utf-8")
                with contextlib.redirect_stderr(io.StringIO()) as errors:
                    self.assertEqual(2, main([str(path)]))
                self.assertIn("DevSkim report error", errors.getvalue())
            with contextlib.redirect_stderr(io.StringIO()):
                self.assertEqual(2, main([str(Path(directory) / "missing.sarif")]))

    def test_multiple_reports(self):
        with tempfile.TemporaryDirectory() as directory:
            paths = [Path(directory) / f"{index}.sarif" for index in range(2)]
            paths[0].write_text(json.dumps(report([])), encoding="utf-8")
            document = report([finding()])
            document["runs"][0]["results"][0]["locations"] = [{
                "physicalLocation": {
                    "artifactLocation": {"uri": "runtime/example.c"},
                    "region": {"startLine": 42},
                },
            }]
            paths[1].write_text(json.dumps(document), encoding="utf-8")
            with contextlib.redirect_stdout(io.StringIO()) as output:
                self.assertEqual(1, main([str(path) for path in paths]))
            self.assertIn("runtime/example.c:42: DS154189", output.getvalue())

    def test_missing_report_among_valid_reports_fails(self):
        with tempfile.TemporaryDirectory() as directory:
            valid = Path(directory) / "runtime.sarif"
            missing = Path(directory) / "src.sarif"
            valid.write_text(json.dumps(report([])), encoding="utf-8")
            with contextlib.redirect_stdout(io.StringIO()), contextlib.redirect_stderr(io.StringIO()) as errors:
                self.assertEqual(2, main([str(valid), str(missing)]))
            self.assertIn(str(missing), errors.getvalue())


if __name__ == "__main__":
    unittest.main()
