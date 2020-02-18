#!/bin/bash -ex


if [[ "$SYSTEM_DEBUG" == "true" ]]; then
  DEBUG="-d"
else
  DEBUG=""
fi
make $DEBUG -C xamarin-macios/builds download -j
make $DEBUG -C xamarin-macios/builds .stamp-mono-ios-sdk-destdir -j
make $DEBUG -C xamarin-macios/tests vsts-device-tests
