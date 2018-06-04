#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

report_error ()
{
	printf "🔥 [Failed to publish results](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
}
trap report_error ERR

# SC2154: ghprbPullId is referenced but not assigned.
# shellcheck disable=SC2154
if test -n "$ghprbPullId"; then
	BRANCH_NAME="pr$ghprbPullId"
elif test -z "$BRANCH_NAME"; then
	echo "Neither BRANCH_NAME nor ghprbPullId is set"
	exit 1
fi
if test -z "$BUILD_NUMBER"; then
	echo "BUILD_NUMBER is not set"
	exit 1
fi
P="jenkins/xamarin-macios/${BRANCH_NAME}/$(git log -1 --pretty=%H)/${BUILD_NUMBER}"

echo "Url Prefix: http://xamarin-storage/$P/jenkins-results"
echo "Periodic Command: --periodic-interval 10 --periodic-command rsync --periodic-command-arguments '-avz --chmod=+r -e ssh $WORKSPACE/jenkins-results builder@xamarin-storage:/volume1/storage/$P'"

mkdir -p "$WORKSPACE/jenkins-results"

# Publish

# Make sure the target directory exists

# SC2029: Note that, unescaped, this expands on the client side. [Referring to $P]
# shellcheck disable=SC2029
ssh builder@xamarin-storage "mkdir -p /volume1/storage/$P"
rsync -avz --chmod=+r -e ssh "$WORKSPACE/jenkins-results" "builder@xamarin-storage:/volume1/storage/$P"
