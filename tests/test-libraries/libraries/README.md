# Test libraries

This directory contains the logic to build multiple very simple native
libraries (.o., .a and .dylib).

For each NAME in the NAMES variable in the Makefile, the makefile will create
a libNAME.o, libNAME.a and a libNAME.dylib with a single getNAME function that
returns a constant char* value "NAME".

The point is to have numerous test libraries that can be included in the same
project for testing how our build logic with regards to native libraries.
