#!/bin/bash -ex

cd "$(dirname "${BASH_SOURCE[0]}")/.."
#WORKSPACE=$(pwd)

rm -Rf ../package
make package
