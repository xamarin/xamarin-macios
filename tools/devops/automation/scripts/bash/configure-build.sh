#!/bin/bash -xe
flags=( "--enable-xamarin" )

flags=("${flags[@]}" --enable-install-source)
./configure "${flags[@]}"
