#!/bin/bash -e

report_error ()
{
	printf "🔥 [Test run failed]($BUILD_URL/Test_Report/) 🔥\\n" >> pr-comments.md

	if test -f ../tests/TestSummary.md; then
		printf "\\n" >> pr-comments.md
		cat ../tests/TestSummary.md >> pr-comments.md
	fi
}
trap report_error ERR

export BUILD_REVISION=jenkins
cd $WORKSPACE
# Unlock
security default-keychain -s builder.keychain
security list-keychains -s builder.keychain
echo "Unlock keychain"
security unlock-keychain -p `cat ~/.config/keychain`
echo "Increase keychain unlock timeout"
security set-keychain-settings -lut 7200

# clean mono keypairs (used in tests)
rm -rf ~/.config/.mono/keypairs/

# Run tests
make -C tests jenkins

printf "✅ [Test run succeeded]($BUILD_URL/Test_Report/)\\n" >> pr-comments.md
