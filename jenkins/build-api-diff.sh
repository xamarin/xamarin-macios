#!/bin/bash -e

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

report_error ()
{
	printf "🔥 [Failed to create API Diff](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
}
trap report_error ERR

export BUILD_REVISION=jenkins
make -j8 -C tools/apidiff jenkins-api-diff

if [[ "x$1" == "x--publish" ]]; then
	URL_PREFIX=$(./jenkins/publish-results.sh | grep "^Url Prefix: " | sed 's/^Url Prefix: //')
	URL="$URL_PREFIX/api-diff/index.html"
else
	URL="$BUILD_URL/API_20diff_20_28from_20stable_29"
fi

printf "✅ [API Diff (from stable)](%s)\\n" "$URL" >> "$WORKSPACE/jenkins/pr-comments.md"
