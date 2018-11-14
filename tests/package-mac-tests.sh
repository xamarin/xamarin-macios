#!/bin/bash -ex

# Clone files instead of copying them on APFS file systems. Much faster.
CP="cp"
if df -t apfs / >/dev/null 2>&1; then
	CP="cp -c"
fi

#git clean -xfdq

DIR=$(pwd)/mac-test-package/mac-test-package
ZIP=$DIR.zip
rm -Rf $DIR
mkdir -p $DIR

make
make .stamp-configure-projects-mac
msbuild bindings-test/bindings-test-mac.csproj
make build-mac-dontlink build-mac-apitest build-mac-introspection build-mac-linksdk build-mac-linkall build-mac-xammac_tests build-mac-system-dontlink -j8

for app in */bin/x86/*/*.app linker/mac/*/bin/x86/*/*.app introspection/Mac/bin/x86/*/*.app; do
	mkdir -p "$DIR/tests/$app"
	$CP -R "$app" "$DIR/tests/$app/.."
done

$CP -p Makefile-mac.inc $DIR/tests
$CP -p common.mk $DIR/tests
$CP -p Makefile $DIR/tests
$CP -p ../Make.config $DIR
$CP -p ../Make.versions $DIR
$CP -p test-dependencies.sh $DIR
$CP -p ../system-dependencies.sh $DIR
mkdir -p $DIR/mk
$CP -p ../Make.config $DIR
$CP -p ../mk/subdirs.mk $DIR/mk
$CP -p ../mk/rules.mk $DIR/mk
$CP -p ../mk/quiet.mk $DIR/mk

# 7za compresses better, because there are many duplicated files
cd mac-test-package && zip -r ../mac-test-package.zip *
