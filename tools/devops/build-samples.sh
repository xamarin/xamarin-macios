#!/bin/bash -eux

cd "$(dirname "${BASH_SOURCE[0]}")/../.."

make -C tests test-system.config
make -C tests/sampletester TESTS_USE_SYSTEM=1