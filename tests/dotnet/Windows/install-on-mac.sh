#!/bin/bash -eux

# Needs the following files:
#
#   NuGet.config
#   global.json
#   WorkloadRollback.json
#   configuration.json
#   nupkg/*.nupkg

set -o pipefail
#IFS=$'\n\t'

cd "$(dirname "${BASH_SOURCE[0]}")"

echo "Hello from Mac 0"

ls -la

echo "Hello from Mac 1"

cat configuration.json

echo "Hello from Mac 1.5"

# shellcheck disable=SC2207
DOTNET_PLATFORMS=($(grep '"DOTNET_PLATFORMS":' configuration.json | sed -e 's/[[:space:]]*"DOTNET_PLATFORMS": "//' -e 's/",[[:space:]]*$//' | tr '[:upper:]' '[:lower:]'))

echo "Hello from Mac 2"

DOTNET_VERSION=$(grep '"DOTNET_VERSION":' configuration.json | sed -e 's/[[:space:]]*"DOTNET_VERSION": "//' -e 's/",[[:space:]]*$//')

if test -z "$DOTNET_VERSION"; then
	echo "No .NET version found"
	cat configuration.json
	exit 2
fi

echo "Hello from Mac 3"

DOTNET_INSTALL_DIR=_build

echo "Hello from Mac 4"

WORKLOAD_ROLLBACK_FILE=WorkloadRollback.json

echo "Hello from Mac 4"

DOTNET_ARCH=$(arch || true)
echo "Hello from Mac 4bis ($DOTNET_ARCH)"

if [[ "$DOTNET_ARCH" != "arm64 " ]]; then

	echo "Hello from Mac 5"

	DOTNET_ARCH=x64
fi


echo "Hello from Mac 6"


if ! test -f dotnet-install.sh; then

	echo "Hello from Mac 7"

	curl --version

	echo "Hello from Mac 8"

	# Common cURL command:
	# --fail: return an exit code if the connection succeeded, but returned an HTTP error code.
	# --location: follow redirects
	# --connect-timeout: if a connection doesn't happen within 15 seconds, then fail (and potentially retry). This is lower than the default to not get stuck waiting for a long time in case something goes wrong (but instead retry).
	# --verbose / --silent: no explanation needed.
	# --show-error: show an error to the terminal even if asked to be --silent.
	CURL=(curl --fail --location --connect-timeout 15 --verbose --show-error)


	echo "Hello from Mac 9"

	# --retry: retry download 20 times
	# --retry-delay: wait 2 seconds between each retry attempt
	# --retry-all-errors: ignore the definition of insanity and retry even for errors that seem like you'd get the same result (such as 404). This isn't the real purpose, because this will also retry errors that will get a different result (such as connection failures / resets), which apparently --retry doesn't cover.
	#                     but --retry-all-errors is not necessarily available on the bots we're using :/
	CURL+=(--retry 20 --retry-delay 2)

	echo "Hello from Mac 10"
	if ! "${CURL[@]}" https://dot.net/v1/dotnet-install.sh --output dotnet-install.sh; then
		echo "curl failed with exit code $?"
		exit 1
	fi
	echo "Hello from Mac 11"
	chmod +x dotnet-install.sh
fi

echo "Hello from Mac 12"

if ! test -d "$DOTNET_INSTALL_DIR"; then
	echo "Hello from Mac 13"
	./dotnet-install.sh --install-dir "$DOTNET_INSTALL_DIR" --version "$DOTNET_VERSION" --architecture $DOTNET_ARCH --no-path
fi

echo "Hello from Mac 14"

"$DOTNET_INSTALL_DIR"/dotnet nuget add source "$PWD"/nupkg --name local-tmp-dir --configfile ./NuGet.config

echo "Hello from Mac 15"

"$DOTNET_INSTALL_DIR"/dotnet workload install "${DOTNET_PLATFORMS[@]}" --from-rollback-file "$WORKLOAD_ROLLBACK_FILE"

echo "Hello from Mac 16"

find "$DOTNET_INSTALL_DIR" -type f -print0 | xargs -0 -- ls -la
