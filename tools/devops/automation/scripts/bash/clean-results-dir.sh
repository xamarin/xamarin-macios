#!/bin/bash -ex

rm -Rvf $(Build.SourcesDirectory)/package
time make -C $(Build.SourcesDirectory)/xamarin-macios/ git-clean-all
