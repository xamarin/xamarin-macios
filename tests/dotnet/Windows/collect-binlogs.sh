#!/bin/bash -eux

# Creates a zip file with all relevant log files from the Mac.
# Zip file: ~/remote_build_testing/windows-remote-logs.zip

cd "$(dirname "${BASH_SOURCE[0]}")"

TOPLEVEL="$(git rev-parse --show-toplevel)"

# Collect and zip up all the binlogs
mkdir -p ~/remote_build_testing/binlogs
rsync -av --prune-empty-dirs --exclude '*/artifacts/' --include '*/' --include '*.binlog' --exclude '*' "$TOPLEVEL/.." ~/remote_build_testing/binlogs

rm -f ~/remote_build_testing/windows-remote-logs.zip
zip -9r ~/remote_build_testing/windows-remote-logs.zip ~/remote_build_testing/binlogs

# Zip up all the logs in ~/Library/Logs/Xamarin.Messaging*
if ls ~/Library/Logs/Xamarin.Messaging* >& /dev/null ; then
	zip -9r ~/remote_build_testing/windows-remote-logs.zip ~/Library/Logs/Xamarin.Messaging*
else
	echo "No logs in ~/Library/Logs/Xamarin.Messaging"
fi
