#!/bin/bash -ex

DATA_REPO_NAME=xamarin-macios-data

git clone https://github.com/xamarin/$DATA_REPO_NAME

DIR=$DATA_REPO_NAME/perf-data/samples/$BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION/$SYSTEM_JOBID
mkdir -p "$DIR"

cp -c tests/sampletester/bin/Debug/tmp-test-dir/execution-logs/*.xml "$DIR/"
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
