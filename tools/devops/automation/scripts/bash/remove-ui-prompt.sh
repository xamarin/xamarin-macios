#!/bin/bash -ex

RC=0
security set-key-partition-list -S apple-tool:,apple: -s -k "$OSX_KEYCHAIN_PASS" login.keychain || RC=$?
if [ $RC -eq 0 ]; then
  echo "Security UI-prompt removed."
else
  echo "Security UI-prompt could NOT be removed."
fi
