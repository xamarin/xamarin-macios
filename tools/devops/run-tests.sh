#!/bin/bash -ex


if [[ "$SYSTEM_DEBUG" == "true" ]]; then
  DEBUG="-d"
else
  DEBUG=""
fi
make -C $DEBUG xamarin-macios/builds download -j
make -C $DEBUG xamarin-macios/builds .stamp-mono-ios-sdk-destdir -j
make -C $DEBUG xamarin-macios/tests vsts-device-tests
