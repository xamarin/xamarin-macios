#!/bin/bash -eu

# This script takes a public rsa key as an argument,
# and adds it to the ~/.ssh/authorized_keys file.

if test -z "${1:-}"; then
	echo "Did not specify the public key (as the first argument)"
	exit 1
fi

KEY="$1"
echo "Authorizing the public key:"
echo "    $KEY"

echo "Contents of ~/.ssh:"
# shellcheck disable=SC2012
ls -la ~/.ssh | sed 's/^/    /'

# Check if the corresponding files exist, and if not, create them with the correct permissions.
if ! test -f ~/.ssh/authorized_keys; then
  if ! test -d ~/.ssh; then
    mkdir -p ~/.ssh
    chmod 700 ~/.ssh
  fi
  touch ~/.ssh/authorized_keys
  chmod 600 ~/.ssh/authorized_keys
fi

# Check if the key has already been authorized
KEYPART="${KEY//= */=}"
if grep "$KEYPART" ~/.ssh/authorized_keys >/dev/null 2>&1; then
  echo "The public key is already in ~/.ssh/authorized_keys."
  exit 0
fi

echo "$KEY" >> ~/.ssh/authorized_keys
ls -la ~/.ssh
