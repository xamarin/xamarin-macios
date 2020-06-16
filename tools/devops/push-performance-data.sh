#!/bin/bash -ex

cd xamarin-macios-data
git checkout main
cp -cr ../logs/ ./

mv ./*/*.zip .
for zip in ./*.zip; do
	unzip "$zip"
done

# Merge each individual xml file into one big xml file
DIR=perf-data/samples/$BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION
cd "$DIR"
# Merge the xml files from each bot into a big per-bot xml file. Don't merge
# the xml from all the bots together into a single enormous xml file, because
# it'll be close to GitHub's size limit per file (limit is 100mb, the enormous
# xml file would be ~80mb now), and might very well pass that one day.
for job in ????????-????-????-????-????????????; do
{
	echo '<?xml version="1.0" encoding="utf-8" standalone="yes"?>'
	echo '<performance version="1.0">'
	find "$job" -name '*perfdata*.xml' -print0 | xargs -0 -n 1 tail -n +2 | grep -F -v -e '<performance>' -e '</performance>'
	echo '</performance>'
} > "data-$job.xml"
done

# Add the big xml files to git
git add data-*.xml
git commit -m "Add performance data for $BUILD_SOURCEBRANCHNAME/$BUILD_SOURCEVERSION."

# Push!
# Try to push 5 times, just in case someone else pushed first.
COUNTER=5
while [[ $COUNTER -gt 0 ]]; do
	if git push; then break; fi
	git pull
	(( COUNTER-- ))
done
