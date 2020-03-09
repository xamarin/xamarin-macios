#!/bin/bash -e

# don't change the current directory here

if [[ "$1" == "--install-old-mono" ]]; then
    cd mac-test-package
    URL=$(grep "^MIN_XM_MONO_URL=" Make.config | sed 's/.*=//')

    COUNTER=0
    echo "Downloading and installing $URL"
    while [[ $COUNTER -lt 5 ]]; do
		EC=0
		curl -s -L "$URL" --output old-mono.pkg || EC=$?
		if [[ $EC -eq 56 ]]; then
			# Sometimes we get spurious "curl: (56) SSLRead() return error -9806" errors. Trying again usually works, so lets try again a few more times.
			# https://github.com/xamarin/maccore/issues/1098
			let COUNTER++ || true
			continue
		fi
		break
	done

	if [[ "x$EC" != "x0" ]]; then
		echo "Failed to provision old mono (exit code: $EC)"
		exit $EC
	fi

	sudo installer -pkg old-mono.pkg -target /
	mono --version
	exit 0
fi


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

# Install 7z. We can't do this from the system-dependencies.sh script, because
# we need 7z to decompress the file where the system-dependencies.sh script
# resides.
# Also ignore any failures, brew may fail if 7z is already installed.
brew install p7zip || true

env
rm -f -- ./*.7z
curl -fL "$URL" --output mac-test-package.7z
rm -rf mac-test-package
7z x mac-test-package.7z
cd mac-test-package

COUNTER=0
while [[ $COUNTER -lt 5 ]]; do
	EC=0
	./system-dependencies.sh --ignore-all --provision-mono || EC=$?
	if [[ $EC -eq 56 ]]; then
		# Sometimes we get spurious "curl: (56) SSLRead() return error -9806" errors. Trying again usually works, so lets try again a few more times.
		# https://github.com/xamarin/maccore/issues/1098
		let COUNTER++ || true
		continue
	fi
	break
done

if [[ "x$EC" != "x0" ]]; then
	echo "Failed to provision dependencies (exit code: $EC)"
	exit $EC
fi

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

./maccore/tools/install-qa-provisioning-profiles.sh --only-create-keychain -v
