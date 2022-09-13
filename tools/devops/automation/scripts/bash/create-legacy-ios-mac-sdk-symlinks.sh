#!/bin/bash -ex

include_xamarin_legacy=$(make -C tools/devops print-variable VARIABLE=INCLUDE_XAMARIN_LEGACY)
include_xamarin_legacy=${include_xamarin_legacy#*=}
if test -z "$include_xamarin_legacy"; then
	echo "Not creating symlinks because legaxy Xamarin is disabled."
	exit
fi

if test -d /Library/Frameworks/Xamarin.Mac.framework/Versions/Current; then
  mac_var=$(make -C tools/devops print-variable VARIABLE=MAC_DESTDIR MAC_DESTDIR=)
  MAC_DESTDIR=${mac_var#*=}
  XM_DEST_DIR="$MAC_DESTDIR/Library/Frameworks/Xamarin.Mac.framework/Versions/"
  mkdir -p "$XM_DEST_DIR"
  ln -s /Library/Frameworks/Xamarin.Mac.framework/Versions/Current "$XM_DEST_DIR/git"
  ls -laR "$XM_DEST_DIR"
fi

if test -d /Library/Frameworks/Xamarin.iOS.framework/Versions/Current; then
  ios_var=$(make -C tools/devops print-variable VARIABLE=IOS_DESTDIR IOS_DESTDIR=)
  IOS_DESTDIR=${ios_var#*=}
  XI_DEST_DIR="$IOS_DESTDIR/Library/Frameworks/Xamarin.iOS.framework/Versions/"
  mkdir -p "$XI_DEST_DIR"
  ln -s /Library/Frameworks/Xamarin.iOS.framework/Versions/Current "$XI_DEST_DIR/git"
  ls -laR "$XI_DEST_DIR"
fi
