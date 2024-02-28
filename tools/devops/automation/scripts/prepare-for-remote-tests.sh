#!/bin/bash -eux

# Install the local .NET we're using into XMA's directory
# (we can't point XMA to our local directory)
mkdir -p ~/Library/Caches/Xamarin/XMA/SDKs
cp -cRH "$BUILD_SOURCESDIRECTORY"/xamarin-macios/builds/downloads/dotnet ~/Library/Caches/Xamarin/XMA/SDKs
sed '/local-tests-feed/d' "$BUILD_SOURCESDIRECTORY"/xamarin-macios/NuGet.config > ~/Library/Caches/Xamarin/XMA/SDKs/dotnet/NuGet.config
