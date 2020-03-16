#!/bin/bash -ex

ls -l
pwd
ls -l xamarin-macios/
ls -l
if [[ "$SYSTEM_DEBUG" == "true" ]]; then
  DEBUG="-d"
else
  DEBUG=""
fi
make $DEBUG -C xamarin-macios/builds download -j
make $DEBUG -C xamarin-macios/builds .stamp-mono-ios-sdk-destdir -j
#make $DEBUG -C xamarin-macios/tests vsts-device-tests
echo "exit 1" > bar.sh
chmod a+x bar.sh
./bar.sh

if [ $? -eq 0 ]; then
  echo "hello"
else
  echo "##vso[task.setvariable variable=TESTSTATE]failure"
fi
