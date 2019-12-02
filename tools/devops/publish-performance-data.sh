#!/bin/bash -ex

DATA_REPO_NAME=xamarin-macios-data

DIR=$DATA_REPO_NAME/perf-data/samples/$BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION/$SYSTEM_JOBID
mkdir -p "$DIR"

XMLS=(xamarin-macios/tests/sampletester/bin/Debug/tmp-test-dir/execution-logs/*.xml)
if ! test -f "${XMLS[0]}"; then
	echo "##vso[task.logissue type=warning]Could not find any performance data to publish"
	exit 0
fi
cp -c xamarin-macios/tests/sampletester/bin/Debug/tmp-test-dir/execution-logs/*.xml "$DIR/"
cd "$DIR"
git add .
git commit -m "Add performance data for $BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION from the $SYSTEM_JOBNAME job."

# Try to push 5 times, just in case someone else pushed first.
COUNTER=5
while [[ $COUNTER -gt 0 ]]; do
	if git push; then break; fi
	git pull
	(( COUNTER-- ))
done
