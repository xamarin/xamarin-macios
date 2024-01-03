#!/bin/bash -eux

# Needs the following files:
#
#   NuGet.config
#   global.json
#   WorkloadRollback.json
#   configuration.json
#   nupkg/*.nupkg

set -o pipefail

if test -z "${1:-}"; then
	echo "The first argument must be the directory of the required files."
	exit 1
fi
REMOTE_BUILD_TESTING_DIR=$1
if ! test -d "$REMOTE_BUILD_TESTING_DIR"; then
	echo "The directory $REMOTE_BUILD_TESTING_DIR doesn't exist"
	exit 1
fi
REQUIRED_FILES=(NuGet.config global.json WorkloadRollback.json configuration.json)
echo "Files in $REMOTE_BUILD_TESTING_DIR:"
find "$REMOTE_BUILD_TESTING_DIR" | xargs ls -lad
for req in ${REQUIRED_FILES[@]}; do
	reqfile=$REMOTE_BUILD_TESTING_DIR/$req
	if ! test -f $reqfile; then
		echo "The file $reqfile doesn't exist."
		exit 1
	fi
fi

cd "$REMOTE_BUILD_TESTING_DIR"

CONFIGURATION_JSON=$REMOTE_BUILD_TESTING_DIR/configuration.json

echo "$CONFIGURATION_JSON:"
cat "$CONFIGURATION_JSON"

# shellcheck disable=SC2207
DOTNET_PLATFORMS=($(grep '"DOTNET_PLATFORMS":' "$CONFIGURATION_JSON" | sed -e 's/[[:space:]]*"DOTNET_PLATFORMS": "//' -e 's/",[[:space:]]*$//' | tr '[:upper:]' '[:lower:]'))
DOTNET_VERSION=$(grep '"DOTNET_VERSION":' "$CONFIGURATION_JSON" | sed -e 's/[[:space:]]*"DOTNET_VERSION": "//' -e 's/",[[:space:]]*$//')

if test -z "$DOTNET_VERSION"; then
	echo "No .NET version found in $CONFIGURATION_JSON"
	exit 1
fi

DOTNET_INSTALL_DIR=$REMOTE_BUILD_TESTING_DIR/build
WORKLOAD_ROLLBACK_FILE=$REMOTE_BUILD_TESTING_DIR/WorkloadRollback.json

DOTNET_ARCH=$(arch || true)

if [[ "$DOTNET_ARCH" != "arm64 " ]]; then
	DOTNET_ARCH=x64
fi

# Download dotnet-install.sh
DOTNET_INSTALL=$REMOTE_BUILD_TESTING_DIR/dotnet-install.sh
if ! test -f "$DOTNET_INSTALL"; then
	# Common cURL command:
	# --fail: return an exit code if the connection succeeded, but returned an HTTP error code.
	# --location: follow redirects
	# --connect-timeout: if a connection doesn't happen within 15 seconds, then fail (and potentially retry). This is lower than the default to not get stuck waiting for a long time in case something goes wrong (but instead retry).
	# --verbose / --silent: no explanation needed.
	# --show-error: show an error to the terminal even if asked to be --silent.
	CURL=(curl --fail --location --connect-timeout 15 --verbose --show-error)

	# --retry: retry download 20 times
	# --retry-delay: wait 2 seconds between each retry attempt
	# --retry-all-errors: ignore the definition of insanity and retry even for errors that seem like you'd get the same result (such as 404). This isn't the real purpose, because this will also retry errors that will get a different result (such as connection failures / resets), which apparently --retry doesn't cover.
	#                     but --retry-all-errors is not necessarily available on the bots we're using :/
	CURL+=(--retry 20 --retry-delay 2)

	if ! "${CURL[@]}" https://dot.net/v1/dotnet-install.sh --output "$DOTNET_INSTALL"; then
		echo "curl failed with exit code $?"
		exit 1
	fi
	chmod +x "$DOTNET_INSTALL"
fi

if ! test -d "$DOTNET_INSTALL_DIR"; then
	"$DOTNET_INSTALL" --install-dir "$DOTNET_INSTALL_DIR" --version "$DOTNET_VERSION" --architecture $DOTNET_ARCH --no-path
fi

"$DOTNET_INSTALL_DIR"/dotnet nuget add source "$REMOTE_BUILD_TESTING_DIR/nupkg" --name "$REMOTE_BUILD_TESTING_DIR/local-tmp-dir" --configfile "$REMOTE_BUILD_TESTING_DIR/NuGet.config"

"$DOTNET_INSTALL_DIR"/dotnet workload install "${DOTNET_PLATFORMS[@]}" --from-rollback-file "$WORKLOAD_ROLLBACK_FILE"

# print out what we installed
find "$DOTNET_INSTALL_DIR" -type f -print0 | xargs -0 -- ls -la
