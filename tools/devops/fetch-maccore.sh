#!/bin/bash -eux

# Make sure we've enabled our xamarin bits
./configure --enable-xamarin

# grab Azure Devop's authorization token from the current repo, and use add it to the global git configuration
AUTH=$(git config -l | grep AUTHORIZATION | sed 's/.*AUTHORIZATION: //')
git config --global http.extraheader "AUTHORIZATION: $AUTH"

# fetch maccore
# the github auth we use only works with https, so change maccore's url to be https:// instead of git@
make reset-maccore MACCORE_MODULE="$(grep ^MACCORE_MODULE mk/xamarin.mk | sed -e 's/.*:= //' -e 's_git@github.com:_https://github.com/_' -e 's/[.]git//')" V=1
