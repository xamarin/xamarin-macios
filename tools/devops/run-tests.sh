#!/bin/bash -ex

make -C xamarin-macios/builds download -j
make -C xamarin-macios/builds .stamp-mono-ios-sdk-destdir -j
make -C xamarin-macios/tests vsts-device-tests
