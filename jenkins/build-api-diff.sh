#!/bin/bash -e

report_error ()
{
	printf "🔥 [Failed to create API Diff]($BUILD_URL/console) 🔥\\n" >> $WORKSPACE/jenkins/pr-comments.md
}
trap report_error ERR

cd $WORKSPACE
export BUILD_REVISION=jenkins
make -j8 -C tools/apidiff jenkins-api-diff

printf "✅ [API Diff (from stable)]($BUILD_URL/API_diff_(from_stable))\\n" >> $WORKSPACE/jenkins/pr-comments.md
