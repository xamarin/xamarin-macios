#!/bin/bash -eux

# we want verbose output from mtouch and mlaunch
echo 123456789 > ~/.mtouch-verbosity
echo 123456789 > ~/.mlaunch-verbosity

make -C tests test-system.config
make -C tests/sampletester TESTS_USE_SYSTEM=1