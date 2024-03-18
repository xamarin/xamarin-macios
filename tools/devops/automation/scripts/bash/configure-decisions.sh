#!/bin/bash -eux

set -o pipefail
IFS=$'\n\t'

# Some debug spew...
env -0 | sort -z | tr '\0' '\n' || true

function AddOutputVariable () {
	set +x
	echo "Setting the variable $1=$2"
	echo "##vso[task.setvariable variable=$1;isOutput=true]$2"
	set -x
}

# Run mac tests if any desktop platform is enabled, and they've not been disabled.
if [[ "${LABELS_SKIP_PACKAGED_MACOS_TESTS:-}" == "True" ]]; then
	# They've been skipped: don't run them
	RUN_MAC_TESTS=false
elif [[ "${LABELS_RUN_PACKAGED_MACOS_TESTS:-}" == "True" ]]; then
	# They've been explicitly enabled: run them
	RUN_MAC_TESTS=true
elif [[ "${LABELS_SKIP_ALL_TESTS:-}" == "True" ]]; then
	# All tests have been skipped
	RUN_MAC_TESTS=false
elif [[ "${CONFIGURE_PLATFORMS_INCLUDE_DOTNET_MACOS:-}" != "" ]]; then
	# Run mac tests if a .NET desktop platform is enabled
	RUN_MAC_TESTS=true
elif [[ "${CONFIGURE_PLATFORMS_INCLUDE_LEGACY_MAC:-}" != "" ]]; then
	# Run mac tests if a legacy desktop platform is enabled
	RUN_MAC_TESTS=true
else
	# Otherwise don't run mac tests
	RUN_MAC_TESTS=false
fi
AddOutputVariable RUN_MAC_TESTS "$RUN_MAC_TESTS"

# Run windows tests if any platform is enabled, and they've not been disabled.
if [[ "${LABELS_SKIP_WINDOWS_TESTS:-}" == "True" ]]; then
	# They've been skipped: don't run them
	RUN_WINDOWS_TESTS=false
elif [[ "${LABELS_RUN_WINDOWS_TESTS:-}" == "True" ]]; then
	# They've been explicitly enabled: run them
	RUN_WINDOWS_TESTS=true
elif [[ "${LABELS_SKIP_ALL_TESTS:-}" == "True" ]]; then
	# All tests have been skipped
	RUN_WINDOWS_TESTS=false
else
	# Otherwise run windows tests (we want to run windows tests if any platform is enabled)
	RUN_WINDOWS_TESTS=true
fi
AddOutputVariable RUN_WINDOWS_TESTS "$RUN_WINDOWS_TESTS"
