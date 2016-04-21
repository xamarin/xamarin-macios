#!/bin/bash -e

START=$(date -u +"%s")

# first argument is directory
D=$1
shift

if test -f $D.config.cache; then
	HAS_CACHE=1
fi

mkdir -p $D
echo Configuring $D
cd $D

if ! "$@" > .stamp-configure-$D.log 2>&1; then
	FAILED=1
	if [[ x"$HAS_CACHE" == "x1" ]]; then
		echo Configuring $D failed, but a cache was used. Will try to configure without the cache.
		rm ../$D.config.cache
		if ! "$@" > .stamp-configure-$D.log 2>&1; then
			echo Configuring $D failed without cache as well.
		else
			FAILED=
		fi
	fi

	if [[ x"$FAILED" == "x1" ]]; then
		echo Configuring $D failed:
		cat .stamp-configure-$D.log | sed "s/^/    /"
		echo
		echo "    *** config.log *** "
		echo
		cat config.log | sed "s/^/    /"
		exit 1
	fi
fi
cd ..
touch .stamp-configure-$D

END=$(date -u +"%s")
DIFF=$(($END-$START))

echo Configured $D in $(($DIFF/60))m $(($DIFF%60))s
