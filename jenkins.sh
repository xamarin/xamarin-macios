#!/bin/bash -e

./configure --disable-ios-device
time make world
