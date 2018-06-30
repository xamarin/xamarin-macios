#!/bin/bash -e

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

report_error ()
{
	printf "🔥 [Build failed](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
}
trap report_error ERR

if [[ x$1 == x--configure-flags ]]; then
	CONFIGURE_FLAGS="$2"
fi

ls -la "$WORKSPACE/jenkins"
echo "$WORKSPACE/jenkins/pr-comments.md:"
cat "$WORKSPACE/jenkins/pr-comments.md" || true

export BUILD_REVISION=jenkins

ENABLE_DEVICE_BUILD=

# SC2154: ghprbPullId is referenced but not assigned.
# shellcheck disable=SC2154
if test -z "$ghprbPullId"; then
	echo "Could not find the environment variable ghprbPullId, so forcing a device build."
	ENABLE_DEVICE_BUILD=1
else
	if ./jenkins/fetch-pr-labels.sh --check=skip-public-jenkins; then
		echo "Skipping execution because the label 'skip-public-jenkins' was found."
		printf "ℹ️ [Skipped execution](%s/console)\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
		exit 0
	fi
	echo "Listing modified files for pull request #$ghprbPullId..."
	if git diff-tree --no-commit-id --name-only -r "origin/pr/$ghprbPullId/merge^..origin/pr/$ghprbPullId/merge" > .tmp-files; then
		echo "Modified files found":
		sed 's/^/    /' .tmp-files || true
		if grep 'external/mono' .tmp-files > /dev/null; then
			echo "Enabling device build because mono was bumped."
		elif grep 'external/llvm' .tmp-files > /dev/null; then
			echo "Enabling device build because llvm was bumped."
		else
			echo "Not enabling device build; neither mono nor llvm was bumped."
		fi
	fi
	rm -f .tmp-files

	if test -z "$ENABLE_DEVICE_BUILD"; then
		if ./jenkins/fetch-pr-labels.sh --check=enable-device-build; then
			ENABLE_DEVICE_BUILD=1
			echo "Enabling device build because the label 'enable-device-build' was found."
		else
			echo "Not enabling device build; no label named 'enable-device-build' was found."
		fi
	fi
fi

if test -z "$ENABLE_DEVICE_BUILD"; then
	CONFIGURE_FLAGS="$CONFIGURE_FLAGS --disable-ios-device"
fi
# shellcheck disable=SC2086
./configure $CONFIGURE_FLAGS

make reset
make git-clean-all
make print-versions

time make -j8
time make install -j8

printf "✅ [Build succeeded](%s/console)\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
