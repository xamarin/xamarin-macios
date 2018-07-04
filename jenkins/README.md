# Jenkins

This directory contains all the scripts to be executed as steps in the jenkins build bots for each of the PR in the project. The expected order in which they have to
be executed is as follows:

* provision-deps.sh : Downloads and installs the project dependencies.
* build.sh : Builds the project.
* run-tests.sh : Runs the tests.
* build-api-diff.sh : Builds the API diff.

# Jenkinsfile

This file contains the logic to run on our internal Jenkins, available here: http://xamarin-jenkins/blue/organizations/jenkins/macios/activity

The Jenkins job is a multi-branch pipeline job, it will execute in the
following conditions:

* For all branches in the xamarin org (not forks) that has a
  jenkins/Jenkinsfile file.
* For all pull requests from people with write access (also conditional on
  having a jenkins/Jenkinsfile file).

In very broad strokes, the Jenkins job will:

* Checkout xamarin-macios
* Build
* Create packages, upload them to Azure and publish the links to the packages
  as GitHub statuses.
* Run our test suite.
* Run selected Xamarin.Mac tests on all macOS versions we support. This is
  done in parallel with the normal test run.

  If a particular macOS version must be excluded temporarily from testing,
  search Jenkinsfile for `excludedOSes` and follow the instructions you'll
  find.
