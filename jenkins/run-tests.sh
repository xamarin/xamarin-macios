#!/bin/bash -e

COMMENT_FILE=$WORKSPACE/jenkins-results/comments

cd $WORKSPACE
# Unlock
security default-keychain -s builder.keychain
security list-keychains -s builder.keychain
echo "Unlock keychain"
security unlock-keychain -p $OSX_KEYCHAIN_PASS
echo "Increase keychain unlock timeout"
security set-keychain-settings -lut 7200

# Run tests
if make -C tests jenkins; then
	RV=$?
	echo "Tests succeeded" > $COMMENT_FILE
else
	RV=$?
	echo "Tests failed" > $COMMENT_FILE
fi

# Lock
security lock-keychain

exit $RV
