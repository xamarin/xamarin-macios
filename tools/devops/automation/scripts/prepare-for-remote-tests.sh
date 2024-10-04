#!/bin/bash -eux

cd "$(dirname "${BASH_SOURCE[0]}")/../../../.."

# Install the local .NET we're using into XMA's directory
# (we can't point XMA to our local directory)
mkdir -p ~/Library/Caches/Xamarin/XMA/SDKs
cp -cRH ./builds/downloads/dotnet ~/Library/Caches/Xamarin/XMA/SDKs
sed '/local-tests-feed/d' ./NuGet.config > ~/Library/Caches/Xamarin/XMA/SDKs/dotnet/NuGet.config

mkdir -p ~/Library/Caches/Xamarin/XMA/SDKs/.home/.nuget/NuGet/
cp ~/Library/Caches/Xamarin/XMA/SDKs/dotnet/NuGet.config ~/Library/Caches/Xamarin/XMA/SDKs/.home/.nuget/NuGet/NuGet.Config

# some diagnostics
cat ~/Library/Caches/Xamarin/XMA/SDKs/dotnet/NuGet.config
cd ~/Library/Caches/Xamarin/XMA/SDKs/dotnet/
./dotnet --info || true
