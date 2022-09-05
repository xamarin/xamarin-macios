#!/bin/bash -eux

set -o pipefail
IFS=$'\n\t '

env | sort

make global.json
make -C builds dotnet SKIP_CUSTOM_DOTNET_RUNTIME_INSTALL=1

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=DOTNET)
DOTNET=${var#*=}
echo "Using dotnet found at $DOTNET"

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=DOTNET_PLATFORMS)
DOTNET_PLATFORMS=${var#*=}
echo "Dotnet platforms are '$DOTNET_PLATFORMS'"

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-abspath-variable VARIABLE=DOTNET_NUPKG_DIR)
DOTNET_NUPKG_DIR=${var#*=}
echo "Using nuget dir $DOTNET_NUPKG_DIR"

ROLLBACK_PATH="$BUILD_SOURCESDIRECTORY/artifacts/WorkloadRollback/WorkloadRollback.json"

echo "Rollback file contents:"
cat "$ROLLBACK_PATH"

mkdir -p "$DOTNET_NUPKG_DIR"
ls -R "$BUILD_SOURCESDIRECTORY/artifacts/not-signed-package"
cp "$BUILD_SOURCESDIRECTORY/artifacts/not-signed-package/"*.nupkg "$DOTNET_NUPKG_DIR"
cp "$BUILD_SOURCESDIRECTORY/artifacts/not-signed-package/"*.pkg "$DOTNET_NUPKG_DIR"
cp "$BUILD_SOURCESDIRECTORY/artifacts/not-signed-package/"*.zip "$DOTNET_NUPKG_DIR"
ls -R "$DOTNET_NUPKG_DIR"

NUGET_SOURCES=$(grep https://pkgs.dev.azure.com ./NuGet.config | sed -e 's/.*value="//'  -e 's/".*//')
SOURCES=()
for source in $NUGET_SOURCES; do
  SOURCES+=(--source)
  SOURCES+=($source)
done

PLATFORMS=()
for platform in $DOTNET_PLATFORMS; do
  CURRENT=$(echo "$platform" | tr "[:upper:]" "[:lower:]")
  PLATFORMS+=("$CURRENT")
done

for platform in $DOTNET_PLATFORMS; do
  unzip -l "$DOTNET_NUPKG_DIR"/Microsoft."$platform".Bundle.*.zip
  unzip "$DOTNET_NUPKG_DIR"/Microsoft."$platform".Bundle.*.zip -d tmpdir
  rsync -av tmpdir/dotnet/sdk-manifests/* "$BUILD_SOURCESDIRECTORY"/xamarin-macios/builds/downloads/dotnet-*/sdk-manifests/
done
find "$BUILD_SOURCESDIRECTORY"/xamarin-macios/builds/downloads/dotnet-*

$DOTNET workload install --from-rollback-file "$ROLLBACK_PATH" --source "$DOTNET_NUPKG_DIR" "${SOURCES[@]}" --verbosity diag "${PLATFORMS[@]}"

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=DOTNET_DIR)
DOTNET_DIR=${var#*=}
ls -lR "$DOTNET_DIR"
