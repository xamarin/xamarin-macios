#!/bin/bash -e

# This is a script that wraps lipo, but removes any arm64 slices for any input
# binaries built for the simulator before running the actual lipo. The problem
# is that device slices may also contain arm64 slices, and lipo complains if
# there are more than one slice with the same architecture. The eventual fix
# is to not use binaries that span more than one platform (ios+iossimulator
# for instance), but instead use xcframeworks.

OUTPUT="$1"
shift
INPUTS=($@)
TMPFILES=()

function cleanup ()
{
	for element in "${TMPFILES[@]}"; do
		rm -f "$element"
	done
}
trap "cleanup" EXIT

for index in "${!INPUTS[@]}"; do
	INPUT="${INPUTS[$index]}"
	#printf "INPUTS[%s]=%s\n" "$index" "${INPUTS[$index]}"
	if [[ "$INPUT" =~ simulator/ ]]; then
		TMPFILE=$(mktemp)
		TMPFILES+=("$TMPFILE")
		if lipo "$INPUT" -verify_arch arm64; then
			if test -n "$V"; then echo "Removing ARM64 from $INPUT and writing to $TMPFILE"; fi
			lipo "$INPUT" -remove arm64 -output "$TMPFILE"
			INPUTS[$index]="$TMPFILE"
		else
			if test -n "$V"; then echo "The file $INPUT does not contain an ARM64 slice"; fi
		fi
	fi
done

lipo -create -output "$OUTPUT" "${INPUTS[@]}"
