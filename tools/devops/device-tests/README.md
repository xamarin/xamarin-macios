# Device test pipelines

This directory contains the different yaml pipelines that are will run the tests on devices. In order to 
fully understand the setup we have to take into account:

## Labs

The team has access to two different labs, these labs use different setups and different pools. We have used
their location to differentiate them:

* cambridge: Lab based in Cambridge that contains iOS, iOS32 and tvOS devices.
* ddfun: Lab mantained by the DevDiv Fundamentals team. Contains iOS and tvOS devices. This labd **DOES NOT** have
    iOS32 or beta devices.

## Pipelines

Although we could parallelize the current pipelines, we have opted to have a pipeline per device. Therefore,
there is a pipeline per device in each of the supported labs. 

* cambridge
    - cambridge-ios-32b-device-tests: Runs tests on the old iOS hardware on 32b.
    - cambridge-ios-device-tests: Runs tests on stable iOS 64b devices.
    - cambridge-ios-beta-device-tests: Runs tests on beta versions of the OS on iOS 64b devices.
    - cambridge-tvos-device-tests: Runs tests on stable tvOS devices.
    - cambridge-tvos-beta-device-tests: Runs tests on beta versions of the OS on tvOS devices.
* ddfun
    - ddfun-ios-device-tests: Runs tests on stable iOS 64b devices.
    - ddfun-tvos-device-tests: Runs tests on stable tvOS devices.

### Understanding the pipelines

All the device tests pipelines are very similar, the only difference resides in the information
that is passed to xharness when executing the tests and the context that is used to notify the
test status on GitHub. Therefore all the pipelines are based in the following:

#### Templates

##### device-tests-stage

This template represents the entry point of the pipeline. The template defines the different jobs
to be executed in the pipeline. Do remember that jobs can be executed in different machines and could
be done in parallel if the dependencies are not correct.

The jobs are:

* tests: Executes the tests on devices. This job uses a template that takes a set of parameters to configure
    the job that executes the tests on the correct devices. This job MUST be executed on a macOS bot that has
    the correct demand. The demands specify the expected device to be attached to the bot.
* publish_html: This job depends on the 'tests' job and will get the html report from the tests and publish it
    to a webpage. Since the job depends on vsdrop, the job must be executed on a Windows machine.

##### device-tests

Main template that contains all the steps to execute the device tests. The template takes the following parameters:

* statusContext: The context of the tests. Used to pair the status of the tests with an execution context, for example 'VSTS iOS device tests'
* testsLabels: The labels to pass to xharness to decide which tests to be executed.
* disableProvisionatorCache: Special parameter that allows to clean up the cache used by the provisionator.
* htmlReportStorage: The storage to be used for the html report. There are two storages available:
    - xamarin-storage: Old storage used by xamarin, to be deprecated.
    - vsdrop: New storage that uses azure blobs.

##### publish-html

Conditional job that will be executed only when the pipeline has been configured to use the 'vsdrop' storage. If the storage selected is
not 'vsdrop' this job is not executed. Its main task is to download the html report that was posted as a build artifact, upload it to vsdrop and
create a browsable static webpage.

#### Variables

The are two main groups of variables used in the pipelines:

- common (under templates/common-variables.yml): Contains a set of group variables that have been created by release engineering and that contain all the different secrets needed by the pipeline.
- lab (under templates/[ddfun|cambridge]-variables: Contains the different configurations per lab. That includes the pools to use and the type of storage.

#### Scripts

All the steps of the device-tests.yml have been moved to different powershell scripts. This are grouped by their usage. They are grouped in

* GitHub: cmdlets that allow to interact with the GitHub REST API.
* VSTS: cmdlets that allow to interact with the VSTS REST API.
* MLaunch: cmdlets that allow to interact with mlaunch.
* System: cmdlets that provide information about the system as well as to cleanup certain resources used by the tests.

All cmdlets have unit tests. In order to run them locally the user has to execute

make -C tools/devops/device-tests run-tests

Tests are written with Pester and the execution will output the number of tests executed and their results.

## Storage

The html report can be stored in two different storages. The xamarin-macios one and the vsdrop one. A
number of steps are executed depending on the storage, therefore, when modifying a step/task take into
account the condition is used. If you look at the steps you might find coditions like the following one:

```yml
- pwsh: |
    Import-Module $Env:SYSTEM_DEFAULTWORKINGDIRECTORY/xamarin-macios/tools/devops/cambridge/templates/scripts/GitHub.psm1 
    New-GitHubSummaryComment -Context "$Env:CONTEXT" 
  env:
    BUILD_REVISION: $(BUILD_REVISION)
    CONTEXT: ${{ parameters.statusContext }}
    GITHUB_TOKEN: $(GitHub.Token)
    XAMARIN_STORAGE_PATH: $(XAMARIN_STORAGE_PATH) 
  displayName: 'Add summaries'
  condition: and(always(), eq('${{ parameters.htmlReportStorage }}', 'xamarin-storage')) 
  timeoutInMinutes: 1
```

In the above example we are interested in:

```yml
  condition: and(always(), eq('${{ parameters.htmlReportStorage}}', 'xamarin-storage')) 
```

The condition states that the step should be executed always when the xamarin-storage is used, else do not execute the step.
