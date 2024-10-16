#!/bin/bash -eux

# I've seen machines with more than 1gb of Xamarin.Messaging logs, so clean that up.
if du -hs ~/Library/Logs/Xamarin.Messaging*; then
	rm -rf ~/Library/Logs/Xamarin.Messaging*
fi

# Make sure we don't have any old stuff installed
if du -hs ~/Library/Caches/Xamarin; then
	rm -rf ~/Library/Caches/Xamarin
fi

# Clean up temporary logs
rm -rf /tmp/com.xamarin.*

# Make sure we don't have stuff from earlier builds.
rm -rf ~/remote_build_testing

# Kill any existing brokers and builders
ps auxww || true
pkill -6 -f Broker.exe || true
pkill -6 -f Build.exe || true
pkill -6 -f Broker.dll || true
pkill -6 -f Build.dll || true
ps auxww || true
