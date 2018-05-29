#!/bin/bash -e

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

report_error ()
{
	printf "🔥 [Test run failed](%s/Test_20Report) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"

	if test -f "$WORKSPACE/tests/TestSummary.md"; then
		printf "\\n" >> "$WORKSPACE/jenkins/pr-comments.md"
		cat "$WORKSPACE/tests/TestSummary.md" >> "$WORKSPACE/jenkins/pr-comments.md"
	fi

	touch "$WORKSPACE/jenkins/failure-stamp"
}
trap report_error ERR

export BUILD_REVISION=jenkins

# Unlock
security default-keychain -s builder.keychain
security list-keychains -s builder.keychain
echo "Unlock keychain"
security unlock-keychain -p "$(cat ~/.config/"$KEYCHAIN"-keychain)"
echo "Increase keychain unlock timeout"
security set-keychain-settings -lut 7200

# Prevent dialogs from asking for permissions.
# http://stackoverflow.com/a/40039594/183422
# Discard output since there can be a *lot* of it.
security set-key-partition-list -S apple-tool:,apple: -s -k "$(cat ~/.config/keychain)" builder.keychain >/dev/null 2>&1

# clean mono keypairs (used in tests)
rm -rf ~/.config/.mono/keypairs/

# Run tests
make -C tests jenkins

printf "✅ [Test run succeeded](%s/Test_20Report/)\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"

if test -f "$WORKSPACE/jenkins/failure-stamp"; then
	echo "Something went wrong:"
	cat "$WORKSPACE/jenkins/pr-comments.md"
	exit 1
fi
