#!/bin/bash -e

#
# This is just a placeholder script until the tool has been implemented.
# Ref: https://github.com/xamarin/xamarin-macios/issues/17398
#

# $1: --assembly
ASSEMBLY="$2"
# $3: --input
INPUT="$4"
# $5: --output
OUTPUT="$6"

cp -c "$INPUT" "$OUTPUT"
