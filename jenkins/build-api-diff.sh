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

printf "✅ [API Diff (from stable)](%s/API_20diff_20_28from_20stable_29)\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
