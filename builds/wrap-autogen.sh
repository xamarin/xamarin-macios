#!/bin/bash -e

START=$(date -u +"%s")

# first argument is directory
D=$1
shift

# second argument is a name
NAME=$1
shift

echo Running autogen for $D
ORIGINAL_CWD=$(pwd)
LOG_FILE=$ORIGINAL_CWD/.stamp-autogen-$NAME.log
cd $D

if ! git submodule update -r > $LOG_FILE 2>&1; then
	echo "Autogen (git submodule update) for $D failed:"
	cat $LOG_FILE | sed "s/^/    /"
	exit 1
fi

if ! NOCONFIGURE=1 ./autogen.sh "$@" > $LOG_FILE 2>&1; then
	echo Autogen for $D failed:
	cat $LOG_FILE | sed "s/^/    /"
	exit 1
fi

END=$(date -u +"%s")
DIFF=$(($END-$START))

echo Ran autogen for $D in $(($DIFF/60))m $(($DIFF%60))s
