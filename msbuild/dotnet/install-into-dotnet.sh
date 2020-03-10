#!/bin/bash -eux

if test -z "${SUDO:-}"; then
	# If we're installing into the default dotnet path, we'll need sudo.
	if test -z "${DOTNET_PATH:-}"; then
		SUDO=sudo
	else
		SUDO=
	fi
fi

if test -z "${DOTNET_PATH:-}"; then
	DOTNET_VERSION="$(dotnet --version)"
	DOTNET_PATH=/usr/local/share/dotnet/sdk/$DOTNET_VERSION/Sdks
fi

IOS_ROOTDIR=${IOS_TARGETDIR:-}/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/dotnet
MAC_ROOTDIR=${MAC_TARGETDIR:-}/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/dotnet

IOS_ROOTDIR=${DOTNET_DESTDIR:-}
MAC_ROOTDIR=${DOTNET_DESTDIR:-}

$SUDO ln -Fhs "$IOS_ROOTDIR/Xamarin.iOS.Sdk" "$DOTNET_PATH/Xamarin.iOS.Sdk"
$SUDO ln -Fhs "$IOS_ROOTDIR/Xamarin.tvOS.Sdk" "$DOTNET_PATH/Xamarin.tvOS.Sdk"
$SUDO ln -Fhs "$IOS_ROOTDIR/Xamarin.watchOS.Sdk" "$DOTNET_PATH/Xamarin.watchOS.Sdk"
$SUDO ln -Fhs "$MAC_ROOTDIR/Xamarin.macOS.Sdk" "$DOTNET_PATH/Xamarin.macOS.Sdk"
