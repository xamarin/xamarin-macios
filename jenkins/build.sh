#!/bin/bash -e

# Script that builds xamarin-macios for CI
#
#     --configure-flags=<flags>: Flags to pass to --configure. Optional
#     --timeout=<timeout>: Time out the build after <timeout> seconds.

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

export FAILURE_REASON_PATH="$WORKSPACE/jenkins/build-failure.md"

report_error ()
{
	printf "🔥 [Build failed](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
	if test -f "$FAILURE_REASON_PATH"; then
		sed 's/^/* /' "$FAILURE_REASON_PATH" >> "$WORKSPACE/jenkins/pr-comments.md"
	fi
}
trap report_error ERR

timeout ()
{
	# create a subprocess that kills this process after a certain number of seconds
	SELF_PID=$$
	(
		sleep "$1"
		echo "Execution timed out after $1 seconds."
		printf "❌ [Build timed out](%s/console)\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
		kill -9 $SELF_PID
	)&
	# kill the subprocess timeout if we exit before we time out
	TIMEOUT_PID=$!
	trap 'kill -9 $TIMEOUT_PID' EXIT
}

while ! test -z "$1"; do
	case "$1" in
		--configure-flags=*)
			CONFIGURE_FLAGS="${1#*=}"
			shift
			;;
		--configure-flags)
			CONFIGURE_FLAGS="$2"
			shift 2
			;;
		--timeout=*)
			TIMEOUT="${1#*=}"
			shift
			;;
		--timeout)
			TIMEOUT="$2"
			shift 2
			;;
		*)
			echo "Unknown argument: $1"
			exit 1
			;;
    esac
done

if test -n "$TIMEOUT"; then
	timeout "$TIMEOUT"
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
		if grep 'mk/mono.mk' .tmp-files > /dev/null; then
			echo "Enabling device build because mono was bumped."
			ENABLE_DEVICE_BUILD=1
		else
			echo "Not enabling device build; mono wasn't bumped."
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

	if ./jenkins/fetch-pr-labels.sh --check=run-sample-tests; then
		echo "The sample tests won't be triggered from public jenkins even if the 'run-sample-tests' label is set (build on internal Jenkins instead)."
		printf "ℹ️ The sample tests won't be triggered from public jenkins even if the 'run-sample-tests' label is set (build on internal Jenkins instead)." >> "$WORKSPACE/jenkins/pr-comments.md"
	fi

	if ./jenkins/fetch-pr-labels.sh --check=disable-packaged-mono; then
		echo "Building mono from source because the label 'disable-packaged-mono' was found."
		CONFIGURE_FLAGS="$CONFIGURE_FLAGS --disable-packaged-mono"
	fi
fi


if test -z "$ENABLE_DEVICE_BUILD"; then
	CONFIGURE_FLAGS="$CONFIGURE_FLAGS --disable-ios-device"
fi

# Enable dotnet bits on the bots
CONFIGURE_FLAGS="$CONFIGURE_FLAGS --enable-dotnet"

echo "Configuring the build with: $CONFIGURE_FLAGS"
# shellcheck disable=SC2086
./configure $CONFIGURE_FLAGS

# If we're building mono from source, we might not have it cloned yet
make reset

time make -j8
time make install -j8

printf "✅ [Build succeeded](%s/console)\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"

if grep MONO_BUILD_FROM_SOURCE=. configure.inc Make.config.inc >& /dev/null; then
	printf "    ⚠️ Mono built from source\\n" >> "$WORKSPACE/jenkins/pr-comments.md"
fi
