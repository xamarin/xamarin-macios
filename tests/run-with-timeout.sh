#!/bin/bash -eu

# This script takes a timeout interval (in seconds) as the first argument.
# The remaining arguments will be executed, and if it takes longer than the timeout,
# the process will be killed.

INTERVAL_SECONDS=1

TIMEOUT=$1
shift

PID=$$
(
	((t = TIMEOUT))

	while ((t > 0)); do
		# Sleep for a bit
		sleep $INTERVAL_SECONDS
		# Check if the process is still around
		if ! kill -0 $PID; then
			# The process is gone! Yay!
			exit 0
		fi
		# Process is still around, keep waiting
		((t -= INTERVAL_SECONDS))
	done

	# ⏰ - prepare to die

	# try to capture the screen
	mkdir -p screenshots-$PID
	screencapture -x screenshots-$PID/capture-$PID.png
	echo "##vso[artifact.upload containerfolder=screenshots;artifactname=capture-$PID.png]$PWD/screenshots-$PID/capture-$PID.png"

	# but first send a QUIT to get a managed stack trace dump
	echo "The command '$*' timed out after $TIMEOUT second(s). Sending SIGQUIT..."
	kill -s SIGQUIT $PID
	sleep 10 # printing the stack trace dump might take a few seconds

	# and then abort since that creates a crash report
	echo "The command '$*' timed out after $TIMEOUT second(s). Sending SIGABRT..."
	kill -s SIGABRT $PID

	# wait 10 seconds to see if the process really exited. Sometimes Mono will hang while processing SIGABRT
	echo "The command '$*' timed out after $TIMEOUT second(s). Waiting for process to exit...."
	C=1
	while kill -0 $PID; do
		sleep 0.1
		((C++))
		if [[ $C -ge 100 ]]; then
			echo "The command '$*' timed out after $TIMEOUT second(s). Sending SIGKILL..."
			kill -s SIGKILL $PID
			break
		fi
	done

	echo "The command '$*' timed out after $TIMEOUT second(s). The process has been killed."

) 2> /dev/null &

exec "$@"
