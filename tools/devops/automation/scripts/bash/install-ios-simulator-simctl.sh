#!/bin/bash -ex

# this is a workaround for the fact that xcodebuild -downloadPlatform iOS does not work on EO machines
xcodebuild -runFirstLaunch

SIMULATOR_FILENAME=iOS_18_beta_3_Simulator_Runtime.dmg
SIMULATOR_VERSION=22A5307f

xcrun simctl runtime list
if xcrun simctl runtime list | grep "iOS.*($SIMULATOR_VERSION)"; then
	echo "The iOS Simulator runtime $SIMULATOR_VERSION is already installed."
	exit 0
fi

echo "The iOS Simulator runtime $SIMULATOR_VERSION is not installed, downloading..."
curl -L -H "Authorization: token $GITHUB_TOKEN" "https://dl.internalx.com/internal-files/xcode.simulator-runtimes/$SIMULATOR_FILENAME" --output "$SIMULATOR_FILENAME"

echo "The iOS Simulator runtime $SIMULATOR_VERSION is not installed, installing..."
xcrun simctl runtime add "$SIMULATOR_FILENAME"

echo "The iOS Simulator runtime $SIMULATOR_VERSION succesfully installed."

rm -f "$SIMULATOR_FILENAME"
