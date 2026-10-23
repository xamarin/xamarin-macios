# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

import argparse
import json
import sys
from pathlib import Path


BANNED_FUNCTION_TAG = "API.DangerousAPI.BannedFunction"


def banned_findings(document):
    if not isinstance(document, dict) or document.get("version") != "2.1.0":
        raise ValueError("Expected SARIF 2.1.0")
    runs = document.get("runs")
    if not isinstance(runs, list) or not runs:
        raise ValueError("Expected at least one SARIF run")

    findings = []
    for run in runs:
        if not isinstance(run, dict):
            raise ValueError("Expected a DevSkim run")
        tool = run.get("tool")
        driver = tool.get("driver") if isinstance(tool, dict) else None
        if not isinstance(driver, dict) or driver.get("name") != "devskim":
            raise ValueError("Expected a DevSkim run")
        for invocation in run.get("invocations", []):
            if invocation.get("executionSuccessful") is False:
                raise ValueError("DevSkim execution was unsuccessful")
            for notification in invocation.get("toolExecutionNotifications", []):
                if notification.get("level") == "error":
                    raise ValueError("DevSkim reported an execution error")
        results = run.get("results")
        if not isinstance(results, list):
            raise ValueError("Expected a results array")
        for result in results:
            if not isinstance(result, dict) or not isinstance(result.get("ruleId"), str):
                raise ValueError("Expected a result with a ruleId")
            properties = result.get("properties")
            tags = properties.get("tags") if isinstance(properties, dict) else None
            # DevSkim 1.0.100 writes rule tags on results, not driver.rules.
            if not isinstance(tags, list) or not all(isinstance(tag, str) for tag in tags):
                raise ValueError("Expected DevSkim result tags")
            if BANNED_FUNCTION_TAG in tags:
                findings.append(result)
    return findings


def main(argv=None):
    parser = argparse.ArgumentParser(description="Fail on DevSkim banned-function findings, at any severity.")
    parser.add_argument("reports", type=Path, nargs="+")
    args = parser.parse_args(argv)
    count = 0
    for report in args.reports:
        try:
            findings = banned_findings(json.loads(report.read_text(encoding="utf-8")))
        except (OSError, ValueError) as error:
            print(f"DevSkim report error in {report}: {error}", file=sys.stderr)
            return 2
        for finding in findings:
            location = (finding.get("locations") or [{}])[0].get("physicalLocation", {})
            uri = location.get("artifactLocation", {}).get("uri", str(report))
            line = location.get("region", {}).get("startLine", 1)
            message = finding.get("message", {}).get("text", "Banned API detected")
            print(f"{uri}:{line}: {finding['ruleId']}: {message}")
        count += len(findings)
    print(f"DevSkim: {count} banned-function finding(s). Full results are in the devskim artifact.")
    return 1 if count else 0


if __name__ == "__main__":
    sys.exit(main())
