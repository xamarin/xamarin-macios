#!/bin/bash -ex

DATA_REPO_NAME=xamarin-macios-data

# grab Azure Devop's authorization token from the current repo, and add it to the global git configuration
AUTH=$(git config -l | grep AUTHORIZATION | sed 's/.*AUTHORIZATION: //')
AUTH_MD5=$(echo "$AUTH" | md5)
git config --global http.extraheader "AUTHORIZATION: $AUTH"
echo "AUTH_MD5=$AUTH_MD5"

# Debug spew, checking if the authorization token is correct
git ls-remote https://github.com/xamarin/maccore || true
git ls-remote https://github.com/xamarin/xamarin-macios-data || true


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
