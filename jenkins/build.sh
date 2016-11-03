#!/bin/bash -e

COMMENT_FILE=$WORKSPACE/jenkins-results/comments

cd $WORKSPACE
export BUILD_REVISION=jenkins
./configure --disable-ios-device

mkdir -p $(dirname $COMMENT_FILE)

if time make world; then
	RV=$?
	echo "Build succeeded" > $COMMENT_FILE
else
	RV=$?
	echo "Build failed" > $COMMENT_FILE
fi

exit $RV