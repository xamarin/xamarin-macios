#!/bin/bash -e

report_error ()
{
	printf "🔥 [Build failed]($BUILD_URL/console) 🔥\\n" >> $WORKSPACE/jenkins/pr-comments.md
}
trap report_error ERR

ls -la $WORKSPACE/jenkins
echo "$WORKSPACE/jenkins/pr-comments.md:"
cat $WORKSPACE/jenkins/pr-comments.md

cd $WORKSPACE
export BUILD_REVISION=jenkins

ENABLE_DEVICE_BUILD=

if test -z $ghprbPullId; then
	echo "Could not find the environment variable ghprbPullId, so won't check if we're doing a device build."
else
	echo "Listing modified files for pull request #$ghprbPullId..."
	if git diff-tree --no-commit-id --name-only -r "origin/pr/$ghprbPullId/merge^..origin/pr/$ghprbPullId/merge" > .tmp-files; then
		echo "Modified files found":
		cat .tmp-files | sed 's/^/    /' || true
		if grep 'external/mono' .tmp-files > /dev/null; then
			echo "Enabling device build because mono was bumped."
		elif grep 'external/llvm' .tmp-files > /dev/null; then
			echo "Enabling device build because llvm was bumped."
		else
			echo "Not enabling device build; neither mono nor llvm was bumped."
		fi
	fi
	rm -f .tmp-files

	if test -z $ENABLE_DEVICE_BUILD; then
		if ./jenkins/fetch-pr-labels.sh --check=enable-device-build; then
			ENABLE_DEVICE_BUILD=1
			echo "Enabling device build because the label 'enable-device-build' was found."
		else
			echo "Not enabling device build; no label named 'enable-device-build' was found."
		fi
	fi
fi

if test -n "$ENABLE_DEVICE_BUILD"; then
	./configure
else
	./configure --disable-ios-device
fi

time make world

printf "✅ [Build succeeded]($BUILD_URL/console)\\n" >> $WORKSPACE/jenkins/pr-comments.md
