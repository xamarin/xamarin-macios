#!/bin/bash -e

gen_sources="$1"
dotnet_sources="$2"
IFS=$'\n' 
arr=($(<$gen_sources))
IFS=' '
read -ra sources_array <<< "$dotnet_sources"
arr+=("${sources_array[@]}")
git_files=($(git ls-files))
IFS=$'\n'
filtered_files=($(printf "%s\n" "${arr[@]}" | grep -vF "${git_files[*]}"))
IFS=','
echo "-embed:${filtered_files[*]}"
unset IFS
