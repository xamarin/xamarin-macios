#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")/../../../../.."


SIMULATOR_FILENAME=$(grep '^XCODE_IOS_SIMULATOR_FILENAME=' Make.config | sed -e 's/^XCODE_IOS_SIMULATOR_FILENAME=//')
SIMULATOR_VERSION=$(grep '^XCODE_IOS_SIMULATOR_VERSION=' Make.config | sed -e 's/^XCODE_IOS_SIMULATOR_VERSION=//')
DEVELOPER_DIR=$(grep '^XCODE_DEVELOPER_ROOT=' Make.config | sed -e 's/^XCODE_DEVELOPER_ROOT=//')
export DEVELOPER_DIR

# this is a workaround for the fact that xcodebuild -downloadPlatform iOS does not work on EO machines
xcodebuild -runFirstLaunch

xcrun simctl runtime list
if xcrun simctl runtime list | grep "iOS.*($SIMULATOR_VERSION)"; then
	echo "The iOS Simulator runtime $SIMULATOR_VERSION is already installed."
	exit 0
fi

echo "The iOS Simulator runtime $SIMULATOR_VERSION is not installed, downloading..."
curl -f -L -H "Authorization: token $GITHUB_TOKEN" "https://dl.internalx.com/internal-files/xcode.simulator-runtimes/$SIMULATOR_FILENAME" --output "$SIMULATOR_FILENAME"

echo "The iOS Simulator runtime $SIMULATOR_VERSION is not installed, installing..."
xcrun simctl runtime add "$SIMULATOR_FILENAME"

echo "The iOS Simulator runtime $SIMULATOR_VERSION succesfully installed."

rm -f "$SIMULATOR_FILENAME"
