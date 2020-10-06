#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")"

# Clone files instead of copying them on APFS file systems. Much faster.
CP="cp"
if df -t apfs / >/dev/null 2>&1; then
	CP="cp -c"
fi

#git clean -xfdq

DIR=$(pwd)/mac-test-package/mac-test-package
ZIP=$DIR.7z
rm -Rf $DIR
mkdir -p $DIR

make test.config
source test.config
export MD_APPLE_SDK_ROOT=$(dirname $(dirname $XCODE_DEVELOPER_ROOT))
export XAMMAC_FRAMEWORK_PATH=$MAC_DESTDIR/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XamarinMacFrameworkRoot=$MAC_DESTDIR/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export TargetFrameworkFallbackSearchPaths=$MAC_DESTDIR/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$MAC_DESTDIR/Library/Frameworks/Mono.framework/External/xbuild

make
make .stamp-configure-projects-mac
../tools/xibuild/xibuild -- /r ../external/Touch.Unit/Touch.Client/macOS/mobile/Touch.Client-macOS-mobile.csproj
../tools/xibuild/xibuild -- /r ../external/Touch.Unit/Touch.Client/macOS/full/Touch.Client-macOS-full.csproj
../tools/xibuild/xibuild -- /r bindings-test/macOS/bindings-test.csproj
make build-mac-dontlink build-mac-apitest build-mac-introspection build-mac-linksdk build-mac-linkall build-mac-xammac_tests build-mac-system-dontlink -j8

for app in */bin/x86/*/*.app */generated-projects/*/bin/x86/*/*.app linker/mac/*/bin/x86/*/*.app linker/mac/*/generated-projects/*/bin/x86/*/*.app introspection/Mac/bin/x86/*/*.app; do
	mkdir -p "$DIR/tests/$app"
	$CP -R "$app" "$DIR/tests/$app/.."
done

$CP -p Makefile-mac.inc $DIR/tests
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
$CP -p ../mk/mono.mk "$DIR/mk"

cd mac-test-package && 7z a ../mac-test-package.7z *
