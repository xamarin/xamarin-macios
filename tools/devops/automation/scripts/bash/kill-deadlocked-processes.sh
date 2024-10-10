#!/bin/bash -e

echo "Looking for processes that have been stuck for more than a day, and will try to kill them."

# Collect the list of processes for the current user, including the CPU time.
# We then split the CPU time into separate fields, so that it's easier to figure out the total number of minutes later on.
IFS=$'\n'
PROCESSES=()
while IFS='' read -r line; do PROCESSES+=("$line"); done < <(ps -o cputime=,pid=,user=,lstart=,args= -U "$USER" -w -w -w | sed -e 's/\([0-9]*\):\([0-9][0-9]\)\.\([0-9][0-9]\)/\1 m \2 s \3 ms/' | sort -nr)

IFS=' '
for process in "${PROCESSES[@]}"; do
	IFS=" " read -r -a FIELDS <<< "$process"
	minutes=${FIELDS[0]}
	pid=${FIELDS[6]}

	echo "$process"

	# looking for processes that have spent more than a day using the CPU (24h * 60min = 1440min)
	if (( minutes > 1440 )); then
		echo "    This process has been stuck for more than $minutes minutes, so assuming it's deadlocked and we'll try to kill it:"
		echo "    kill -9 $pid"
		kill -9 "$pid" | sed 's/^/        /' || true
	fi
done

echo "No (more) processes stuck for more than a day."
