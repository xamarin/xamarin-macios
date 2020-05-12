#!/bin/bash -x

# we do not want errors to fail the script, we want to print as much info as possible, so we don't pass -e to bash

ls -l
pwd
ls -l xamarin-macios/
ls -l
if [[ "$SYSTEM_DEBUG" == "true" ]]; then
  DEBUG="-d"
else
  DEBUG=""
fi
#make $DEBUG -C xamarin-macios/builds download -j
#make $DEBUG -C xamarin-macios/builds .stamp-mono-ios-sdk-destdir -j
#make $DEBUG -C xamarin-macios/tests vsts-device-tests
echo "exit 100" > bar.sh
chmod a+x bar.sh
./bar.sh
