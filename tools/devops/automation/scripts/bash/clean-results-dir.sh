#!/bin/bash -ex

rm -Rvf package
time make -C xamarin-macios/ git-clean-all
