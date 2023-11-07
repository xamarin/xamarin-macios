#!/bin/bash -ex

DIR=perf-data/samples/$BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION/$SYSTEM_JOBID
mkdir -p "$DIR"

XMLS=(xamarin-macios/tests/sampletester/bin/Debug/tmp-test-dir/execution-logs/*.xml)
if ! test -f "${XMLS[0]}"; then
	echo "##vso[task.logissue type=warning]Could not find any performance data to publish"
	exit 0
fi
cp -c xamarin-macios/tests/sampletester/bin/Debug/tmp-test-dir/execution-logs/*.xml "$DIR/"

mkdir -p logs
zip -9r "logs/execution-logs-$SYSTEM_JOBID.zip" perf-data
