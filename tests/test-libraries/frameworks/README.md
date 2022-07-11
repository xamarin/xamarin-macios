# Test libraries

This directory contains the logic to build multiple very simple native
frameworks (.framework), xc frameworks (.xcframework) and plugins (.bundle).

For each NAME in the NAMES variable in the Makefile, the makefile will create
a framework/xcframework/plugin with a dynamic library that exports a single
getNAME function which returns a constant char* value "NAME".

The point is to have numerous test frameworks/plugins that can be included in
the same project for testing how our build logic with regards to native
libraries.

A binding resource package is also created for each framework (but not plugin).
