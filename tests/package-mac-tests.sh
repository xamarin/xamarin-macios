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
XCODE_DEVELOPER_ROOT=$(grep ^XCODE_DEVELOPER_ROOT= test.config | sed 's/.*=//')
MAC_DESTDIR=$(grep ^MAC_DESTDIR= test.config | sed 's/.*=//')
export MD_APPLE_SDK_ROOT="$(dirname "$(dirname "$XCODE_DEVELOPER_ROOT")")"
export XAMMAC_FRAMEWORK_PATH=$MAC_DESTDIR/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XamarinMacFrameworkRoot=$MAC_DESTDIR/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export TargetFrameworkFallbackSearchPaths=$MAC_DESTDIR/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$MAC_DESTDIR/Library/Frameworks/Mono.framework/External/xbuild

make
make .stamp-xharness-configure
../tools/xibuild/xibuild -- /r ../external/Touch.Unit/Touch.Client/macOS/mobile/Touch.Client-macOS-mobile.csproj
../tools/xibuild/xibuild -- /r ../external/Touch.Unit/Touch.Client/macOS/full/Touch.Client-macOS-full.csproj
../tools/xibuild/xibuild -- /r bindings-test/macOS/bindings-test.csproj

make -C bindings-test/dotnet/macOS build
make -C bindings-test/dotnet/MacCatalyst build

TEST_SUITES+=(build-dontlink)
TEST_SUITES+=(build-linksdk)
TEST_SUITES+=(build-linkall)
TEST_SUITES+=(build-introspection)
TEST_SUITES+=(build-xammac_tests)
TEST_SUITES+=(build-monotouch-test)
TEST_SUITES+=(build-introspection)

make -f packaged-macos-tests.mk "${TEST_SUITES[@]}" -j

for app in */bin/x86/*/*.app linker/mac/*/bin/x86/*/*.app linker/mac/*/generated-projects/*/bin/x86/*/*.app introspection/Mac/bin/x86/*/*.app; do
	mkdir -p "$DIR/tests/$app"
	$CP -R "$app" "$DIR/tests/$app/.."
done

for app in linker/*/*/dotnet/*/bin/*/*/*/*.app */dotnet/*/bin/*/*/*/*.app; do
	mkdir -p "$DIR/tests/$app"
	$CP -R "$app" "$DIR/tests/$app/.."
done

$CP -p packaged-macos-tests.mk "$DIR/tests"
$CP -p run-with-timeout.sh "$DIR/tests"
$CP -p ../Make.config "$DIR"
$CP -p ../Make.versions "$DIR"
$CP -p test-dependencies.sh "$DIR"
$CP -p ../system-dependencies.sh "$DIR"
mkdir -p "$DIR/mk"
$CP -p ../Make.config "$DIR"
$CP -p ../mk/subdirs.mk "$DIR/mk"
$CP -p ../mk/rules.mk "$DIR/mk"
$CP -p ../mk/quiet.mk "$DIR/mk"
$CP -p ../mk/mono.mk "$DIR/mk"

cd mac-test-package && 7z a ../mac-test-package.7z ./*
