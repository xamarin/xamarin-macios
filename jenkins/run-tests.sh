#!/bin/bash -e

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

# Print out environment for debug purposes
env | sort

report_error ()
{
	printf "🔥 [Test run failed](%s) 🔥\\n" "$URL" >> "$WORKSPACE/jenkins/pr-comments.md"

	if test -f "$WORKSPACE/tests/TestSummary.md"; then
		printf "\\n" >> "$WORKSPACE/jenkins/pr-comments.md"
		cat "$WORKSPACE/tests/TestSummary.md" >> "$WORKSPACE/jenkins/pr-comments.md"
	fi

	touch "$WORKSPACE/jenkins/failure-stamp"
}
trap report_error ERR

# SC2154: ghprbPullId is referenced but not assigned.
# shellcheck disable=SC2154
if test -n "$ghprbPullId" && ./jenkins/fetch-pr-labels.sh --check=skip-public-jenkins; then
	echo "Skipping tests because the label 'skip-public-jenkins' was found."
	exit 0
fi

TARGET=jenkins
PUBLISH=
KEYCHAIN=builder
KEYCHAIN_PWD_FILE=~/.config/keychain
while ! test -z "$1"; do
	case "$1" in
		--target=*)
			TARGET="${1:9}"
			shift
			;;
		--keychain=*)
			KEYCHAIN="${1:11}"
			KEYCHAIN_PWD_FILE=~/.config/$KEYCHAIN-keychain
			shift
			;;
		--publish)
			PUBLISH=1
			shift
			;;
		*)
			echo "Unknown argument: $1"
			exit 1
			;;
    esac
done

if test -n "$PUBLISH"; then
	PUBLISH_OUTPUT=$(./jenkins/publish-results.sh)
	URL_PREFIX=$(echo "$PUBLISH_OUTPUT" | grep "^Url Prefix: " | sed 's/^Url Prefix: //')
	URL="$URL_PREFIX/tests/index.html"
	TESTS_PERIODIC_COMMAND=$(echo "$PUBLISH_OUTPUT" | grep "^Periodic Command: " | sed 's/^Periodic Command: //')
	export TESTS_PERIODIC_COMMAND
else
	URL="$BUILD_URL/Test_20Report"
fi


export BUILD_REVISION=jenkins

# Unlock
if ! test -f ~/Library/Keychains/"$KEYCHAIN".keychain-db; then
	echo "The '$KEYCHAIN' keychain is not available."
	exit 1
fi
security default-keychain -s "$KEYCHAIN.keychain"
security list-keychains -s "$KEYCHAIN.keychain"
echo "Unlock keychain"
security unlock-keychain -p "$(cat "$KEYCHAIN_PWD_FILE")"
echo "Increase keychain unlock timeout to 6 hours"
security set-keychain-settings -lut 21600
security -v find-identity "$KEYCHAIN.keychain"

# Prevent dialogs from asking for permissions.
# http://stackoverflow.com/a/40039594/183422
# Discard output since there can be a *lot* of it.
security set-key-partition-list -S apple-tool:,apple:,codesign: -s -k "$(cat "$KEYCHAIN_PWD_FILE")" "$KEYCHAIN.keychain" >/dev/null 2>&1

# clean mono keypairs (used in tests)
rm -rf ~/.config/.mono/keypairs/

# Run tests
RC=0
make -C tests "$TARGET" || RC=$?

# upload of the final html report
if test -n "$PUBLISH"; then
	./jenkins/publish-results.sh
fi

if [[ x$RC != x0 ]]; then
	report_error
	exit $RC
fi

printf "✅ [Test run succeeded](%s)\\n" "$URL" >> "$WORKSPACE/jenkins/pr-comments.md"

if test -f "$WORKSPACE/jenkins/failure-stamp"; then
	echo "Something went wrong:"
	cat "$WORKSPACE/jenkins/pr-comments.md"
	exit 1
fi
