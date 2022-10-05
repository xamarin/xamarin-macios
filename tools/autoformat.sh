#!/bin/bash -ex

# Go to the top level directory
cd "$(git rev-parse --show-toplevel)"
SRC_DIR=$(pwd)

# Go one directory up, to avoid any global.json in xamarin-macios
cd ..

# Start formatting!
dotnet format "$SRC_DIR/tools/xibuild/xibuild.csproj"
dotnet format whitespace "$SRC_DIR/tests/cecil-tests/cecil-tests.csproj"
dotnet format whitespace "$SRC_DIR/msbuild/Messaging/Xamarin.Messaging.Build/Xamarin.Messaging.Build.csproj"
dotnet format whitespace "$SRC_DIR/msbuild/Xamarin.Localization.MSBuild/Xamarin.Localization.MSBuild.csproj"
dotnet format whitespace "$SRC_DIR/msbuild/Xamarin.Mac.Tasks/Xamarin.Mac.Tasks.csproj"
dotnet format whitespace "$SRC_DIR/msbuild/Xamarin.MacDev.Tasks/Xamarin.MacDev.Tasks.csproj"
dotnet format whitespace "$SRC_DIR/msbuild/Xamarin.iOS.Tasks.Windows/Xamarin.iOS.Tasks.Windows.csproj"
dotnet format whitespace "$SRC_DIR/msbuild/Xamarin.iOS.Tasks/Xamarin.iOS.Tasks.csproj"
dotnet format whitespace "$SRC_DIR/tools/dotnet-linker/dotnet-linker.csproj"

# dotnet format "$SRC_DIR/[...]"
# add more projects here...

cd "$SRC_DIR"
