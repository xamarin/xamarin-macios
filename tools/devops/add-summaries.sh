#!/bin/bash -e

set +x

X="#vso"

FILE=$PWD/xamarin-macios/tests/TestSummary.md
if ! test -f "$FILE"; then
echo ":fire: Tests failed catastrophically (no summary found)" > "$FILE"
fi

echo "#${X}[task.addattachment type=Distributedtask.Core.Summary;name=Test results;]$FILE"

echo "[Jenkins build]($WRENCH_URL)" > Wrench.md
echo "#${X}[task.addattachment type=Distributedtask.Core.Summary;name=Jenkins;]$PWD/Wrench.md"
