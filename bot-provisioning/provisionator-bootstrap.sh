#!/bin/bash -e
#
# Remove the following GUID if you do not wish for this script self-update:
# D6F71FB5-F2A7-4A62-86D3-10DFE08301CC
# https://github.com/xamarin/provisionator

while getopts ":t:" opt; do
  case "${opt}" in
    t)
      TOKEN=${OPTARG}
      shift 2
      ;;
  esac
done

function selfdir { (cd "$(dirname "$1")"; echo "$PWD"; ) }

selfdir=$(selfdir "$0")

channel="${PROVISIONATOR_CHANNEL:-latest}"

# Grab auth token from Environment first
# Otherwise set to provided option, if available
base_url="https://dl.internalx.com/provisionator/664bd334021e3102cdef1af66c4fc9f1b2ecd2a21b47419e80d08da1f6c61c2a/${channel}"
auth_token=$AUTH_TOKEN_GITHUB_COM
if [ -z $auth_token ]; then
  auth_token=$TOKEN
fi
if [ -z $auth_token ]; then
  echo "WARNING: Please set AUTH_TOKEN_GITHUB_COM to a GitHub PAT before running provisionator or run provisionator.sh -t <TOKEN> ..."
  echo "At a future point, Provisionator will not be able to bootstrap without the PAT set properly"
  base_url="https://bosstoragemirror.blob.core.windows.net/provisionator/664bd334021e3102cdef1af66c4fc9f1b2ecd2a21b47419e80d08da1f6c61c2a/${channel}"
fi

latest_version_url="${base_url}/version"

archive_name="provisionator.osx.10.11-x64.zip"
archive_path="${selfdir}/${archive_name}"
archive_extract_path="${selfdir}/_provisionator"
archive_url="${base_url}/${archive_name}"
binary_path="${archive_extract_path}/provisionator"

set +e
if [ -z $auth_token ]; then
  latest_version="$(curl -fsL "${latest_version_url}")"
else
  latest_version="$(curl -fsL -H "Authorization: token ${auth_token}" "${latest_version_url}")"
fi
if [ $? != 0 ]; then
  echo "Unable to determine latest version from ${latest_version_url}"
  echo "Please check to make sure a valid GitHub PAT was provided"
  exit 1
fi
set -e

function update_in_place {
  echo "Downloading Provisionator $latest_version..."
  local progress_type="-s"
  tty -s && progress_type="-#"
  if [ -z $auth_token ]; then
    curl -fL $progress_type -o "$archive_path" "$archive_url"
  else
    curl -fLH "Authorization: token ${auth_token}"  $progress_type -o "$archive_path" "$archive_url"
  fi
  rm -rf "$archive_extract_path"
  unzip -q -o -d "$archive_extract_path" "$archive_path"
  rm -f "$archive_path"
}

if [ -f "$binary_path" ]; then
  chmod +x "$binary_path"
  current_version="$("$binary_path" -version 2>&1 || true)"
  if [ "$latest_version" != "$current_version" ]; then
    update_in_place
  fi
else
  update_in_place
fi

# Opt out of macOS backward compat so correct Big Sur OS versions can be obtained.
# More info https://github.com/ziglang/zig/issues/7569
export SYSTEM_VERSION_COMPAT=0

exec caffeinate "$binary_path" "$@"
