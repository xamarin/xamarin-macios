#!/bin/bash -e

cd $WORKSPACE
export BUILD_REVISION=jenkins
make -j8 -C tools/apidiff jenkins-api-diff
