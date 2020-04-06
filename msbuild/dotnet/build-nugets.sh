#!/bin/bash -e

set -o pipefail

if test -n "$V"; then set +x; fi
if test -z "$TOP"; then echo "TOP not set"; exit 1; fi
if test -z "$DOTNET_DESTDIR"; then echo "DOTNET_DESTDIR not set"; exit 1; fi
if test -z "$MAC_DESTDIR"; then echo "MAC_DESTDIR not set"; exit 1; fi
if test -z "$IOS_DESTDIR"; then echo "IOS_DESTDIR not set"; exit 1; fi
if test -z "$MAC_FRAMEWORK_DIR"; then echo "MAC_FRAMEWORK_DIR not set"; exit 1; fi
if test -z "$MONOTOUCH_PREFIX"; then echo "MONOTOUCH_PREFIX not set"; exit 1; fi

cp="cp -c"

# the Xamarin.*OS.App.Ref nuget
create_ref_nuget ()
{
	local platform=$1
	local assembly_infix=$2
	#shellcheck disable=SC2155
	local platform_lower=$(echo "$platform" | tr '[:upper:]' '[:lower:]')
	local packageid=Xamarin.$platform.App.Ref
	local destdir=$DOTNET_DESTDIR/$packageid

	rm -Rf "$destdir"
	mkdir -p "$destdir/data"
	mkdir -p "$destdir/ref/netcoreapp5.0"

	$cp "$TOP/src/build/dotnet/$platform_lower/ref/Xamarin.$assembly_infix.dll" "$destdir/ref/netcoreapp5.0/"
	# FrameworkList.xml is generated
	#$cp "$TOP/msbuild/dotnet/package/$packageid/FrameworkList.xml" "$destdir/data/"

	chmod -R +r "$destdir"
}
create_ref_nuget "iOS"     "iOS"
create_ref_nuget "tvOS"    "TVOS"
create_ref_nuget "watchOS" "WatchOS"
create_ref_nuget "macOS"   "Mac"

# the Xamarin.*OS.App.Runtime.<RID> nugets
create_runtime_packs ()
{
	local platform=$1
	local assembly_infix=$2
	local arches=$3
	#shellcheck disable=SC2155
	local platform_lower=$(echo "$platform" | tr '[:upper:]' '[:lower:]')

	for arch in $arches; do
		local rid=$platform_lower-$arch
		local packageid=Xamarin.$platform.App.Runtime.$rid
		local destdir=$DOTNET_DESTDIR/$packageid

		rm -Rf "$destdir"
		mkdir -p "$destdir/data"
		mkdir -p "$destdir/runtimes/$rid/lib/netcoreapp5.0"

		local bitsize
		case $arch in
			arm64|x64)
				bitsize=64
				;;
			arm|x86)
				bitsize=32
				;;
			*)
				echo "Unknown arch: $arch"
				exit 1
				;;
		esac

		$cp "$TOP/src/build/dotnet/$platform_lower/$bitsize/Xamarin.$assembly_infix.dll" "$destdir/runtimes/$rid/lib/netcoreapp5.0"
		# RuntimeList.xml is generated
		#$cp "$TOP/msbuild/dotnet/package/$packageid/RuntimeList.xml" "$destdir/data/"

		chmod -R +r "$destdir"
	done
}
create_runtime_packs  "iOS"     "iOS"     "arm64 arm x64 x86"
create_runtime_packs  "tvOS"    "TVOS"    "arm64 x64"
create_runtime_packs  "watchOS" "WatchOS" "arm x86"
create_runtime_packs  "macOS"   "Mac"     "x64"

copy_ios_native_libs_to_runtime_pack ()
{
	local platform=$1
	local sdk=$2
	local fat=$3
	local rid_family=$4
	local architectures=$5
	#shellcheck disable=SC2155
	local platform_lower=$(echo "$platform" | tr '[:upper:]' '[:lower:]')
	local rid=$platform_lower-$rid_family
	local packageid=Xamarin.$platform.App.Runtime.$rid
	local destdir=$DOTNET_DESTDIR/$packageid/runtimes/$rid/native
	local sdk_dir="$TOP/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/SDKs/$sdk.sdk"
	local lib_dir="$sdk_dir/usr/lib/"
	local include_dir="$sdk_dir/usr/include/"

	mkdir -p "$destdir"

	local thin=()
	for arch in $architectures; do
		thin+=(-extract_family "$arch")
	done

	local inputs=("$lib_dir"/libapp.a)
	inputs+=("$lib_dir"/libextension.a)
	inputs+=("$lib_dir"/libtvextension.a)
	inputs+=("$lib_dir"/libwatchextension.a)
	inputs+=("$lib_dir"/libxamarin*)
	inputs+=("$lib_dir"/*registrar.a)
	for element in "${inputs[@]}"; do
		if [[ x$fat == x1 ]]; then
			lipo "$element" "${thin[@]}" -output "$destdir/$(basename "$element")"
		else
			$cp "$element" "$destdir"
		fi
	done

	mkdir -p "$destdir/Frameworks"
	local frameworks=()
	frameworks+=("$sdk_dir"/Frameworks/Xamarin.framework "$sdk_dir"/Frameworks/Xamarin-debug.framework)
	for element in "${frameworks[@]}"; do
		local fw_name
		fw_name=$(basename "$element" .framework)
		$cp -r "$element" "$destdir/Frameworks/"
		if [[ x$fat == x1 ]]; then
			lipo "$element/$fw_name" "${thin[@]}" -output "$destdir/Frameworks/$fw_name.framework/$fw_name"
		fi
	done

	$cp "$TOP"/tools/mtouch/simlauncher.mm "$destdir"

	$cp -r "$include_dir/xamarin" "$destdir/"
}
copy_ios_native_libs_to_runtime_pack "iOS"     "MonoTouch.iphoneos"        1 "arm64" "arm64"
copy_ios_native_libs_to_runtime_pack "iOS"     "MonoTouch.iphoneos"        1 "arm"   "armv7 armv7s"
copy_ios_native_libs_to_runtime_pack "iOS"     "MonoTouch.iphonesimulator" 1 "x64"   "x86_64"
copy_ios_native_libs_to_runtime_pack "iOS"     "MonoTouch.iphonesimulator" 1 "x86"   "i386"
copy_ios_native_libs_to_runtime_pack "tvOS"    "Xamarin.AppleTVOS"         0 "arm64" "arm64"
copy_ios_native_libs_to_runtime_pack "tvOS"    "Xamarin.AppleTVSimulator"  0 "x64"   "x86_64"
copy_ios_native_libs_to_runtime_pack "watchOS" "Xamarin.WatchOS"           0 "arm"   "armv7k arm64_32"
copy_ios_native_libs_to_runtime_pack "watchOS" "Xamarin.WatchSimulator"    0 "x86"   "i386"

# the Xamarin.*OS.Sdk nugets
create_sdk_nugets ()
{
	local platform=$1
	local legacy_destdir=$2
	#shellcheck disable=SC2155
	local platform_lower=$(echo "$platform" | tr '[:upper:]' '[:lower:]')
	local packageid=Xamarin.$platform.Sdk
	local destdir=$DOTNET_DESTDIR/$packageid

	rm -Rf "$destdir"
	mkdir -p "$destdir/tools"
	mkdir -p "$destdir/targets"
	mkdir -p "$destdir/Sdk"

	$cp "$legacy_destdir/Version" "$destdir/"
	$cp "$legacy_destdir/buildinfo" "$destdir/tools/"

	$cp "$TOP/msbuild/dotnet/Xamarin.$platform.Sdk/Sdk/"* "$destdir/Sdk/"
	$cp "$TOP/msbuild/dotnet/targets/"* "$destdir/targets/"
	$cp "$TOP/msbuild/dotnet/Xamarin.$platform.Sdk/targets/"* "$destdir/targets/"

	$cp -r "$legacy_destdir/lib/msbuild" "$destdir/tools/"

	# linker assembly
	$cp -r "$TOP/tools/dotnet-linker/bin/Debug/netcoreapp3.0" "$destdir/tools/dotnet-linker"

	chmod -R +r "$destdir"
}
create_sdk_nugets  "iOS"     "$IOS_DESTDIR$MONOTOUCH_PREFIX"
create_sdk_nugets  "tvOS"    "$MAC_DESTDIR$MAC_FRAMEWORK_DIR/Versions/Current"
create_sdk_nugets  "watchOS" "$IOS_DESTDIR$MONOTOUCH_PREFIX"
create_sdk_nugets  "macOS"   "$IOS_DESTDIR$MONOTOUCH_PREFIX"
