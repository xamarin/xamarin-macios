#!/bin/bash -ex

XCODE_SELECT=$(xcode-select -p)
if [[ -d $XCODE_SELECT ]]; then
  echo "Using Xcode in path $XCODE_SELECT"
else
  echo "Setting Xcode to point to the default location."
  xcode-select -s "/Applications/Xcode.app/Contents/Developer"
fi
