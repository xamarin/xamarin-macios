#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")"

# Clone files instead of copying them on APFS file systems. Much faster.
CP="cp"
if df -t apfs / >/dev/null 2>&1; then
	CP="cp -c"
fi

#git clean -xfdq

DIR=$(pwd)/mac-test-package/mac-test-package
rm -Rf "$DIR"
mkdir -p "$DIR"

make test.config
cat test.config
INCLUDE_MAC=$(grep ^INCLUDE_MAC= test.config | sed 's/.*=//')
INCLUDE_MACCATALYST=$(grep ^INCLUDE_MACCATALYST= test.config | sed 's/.*=//')
XCODE_DEVELOPER_ROOT=$(grep ^XCODE_DEVELOPER_ROOT= test.config | sed 's/.*=//')
export MD_APPLE_SDK_ROOT="$(dirname "$(dirname "$XCODE_DEVELOPER_ROOT")")"
export RootTestsDirectory="$(pwd)"

make

TEST_SUITE_DEPENDENCIES+=(bindings-test)
TEST_SUITE_DEPENDENCIES+=(EmbeddedResources)
TEST_SUITE_DEPENDENCIES+=(fsharplibrary)
TEST_SUITE_DEPENDENCIES+=(BundledResources)

for dep in "${TEST_SUITE_DEPENDENCIES[@]}"; do
	if test -n "$INCLUDE_MAC"; then
		make -C "$dep"/dotnet/macOS build
	fi
	if test -n "$INCLUDE_MACCATALYST"; then
		make -C "$dep"/dotnet/MacCatalyst build
	fi
done

TEST_SUITES+=(build-dontlink)
TEST_SUITES+=(build-linksdk)
TEST_SUITES+=(build-linkall)
TEST_SUITES+=(build-introspection)
TEST_SUITES+=(build-monotouch-test)

make -f packaged-macos-tests.mk "${TEST_SUITES[@]}" $MAKE_FLAGS

for app in linker/*/*/dotnet/*/bin/*/*/*/*.app */dotnet/*/bin/*/*/*/*.app; do
	mkdir -p "$DIR/tests/$app"
	$CP -R "$app" "$DIR/tests/$app/.."
done

$CP -p packaged-macos-tests.mk "$DIR/tests"
$CP -p run-with-timeout.* "$DIR/tests"
$CP -p ../Make.config "$DIR"
$CP -p ../Make.versions "$DIR"
$CP -p test-dependencies.sh "$DIR"
$CP -p ../system-dependencies.sh "$DIR"
$CP -p ../configure.inc "$DIR"
mkdir -p "$DIR/mk"
$CP -p ../mk/subdirs.mk "$DIR/mk"
$CP -p ../mk/rules.mk "$DIR/mk"
$CP -p ../mk/quiet.mk "$DIR/mk"
$CP -p ../mk/mono.mk "$DIR/mk"

cd mac-test-package && 7z a ../mac-test-package.7z ./*
