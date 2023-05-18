#!/bin/bash -xe

CONFIGURE_FLAGS="--enable-xamarin"

if [[ "$EnableDotNet" == "True" ]]; then
  echo "Enabling dotnet builds."
  CONFIGURE_FLAGS="$CONFIGURE_FLAGS --enable-dotnet"
fi

CONFIGURE_FLAGS="$CONFIGURE_FLAGS --enable-install-source"
echo "Configuration flags are '$CONFIGURE_FLAGS'"

./configure $CONFIGURE_FLAGS
echo $(cat ./configure.inc)
