#!/bin/bash -eu


REVERSE=
DOTNET_MOBILEDOCS_PATH=
while [ -n "${1:-}" ]; do
	case $1 in
		-h | --help)
			echo "$(basename "$0"): [-r | --reverse]"
			echo "    Copy the docs that go to our website from this repository to the correct location in the dotnet/docs-mobile repository."
			echo ""
			echo "    Options:"
			echo "        -h --help:                 Show this help"
			echo "        -v | --verbose:            Enable verbose script"
			echo "        --docs-mobile-repo=<path>: The path to the dotnet/docs-mobile repository. The script will try to to find this path if not specified."
			echo "        -r | --reverse:            Copy the docs in the reverse directory: from dotnet/docs-mobile to this repository."
			exit 0
			;;
		-v | --verbose)
			set -x
			shift
			;;
		-r | --reverse)
			REVERSE=1
			shift
			;;
		--docs-mobile-repo=*)
			DOTNET_MOBILEDOCS_PATH="${1//*=/}"
			shift
			;;
		--docs-mobile-repo)
			DOTNET_MOBILEDOCS_PATH="$2"
			shift 2
			;;
		*)
			echo "Unknown argument: $1"
			exit 1
			;;
	esac
done

if test -n "${DOTNET_MOBILEDOCS_PATH:-}"; then
	if ! test -d "$DOTNET_MOBILEDOCS_PATH"; then
		echo "The directory '$DOTNET_MOBILEDOCS_PATH' does not exist."
		exit 1
	fi
	# resolve DOTNET_MOBILEDOCS_PATH to a full path (because we're changing the current directory later)
	pushd . > /dev/null
	cd "$DOTNET_MOBILEDOCS_PATH"
	DOTNET_MOBILEDOCS_PATH=$(pwd)
	popd > /dev/null
fi

cd "$(dirname "${BASH_SOURCE[0]}")"

if test -z "${DOTNET_MOBILEDOCS_PATH:-}"; then
	current=$(pwd)

	parent=$(dirname "$current")
	while true; do
		if [[ $parent == $current ]]; then
			echo "Unable to determine the location of the dotnet/docs-mobile repository."
			exit 1
		fi

		DOTNET_MOBILEDOCS_PATH=$current/dotnet/docs-mobile
		if test -d "$DOTNET_MOBILEDOCS_PATH"; then
			break;
		fi

		current=$parent
		parent=$(dirname "$current")
	done
fi

if test -z "$REVERSE"; then
	echo "Copying docs/building-apps/* to $DOTNET_MOBILEDOCS_PATH/docs/ios/building-apps"
	cp -cr building-apps $DOTNET_MOBILEDOCS_PATH/docs/ios/
	echo "✅ Successfully copied docs/building-apps/* to $DOTNET_MOBILEDOCS_PATH/docs/ios/building-apps."
else
	echo "Copying $DOTNET_MOBILEDOCS_PATH/docs/ios/building-apps/* to docs/building-apps"
	cp -cr $DOTNET_MOBILEDOCS_PATH/docs/ios/building-apps .
	echo "✅ Successfully copied $DOTNET_MOBILEDOCS_PATH/docs/ios/building-apps to docs/building-apps/building-apps."
fi
