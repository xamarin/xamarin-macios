#!/bin/bash -ex

cd xamarin-macios-data
git checkout master
cp -cr ../logs/ ./

mv ./*/*.zip .
for zip in ./*.zip; do
	unzip "$zip"
done

# Merge each individual xml file into one big xml file
DIR=perf-data/samples/$BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION
cd "$DIR"
{
	echo '<?xml version="1.0" encoding="utf-8" standalone="yes"?>'
	echo '<performance>'
	find . -name '*perfdata*.xml' -print0 | xargs -0 -n 1 tail -n +2 | grep -F -v -e '<performance>' -e '</performance>'
	echo '</performance>'
} > data.xml

# Add the big xml file to git
git add data.xml
git commit -m "Add performance data for $BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION."

# Push!
# Try to push 5 times, just in case someone else pushed first.
COUNTER=5
while [[ $COUNTER -gt 0 ]]; do
	if git push; then break; fi
	git pull
	(( COUNTER-- ))
done
