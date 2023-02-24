#!/bin/bash -eu

WHITE=$(tput setaf 7 || true)
RED=$(tput setaf 9 || true)
CLEAR=$(tput sgr0 || true)

OUTPUTPATH=
while [[ $# -gt 0 ]]; do
	case $1 in
		--help | -\? | -h)
			echo "$(basename "$0"): --output=<file>"
			echo "    Find all the 'PackageReference' items in the repository and creates a project listing them all as 'PackageDownload' items."
			echo "    Options:"
			echo "        -h --help:         Show this help"
			echo "        -v --verbose:      Enable verbose script"
			echo "        --output=<value>:  The output file"
			exit 0
			;;
		--verbose | -v)
			set -x
			shift
			;;
		--output)
			OUTPUTPATH="$2"
			shift 2
			;;
		--output=*)
			OUTPUTPATH="${1#*=}"
			shift
			;;
		*)
			echo "${RED}$(basename "$0"): Unknown option: ${WHITE}$1${RED}. Pass --help to view the available options.${CLEAR}"
			exit 1
			;;
	esac
done

TMPPATH=$(PWD)/packagereferences.tmp.csproj

# Go to the root directory
cd "$(git rev-parse --show-toplevel)"

# Find all <PackageReference /> items that has an Include="..." and a Version="..."
git grep -e '<PackageReference.*Include="[a-zA-Z0-9._-]*".*Version="[a-zA-Z0-9._-]*".*>' -h > "$TMPPATH"

# Replace double double quotes with a single double quote. This happens in source code that generates project files (for tests).
sed -i '' 's/""/"/g' "$TMPPATH"

# Remove packages that we build locally
sed -i '' '/Xamarin.Tests.FrameworksInRuntimesNativeDirectory/d' "$TMPPATH"
sed -i '' '/Xamarin.Tests.DynamicLibrariesInRuntimesNativeDirectory/d' "$TMPPATH"

# Get only the name and version of each package, and write that back in a PackageDownload item
sed -i '' 's@.*<PackageReference.*Include="\([a-zA-Z0-9._-]*\)".*Version="\([a-zA-Z0-9._-]*\)".*>.*@\t\t<PackageDownload Include="\1" Version="[\2]" />@g' "$TMPPATH"

# Sort the references and only list each once.
sort -u -o "$TMPPATH" "$TMPPATH"

# Create the complete/valid csproj
{
	echo $'<Project>'
	echo $'\t<ItemGroup>'
	cat "$TMPPATH"
	echo $'\t</ItemGroup>'
	echo $'</Project>'
} > "$TMPPATH.2"
mv "$TMPPATH.2" "$TMPPATH"

mv "$TMPPATH" "$OUTPUTPATH"
