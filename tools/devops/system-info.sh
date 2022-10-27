#!/bin/bash -x

# we do not want errors to fail the script, we want to print as much info as possible, so we don't pass -e to bash

uname -a
ls -la /Library/Frameworks/Xamarin.iOS.framework/Versions
ls -la /Library/Frameworks/Xamarin.Mac.framework/Versions
ls -la /Library/Frameworks/ObjectiveSharpie.framework/Versions/
ls -lad /Applications/Xcode*
xcode-select -p
mono --version
env | sort
uptime
ps aux

exit 0
