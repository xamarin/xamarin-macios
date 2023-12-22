#!/bin/bash -eux

set -o pipefail
IFS=$'\n\t'

# Clean up some logs.

# I've seen machines with more than 1gb of Xamarin.Messaging logs, so clean that up.
if du -hs ~/Library/Logs/Xamarin.Messaging-*; then
	rm -rf ~/Library/Logs/Xamarin.Messaging-*
fi

# Make sure we don't have any old stuff installed
if du -hs ~/Libary/Caches/Xamarin; then
	rm -rf ~/Libary/Caches/Xamarin
fi

if du -hs ~/Library/Caches/Xamarin/XMA/Agents; then
	rm -rf ~/Library/Caches/Xamarin/XMA/Agents
fi
