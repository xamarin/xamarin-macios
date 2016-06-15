#!/bin/bash -e

./configure --disable-ios-device
time make world

make -j8 -C tools/apidiff jenkins-api-diff
make -C tests jenkins || true
