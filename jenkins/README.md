# Jenkins

This directory contains all the scripts to be executed as steps in the jenkins build bots for each of the PR in the project. The expected order in which they have to
be executed is as follows:

* provision-deps.sh : Downloads and installs the project dependencies.
* build.sh : Builds the project.
* run-tests.sh : Runs the tests.
* build-api-diff.sh : Builds the API diff.
