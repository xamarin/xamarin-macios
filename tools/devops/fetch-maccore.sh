#!/bin/bash -eux

# maccore is already checked out, but our script does a different remote
# ('xamarin' vs 'origin'), so add the different remote, and at the same time
# use https for the repository (instead of git@), since GitHub auth on Azure
# Devops only works with https.

cd "$(dirname "${BASH_SOURCE[0]}")"
cd "$(git rev-parse --show-toplevel)/../maccore"
git remote add -f xamarin https://github.com/xamarin/maccore
cd ../xamarin-macios

# Make sure we've enabled our xamarin bits
./configure --enable-xamarin

# fetch the hash we want
make reset-maccore V=1
