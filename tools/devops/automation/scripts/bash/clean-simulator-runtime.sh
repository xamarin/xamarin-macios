#!/bin/bash -eux

# find if there are any duplicated simulator runtimes for a given platform

set -o pipefail
IFS=$'\n\t'

# delete all watchOS simulators, we don't need them anymore
for i in $(xcrun simctl runtime list | grep "watchOS.*Ready" | sed -e 's/.* - //'  -e 's/ .*//'); do
  xcrun simctl runtime delete "$i"
done

xcrun simctl runtime list -j > simruntime.json
cat simruntime.json

if grep -e '"identifier" : ' -e '"runtimeIdentifier" : ' simruntime.json | tr '\n' ' ' | sed -e 's/,//g' -e 's/"//g' -e 's/runtimeIdentifier : //g' -e $'s/identifier : /@/g' | tr '@' '\n' | awk NF | sed 's/^[[:blank:]]*//' > simruntime-lines.txt; then
  cat simruntime-lines.txt
fi

if sed -e 's/.*com.apple/com.apple/g' simruntime-lines.txt > simruntime-runtimes.txt; then
  cat simruntime-runtimes.txt
fi

if sort simruntime-runtimes.txt | uniq -c | sort -n | sed 's/^[[:blank:]]*//' > simruntime-runtimes-by-count.txt; then
  cat simruntime-runtimes-by-count.txt
fi

if grep -v '^1 ' simruntime-runtimes-by-count.txt | sed 's/^[0-9 ]*//' > simruntime-duplicated-runtimes.txt; then
  cat simruntime-duplicated-runtimes.txt
fi

while IFS= read -r simruntime
do
  echo "Duplicated: $simruntime"
  grep "$simruntime" simruntime-lines.txt | sed 's/ .*//' | while IFS= read -r id
  do
    echo "    sudo xcrun simctl runtime delete $id"
    if ! sudo xcrun simctl runtime delete "$id"; then
      echo "    failed to delete runtime $id"
    else
      echo "    deleted runtime $id"
    fi
  done
done < simruntime-duplicated-runtimes.txt || true

xcrun simctl runtime list -v || true
xcrun simctl runtime match list -v || true

# try to detach all simulator runtimes
for dir in /Library/Developer/CoreSimulator/Volumes/*; do
  sudo diskutil eject "$dir" || true
done
# kill the com.apple.CoreSimulator.simdiskimaged service
sudo launchctl kill -9 system/com.apple.CoreSimulator.simdiskimaged || true
# kill the com.apple.CoreSimulator.CoreSimulatorService service
# it seems this service starts the simdiskimaged service if it's not running.
sudo pkill -9 com.apple.CoreSimulator.CoreSimulatorService || true
# the disk image service should now restart when needed, and reload the re-attach all the simulator runtimes.

xcrun simctl runtime match list -v || true
