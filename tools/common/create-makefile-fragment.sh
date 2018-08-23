#!/bin/bash -e

# This script takes a csproj as input and creates a file with a make variable
# that lists the input files for that csproj.
#
# The idea is to use this make variable as a dependency to a make rule that
# builds the csproj.
#
# The advantage of using make to determine if a csproj needs to be rebuilt is
# that it's much faster when nothing needs to be done: make takes ~0.015s to
# figure this out, msbuild takes ~1.5s. Multiply by the number of csprojs
# (currently 3: mtouch, mmp, generator) by the number of times built every day
# (dozens), and seconds adds up as years pass.
#

cd "$(dirname "$0")"

if test -z "$1"; then
	echo "Must specify the project file to process."
	exit 1
fi

# Input: the csproj file to process.
# Output: /path/to/proj.csproj.inc

PROJECT_FILE="$1"
PROJECT=$(basename -s .csproj "$PROJECT_FILE")
PROJECT_DIR=$(dirname "$PROJECT_FILE")
FRAGMENT_PATH=$PROJECT_FILE.inc
REFERENCES_PATH=$PROJECT-references.txt

# ProjectInspector.csproj is an MSBuild file with a target
# (WriteProjectReferences) that takes another project file as input (the
# ProjectFile variable) and writes all the project references (recursively) to
# a file (the ReferenceListPath variable).
msbuild ProjectInspector.csproj "/t:WriteProjectReferences" "/p:ProjectFile=$PROJECT_FILE" "/p:ReferenceListPath=$REFERENCES_PATH" /verbosity:quiet /nologo

# Now we have a list of all the project referenced by the input project. The
# ProjectInspector.csproj also has a target (WriteInputs) that can list all
# the inputs for a project.
#
# The caveat here is that ProjectInspector.csproj must be in the same
# directory as the inspected project, because relative paths in the inspected
# project are evaluated according to the location of the main project file
# (our ProjectInspector.csproj). Relative paths can't be fixed manually
# afterwards, because relative paths with patterns (where MSBuild evaluates
# the patterns) won't come out right.
#
# So we copy ProjectInspector.csproj to the inspected project's directory and
# execute it from there.

TMPPROJ=

# Make sure we don't leave temporary files around
function delete_tmpproj
{
	rm -f "$REFERENCES_PATH"
	if test -n "$TMPPROJ"; then
		rm -f "$TMPPROJ"
	fi
}
trap delete_tmpproj EXIT
trap delete_tmpproj ERR

echo "$PROJECT_FILE" >> "$REFERENCES_PATH"

INPUT_PATHS=()
for proj in $(sort "$REFERENCES_PATH" | uniq); do
	TMPPROJ=$PROJECT_DIR/ProjectInspector.csproj
	# Copy ProjectInspector.csproj to target directory.
	cp ProjectInspector.csproj "$TMPPROJ"

	# Execute the WriteInputs target
	proj_name=$(basename "$proj")
	proj_dir=$(dirname "$proj")
	inputs_path=$PWD/$proj_name.inputs
	INPUT_PATHS+=("$inputs_path")
	msbuild "$TMPPROJ" "/t:WriteInputs" "/p:ProjectFile=$proj" "/p:InputsPath=$inputs_path" /verbosity:quiet /nologo

	# The output contains relative paths, relative to the csproj directory
	# Change those to full paths by prepending the csproj directory.
	sed -i '' "s@^@$proj_dir/@" "$inputs_path"

	# Change to Make syntax. This is horrifically difficult in MSBuild,
	# because MSBuild blindly replaces backslashes with forword slashes (i.e.
	# windows paths to unix paths...)
	sed -i '' "s_^\\(.*\\)\$_    \\1 \\\\_" "$inputs_path"

	# Clean up
	rm -f "$TMPPROJ"
done

# Create the csproj.inc with the Makefile variable

# First add a dependency for the csproj.inc so that it's rebuilt if the project itself changes.
echo "${PROJECT}.csproj.inc: ${PROJECT_FILE} $PWD/ProjectInspector.csproj" > "$FRAGMENT_PATH"
# Now create the variable with all the input files.
echo "${PROJECT}_dependencies = \\" >> "$FRAGMENT_PATH"
sort "${INPUT_PATHS[@]}" | uniq >> "$FRAGMENT_PATH"

# Simplify paths somewhat by removing the current directory
sed -i '' "s@$PROJECT_DIR/@@" "$FRAGMENT_PATH"

# Cleanup
rm -f "${INPUT_PATHS[@]}"
