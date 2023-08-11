#!/bin/bash -eux

set -o pipefail
IFS=$'\n\t '

env | sort

# This script can be executed locally by downloading the 'WorkloadRollback'
# and 'not-signed-package' artifacts from an Azure DevOps build, and then
# extracting the files into the xamarin-macios/../artifacts directory.

# If BUILD_SOURCESDIRECTORY is not set, it's likely we're executing locally.
# In which case we can figure out where we are from the current git checkout
# (and also set BUILD_SOURCESDIRECTORY accordingly, since the rest of the
# script needs it).
if test -z "${BUILD_SOURCESDIRECTORY:-}"; then
  BUILD_SOURCESDIRECTORY="$(git rev-parse --show-toplevel)/.."
fi
# Don't assume we're in the right directory (makes it easier to run the script
# locally).
cd "$BUILD_SOURCESDIRECTORY/xamarin-macios"

# Validate a few things
ARTIFACTS_PATH=$BUILD_SOURCESDIRECTORY/artifacts
if ! test -d "$ARTIFACTS_PATH"; then
  echo "The path to the artifects ($ARTIFACTS_PATH) does not exist!"
  exit 1
elif [[ $(find "$ARTIFACTS_PATH/${MACIOS_UPLOAD_PREFIX}not-signed-package" -type f -name '*.nupkg' -or -name '*.pkg' -or -name '*.zip' | wc -l) -lt 1 ]]; then
  echo "No artifacts found in $ARTIFACTS_PATH/not-signed-package"
  echo "If you're running this locally, download the '${MACIOS_UPLOAD_PREFIX}not-signed-package' artifact and extract it into $ARTIFACTS_PATH/${MACIOS_UPLOAD_PREFIX}not-signed-package"
  exit 1
fi

ROLLBACK_PATH="$ARTIFACTS_PATH/${MACIOS_UPLOAD_PREFIX}WorkloadRollback/WorkloadRollback.json"
if ! test -f "$ROLLBACK_PATH"; then
  echo "The rollback file $ROLLBACK_PATH does not exist!"
  exit 1
fi

#  Start working
make global.json

# Check if .NET is even enabled
var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=ENABLE_DOTNET)
ENABLE_DOTNET=${var#*=}
if test -z "$ENABLE_DOTNET"; then
  echo "Not installing anything, because .NET is not enabled."
  exit 0
fi

make -C builds dotnet

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=DOTNET)
DOTNET=${var#*=}
echo "Using dotnet found at $DOTNET"

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=DOTNET_PLATFORMS)
DOTNET_PLATFORMS=${var#*=}
echo "Dotnet platforms are '$DOTNET_PLATFORMS'"

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-abspath-variable VARIABLE=DOTNET_NUPKG_DIR)
DOTNET_NUPKG_DIR=${var#*=}
echo "Using nuget dir $DOTNET_NUPKG_DIR"

echo "Rollback file contents:"
cat "$ROLLBACK_PATH"

mkdir -p "$DOTNET_NUPKG_DIR"
ls -R "$ARTIFACTS_PATH/${MACIOS_UPLOAD_PREFIX}not-signed-package"
cp "$ARTIFACTS_PATH/${MACIOS_UPLOAD_PREFIX}not-signed-package/"*.nupkg "$DOTNET_NUPKG_DIR"
cp "$ARTIFACTS_PATH/${MACIOS_UPLOAD_PREFIX}not-signed-package/"*.pkg "$DOTNET_NUPKG_DIR"
cp "$ARTIFACTS_PATH/${MACIOS_UPLOAD_PREFIX}not-signed-package/"*.zip "$DOTNET_NUPKG_DIR"
ls -R "$DOTNET_NUPKG_DIR"

NUGET_SOURCES=$(grep https://pkgs.dev.azure.com ./NuGet.config | sed -e 's/.*value="//'  -e 's/".*//')
SOURCES=()
for source in $NUGET_SOURCES; do
  SOURCES+=(--source)
  SOURCES+=("$source")
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
