#!/bin/bash -e

PLATFORM=$1
OUTPUT_DIR=$2
FORMAT=$3
HTML=$4
MARKDOWN=$5
OUTPUT=$6

if test -n "$7"; then
	shift 6
	echo "Too many arguments: $*"
	exit 1
fi

if [[ $FORMAT == html ]]; then
	echo -n "<li>" >> "$OUTPUT"

	if ! test -f "$OUTPUT_DIR/$HTML"; then
		echo -n "<s>$PLATFORM</s> (no diff detected)" >> "$OUTPUT"
	elif ! test -s "$OUTPUT_DIR/$HTML"; then
		echo -n "<s>$PLATFORM</s> (empty diff detected)" >> "$OUTPUT"
	elif grep "BreakingChangesDetected" "$OUTPUT_DIR/$HTML" >/dev/null 2>&1; then
		echo -n "$PLATFORM: <a href='$HTML'>html</a> <a href='$MARKDOWN'>markdown</a> ($HTML_BREAKING_CHANGES_MESSAGE)" >> "$OUTPUT"
	elif ! grep "No change detected" "$OUTPUT_DIR/$HTML" >/dev/null 2>&1; then
		echo -n "$PLATFORM: <a href='$HTML'>html</a> <a href='$MARKDOWN'>markdown</a> ($HTML_NO_BREAKING_CHANGES_MESSAGE)" >> "$OUTPUT"
	else
		echo -n "<s>$PLATFORM</s> (no change detected)" >> "$OUTPUT"
	fi

	echo "</li>" >> "$OUTPUT"

elif [[ $FORMAT == markdown ]]; then
	echo -n "* " >> "$OUTPUT"

	if ! test -f "$OUTPUT_DIR/$HTML"; then
		echo -n "~$PLATFORM~: (no diff detected)" >> "$OUTPUT"
	elif ! test -s "$OUTPUT_DIR/$HTML"; then
		echo -n "~$PLATFORM~: (empty diff detected)" >> "$OUTPUT"
	elif grep "BreakingChangesDetected" "$OUTPUT_DIR/$HTML" >/dev/null 2>&1; then
		echo -n "$PLATFORM: [vsdrops]($HTML) [gist]($MARKDOWN) ($MARKDOWN_BREAKING_CHANGES_MESSAGE)" >> "$OUTPUT"
	elif ! grep "No change detected" "$OUTPUT_DIR/$HTML" >/dev/null 2>&1; then
		echo -n "$PLATFORM: [vsdrops]($HTML) [gist]($MARKDOWN) ($MARKDOWN_NO_BREAKING_CHANGES_MESSAGE)" >> "$OUTPUT"
	else
		echo -n "~$PLATFORM~ (no change detected)" >> "$OUTPUT"
	fi

	echo "" >> "$OUTPUT"
else
	echo "Unknown output format: $FORMAT"
	exit 1
fi

