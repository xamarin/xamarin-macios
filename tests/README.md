# Various tests to be executed prior to releases

## Test permissions

There are a number of tests that acccess the file system and the bluetooh. For these tests to correctly execute you have to ensure that
the terminal application that you are using to execute the tests has access to the full filesystem and the bluetooth. If you do not do this
step the macOS tests will crash.

## Test solutions

Many of the test solutions and test projects are generated, and will
only be available after running `make` once.

* tests.sln: This is the base test solution for Xamarin.iOS, which targets iOS using the Unified API. _Not_ generated.
* tests-tvos.sln: All the TVOS test projects. Generated.
* tests-watchos.sln: All the WatchOS test projects. Generated.
* tests-mac.sln: This is the base test solution for Xamarin.Mac. _Not_ generated.

### Test solution/project generation

The tool that generates the test solutions / projects is called xharness,
and lives in the xharness subdirectory.

## Types of Tests

### Unit Tests

Most of the projects are using NUnit[Lite] and looks like unit tests.
They are meant to be executed on the target: simulator, devices, OSX.

In reality most of them are regression tests - but that does not change
the need to execute and continually expand them to cover new code.


### Introspection Tests

Introspection tests are executed on target (both simulator and device for
iOS) or a specific version of OSX. The application proceed to analyze itself
using:

* `System.Reflection` for managed code; and
* the ObjectiveC runtime library for native code

and compare the results. E.g. if using .NET reflection it can see a binding
for a `NSBundle` type then it should be able to find a native `NSBundle` 
type using the ObjC runtime functions. Otherwise an error is raised...

Since the application analyze itself it must contains everything we wish
to test. That's why the introspection tests are part of the `dontlink.app`
application (for iOS) and the dontlink-mac project (for OSX).

Pros

* They always tell the truth, which can differ from documentation

Cons

* Incomplete - Not everything is encoded in the metadata / executable;
* Too complete - Not every truth is good to be known (or published)


### Extrospection Tests ###

Extrospection tests takes data from some outside sources and see if our
implementation match the information, e.g.

* Header files from the SDK;
* Rules, like Gendarme or FxCop;

Since this is done externally there's no need to run them on the devices,
simulator or even a specific version of OSX.

Pro

* There is more data available, e.g. information lost when compiling

Con

* The data might not represent the truth (errors, false positives...)

### Xamarin.Mac

Many tests when run for macOS use a integration [hack](https://github.com/xamarin/xamarin-macios/blob/main/tests/common/mac/MacMain.cs) which helps handle a number of issues:

- Allowing command line arguments to tests while excluding "psn" arguments passed in while debugging with Visual Studio for Mac
- Invoking `_exit` to work around a number of post-test hangs. See the [bug](https://bugzilla.xamarin.com/show_bug.cgi?id=52604) for details.
- Add a number of "default" excludes for mono BCL tests

One very useful "hack" this support adds is the ability to run a single test from the command line via the `XM_TEST_NAME` environmental variable. For example

```
XM_TEST_NAME=MonoTouchFixtures.Security.KeyTest.CreateRandomKeyWithParametersTests make run-mac-unified-xammac_tests
```

# Test Suites

## *-tests : where * is the assembly name, e.g. monotouch

Use the project defaults for linking, i.e. 

* "Don't link" for simulator

* "Link SDK assemblies only" for devices

## dontlink

* regression testing without using the linker

* both simulator and devices are set to "Don't link"

## linkall

* regression testing using the linker on the all assemblies

* "Link all assemblies" for both simulator/devices

## linksdk

* regression testing using the linker on the SDK assemblies

* "Link SDK assemblies only" for both simulator/devices

## bcl-test

These are the Mono BCL test suite tweaked to run on the mobile profile.
It reuse the files directly from mono's repository (linking, not copying).

As other unit tests the configuration is set to mimick normal apps, e.g.

* "Don't link" for simulator

* "Link SDK assemblies only" for devices


# Common make targets

Run every test in both the simulator and on device, using both the compat and the new profile (for the simulator both in 32 and 64bit mode).

    $ make run

Run every test in the simulator, using both the compat and the new profile (both 32 and 64bit simulators).

	$ make run-all-sim

Run every test on device, using both the compat and the new profile

	$ make run-all-dev

# Detailed make targets

* Main target

    * run-*what*-*where*-*project*: Builds, installs (if applicable) and runs the specified test project on the specified platform. This is the most common target to use.
    * build-*what*-*where*-*project*: Will build the specified test project for the specified platform and target.
    * install-*what*-*where*-*project*: Will install the specified test project on a connected device. There's currently no way to select the device, so ensure you've only one connected (if many devices are connected, it's random which will used).
    * exec-*what*-*where*-*project*: Will run the specified test project in the simulator or on a device.

* What

    * -ios-: iOS.
    * -tvos-: TVOS.
    * -watchos-: WatchOS

    If "What" is skipped, all variations are executed sequentially.

* Where

    * -simclassic-: Simulator using the Classic API. Only applicable when platform is iOS.
    * -simunified-: Simulator using Unified API. The build will contain both an i386 and an x86_64 binary. Only applicable to the build-* target, while the -sim32- and -sim64- are only applicable to the exec-* targets. Only applicable when the platform is iOS.
    * -sim32-: 32bits iOS simulator using the Unified API. Not applicable to other platforms.
    * -sim64-: 64bits iOS simulator using the Unified API. Not applicable to other platforms.
    * -sim-:
        * iOS: Both the -simclassic- and -simunified- versions.
        * WatchOS/TVOS: The WatchOS/TVOS simulator.
    * -devclassic-: Device using the Classic API. Only applicable when the platform is iOS.
    * -devunified-: Device using the Unified API. The build will contain both an armv7 and an arm64 binary. It's not possible to select a 32/64bit version, you'll run what your device supports. Only applicable when the platform is iOS.
    * -dev-:
        * iOS: Both the -devclassic- and -devunified- versions.
        * WatchOS/TVOS: A Watch or TV device.

* Examples

    $ make run-ios-sim32-monotouchtest: This will run `monotouch-test` using the Unified API in a 32-bit simulator.
    $ make run-tvos-dev-dont\ link: This will run `dont link` on an Apple TV device.

# Utility run-* targets

These targets will build, install (if applicable) and run the specified project(s).

* Simulator
    * run-sim-*project*: Builds and runs the specified test project in the simulator in compat, 32 and 64bit mode.
    * run-sim: Builds and runs all the non-bcl test projects in the simulator in compat, 32 and 64bit mode.

* Device
    * run-dev-*project*: Builds and runs the specified non-bcl test project on a device in compat and native mode (if it's 32 and 64bit depends on the device; 64bit devices will run in 64bit mode and the same for 32bit devices).
    * run-devcompat: Run all the non-bcl test projects on device, in compat mode.
    * run-devdual: Run all the non-bcl test projects on device, in native mode (if it's 32 and 64bit depends on the device; 64bit devices will run in 64bit mode and the same for 32bit devices).
    * run-dev: Run all the non-bcl test projects on device, in both compat and native mode.

