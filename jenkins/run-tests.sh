#!/bin/bash -e

export BUILD_REVISION=jenkins
cd $WORKSPACE
# Unlock
security default-keychain -s builder.keychain
security list-keychains -s builder.keychain
echo "Unlock keychain"
security unlock-keychain -p $OSX_KEYCHAIN_PASS
echo "Increase keychain unlock timeout"
security set-keychain-settings -lut 7200

# Run tests
make -C tests jenkins

# go through the diff xml results and do an xslt transformation for the old Test Report
for i in tests/logs/*/*.xml; do
	logname=$(basename "${i%.*}");
	dirname=$(dirname "$i");
	full_path="$dirname/$logname.log";
	xsltproc jenkins/nunit-summary.xslt $i > $full_path;
done
# Lock
security lock-keychain
