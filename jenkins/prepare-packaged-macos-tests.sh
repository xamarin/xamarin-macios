#!/bin/bash -e

# don't change the current directory here

URL=$1
MACCORE_HASH=$2
if test -z "$URL"; then
	echo "First argument must be the url for the packaged mac tests."
	exit 1
fi
if test -z "$MACCORE_HASH"; then
	echo "Second argument must be the maccore hash."
	exit 1
fi

# Allow some bot-specific configuration.
# Some bots use this to change PATH to where a custom version of ruby is installed.
if test -f ~/.jenkins-profile; then
	# SC1090: Can't follow non-constant source. Use a directive to specify location.
	# shellcheck disable=SC1090
	source ~/.jenkins-profile;
fi

env
rm -f -- ./*.zip
curl -fL "$URL" --output mac-test-package.zip
rm -rf mac-test-package
unzip -o mac-test-package.zip
cd mac-test-package && ./system-dependencies.sh --provision-mono --ignore-autotools --ignore-xamarin-studio --ignore-xcode --ignore-osx --ignore-cmake

# fetch script to install provisioning profiles and run it
if test -d maccore; then
    cd maccore
    if ! git fetch origin; then
        cd ..
        rm -Rf maccore
    fi
fi
if ! test -d maccore; then
    git clone git@github.com:xamarin/maccore
    cd maccore
fi
git reset --hard "$MACCORE_HASH"
cd ..

./maccore/tools/install-qa-provisioning-profiles.sh
