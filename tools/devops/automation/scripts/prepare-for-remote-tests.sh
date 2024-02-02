#!/bin/bash -eux

# I've seen machines with more than 1gb of Xamarin.Messaging logs, so clean that up.
if du -hs ~/Library/Logs/Xamarin.Messaging*; then
	rm -rf ~/Library/Logs/Xamarin.Messaging*
fi

# Make sure we don't have any old stuff installed
if du -hs ~/Library/Caches/Xamarin; then
	rm -rf ~/Library/Caches/Xamarin
fi

# Make sure we don't have stuff from earlier builds.
rm -rf ~/remote_build_testing

# Install the local .NET we're using into XMA's directory
# (we can't point XMA to our local directory)
mkdir -p ~/Library/Caches/Xamarin/XMA/SDKs
cp -cRH "$BUILD_SOURCESDIRECTORY"/xamarin-macios/builds/downloads/dotnet ~/Library/Caches/Xamarin/XMA/SDKs
