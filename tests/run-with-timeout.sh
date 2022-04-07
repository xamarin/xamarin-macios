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
	kill -s SIGKILL $PID
	echo "The command '$*' timed out after $TIMEOUT second(s)."
) 2> /dev/null &

exec "$@"
