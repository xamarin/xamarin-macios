#!/bin/bash -e
cd $WORKSPACE
export BUILD_REVISION=jenkins
./configure --disable-ios-device
time make world
