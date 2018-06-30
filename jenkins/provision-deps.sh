#!/bin/bash -e

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

report_error ()
{
	echo "🔥 [Provisioning failed]($BUILD_URL/console) 🔥" >> "$WORKSPACE/jenkins/pr-comments.md"
}
trap report_error ERR

# SC2154: ghprbPullId is referenced but not assigned.
# shellcheck disable=SC2154
if test -n "$ghprbPullId" && ./jenkins/fetch-pr-labels.sh --check=skip-public-jenkins; then
	echo "Skipping provisioning diff because the label 'skip-public-jenkins' was found."
	exit 0
fi

./system-dependencies.sh --provision-all

echo "✅ [Provisioning succeeded]($BUILD_URL/console)" >> "$WORKSPACE/jenkins/pr-comments.md"
