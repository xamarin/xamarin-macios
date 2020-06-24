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

There are certain environment variables that are created by steps in the first job that are later shared with the second job. In order to do that you
have to ensure that you do the following:

```yaml
- bash: |
    set -x
    set -e
    echo 'Use xamarin storage $USE_XAMARIN_STORAGE'

    cd $WORKING_DIR/xamarin-macios
    if [[ "$USE_XAMARIN_STORAGE" == "True" ]]; then
      EC=0
      ssh builder@xamarin-storage "mkdir -p /volume1/storage/$XAMARIN_STORAGE_PATH" || EC=$?
      if [ $EC -eq 0 ]; then
        echo '##vso[task.setvariable variable=XAMARIN_STORAGE_FAILED;isOutput=true]false'
        echo '##vso[task.setvariable variable=XAMARIN_STORAGE_PATH;isOutput=true]$XAMARIN_STORAGE_PATH'
        export TESTS_PERIODIC_COMMAND="--periodic-interval 10 --periodic-command rsync --periodic-command-arguments '-avz -e \"ssh\" $PWD/jenkins-results builder@xamarin-storage:/volume1/storage/$XAMARIN_STORAGE_PATH'"
      else
        echo '##vso[task.setvariable variable=XAMARIN_STORAGE_FAILED;isOutput=true]true'
        echo '##vso[task.setvariable variable=XAMARIN_STORAGE_PATH;isOutput=true]""'
      fi
    fi

    make -C builds download .stamp-mono-ios-sdk-destdir -j
    MONO_ENV_OPTIONS=--trace=E:all make -C tests vsts-device-tests
  env:
    WORKING_DIR: $(System.DefaultWorkingDirectory) 
    XAMARIN_STORAGE_PATH: $(XAMARIN_STORAGE_PATH)
    TESTS_EXTRA_ARGUMENTS: ${{ parameters.testsLabels }}
    USE_XAMARIN_STORAGE: '${{ parameters.useXamarinStorage }}'
    VSDROPS_URI: '${{ parameters.vsdropsPrefix }}/$(Build.BuildNumber)/$(Build.BuildId);/' # uri used to create the vsdrops index using full uri
  displayName: 'Run tests'
  name: runTests 
  timeoutInMinutes: 600
```

There are two details we have to pay attention here, first:

```yaml
name: runTests 
```

We need to set the name of the step. This should not be confused with the displayName which is simply a string used in the vsts webpage.
Later we can access the output variables of the step in the following way:

```yaml

XAMARIN_STORAGE_PATH: $[ dependencies.tests.outputs['runTests.XAMARIN_STORAGE_PATH'] ]
```

The other piece of code is

```yaml

echo '##vso[task.setvariable variable=XAMARIN_STORAGE_FAILED;isOutput=true]true'
echo '##vso[task.setvariable variable=XAMARIN_STORAGE_PATH;isOutput=true]""'
```

As you can see, we use **isOutput=true** which means that the environment variable will be registered as an output variable of the step.

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

The html report can be stored in two different places:

* vsdrops
* xamarin-storage

xamarin-storage has two important details to take into account:

1 not all labs have access to it
2 it will disappear in the future

Until xamarin-storage is fully removed, we allow certain pipelines to use it. This is done via the 
'useXamarinStorage' parameter in the templates. To make things easier to maintain, this value is set
in the variable templates per lab. Unfortunatly, variables do not take types (boolean in this case)
so we set a string to be 'true' or 'false' and we use eq to convert it to a boolean and pass it to
the templates:

```yml
stages:
- template: templates/device-tests-stage.yml
  parameters:
    iOSDevicePool: ${{ variables.iOSDevicePool }}
    WindowsDevicePool: ${{ variables.WindowsDevicePool }}
    useXamarinStorage: eq('${{ variables.useXamarinStorage }}', 'true')
    testsLabels: '--label=run-tvos-tests,run-non-monotouch-tests,run-monotouch-tests,run-mscorlib-tests'
    statusContext: 'VSTS: device tests tvOS (DDFun)'
    iOSDeviceDemand: 'tvos' 
```

In the above example we are interested in:

```yml
  useXamarinStorage: eq('${{ variables.useXamarinStorage }}', 'true')
```

eq will return true or false in the string comparison and that way we do have the correct type.
