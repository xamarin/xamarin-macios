#!/bin/bash -xe
flags=(--enable-xamarin)

if [[ "$ENABLE_DOT_NET" == "True" ]]; then
  echo "Enabling dotnet builds."
  flags=("${flags[@]}" --enable-dotnet)
fi

flags=("${flags[@]}" --enable-install-source)

FLAGS="${flags[@]}"
echo "Configuration flags are $FLAGS"

./configure "$FLAGS"
