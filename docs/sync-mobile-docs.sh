#!/bin/bash -eu

cd "$(dirname "${BASH_SOURCE[0]}")"

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

cp -cr building-apps $DOTNET_MOBILEDOCS_PATH/docs/ios/

echo "✅ Successfully copied docs/building-apps/* to $DOTNET_MOBILEDOCS_PATH/docs/ios/building-apps."
