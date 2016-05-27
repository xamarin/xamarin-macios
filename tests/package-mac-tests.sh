#!/bin/bash -ex

#git clean -xfdq

DIR=$(pwd)/mac-test-package/mac-test-package
ZIP=$DIR.zip
rm -Rf $DIR
mkdir -p $DIR

make build-mac

for app in */bin/x86/*/*.app; do
	mkdir -p $DIR/tests/$app
	cp -R $app $DIR/tests/$app/..
done

cp -p Makefile-mac.inc $DIR/tests
cp -p common.mk $DIR/tests
cp -p Makefile $DIR/tests
cp -p ../Make.config $DIR
mkdir -p $DIR/../xamarin-macios/mk
cp -p ../../xamarin-macios/Make.config $DIR/../xamarin-macios
cp -p ../../xamarin-macios/mk/subdirs.mk $DIR/../xamarin-macios/mk
cp -p ../../xamarin-macios/mk/rules.mk $DIR/../xamarin-macios/mk
cp -p ../../xamarin-macios/mk/quiet.mk $DIR/../xamarin-macios/mk

# 7za compresses better, because there are many duplicated files
cd mac-test-package && zip -r ../mac-test-package.zip *
