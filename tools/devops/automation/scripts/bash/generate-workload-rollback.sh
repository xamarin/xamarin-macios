#!/bin/bash -eux

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

WORKLOAD_DST="$BUILD_SOURCESDIRECTORY/WorkloadRollback.json"

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=DOTNET_PLATFORMS)
DOTNET_PLATFORMS=${var#*=}
echo "Dotnet platforms are '$DOTNET_PLATFORMS'"

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=MACIOS_MANIFEST_VERSION_BAND)
MACIOS_MANIFEST_VERSION_BAND=${var#*=}

echo "{" > "$WORKLOAD_DST"
for platform in $DOTNET_PLATFORMS; do
    CURRENT_UPPER=$(echo "$platform" | tr "[:lower:]" "[:upper:]")
    CURRENT_LOWER=$(echo "$platform" | tr "[:upper:]" "[:lower:]")

    var=$(make -C "$BUILD_SOURCESDIRECTORY"/xamarin-macios/tools/devops print-variable "VARIABLE=${CURRENT_UPPER}_NUGET_VERSION_FULL")
    NUGET_VERSION_FULL=${var#*=}
    NUGET_VERSION_FULL=$(echo "$NUGET_VERSION_FULL" | cut -d "+" -f1)

    echo "\"microsoft.net.sdk.$CURRENT_LOWER\": \"$NUGET_VERSION_FULL/$MACIOS_MANIFEST_VERSION_BAND\"," >> "$WORKLOAD_DST"
done

# Remove the trailing comma from the last entry, because json :/
sed -i '' '$ s/,$//' "$WORKLOAD_DST"

echo "}" >>  "$WORKLOAD_DST"

echo "Rollback file contents:"
cat "$WORKLOAD_DST"
