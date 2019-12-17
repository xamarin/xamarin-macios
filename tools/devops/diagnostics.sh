#!/bin/bash -x

cd "$(dirname "${BASH_SOURCE[0]}")"

cd ../../../maccore
git remote -v
git show
git branch
git status
git config -l

./tools/provisioning-profiles/fetch-updated-certificates-and-profiles.sh -v
../../../maccore/tools/install-qa-provisioning-profiles.sh -v

exit 0
