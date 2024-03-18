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
INCLUDE_XAMARIN_LEGACY=$(grep ^INCLUDE_XAMARIN_LEGACY= test.config | sed 's/.*=//')
ENABLE_DOTNET=$(grep ^ENABLE_DOTNET= test.config | sed 's/.*=//')
INCLUDE_MAC=$(grep ^INCLUDE_MAC= test.config | sed 's/.*=//')
INCLUDE_MACCATALYST=$(grep ^INCLUDE_MACCATALYST= test.config | sed 's/.*=//')
XCODE_DEVELOPER_ROOT=$(grep ^XCODE_DEVELOPER_ROOT= test.config | sed 's/.*=//')
MAC_DESTDIR=$(grep ^MAC_DESTDIR= test.config | sed 's/.*=//')
export MD_APPLE_SDK_ROOT="$(dirname "$(dirname "$XCODE_DEVELOPER_ROOT")")"
export XAMMAC_FRAMEWORK_PATH=$MAC_DESTDIR/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XamarinMacFrameworkRoot=$MAC_DESTDIR/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export TargetFrameworkFallbackSearchPaths=$MAC_DESTDIR/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$MAC_DESTDIR/Library/Frameworks/Mono.framework/External/xbuild
export RootTestsDirectory="$(pwd)"

make
make .stamp-xharness-configure
if test -n "$INCLUDE_XAMARIN_LEGACY"; then
	../tools/xibuild/xibuild -- /r ../external/Touch.Unit/Touch.Client/macOS/mobile/Touch.Client-macOS-mobile.csproj
	../tools/xibuild/xibuild -- /r ../external/Touch.Unit/Touch.Client/macOS/full/Touch.Client-macOS-full.csproj
	../tools/xibuild/xibuild -- /r bindings-test/macOS/bindings-test.csproj
fi

TEST_SUITE_DEPENDENCIES+=(bindings-test)
TEST_SUITE_DEPENDENCIES+=(EmbeddedResources)
TEST_SUITE_DEPENDENCIES+=(fsharplibrary)
TEST_SUITE_DEPENDENCIES+=(BundledResources)

if test -n "$ENABLE_DOTNET"; then
	for dep in "${TEST_SUITE_DEPENDENCIES[@]}"; do
		if test -n "$INCLUDE_MAC"; then
			make -C "$dep"/dotnet/macOS build
		fi
		if test -n "$INCLUDE_MACCATALYST"; then
			make -C "$dep"/dotnet/MacCatalyst build
		fi
	done
fi

TEST_SUITES+=(build-dontlink)
TEST_SUITES+=(build-linksdk)
TEST_SUITES+=(build-linkall)
TEST_SUITES+=(build-introspection)
if test -n "$INCLUDE_XAMARIN_LEGACY"; then
	TEST_SUITES+=(build-xammac_tests)
fi
TEST_SUITES+=(build-monotouch-test)

# Don't build in parallel in CI, it fails randomly due to trying to write to the same files.
if test -z "$BUILD_REVISION"; then
	MAKE_FLAGS=-j
fi

make -f packaged-macos-tests.mk "${TEST_SUITES[@]}" $MAKE_FLAGS

if test -n "$INCLUDE_XAMARIN_LEGACY"; then
	for app in */bin/x86/*/*.app linker/mac/*/bin/x86/*/*.app linker/mac/*/generated-projects/*/bin/x86/*/*.app introspection/Mac/bin/x86/*/*.app; do
		mkdir -p "$DIR/tests/$app"
		$CP -R "$app" "$DIR/tests/$app/.."
	done
fi

if test -n "$ENABLE_DOTNET"; then
	for app in linker/*/*/dotnet/*/bin/*/*/*/*.app */dotnet/*/bin/*/*/*/*.app; do
		mkdir -p "$DIR/tests/$app"
		$CP -R "$app" "$DIR/tests/$app/.."
	done
fi

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
