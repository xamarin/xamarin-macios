#!/bin/bash -eux

WORKLOAD_DST="$BUILD_SOURCESDIRECTORY/WorkloadRollback.json"

var=$(make -C "$BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops" print-variable VARIABLE=DOTNET_PLATFORMS)
DOTNET_PLATFORMS=${var#*=}
echo "Dotnet platforms are '$DOTNET_PLATFORMS'"

echo "{" > "$WORKLOAD_DST"
for platform in $DOTNET_PLATFORMS; do
    CURRENT_UPPER=$(echo "$platform" | tr "[:lower:]" "[:upper:]")
    CURRENT_LOWER=$(echo "$platform" | tr "[:upper:]" "[:lower:]")

    var=$(make -C "$BUILD_SOURCESDIRECTORY"/xamarin-macios/tools/devops print-variable "VARIABLE=${CURRENT_UPPER}_NUGET_VERSION_FULL")
    NUGET_VERSION_FULL=${var#*=}
    NUGET_VERSION_FULL=$(echo "$NUGET_VERSION_FULL" | cut -d "+" -f1)

    echo "\"microsoft.net.sdk.$CURRENT_LOWER\": \"$NUGET_VERSION_FULL\"," >> "$WORKLOAD_DST"
done

# Remove the trailing comma from the last entry, because json :/
sed -i '' '$ s/,$//' "$WORKLOAD_DST"

echo "}" >>  "$WORKLOAD_DST"

echo "Rollback file contents:"
cat "$WORKLOAD_DST"
