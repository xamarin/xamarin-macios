#!/bin/bash -ex

export BUILD_SOURCESDIRECTORY=/Users/rolf/work/maccore/windows/xamarin-macios
donut run --project $BUILD_SOURCESDIRECTORY/tools/devops/automation/scripts/csharp/create-html-report/create-html-report.csproj
