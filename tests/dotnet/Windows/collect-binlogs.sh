#!/bin/bash -eux

# Creates a zip file with all relevant log files from the Mac.
# Zip file: ~/remote_build_testing/windows-remote-logs.zip

cd "$(dirname "${BASH_SOURCE[0]}")"

TOPLEVEL="$(git rev-parse --show-toplevel)"

# Abort any agents that are still alive.
# Aborting creates a crash report, and we can investigate why they got stuck.
ps auxww || true
pkill -6 -f Broker.exe || true
pkill -6 -f Build.exe || true
pkill -6 -f Broker.dll || true
pkill -6 -f Build.dll || true
ps auxww || true

# Collect and zip up all the binlogs
mkdir -p ~/remote_build_testing/binlogs
rsync -avv --prune-empty-dirs --exclude 'artifacts/' --include '*/' --include '*.binlog' --exclude '*' "$TOPLEVEL/.." ~/remote_build_testing/binlogs

rm -f ~/remote_build_testing/windows-remote-logs.zip
zip -9r ~/remote_build_testing/windows-remote-logs.zip ~/remote_build_testing/binlogs

if test -d ~/Library/Caches/Xamarin/XMA/Agents/Build; then
	find ~/Library/Caches/Xamarin/XMA/Agents/Build -type f -print0 | xargs -0 shasum -a 256 > ~/remote_build_testing/Agents_Build_Checksums.txt
	zip -9r ~/remote_build_testing/windows-remote-logs.zip ~/remote_build_testing/Agents_Build_Checksums.txt
fi

# Zip up all the logs in ~/Library/Logs/Xamarin.Messaging*
if ls ~/Library/Logs/Xamarin.Messaging* >& /dev/null ; then
	zip -9r ~/remote_build_testing/windows-remote-logs.zip ~/Library/Logs/Xamarin.Messaging*
else
	echo "No logs in ~/Library/Logs/Xamarin.Messaging"
fi

# Zip up all the logs in /tmp/com.xamarin.*
if ls /tmp/com.xamarin.* >& /dev/null ; then
	zip -9r ~/remote_build_testing/windows-remote-logs.zip /tmp/com.xamarin.*
else
	echo "No logs in /tmp/com.xamarin.*"
fi

ps auxww > ~/remote_build_testing/processes.txt || true

# Collect any crash reports.
zip -9r ~/remote_build_testing/windows-remote-logs.zip ~/Library/Logs/DiagnosticReports || true

ls -la ~/Library/Caches/Xamarin/XMA/SDKs/dotnet/ >> ~/remote_build_testing/dotnet-debug.txt 2>&1 || true
cat ~/Library/Caches/Xamarin/XMA/SDKs/dotnet/NuGet.config >> ~/remote_build_testing/dotnet-debug.txt 2>&1 || true
cat ~/Library/Caches/Xamarin/XMA/SDKs/.home/.nuget/NuGet/NuGet.Config >> ~/remote_build_testing/dotnet-debug.txt 2>&1  || true
