#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")"

# Clone files instead of copying them on APFS file systems. Much faster.
CP="cp"
if df -t apfs / >/dev/null 2>&1; then
	CP="cp -c"
fi

#git clean -xfdq

rm -Rf "$(pwd)/dotnet-test-package"
DIR=$(pwd)/dotnet-test-package/xamarin-macios
ZIP=$DIR.7z
mkdir -p $DIR
mkdir -p $DIR/tests/dotnet/UnitTests/bin/Debug/net5.0/
mkdir -p $DIR/.git

make -j8
make -C dotnet/UnitTests publish

$CP -r dotnet $DIR/tests/
rm -Rf $DIR/tests/dotnet/packages

# Various files to make 'make' work
$CP -p ../Make.config $DIR
$CP -p ../Make.versions $DIR
$CP -p ../Make.config $DIR
mkdir -p $DIR/mk
$CP -p ../Make.config $DIR
$CP -p ../mk/subdirs.mk $DIR/mk
$CP -p ../mk/rules.mk $DIR/mk
$CP -p ../mk/quiet.mk $DIR/mk
$CP -p ../mk/mono.mk "$DIR/mk"

# Files to make the unit tests run
$CP -p ../global.json $DIR
$CP -p ../NuGet.config $DIR
$CP -p test.config $DIR/tests

# Zip it all up
rm -f dotnet-test-package.7z
cd dotnet-test-package
7z a ../dotnet-test-package.7z *
