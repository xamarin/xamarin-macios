#!/bin/bash -e

export BUILD_REVISION=jenkins
cd $WORKSPACE
export BUILD_REVISION=jenkins
# Unlock
security default-keychain -s builder.keychain
security list-keychains -s builder.keychain
echo "Unlock keychain"
security unlock-keychain -p $OSX_KEYCHAIN_PASS
echo "Increase keychain unlock timeout"
security set-keychain-settings -lut 7200

# Run tests
make -C tests jenkins

# Lock
security lock-keychain
