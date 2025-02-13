#!/bin/bash -eux

WHITE=$(tput setaf 7 2>/dev/null || true)
BLUE=$(tput setaf 6 2>/dev/null || true)
RED=$(tput setaf 9 2>/dev/null || true)
CLEAR=$(tput sgr0 2>/dev/null || true)

# verify that we have the xamarin-macios.keychain, which is where the certificates will be installed
KEYCHAIN=xamarin-macios
ONLY_CREATE_KEYCHAIN=
VERBOSE=

#cd "$(dirname "${BASH_SOURCE[0]}")"

while ! test -z "${1:-}"; do
	case "$1" in
	--keychain=*)
		KEYCHAIN="${1:11}"
		shift
		;;
	--keychain)
		KEYCHAIN="$2"
		shift 2
		;;
	--only-create-keychain)
		ONLY_CREATE_KEYCHAIN=1
		shift
		;;
	-v|--verbose)
		VERBOSE=-vvvv
		set -x
		shift
		;;
	*)
		echo "${RED}Unknown argument: $1${CLEAR}"
		exit 1
		;;
	esac
done

echo "${BLUE}Installing certificates and provisioning profiles to the keychain '${WHITE}${KEYCHAIN}${BLUE}'${CLEAR}"

IFS="." read -r -a VERSIONS <<< "$(sw_vers -productVersion)"
majorVersion="${VERSIONS[0]}"
minorVersion="${VERSIONS[1]}"
echo "macOS version: ${majorVersion}.${minorVersion}"
if [[ "$majorVersion" -gt 10 || ("$majorVersion" -eq 10 && "$minorVersion" -gt 11) ]]; then
	echo "keychain file format: Sierra (10.12) and above"
	KEYCHAIN_FILE=~/Library/Keychains/$KEYCHAIN.keychain-db
else
	echo "keychain file format: El Capitan (10.11) and below"
	KEYCHAIN_FILE=~/Library/Keychains/$KEYCHAIN.keychain
fi

if test -f "$KEYCHAIN_FILE"; then
	echo "${BLUE}Deleting previous keychain '${WHITE}$KEYCHAIN_FILE${BLUE}'${CLEAR}"
	rm -f "$KEYCHAIN_FILE"
fi

echo "${BLUE}Creating new keychain${CLEAR}"
KEYCHAIN_PWD_FILE=~/.config/"$KEYCHAIN"-keychain
mkdir -p ~/.config
echo "$(openssl rand -hex 10)_random_password_created_automatically_with_greppable_string" > "$KEYCHAIN_PWD_FILE"
security create-keychain -p "$(cat "$KEYCHAIN_PWD_FILE")" "$KEYCHAIN.keychain"

# unlock the keychain
echo "${BLUE}Unlocking keychain${CLEAR}"
security unlock-keychain -p "$(cat "$KEYCHAIN_PWD_FILE")" "$KEYCHAIN_FILE"

echo "${BLUE}Increasing keychain unlock timeout for '${WHITE}$KEYCHAIN_FILE${BLUE} to 6 hours${CLEAR}"
security set-keychain-settings -lut 21600 "$KEYCHAIN_FILE"

echo "${BLUE}Adding certificate(s) to keychain:${CLEAR}"
for cert in provisioning-profiles/certificates-and-profiles/*.cer; do
	echo "    $cert"
	/usr/bin/security import "$cert" -P 1234 -A -t cert -k "$KEYCHAIN.keychain"
done

if test -z "$ONLY_CREATE_KEYCHAIN"; then
	if test -z "${IGNORE_LOGIN_KEYCHAIN:-}"; then
	if ! security list-keychains | grep -q "login.keychain"; then
		echo "Adding login.keychain to the keychain search list"
		security list-keychains -s "$(security list-keychains | sed -e s/\"//g)" login.keychain
	else
		echo "login.keychain already included in the keychain search list"
	fi
	fi

	echo "${BLUE}Provisioning${CLEAR}"

	# The provisionator caches downloaded files, assuming they won't change.
	# But we update certificates and provisioning profiles on the server
	# when they expire, which means we always have to download them because
	# we don't know if they've been updated. So force the provisionator to
	# always download by removing any cached content.
	rm -f ~/Library/Caches/com.xamarin.provisionator/Provisions/*p12
	rm -f ~/Library/Caches/com.xamarin.provisionator/Provisions/*mobileprovision

	PROVISION_KEYCHAIN=$KEYCHAIN ./provisioning-profiles/provisionator.sh $VERBOSE

	# Install any certificates or provisioning profiles in the provisioning-profiles subdirectory.
	shopt -s nullglob
	for p12 in provisioning-profiles/*.p12; do
	echo "${BLUE}Installing the certificate '${WHITE}$p12${BLUE}'${CLEAR}"
	security import "$p12" -P '' -A -t cert -f pkcs12 -k "$KEYCHAIN_FILE"
	done

	for source in *.mobileprovision; do
	fn="$(basename "$source")"
	target="$HOME/Library/MobileDevice/Provisioning Profiles/$fn"
	if ! test -f "$target" || ! diff "$source" "$target" >/dev/null 2>&1; then
		cp "$source" "$target"
		echo "${BLUE}Installing provisioning profile '${WHITE}$fn${BLUE}'${CLEAR}"
	else
		echo "${BLUE}Provisioning profile '${WHITE}$fn${BLUE}' is up-to-date${CLEAR}"
	fi
	done

	echo "${BLUE}Setting partition list on all certificates in the keychain to avoid permission dialogs${CLEAR}"
	security set-key-partition-list -S "apple-tool:,apple:,codesign:,unsigned:" -s -k "$(cat "$KEYCHAIN_PWD_FILE")" "$KEYCHAIN_FILE" >/dev/null 2>&1
fi

# Include our keychain in the keychain search list and make it the default keychain
if ! security list-keychains | grep -q "$KEYCHAIN.keychain"; then
	echo "Adding $KEYCHAIN.keychain to the keychain search list"
	security list-keychains -s "$(security list-keychains | sed -e s/\"//g)" "$KEYCHAIN.keychain"
else
	echo "$KEYCHAIN.keychain already included in the keychain search list"
fi

security default-keychain -s "$KEYCHAIN.keychain"

echo "Keychain search list"
security list-keychains

# Make sure our cache of the provisioning profile data (in ~/Library/Xamarin/Provisioning Profiles.index) is updated
if test -d "$HOME/Library/MobileDevice/Provisioning Profiles"; then
	touch "$HOME/Library/MobileDevice/Provisioning Profiles"
fi

if test -n "$VERBOSE"; then
	echo "Provisioning profile index:"
	ls -la "$HOME/Library/Xamarin/Provisioning Profiles.index" || true
	echo "Provisionig profiles:"
	ls -lad "$HOME/Library/MobileDevice/Provisioning Profiles" || true
	ls -la "$HOME/Library/MobileDevice/Provisioning Profiles" || true
fi

echo "${BLUE}All done${CLEAR} ✅"

