#!/bin/bash -e

BASE_HASH=
COMP_HASH=HEAD
WORKING_DIR=

WHITE=$(tput setaf 7 2>/dev/null || true)
BLUE=$(tput setaf 6 2>/dev/null || true)
RED=$(tput setaf 9 2>/dev/null || true)
CLEAR=$(tput sgr0 2>/dev/null || true)

# Clone files on AFPS, instead of copying them. Much faster.
CP="cp"
if df -t apfs / >/dev/null 2>&1; then
	CP="cp -c"
fi

function report_error_line ()
{
	echo "$@"
	if test -n "$FAILURE_FILE"; then
		# remove color codes when writing to failure file
		# shellcheck disable=SC2001
		echo "$@" | sed $'s,\x1b\\[[0-9;]*[a-zA-Z],,g' >> "$FAILURE_FILE"
	fi
}

function show_help ()
{
	echo "$(basename "$0"): Compare the managed API and generate a diff for the generated code between the currently built assemblies and a specific hash."
	echo "Usage is: $(basename "$0") --base=[TREEISH] [options]"
	echo "   -h, -?, --help          Displays the help"
	echo "   -b, --base=[TREEISH]    The treeish to compare the currently built assemblies against."
	#not quite implemented yet# echo "   -c, --compare=[TREEISH] Optional, if specified use this hash to build the 'after' assemblies for the comparison."
	echo ""
	printf "${BLUE} WARNING: This tool will temporarily change the current HEAD of your git checkout.${CLEAR}\\n"
	printf "${BLUE} WARNING: The tool will try to restore the current HEAD when done (or if cancelled), but this is not guaranteed.${CLEAR}\\n"
	echo ""
}

ORIGINAL_ARGS=("$@")
FAILURE_FILE=
while ! test -z "$1"; do
	case "$1" in
		--help|-\?|-h)
			show_help
			exit 0
			;;
		--base=*|-b=*)
			BASE_HASH="${1#*=}"
			shift
			;;
		--base|-b)
			BASE_HASH="$2"
			shift 2
			;;
		--compare=*|-c=*)
			COMP_HASH="${1#*=}"
			shift
			;;
		--compare|-c)
			COMP_HASH="$2"
			shift 2
			;;
		--impl-working-dir=*)
			WORKING_DIR="${1#*=}"
			shift
			;;
		--failure-file=*)
			FAILURE_FILE="${1#*=}"
			shift
			;;
		--failure-file)
			FAILURE_FILE="$2"
			shift 2
			;;
		*)
			echo "Error: Unknown argument: $1"
			exit 1
			;;
	esac
done

if test -z "$BASE_HASH"; then
	report_error_line "${RED}Error: It's required to specify the hash to compare against (--base=HASH).${CLEAR}"
	exit 1
fi

ROOT_DIR=$(git rev-parse --show-toplevel)

# Go to the root directory of the git repo, so that we don't run into any surprises with paths.
# Also make ROOT_DIR an absolute path.
cd "$ROOT_DIR"
ROOT_DIR=$(pwd)

# Only show git colors locally. The normal "has-controlling-terminal" doesn't work, because
# we always capture the output to indent it (thus the git processes never have a controlling
# terminal)
if test -z "$BUILD_REVISION"; then
	GIT_COLOR=--color=always
	GIT_COLOR_P="-c color.status=always"
fi

if [ -n "$(git status --porcelain --ignore-submodule)" ]; then
	report_error_line "${RED}** Error: Working directory isn't clean:${CLEAR}"
	git $GIT_COLOR_P status --ignore-submodule | sed 's/^/    /' | while read line; do report_error_line "$line"; done
	exit 1
fi

echo "Comparing the changes between $WHITE$BASE_HASH$CLEAR and $WHITE$COMP_HASH$CLEAR:"
git log "$BASE_HASH..$COMP_HASH" --oneline $GIT_COLOR | sed 's/^/    /'

# Resolve any treeish hash value (for instance HEAD^4) to the unique (MD5) hash
COMP_HASH=$(git log -1 --pretty=%H "$COMP_HASH")
BASE_HASH=$(git log -1 --pretty=%H "$BASE_HASH")

# We'll clone xamarin-macios again into a different directory, and build it

GENERATOR_DIFF_FILE=
APIDIFF_FILE=

OUTPUT_DIR=$ROOT_DIR/tools/comparison
OUTPUT_SRC_DIR=$OUTPUT_DIR/src

function upon_exit ()
{
	if ! test -z "$GENERATOR_DIFF_FILE"; then
		echo "Generator diff: $GENERATOR_DIFF_FILE"
	fi
	if ! test -z "$APIDIFF_FILE"; then
		echo "API diff: $APIDIFF_FILE"
	fi

	if test -f "$ROOT_DIR/NuGet.config.disabled"; then
		mv "$ROOT_DIR/NuGet.config.disabled" "$ROOT_DIR/NuGet.config"
	fi

	# Clean up after ourselves (but leave the comparison)
	rm -Rf "$OUTPUT_SRC_DIR"
	rm -Rf "$OUTPUT_DIR/build"
	rm -Rf "$OUTPUT_DIR/build-new"
}

trap upon_exit EXIT

if test -z "$CURRENT_BRANCH"; then
	echo "${BLUE}Current hash: ${WHITE}$(git log -1 --pretty="%h: %s")${BLUE} (detached)${CLEAR}"
else
	echo "${BLUE}Current branch: ${WHITE}$CURRENT_BRANCH${BLUE} (${WHITE}$(git log -1 --pretty="%h: %s")${BLUE})${CLEAR}"
fi
echo "${BLUE}Checking out ${WHITE}$(git log -1 --pretty="%h: %s" "$BASE_HASH")${CLEAR}...${CLEAR}"

rm -Rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"
mkdir -p "$OUTPUT_SRC_DIR"

cd "$OUTPUT_SRC_DIR"
git clone https://github.com/xamarin/xamarin-macios --reference "$ROOT_DIR"
cd xamarin-macios
git reset --hard "$BASE_HASH"
cp "$ROOT_DIR/configure.inc" .
make reset
make check-versions
if ! ./system-dependencies.sh; then
	report_error_line "${RED}Error: The system requirements for the hash to compare against ($WHITE$BASE_HASH$CLEAR) are different than for the current hash. Comparison is currently not supported in this scenario.${CLEAR}"
	exit 1
fi
if ! make all -j8; then
	report_error_line "${RED}Error: 'make' failed for the hash $WHITE$BASE_HASH$CLEAR.${CLEAR}"
	exit 1
fi
if ! make install -j8; then
	report_error_line "${RED}Error: 'make install' failed for the hash $WHITE$BASE_HASH$CLEAR.${CLEAR}"
	exit 1
fi

#
# API diff
#
#   First we calculate the apidiff references for the hash we're comparing against
#   Then we restore the original hash, and finally we calculate the api diff.
#

# Calculate apidiff references according to the temporary build
echo "${BLUE}Updating apidiff references...${CLEAR}"
if ! make update-refs -C "$ROOT_DIR/tools/apidiff" -j8 APIDIFF_DIR="$OUTPUT_DIR/apidiff" IOS_DESTDIR="$OUTPUT_SRC_DIR/xamarin-macios/_ios-build" MAC_DESTDIR="$OUTPUT_SRC_DIR/xamarin-macios/_mac-build" DOTNET_DESTDIR="$OUTPUT_SRC_DIR/xamarin-macios/_build"; then
	EC=$?
	report_error_line "${RED}Failed to update apidiff references${CLEAR}"
	exit "$EC"
fi

#
# Generator diff
#

# We make a copy of the generated source code to compare against,
# so that we can remove files we don't want to compare without
# affecting that build.
$CP -R "$ROOT_DIR/src/build" "$OUTPUT_DIR/build-new"
$CP -R "$OUTPUT_SRC_DIR/xamarin-macios/src/build" "$OUTPUT_DIR/build"

# delete files we don't care are different
cd "$OUTPUT_DIR"
find "$OUTPUT_DIR/build" "$OUTPUT_DIR/build-new" '(' \
	-name 'compiler' -or \
	-name 'bgen' -or \
	-name '*.dll' -or \
	-name '*.editorconfig' -or \
	-name '*.pdb' -or \
	-name '*generated-sources' -or \
	-name 'generated_sources' -or \
	-name '*.exe' -or \
	-name '*.rsp' -or \
	-name 'AssemblyInfo.cs' -or \
	-name 'Constants.cs' -or \
	-name 'generator.csproj*' -or \
	-name 'bgen.csproj.*' -or \
	-name 'bgen.runtimeconfig.dev.json' -or \
	-name 'PublishOutputs.*.txt' -or \
	-name '*.binlog' -or \
	-name '*.cache' \
	')' -delete
mkdir -p "$OUTPUT_DIR/generator-diff"
GENERATOR_DIFF_FILE="$OUTPUT_DIR/generator-diff/index.html"
git diff --no-index build build-new > "$OUTPUT_DIR/generator-diff/generator.diff" || true
"$ROOT_DIR/tools/diff-to-html" "$OUTPUT_DIR/generator-diff/generator.diff" "$GENERATOR_DIFF_FILE"

# Now compare the current build against those references
echo "${BLUE}Running apidiff...${CLEAR}"
APIDIFF_FILE=$OUTPUT_DIR/apidiff/api-diff.html
if ! make all-local -C "$ROOT_DIR/tools/apidiff" -j8 APIDIFF_DIR="$OUTPUT_DIR/apidiff"; then
	EC=$?
	report_error_line "${RED}Failed to run apidiff${CLEAR}"
	exit "$EC"
fi

# Now create the markdowns with these references
echo "${BLUE}Creating markdowns...${CLEAR}"
if ! make all-markdowns -C "$ROOT_DIR/tools/apidiff" -j8 APIDIFF_DIR="$OUTPUT_DIR/apidiff"; then
	EC=$?
	report_error_line "${RED}Failed to create markdowns${CLEAR}"
	exit "$EC"
fi
