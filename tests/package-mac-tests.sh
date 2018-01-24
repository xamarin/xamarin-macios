#!/bin/bash -ex

#git clean -xfdq

DIR=$(pwd)/mac-test-package/mac-test-package
ZIP=$DIR.zip
rm -Rf $DIR
mkdir -p $DIR

make build-mac

for app in */bin/x86/*/*.app linker-mac/*/bin/x86/*/*.app; do
	mkdir -p "$DIR/tests/$app"
	cp -R "$app" "$DIR/tests/$app/.."
done

cp -p Makefile-mac.inc $DIR/tests
cp -p common.mk $DIR/tests
cp -p Makefile $DIR/tests
cp -p ../Make.config $DIR
cp -p test-dependencies.sh $DIR
cp -p ../system-dependencies.sh $DIR
mkdir -p $DIR/mk
cp -p ../Make.config $DIR
cp -p ../mk/subdirs.mk $DIR/mk
cp -p ../mk/rules.mk $DIR/mk
cp -p ../mk/quiet.mk $DIR/mk

# 7za compresses better, because there are many duplicated files
cd mac-test-package && zip -r ../mac-test-package.zip *
