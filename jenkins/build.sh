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
		echo "Downloading labels for pull request #$ghprbPullId..."
		if curl https://api.github.com/repos/xamarin/xamarin-macios/issues/$ghprbPullId/labels > .tmp-labels; then
			echo "Labels found:"
			cat .tmp-labels | grep "\"name\":" | sed 's/name": \"//' | sed 's/.*\"\(.*\)\".*/    \1/' || true
			if grep '\"enable-device-build\"' .tmp-labels >/dev/null; then
				ENABLE_DEVICE_BUILD=1
				echo "Enabling device build because the label 'enable-device-build' was found."
			else
				echo "Not enabling device build; no label named 'enable-device-build' was found."
			fi
		else
			echo "Failed to fetch labels for the pull request $ghprbPullId, so won't check if we're doing a device build."
		fi
		rm -f .tmp-labels
	fi
fi

if test -n "$ENABLE_DEVICE_BUILD"; then
	./configure
else
	./configure --disable-ios-device
fi

time make world

printf "✅ [Build succeeded]($BUILD_URL/console)\\n" >> $WORKSPACE/jenkins/pr-comments.md
