#!/bin/bash -eux

set -o pipefail

if test -z "$TOP"; then echo "TOP not set"; exit 1; fi
if test -z "$MACOS_DOTNET_DESTDIR"; then echo "MACOS_DOTNET_DESTDIR not set"; exit 1; fi
if test -z "$IOS_DOTNET_DESTDIR"; then echo "IOS_DOTNET_DESTDIR not set"; exit 1; fi
if test -z "$TVOS_DOTNET_DESTDIR"; then echo "TVOS_DOTNET_DESTDIR not set"; exit 1; fi
if test -z "$WATCHOS_DOTNET_DESTDIR"; then echo "WATCHOS_DOTNET_DESTDIR not set"; exit 1; fi
if test -z "$MAC_DESTDIR"; then echo "MAC_DESTDIR not set"; exit 1; fi
if test -z "$IOS_DESTDIR"; then echo "IOS_DESTDIR not set"; exit 1; fi
if test -z "$MAC_FRAMEWORK_DIR"; then echo "MAC_FRAMEWORK_DIR not set"; exit 1; fi
if test -z "$MONOTOUCH_PREFIX"; then echo "MONOTOUCH_PREFIX not set"; exit 1; fi
if test -z "$DOTNET_IOS_SDK_DESTDIR"; then echo "DOTNET_IOS_SDK_DESTDIR not set"; exit 1; fi

cp="cp -c"

copy_files ()
{
	local dotnet_destdir=$1
	local destdir=$2
	local platform=$3
	#shellcheck disable=SC2155
	local platform_lower=$(echo "$platform" | tr '[:upper:]' '[:lower:]')
	local arches_64=$4
	local arches_32=$5
	local arches="$arches_64 $arches_32"
	local assembly_infix=$6
	local framework=$7

	rm -Rf "$dotnet_destdir"

	mkdir -p "$dotnet_destdir"
	mkdir -p "$dotnet_destdir/lib/$framework"
	mkdir -p "$dotnet_destdir/lib/Xamarin.$assembly_infix/v1.0/RedistList"
	mkdir -p "$dotnet_destdir/runtimes"
	for arch in $arches; do
		mkdir -p "$dotnet_destdir"/runtimes/"$platform_lower"-"$arch"/lib/netstandard1.0
	done
	mkdir -p "$dotnet_destdir/targets"
	mkdir -p "$dotnet_destdir/tools"
	mkdir -p "$dotnet_destdir/Sdk"
	mkdir -p "$dotnet_destdir/tools"
	mkdir -p "$dotnet_destdir/tools/bin"
	for arch in $arches; do
		mkdir -p "$dotnet_destdir/tools/bin/$arch"
	done
	mkdir -p "$dotnet_destdir/tools/include"
	mkdir -p "$dotnet_destdir/tools/lib"

	$cp "$destdir/Version" "$dotnet_destdir/"
	$cp "$destdir/buildinfo" "$dotnet_destdir/tools/"

	$cp "$TOP/msbuild/dotnet5/Xamarin.$platform.Sdk/Sdk/"* "$dotnet_destdir/Sdk/"
	$cp "$TOP/msbuild/dotnet5/targets/"* "$dotnet_destdir/targets/"
	$cp "$TOP/msbuild/dotnet5/Xamarin.$platform.Sdk/targets/"* "$dotnet_destdir/targets/"

	$cp -r "$destdir/lib/msbuild" "$dotnet_destdir/tools/"

	for arch in $arches; do
		case $arch in
		arm | armv7 | armv7s | armv7k | arm64_32 | x86)
			bitness=32
			;;
		arm64 | x64)
			bitness=64
			;;
		*)
			echo "Unknown arch: $arch"
			exit 1
			;;
		esac
		mkdir -p "$dotnet_destdir/runtimes/$platform_lower-$arch/lib/$framework"
		$cp "$TOP/src/build/dotnet/$platform_lower/$bitness/Xamarin.$assembly_infix.dll" "$dotnet_destdir/runtimes/$platform_lower-$arch/lib/$framework/"
		$cp "$TOP/src/build/dotnet/$platform_lower/$bitness/Xamarin.$assembly_infix.pdb" "$dotnet_destdir/runtimes/$platform_lower-$arch/lib/$framework/"
	done

	cp "$TOP/src/build/dotnet/$platform_lower/Xamarin.$assembly_infix.dll" "$dotnet_destdir/lib/Xamarin.$assembly_infix/v1.0/"
	# cp "$TOP/src/build/dotnet/$platform_lower/Xamarin.$assembly_infix.pdb" "$dotnet_destdir/lib/Xamarin.$assembly_infix/v1.0/"

	if [[ "$platform" == "iOS" ]]; then
		for arch in arm64 arm x64; do
			# FIXME: pending x86
			$cp "$DOTNET_IOS_SDK_DESTDIR/debug/netcoreapp5.0-$platform-Debug-$arch/"* "$dotnet_destdir/runtimes/$platform_lower-$arch/lib/$framework/"
		done

		for dir in "$dotnet_destdir"/runtimes/"$platform_lower"-*/lib/"$framework/"; do
			(
				cd "$dir"
				for lib in *.dylib; do
					if [[ "$lib" == "*.dylib" ]]; then
						break;
					fi
					install_name_tool -id "@executable_path/$lib" "$lib"
				done
			)
		done
	fi

	$cp "$DOTNET_BCL_DIR/"* "$dotnet_destdir/lib/Xamarin.$assembly_infix/v1.0/"

	$cp "$destdir/lib/mono/Xamarin.$assembly_infix/RedistList/FrameworkList.xml" "$dotnet_destdir/lib/Xamarin.$assembly_infix/v1.0/RedistList/"

	# <!-- simlauncher -->
	# <Content Include="$(_iOSCurrentPath)\bin\simlauncher-*" Condition=" '$(_PlatformName)' == 'iOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	if [[ "$platform" != "macOS" ]]; then
		$cp "$destdir/bin/simlauncher"* "$dotnet_destdir/tools/bin/"
	fi

	# <!-- generator -->
	# <Content Include="$(_iOSCurrentPath)\bin\bgen">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\lib\bgen\**">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\lib\bgen</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\bin\btouch" Condition=" '$(_PlatformName)' == 'iOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\bin\btv" Condition=" '$(_PlatformName)' == 'tvOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\bin\bwatch" Condition=" '$(_PlatformName)' == 'watchOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	$cp "$destdir/bin/bgen" "$dotnet_destdir/tools/bin/"
	$cp -r "$destdir/lib/bgen" "$dotnet_destdir/tools/lib/"
	if [[ "$platform" == "iOS" ]]; then
		$cp "$destdir/bin/btouch" "$dotnet_destdir/tools/bin/"
	fi
	if [[ "$platform" == "tvOS" ]]; then
		$cp "$destdir/bin/btv" "$dotnet_destdir/tools/bin/"
	fi
	if [[ "$platform" == "watchOS" ]]; then
		$cp "$destdir/bin/bwatch" "$dotnet_destdir/tools/bin/"
	fi
	if [[ "$platform" == "macOS" ]]; then
		$cp "$destdir/bin/bmac" "$dotnet_destdir/tools/bin/"
	fi

	# <!-- mtouch -->
	# <Content Include="$(_iOSCurrentPath)\bin\mtouch" Condition=" '$(_PlatformName)' != 'macOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\lib\mtouch\**" Condition=" '$(_PlatformName)' != 'macOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\lib\mtouch</PackagePath>
	# </Content>
	if [[ "$platform" != "macOS" ]]; then
		$cp "$destdir/bin/mtouch"* "$dotnet_destdir/tools/bin/"
		$cp -r "$destdir/lib/mtouch" "$dotnet_destdir/tools/lib/"
	fi

	# <!-- mmp -->
	# <Content Include="$(_macOSCurrentPath)\bin\mmp" Condition=" '$(_PlatformName)' == 'macOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_macOSCurrentPath)\lib\mmp\**" Condition=" '$(_PlatformName)' == 'macOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\lib\mmp</PackagePath>
	# </Content>
	if [[ "$platform" == "macOS" ]]; then
		$cp "$destdir/bin/mmp"* "$dotnet_destdir/tools/bin/"
		$cp -r "$destdir/lib/mmp" "$dotnet_destdir/tools/lib/"
	fi
	# <!-- mlaunch -->
	# <Content Include="$(_iOSCurrentPath)\bin\mlaunch" Condition=" '$(_PlatformName)' != 'macOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\lib\mlaunch\**" Condition=" '$(_PlatformName)' != 'macOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\lib\mlaunch</PackagePath>
	# </Content>
   	if [[ "$platform" != "macOS" ]]; then
		$cp "$destdir/bin/mlaunch"* "$dotnet_destdir/tools/bin/"
		$cp -r "$destdir/lib/mlaunch" "$dotnet_destdir/tools/lib/"
	fi

	# <!-- AOT compilers -->
	# <Content Include="$(_iOSCurrentPath)\bin\arm64-darwin-mono-sgen" Condition=" '$(_PlatformName)' == 'iOS' Or '$(_PlatformName)' == 'tvOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\bin\arm-darwin-mono-sgen" Condition=" '$(_PlatformName)' == 'iOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\bin\arm64_32-darwin-mono-sgen" Condition=" '$(_PlatformName)' == 'watchOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\bin\armv7k-unknown-darwin-mono-sgen" Condition=" '$(_PlatformName)' == 'watchOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>$(_BinDir)</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\LLVM\bin\llc;$(_iOSCurrentPath)\LLVM\bin\opt">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\LLVM\bin\</PackagePath>
	# </Content>

	# <!-- SDK frameworks -->
	# <Content Include="$(_iOSCurrentPath)\SDKs\MonoTouch.iphonesimulator.sdk\**" Condition=" '$(_PlatformName)' == 'iOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\SDKS\MonoTouch.iphonesimulator.sdk</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\SDKs\MonoTouch.iphoneos.sdk\**" Condition=" '$(_PlatformName)' == 'iOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\SDKS\MonoTouch.iphoneos.sdk</PackagePath>
	# </Content>

	if [[ "$platform" == "iOS" ]]; then
		for arch in $arches; do
		case $arch in
			arm | armv7 | armv7s | armv7k | arm64_32 | arm64)
				platform_infix=iphoneos
				;;
			x86 | x64)
				platform_infix=iphonesimulator
				;;
			*)
				echo "Unknown arch: $arch"
				exit 1
				;;
			esac
			mkdir -p "$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/Xamarin.$platform.registrar.a"	"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/libapp.a"						"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/libextension.a"				"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/libtvextension.a"				"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/libwatchextension.a"			"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/libxamarin-debug.a"			"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/libxamarin-debug.dylib"		"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/libxamarin.a"					"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
			$cp "$destdir/SDKs/MonoTouch.$platform_infix.sdk/usr/lib/libxamarin.dylib"				"$dotnet_destdir/runtimes/$platform_lower-$arch/native/"

			$cp -r "$destdir/SDKs/MonoTouch.$platform_infix.sdk/Frameworks"/Xamarin*.framework*	 "$dotnet_destdir/runtimes/$platform_lower-$arch/native/"
		done

		# FIXME: missing x86_64
		for arch in x64 arm arm64 x64; do
			unzip -oj -d "$dotnet_destdir/runtimes/$platform_lower-$arch/native/"			"$DOTNET_IOS_SDK_DESTDIR"/debug/runtime."$platform_lower".'*'-"$arch".Microsoft.NETCore.Runtime.Mono.5.0.0-dev.nupkg runtimes/"$platform_lower".'*'-"$arch"/native/'libmono.*'
			unzip -oj -d "$dotnet_destdir/runtimes/$platform_lower-$arch/lib/$framework/"	"$DOTNET_IOS_SDK_DESTDIR"/debug/runtime."$platform_lower".'*'-"$arch".Microsoft.NETCore.Runtime.Mono.5.0.0-dev.nupkg runtimes/"$platform_lower".'*'-"$arch"/lib/netstandard1.0/'*.dll'
		done

		unzip -oj -d "$dotnet_destdir/tools/bin/" "$DOTNET_IOS_SDK_DESTDIR/debug/runtime.ios.7-arm64.Microsoft.NETCore.Tool.MonoAOT.5.0.0-dev.nupkg" 'tools/mono-aot-cross'
		mv "$dotnet_destdir/tools/bin/mono-aot-cross" "$dotnet_destdir/tools/bin/arm64-darwin-mono-sgen"
		chmod +x "$dotnet_destdir/tools/bin/arm64-darwin-mono-sgen"

		unzip -oj -d "$dotnet_destdir/tools/bin/"   "$DOTNET_IOS_SDK_DESTDIR/debug/runtime.ios.7-arm.Microsoft.NETCore.Tool.MonoAOT.5.0.0-dev.nupkg"   'tools/mono-aot-cross'
		mv "$dotnet_destdir/tools/bin/mono-aot-cross" "$dotnet_destdir/tools/bin/arm-darwin-mono-sgen"
		chmod +x "$dotnet_destdir/tools/bin/arm-darwin-mono-sgen"

		$cp -r "$TOP"/runtime/xamarin "$dotnet_destdir/tools/include/"
		rm -f "$dotnet_destdir/tools/include/launch.h" # this file is macOS only
	fi


	# <Content Include="$(_iOSCurrentPath)\SDKs\Xamarin.AppleTVSimulator.sdk\**" Condition=" '$(_PlatformName)' == 'tvOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\SDKS\Xamarin.AppleTVSimulator.sdk</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\SDKs\Xamarin.AppleTVOS.sdk\**" Condition=" '$(_PlatformName)' == 'tvOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\SDKS\Xamarin.AppleTVOS.sdk</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\SDKs\Xamarin.WatchSimulator.sdk\**" Condition=" '$(_PlatformName)' == 'watchOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\SDKS\Xamarin.WachSimulator.sdk</PackagePath>
	# </Content>
	# <Content Include="$(_iOSCurrentPath)\SDKs\Xamarin.WatchOS.sdk\**" Condition=" '$(_PlatformName)' == 'watchOS'">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\SDKS\Xamarin.WatchOS.sdk</PackagePath>
	# </Content>
	# <Content Include="$(_macOSCurrentPath)\lib\*.dylib;$(_macOSCurrentPath)\lib\*.a" Condition=" '$(_PlatformName)' == 'macOS' ">
	#   <Pack>true</Pack>
	#   <PackagePath>tools\lib</PackagePath>
	# </Content>

	chmod -R +r "$dotnet_destdir"
}

copy_files "$MACOS_DOTNET_DESTDIR"   "$MAC_DESTDIR$MAC_FRAMEWORK_DIR/Versions/Current" macOS	"x64"	   ""					Mac	 xamarinmac10
copy_files "$IOS_DOTNET_DESTDIR"	 "$IOS_DESTDIR$MONOTOUCH_PREFIX"				   iOS	  "x64 arm64" "x86 arm"			 iOS	 xamarinios10
copy_files "$TVOS_DOTNET_DESTDIR"	"$IOS_DESTDIR$MONOTOUCH_PREFIX"				   tvOS	 "x64 arm64" ""					TVOS	xamarintvos10
copy_files "$WATCHOS_DOTNET_DESTDIR" "$IOS_DESTDIR$MONOTOUCH_PREFIX"				   watchOS  ""		  "x86 armv7k arm64_32" WatchOS xamarinwatchos10
