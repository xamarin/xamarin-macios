#!/bin/bash -ex
#
# productsign.sh: run productsign against any installer .pkg
# files in the package output directory for the lane, signing
# with the Xamarin Developer Installer identity and verifying
# the signature's fingerprint after the fact.
#
# Author:
#   Aaron Bockover <abock@xamarin.com>
#
# Copyright 2014 Xamarin, Inc.
#

PRODUCTSIGN_KEYCHAIN="login.keychain"
PRODUCTSIGN_KEYCHAIN_PASSWORD="beefydude"
PRODUCTSIGN_IDENTITY="Developer ID Installer: Xamarin Inc"
PRODUCTSIGN_FINGERPRINT="3F:BE:54:B1:41:8B:F1:20:FA:B4:9D:A7:F2:5E:72:95:5A:49:21:D6"

if [ -z "$BUILD_REVISION" ]; then
	echo BUILD_REVISION is unset. Using BUILD_SOURCEVERSION.
	BUILD_REVISION=$BUILD_SOURCEVERSION
fi

if [ -z "$BUILD_SOURCEVERSION" ]; then
	echo BUILD_SOURCEVERSION is unset. Bailing.
	exit 1
fi

mkdir -p "/tmp/$BUILD_REVISION"

echo Before signing
ls -l ../package

security -v find-identity
security unlock-keychain -p "$PRODUCTSIGN_KEYCHAIN_PASSWORD" "$PRODUCTSIGN_KEYCHAIN"

for pkg in ../package/*.pkg; do
	productsign -s "$PRODUCTSIGN_IDENTITY" "$pkg" "/tmp/$BUILD_REVISION/$(basename "$pkg")"
done

echo Signing output
ls -l /tmp/$BUILD_REVISION

mv /tmp/$BUILD_REVISION/* ../package

echo After signing
ls -l ../package

echo 'setns x=http://www.w3.org/2000/09/xmldsig#' > shell.xmllint
echo 'cat (//xar/toc/signature/x:KeyInfo/x:X509Data/x:X509Certificate)[1]/text()' >> shell.xmllint

echo Signature Verification
for pkg in ../package/*.pkg; do
	pkgutil --check-signature "$pkg"
	xar -f "$pkg" --dump-toc="$pkg.toc"
	(
		echo '-----BEGIN CERTIFICATE-----' &&
		xmllint --shell "$pkg.toc" < shell.xmllint | grep -Ev '^/' &&
		echo '-----END CERTIFICATE-----'
	) | openssl x509 -fingerprint | grep "$PRODUCTSIGN_FINGERPRINT" || exit 1
done

rm shell.xmllint