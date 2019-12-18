#!/bin/bash -ex

cd xamarin-macios-data
git checkout master
cp -cr ../logs/ ./

mv ./*/*.zip .
unzip ./*.zip
rm -f ./*.zip

git add .
git commit -m "Add performance data for $BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION."

# Try to push 5 times, just in case someone else pushed first.
COUNTER=5
while [[ $COUNTER -gt 0 ]]; do
	if git push; then break; fi
	git pull
	(( COUNTER-- ))
done
