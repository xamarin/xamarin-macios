#!/bin/bash -ex

# git clone https://github.com/xamarin/xamarin-macios-performance-data
mkdir xamarin-macios-performance-data
( cd xamarin-macios-performance-data && git init . )

DIR=xamarin-macios-performance-data/$BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION/$SYSTEM_JOBID
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
