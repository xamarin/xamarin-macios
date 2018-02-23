---
id: 9F76162B-D622-45DA-996B-2FBF8017E208
title: Xamarin.iOS errors
dateupdated: 2017-06-26
---

[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)
[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)
[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)

# MT0xxx: mtouch error messages

E.g. parameters, environment, missing tools.

<!--
 MT0xxx mtouch itself, e.g. parameters, environment (e.g. missing tools)
 https://github.com/xamarin/xamarin-macios/blob/master/tools/mtouch/error.cs
	-->

### <a name="MT0000"/>MT0000: Unexpected error - Please fill a bug report at http://bugzilla.xamarin.com

An unexpected error condition occurred. Please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with as much information as possible, including:

* Full build logs, with maximum verbosity (e.g. `-v -v -v -v` in the **Additional mtouch arguments**);
* A minimal test case that reproduce the error; and
* All version informations

The easiest way to get exact version information is to use the **Xamarin Studio** menu, **About Xamarin Studio** item, **Show Details** button and copy/paste the version informations (you can use the **Copy Information** button).

### <a name="MT0001"/>MT0001: '-devname' was provided without any device-specific action

This is a warning that is emitted if -devname is passed to mtouch when no device-specific action (-logdev/-installdev/-killdev/-launchdev/-listapps) was requested.

### <a name="MT0002"/>MT0002: Could not parse the environment variable *.

This error happens if you try to set an invalid environment key=value variable pair. The correct format is: `mtouch --setenv=VARIABLE=VALUE`

### <a name="MT0003"/>MT0003: Application name '*.exe' conflicts with an SDK or product assembly (.dll) name.

The executable assembly's name and the application's name can't match the name of any dll in the app. Please modify the name of your executable.

### <a name="MT0004"/>MT0004: New refcounting logic requires SGen to be enabled too.

If you enable the refcounting extension you must also enable the SGen garbage collector in the project's iOS Build options (Advanced tab).

Starting with Xamarin.iOS 7.2.1 this requirement has been lifted, the new refcounting logic can be enabled with both Boehm and SGen Garbage Collectors.

### <a name="MT0005"/>MT0005: The output directory * does not exist.

Please create the directory.

This error is not generated anymore, mtouch will automatically create the directory if it doesn't exist.

### <a name="MT0006"/>MT0006: There is no devel platform at *, use --platform=PLAT to specify the SDK.

Xamarin.iOS cannot find the SDK directory at the location mentioned in the error message. Please verify that the path is correct.

### <a name="MT0007"/>MT0007: The root assembly * does not exist.

Xamarin.iOS cannot find the assembly at the location mentioned in the error message. Please verify that the path is correct.

### <a name="MT0008"/>MT0008: You should provide one root assembly only, found # assemblies: *.

More than one root assembly was passed to mtouch, while there can be only one root assembly.

### <a name="MT0009"/>MT0009: Error while loading assemblies: *.

An error occurred while loading the assemblies from the root assembly references. More information may be provided in the build output.

### <a name="MT0010"/>MT0010: Could not parse the command line arguments: *.

An error occurred while parsing the command line arguments. Please verify that they are all correct.

### <a name="MT0011"/>MT0011: * was built against a more recent runtime (*) than MonoTouch supports.

This warning is typically reported because the project has a reference to a class library that was not built using the Xamarin.iOS BCL.

The same way an app using the .NET 4.0 SDK may not work on a system only supporting .NET 2.0, a library built using .NET 4.0 may not work on Xamarin.iOS, it may use API not present on Xamarin.iOS.

The general solution is to build the library as a Xamarin.iOS Class Library. This can be accomplished by creating a new Xamarin.iOS Class Library project and add all the source files to it. If you do not have the source code for the library, you should contact the vendor and request that they provide a Xamarin.iOS-compatible version of their library.

### <a name="MT0012"/>MT0012: Incomplete data is provided to complete *.

This error is not reported anymore in the current version of Xamarin.iOS.

### <a name="MT0013"/>MT0013: Profiling support requires sgen to be enabled too.

SGen (--sgen) must be enabled if profiling (--profiling) is enabled.

### <a name="MT0014"/>MT0014: The iOS * SDK does not support building applications targeting *.

This can happen in the following circumstances:

*  ARMv6 is enabled and Xcode 4.5 or later is installed.
*  ARMv7s is enabled and Xcode 4.4 or earlier is installed.


Please verify that the installed version of Xcode supports the selected architectures.

### <a name="MT0015"/>MT0015: Invalid ABI: *. Supported ABIs are: i386, x86_64,  armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, arm64 and arm64+llvm.

An invalid ABI was passed to mtouch. Please specify a valid ABI.

### <a name="MT0016"/>MT0016: The option * has been deprecated.

The mentioned mtouch option has been deprecated and will be ignored.

### <a name="MT0017"/>MT0017: You should provide a root assembly.

It is required to specify a root assembly (typically the main executable) when building an app.

### <a name="MT0018"/>MT0018: Unknown command line argument: *.

Mtouch does not recognize the command line argument mentioned in the error message.

### <a name="MT0019"/>MT0019: Only one --[log|install|kill|launch]dev or --[launch|debug]sim option can be used.

There are several options for mtouch that can't be used simultaneously:

-  --logdev
-  --installdev
-  --killdev
-  --launchdev
-  --launchdebug
-  --launchsim




### <a name="MT0020"/>MT0020 The valid options for '\*' are '\*'.



### <a name="MT0021"/>MT0021 Cannot compile using gcc/g++ (--use-gcc) when using the static registrar (this is the default when compiling for device). Either remove the --use-gcc flag or use the dynamic registrar (--registrar:dynamic).



### <a name="MT0022"/>MT0022 The options '--unsupported--enable-generics-in-registrar' and '--registrar' are not compatible.

Remove both the options `--unsupported--enable-generics-in-registrar` and `--registrar`. Starting with Xamarin.iOS 7.2.1 the default registrar supports generics.

This error is no longer shown (the command-line argument `--unsupported--enable-generics-in-registrar` has been removed from mtouch).

### <a name="MT0023"/>MT0023 Application name '*.exe' conflicts with another user assembly.

The executable assembly's name and the application's name can't match the name of any dll in the app. Please modify the name of your executable.

### <a name="MT0024"/>MT0024 Could not find required file '*'.



### <a name="MT0025"/>MT0025 No SDK version was provided. Please add `--sdk=X.Y` to specify which iOS SDK should be used to build your application.



### <a name="MT0026"/>MT0026 Could not parse the command line argument '*': *



### <a name="MT0027"/>MT0027 The options '\*' and '\*' are not compatible.



### <a name="MT0028"/>MT0028 Cannot enable PIE (-pie) when targeting iOS 4.1 or earlier. Please disable PIE (-pie:false) or set the deployment target to at least iOS 4.2



### <a name="MT0029"/>MT0029: REPL (--enable-repl) is only supported in the simulator (--sim).

REPL is only supported if you're building for the simulator. This means that if you pass `--enable-repl` to mtouch, you must also pass `--sim`.

### <a name="MT0030"/>MT0030: The executable name (\*) and the app name (\*) are different, this may prevent crash logs from getting symbolicated properly.

When Xcode symbolicates (translates memory addresses to function names and file/line numbers) the process may fail if the executable and app have different names (without the extension).

To fix this either change 'Application Name' in the project's Build/iOS Application options, or change 'Assembly Name' in the project's Build/Output options.

### <a name="MT0031"/>MT0031: The command line arguments '--enable-background-fetch' and '--launch-for-background-fetch' require '--launchsim' too.

### <a name="MT0032"/>MT0032: The option '--debugtrack' is ignored unless '--debug' is also specified.

### <a name="MT0033"/>MT0033  A Xamarin.iOS project must reference either monotouch.dll or Xamarin.iOS.dll

### <a name="MT0034"/>MT0034  Cannot include both 'monotouch.dll' and 'Xamarin.iOS.dll' in the same Xamarin.iOS project - '\*' is referenced explicitly, while '\*' is referenced by '*'.

<!-- MT0035 unused -->

### <a name="MT0036"/>MT0036  Cannot launch a * simulator for a * app. Please enable the correct architecture(s) in your project's iOS Build options (Advanced page).

### <a name="MT0037"/>MT0037  monotouch.dll is not 64-bit compatible. Either reference Xamarin.iOS.dll, or do not build for a 64-bit architecture (ARM64 and/or x86_64).

### <a name="MT0038"/>MT0038  The old registrars (--registrar:oldstatic|olddynamic) are not supported when referencing Xamarin.iOS.dll.

### <a name="MT0039"/>MT0039  Applications targeting ARMv6 cannot reference Xamarin.iOS.dll.

### <a name="MT0040"/>MT0040  Could not find the assembly '\*', referenced by '\*'.

### <a name="MT0041"/>MT0041  Cannot reference both 'monotouch.dll' and 'Xamarin.iOS.dll'.

### <a name="MT0042"/>MT0042  No reference to either monotouch.dll or Xamarin.iOS.dll was found. A reference to monotouch.dll will be added.

### <a name="MT0043"/>MT0043: The Boehm garbage collector is currently not supported when referencing 'Xamarin.iOS.dll'. The SGen garbage collector has been selected instead.

Only the SGen garbage collector is supported with Unified projects. Ensure there are no additional mtouch flags specifying Boehm as the garbage collector.

### <a name="MT0044"/>MT0044: --listsim is only supported with Xcode 6.0 or later.

Install a newer Xcode version.

### <a name="MT0045"/>MT0045: --extension is only supported when using the iOS 8.0 (or later) SDK.

<!-- MT0046 is not reported anymore -->

### <a name="MT0047"/>MT0047: The minimum deployment target for Unified applications is 5.1.1, the current deployment target is '*'. Please select a newer deployment target in your project's iOS Application options.

<!-- MT0048 is not reported anymore -->

### <a name="MT0049"/>MT0049: *.framework is supported only if deployment target is 8.0 or later. * features might not work correctly.

The specified framework is not supported in the iOS version the deployment target refers to. Either update the deployment target to a newer iOS version, or remove usage of the specified framework from the app.

<!-- MT0050 is not reported anymore -->

### <a name="MT0051"/>MT0051: Xamarin.iOS * requires Xcode 5.0 or later. The current Xcode version (found in *) is *.

Install a newer Xcode.

### <a name="MT0052"/>MT0052: No command specified.

No action was specified for mtouch.

<!-- 0053 is used by mmp -->

### <a name="MT0054"/>MT0054: Unable to canonicalize the path '*': *

This is an internal error. If you see this error, please file a bug [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT0055"/>MT0055: The Xcode path '*' does not exist.

The Xcode path passed using `--sdkroot` does not exist. Please specify a valid path.

### <a name="MT0056"/>MT0056: Cannot find Xcode in the default location (/Applications/Xcode.app). Please install Xcode, or pass a custom path using --sdkroot <path>.
### <a name="MT0057"/>MT0057: Cannot determine the path to Xcode.app from the sdk root '*'. Please specify the full path to the Xcode.app bundle.

The path passed using `--sdkroot` does not specify a valid Xcode app. Please specify a path to an Xcode app.

### <a name="MT0058"/>MT0058: The Xcode.app '\*' is invalid (the file '\*' does not exist).

The path passed using `--sdkroot` does not specify a valid Xcode app. Please specify a path to an Xcode app.

### <a name="MT0059"/>MT0059: Could not find the currently selected Xcode on the system: *

### <a name="MT0060"/>MT0060: Could not find the currently selected Xcode on the system. 'xcode-select --print-path' returned '*', but that directory does not exist.

### <a name="MT0061"/>MT0061: No Xcode.app specified (using --sdkroot), using the system Xcode as reported by 'xcode-select --print-path': *

This is a informational warning, explaining which Xcode will be used, since none was specified.

### <a name="MT0062"/>MT0062: No Xcode.app specified (using --sdkroot or 'xcode-select --print-path'), using the default Xcode instead: *

This is a informational warning, explaining which Xcode will be used, since none was specified.

### <a name="MT0063"/>MT0063: Cannot find the executable in the extension * (no CFBundleExecutable entry in its Info.plist)

Every Info.plist must have an executable (using the CFBundleExecutable entry), however an entry should be generated automatically during the build.

This usually indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0064"/>MT0064: Xamarin.iOS only supports embedded frameworks with Unified projects.

Xamarin.iOS only supports embedded frameworks when using the Unified API; please update your project to use the Unified API.

### <a name="MT0065"/>MT0065: Xamarin.iOS only supports embedded frameworks when deployment target is at least 8.0 (current deployment target: * embedded frameworks: *)

Xamarin.iOS only supports embedded frameworks when the deployment target is at least 8.0 (because earlier versions of iOS does not support embedded frameworks).

Please update the deployment target in the project's Info.plist to 8.0 or higher.

### <a name="MT0066"/>MT0066: Invalid build registrar assembly: *

This usually indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0067"/>MT0067: Invalid registrar: *

This usually indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0068"/>MT0068: Invalid value for target framework: *.

An invalid target framework was passed using the --target-framework argument. Please specify a valid target framework.

<!--### <a name="MT0069"/>MT0069: currently unused -->

### <a name="MT0070"/>MT0070: Invalid target framework: *. Valid target frameworks are: *.

An invalid target framework was passed using the --target-framework argument. Please specify a valid target framework.

### <a name="MT0071"/>MT0071: Unknown platform: *. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.

This usually indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0072"/>MT0072: Extensions are not supported for the platform '*'.

This usually indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0073"/>MT0073: Xamarin.iOS * does not support a deployment target of * (the minimum is *). Please select a newer deployment target in your project's Info.plist.

The minimum deployment target is the one specified in the error message; please select a newer deployment target in the project's Info.plist.

If updating the deployment target is not possible, then please use an older version of Xamarin.iOS.

### <a name="MT0074"/>MT0074: Xamarin.iOS * does not support a deployment target of * (the maximum is *). Please select an older deployment target in your project's Info.plist or upgrade to a newer version of Xamarin.iOS.

Xamarin.iOS does not support setting the minimum deployment target to a higher version than the version this particular version of Xamarin.iOS was built for.

Please select an older minimum deployment target in the project's Info.plist, or upgrade to a newer version of Xamarin.iOS.

### <a name="MT0075"/>MT0075: Invalid architecture '*' for * projects. Valid architectures are: *

An invalid architecture was specified. Please verify that architecture is valid.

### <a name="MT0076"/>MT0075: No architecture specified (using the --abi argument). An architecture is required for * projects.

This usually indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0077"/>MT0076: WatchOS projects must be extensions.

This usually indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0078"/>MT0077: Incremental builds are enabled with a deployment target < 8.0 (currently *). This is not supported (the resulting application will not launch on iOS 9), so the deployment target will be set to 8.0.

This is a warning informing that the deployment target has been set to 8.0 for this build so that incremental builds work properly.

Incremental builds are only supported when the deployment target is at least 8.0 (because the resulting application will not launch on iOS 9 otherwise).

### <a name="MT0079"/>MT0078: The recommended Xcode version for Xamarin.iOS * is Xcode * or later. The current Xcode version (found in *) is *.

This is a warning informing that the current version of Xcode is not the recommended version of Xcode for this version of Xamarin.iOS.

Please upgrade Xcode in order to ensure optimal behavior.

### <a name="MT0080"/>MT0080: Disabling NewRefCount, --new-refcount:false, is deprecated.

This is a warning informing that the request to disable the new refcount (--new-refcount:false) has been ignored.

The new refcount feature is now mandatory for all projects, and it's thus not possible to disable anymore.

### <a name="MT0081"/>MT0081: The command line argument --download-crash-report also requires --download-crash-report-to.
### <a name="MT0082"/>MT0082: REPL (--enable-repl) is only supported when linking is not used (--nolink).
### <a name="MT0083"/>MT0083: Asm-only bitcode is not supported on watchOS. Use either --bitcode:marker or --bitcode:full.
### <a name="MT0084"/>MT0084: Bitcode is not supported in the simulator. Do not pass --bitcode when building for the simulator.
### <a name="MT0085"/>MT0085: No reference to '*' was found. It will be added automatically.
### <a name="MT0086"/>MT0086: A target framework (--target-framework) must be specified when building for TVOS or WatchOS.

This usually indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0087"/>MT0087: Incremental builds (--fastdev) is not supported with the Boehm GC. Incremental builds will be disabled.

### <a name="MT0088"/>MT0088: The GC must be in cooperative mode for watchOS apps. Please remove the --coop:false argument to mtouch.

### <a name="MT0089"/>MT0089: The option '\*' cannot take the value '\*' when cooperative mode is enabled for the GC.

### <a name="MT0091"/>MT0091: This version of Xamarin.iOS requires the * SDK (shipped with Xcode *). Either upgrade Xcode to get the required header files or set the managed linker behaviour to Link Framework SDKs Only (to try to avoid the new APIs).

Xamarin.iOS requires the header files, from the SDK version specified in the error message, to build your application. The recommended way to fix this error is to upgrade Xcode to get the required SDK, this will include all the required header files. If you have multiple versions of Xcode installed, or want to use an Xcode in a non-default location, make sure to set the correct Xcode location in your IDE's preferences.

A potential, alternative solution is to enable the managed linker. This will remove unused API including, in most cases, the new API where the header files are missing (or incomplete). However this will not work if your project uses API that was introduced in a newer SDK than the one your Xcode provides.

A last-straw solution would be to use an older version of Xamarin.iOS, one that supports the SDK your project requires.

<!-- MT0092 used by mlaunch -->

### <a name="MT0093"/>MT0093: Could not find 'mlaunch'.

<!-- MT0094 is not reported anymore -->

### <a name="MT0095"/>MT0095: Aot files could not be copied to the destination directory {dest}: {error}

### <a name="MT0096"/>MT0096: No reference to Xamarin.iOS.dll was found.

<!-- MT0097: used by mmp -->
<!-- MT0098: used by mmp -->

### <a name="MT0099"/>MT0099: Internal error *. Please file a bug report with a test case (http://bugzilla.xamarin.com).

This error message is reported when an internal consistency check in Xamarin.iOS fails.

This indicates a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0100"/>MT0100: Invalid assembly build target: '*'. Please file a bug report with a test case (http://bugzilla.xamarin.com).

This error message is reported when an internal consistency check in Xamarin.iOS fails.

This is always a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

### <a name="MT0101"/>MT0101: The assembly '*' is specified multiple times in --assembly-build-target arguments.

The assembly mentioned in the error message is specified multiple times in --assembly-build-target arguments. Please make sure each assembly is only mentioned once.

### <a name="MT0102"/>MT0102: The assemblies '\*' and '\*' have the same target name ('\*'), but different targets ('\*' and '*').

The assemblies mentioned in the error message have conflicting build targets.

For example:

    --assembly-build-target:Assembly1.dll=framework=MyBinary --assembly-build-target:Assembly2.dll=dynamiclibrary=MyBinary

This example is trying to create both a dynamic library and a framework with the same make (`MyBinary`).

### <a name="MT0103"/>MT0103: The static object '\*' contains more than one assembly ('\*'), but each static object must correspond with exactly one assembly.

The assemblies mentioned in the error message are all compiled to a single static object. This is not allowed, every assembly must be compiled to a different static object.

For example:

    --assembly-build-target:Assembly1.dll=staticobject=MyBinary --assembly-build-target:Assembly2.dll=staticobject=MyBinary

This example tries to build a static object (`MyBinary`) comprised of two assemblies (`Assembly1.dll` and `Assembly2.dll`), which is not allowed.

### <a name="MT0105"/>MT0105: No assembly build target was specified for '*'.

When specifying the assembly build target using `--assembly-build-target`, every assembly in the app must have a build target assigned.

This error is reported when the assembly mentioned in the error message does not have an assembly build target assigned.

See the documentation about `--assembly-build-target` for further information.

### <a name="MT0106"/>MT0106: The assembly build target name '\*' is invalid: the character '\*' is not allowed.

The assembly build target name must be a valid filename.

For example these values will trigger this error:

    --assembly-build-target:Assembly1.dll=staticobject=my/path.o

because `my/path.o` is not a valid filename due to the directory separator character.

### <a name="MT0107"/>MT0107: The assemblies '\*' have different custom LLVM optimizations (\*), which is not allowed when they are all compiled to a single binary.

### <a name="MT0108"/>MT0108: The assembly build target '*' did not match any assemblies.

### <a name="MT0109"/>MT0109: The assembly '{0}' was loaded from a different path than the provided path (provided path: {1}, actual path: {2}).

This is a warning indicating that an assembly referenced by the application was loaded from a different location than requested.

This might mean that the app is referencing multiple assemblies with the same name, but from different locations, which might lead to unexpected results (only the first assembly will be used).

### <a name="MT0110"/>MT0110: Incremental builds have been disabled because this version of Xamarin.iOS does not support incremental builds in projects that include third-party binding libraries and that compiles to bitcode.

Incremental builds have been disabled because this version of Xamarin.iOS does not support incremental builds in projects that include third-party binding libraries and that compiles to bitcode (tvOS and watchOS projects).

No action is required on your part, this message is purely informational.

For further information see bug #[51710](https://bugzilla.xamarin.com/show_bug.cgi?id=51710).

This warning is not reported anymore.

### <a name="MT0111"/>MT0111: Bitcode has been enabled because this version of Xamarin.iOS does not support building watchOS projects using LLVM without enabling bitcode.

Bitcode has been enabled automatically because this version of Xamarin.iOS does not support building watchOS projects using LLVM without enabling bitcode.

No action is required on your part, this message is purely informational.

For further information see bug #[51634](https://bugzilla.xamarin.com/show_bug.cgi?id=51634).

### <a name="MT0112"/>MT0112: Native code sharing has been disabled because *

There are multiple reasons code sharing can be disabled:

* because the container app's deployment target is earlier than iOS 8.0 (it's *)).

Native code sharing requires iOS 8.0 because native code sharing is implemented using user frameworks, which was introduced with iOS 8.0.

* because the container app includes I18N assemblies (*).

Native code sharing is currently not supported if the container app includes I18N assemblies.

* because the container app has custom xml definitions for the managed linker (*).

Native code sharing requires is not supported for projects that use custom xml definitions for the managed linker.


### <a name="MT0113"/>MT0113: Native code sharing has been disabled for the extension '*' because *.

* because the bitcode options differ between the container app (\*) and the extension (\*).

  Native code sharing requires that the bitcode options match between the projects that share code.

* because the --assembly-build-target options are different between the container app (\*) and the extension (\*).

  Native code sharing requires that the --assembly-build-target options are identical between the projects that share code.

  This condition can occur if incremental builds are not either enabled or disabled in all the projects.

* because the I18N assemblies are different between the container app (\*) and the extension (\*).

  Native code sharing is currently not supported for extensions that include I18N assemblies.

* because the arguments to the AOT compiler are different between the container app (\*) and the extension (\*).

  Native code sharing requires that the arguments to the AOT compiler do not differ between projects that share code.

* because the other arguments to the AOT compiler are different between the container app (\*) and the extension (\*).

  Native code sharing requires that the arguments to the AOT compiler do not differ between projects that share code.

  This condition occurs if the 'Perform all 32-bit float operations as 64-bit float' differs between projects.

* because LLVM is not enabled or disabled in both the container app (\*) and the extension (\*).

  Native code sharing requires that LLVM is either enabled or disabled for all projects that share code.

* because the managed linker settings are different between the container app (\*) and the extension (\*).

  Native code sharing requires that the managed linker settings are identical for all projects that share code.

* because the skipped assemblies for the managed linker are different between the container app (\*) and the extension (\*).

  Native code sharing requires that the managed linker settings are identical for all projects that share code.

* because the extension has custom xml definitions for the managed linker (*).

  Native code sharing requires is not supported for projects that use custom xml definitions for the managed linker.

* because the container app does not build for the ABI * (while the extension is building for this ABI).

  Native code sharing requires that the container app builds for all the architectures any app extension builds for.

  For instance: this condition occurs when an extension builds for ARM64+ARMv7, but the container app only builds for ARM64.

* because the container app is building for the ABI \*, which is not compatible with the extension's ABI (\*).

  Native code sharing requires that all the projects build for the exact same API.

  For instance: this condition occurs when an extension builds for ARMv7+llvm+thumb2, but the container app only builds for ARMv7+llvm.

* because the container app is referencing the assembly '\*' from '\*', while the extension references a different version from '*'.

  Native code sharing requires that all the projects that share code use the same versions for all assemblies.

<!-- MT0114: used by mmp -->

### <a name="MT0115"/>MT0115: It is recommended to reference dynamic symbols using code (--dynamic-symbol-mode=code) when bitcode is enabled.

Xamarin.iOS projects will often reference native symbols dynamically, which
means that the native linker might remove such native symbols during the
native linking process, because the native linker does not see that these
symbols are used.

Usually Xamarin.iOS will ask the native linker to keep such symbols (using the
`-u symbol` linker flag), but when compiling for bitcode the native linker
does not accept the `-u` flag.

Xamarin.iOS has implemented an alternative solution: we generate extra native
code which references these symbols, and thus the native linker will see that
these symbols are used. This is automatically done when compiling to bitcode.

If `--dynamic-symbol-mode=linker` is passed to mtouch, this alternative
solution will be disabled, and Xamarin.iOS will try to pass `-u` to the native
linker. This will most likely result in native linker errors.

The solution is to remove the `--dynamic-symbol-mode=linker` argument from the additional mtouch arguments in the project's Build options.

### <a name="MT0116"/>MT0116: Invalid architecture: {arch}. 32-bit architectures are not supported when deployment target is 11 or later. Make sure the project does not build for a 32-bit architecture.

iOS 11 does not contain support for 32-bit applications, so it's not supported
to build for a 32-bit application when the deployment target is iOS 11 or
later.

Either change the target architecture in the project's iOS build options to
arm64, or change the deployment target in the project's Info.plist to an
earlier iOS version.

### <a name="MT0117"/>MT0117: Can't launch a 32-bit app on a simulator that only supports 64-bit.

### <a name="MT0118"/>MT0116: Aot files could not be found at the expected directory '{msymdir}'.

<!-- 0119 - 0123: free to use -->

### <a name="MT0123"/>MT0123: The executable assembly * does not reference *.

No reference could be found to the platform assembly (Xamarin.iOS.dll / Xamarin.TVOS.dll / Xamarin.WatchOS.dll) in the executable assembly.

This typically happens where there is no code in the executable project that uses anything from the platform assembly; for instance an empty Main method (and no other code) would show this error:

```csharp
class Program {
    void Main (string[] args)
    {
    }
}
```

Using an API from the platform assembly will solve the error:

```csharp
class Program {
    void Main (string[] args)
    {
        System.Console.WriteLine (typeof (UIKit.UIWindow));
    }
}
```

### <a name="MT0124"/>MT0124: Could not set the current language to '{lang}' (according to LANG={LANG}): {exception}

This is a warning, indicating that the current language couldn't be set to the language in the error message.

The current language will default to the system language.

### <a name="MT0125"/>MT0125: The --assembly-build-target command-line argument is ignored in the simulator.

No action is required, this message is purely informational.

### <a name="MT0126"/>MT0126: Incremental builds have been disabled because incremental builds are not supported in the simulator.

No action is required, this message is purely informational.

### <a name="MT0127"/>MT0127: Incremental builds have been disabled because this version of Xamarin.iOS does not support incremental builds in projects that include more than one third-party binding libraries.

Incremental builds have been disabled automatically because this version of Xamarin.iOS does not always build projects with multiple third-party binding libraries correctly.

No action is required, this message is purely informational.

For further information see bug #[52727](https://bugzilla.xamarin.com/show_bug.cgi?id=52727).

### <a name="MT0128"/>MT0128: Could not touch the file '*': *

A failure occurred when touching a file (which is done to ensure partial builds are done correctly).

This warning can most likely be ignored; in case of any problems file a bug (https://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS)) and it will be investigated.

### <a name="MT0129"/>MT0129: Debugging symbol file for '*' does not match the assembly and is ignored.

The debugging symbols, either a .pdb (portable pdb only) or a .mdb file, for the specified assembly could not be loaded.

This generally means the assembly is newer or older than the symbols. Since they do not match they cannot be used and the symbols are ignored.

This warning won't affect the application being built, however you might not be able to debug it entirely (in particular the code from specified assembly). Also exceptions, stack traces and crash reports might be missing some information.

Please report this issue to the publisher of the assembly package (e.g. nuget author) so this can be fixed in their future releases.

### <a name="MT0130"/>MT0130: No root assemblies found. You should provide at least one root assembly.
When running --runregistrar, at least one root assembly should be provided.

### <a name="MT0131"/>MT0131: Product assembly '{0}' not found in assembly list: '{1}'

When running --runregistrar, the assembly list should include the product assembly, Xamarin.iOS, Xamarin.WatchOS, Xamarin.TVOS.

### <a name="MT0132/>MT0132: Unknown optimization: *. Valid values are: *

The specified optimization was not recognized.

The accepted format is `[+|-]optimization-name`, where `optimization-name` is one of the values listed in the error message.

See [Build optimizations](https://developer.xamarin.com/guides/cross-platform/macios/build-optimization/) for a complete description of each optimization.

### <a name="MT0133"/>MT0133: Found more than 1 assembly matching '{0}' choosing first: '{1}'

When using --recursive-directories, only 1 assembly should match

<!-- 0134 used by mmp -->

# MT1xxx: Project related error messages

### MT10xx: Installer / mtouch

<!--
 MT1xxx file copy / symlinks (project related)
  MT10xx installer.cs / mtouch.cs
  -->

### <a name="MT1001"/>MT1001 Could not find an application at the specified directory



### <a name="MT1002"/>MT1002 Could not create symlinks, files were copied



### <a name="MT1003"/>MT1003 Could not kill the application '*'. You may have to kill the application manually.



### <a name="MT1004"/>MT1004 Could not get the list of installed applications.



### <a name="MT1005"/>MT1005 Could not kill the application '\*' on the device '\*': *- You may have to kill the application manually.



### <a name="MT1006"/>MT1006 Could not install the application '\*' on the device '\*': *.



### <a name="MT1007"/>MT1007 Failed to launch the application '\*' on the device '\*': *. You can still launch the application manually by tapping on it.



### <a name="MT1008"/>MT1008: Failed to launch the simulator



This error is reported if mtouch failed to launch the
  simulator.   This can happen sometimes because there is
  already a stale or dead simulator process running.

The following command issued on the Unix command line can
  be used to kill stuck simulator processes:

```
$ launchctl list|grep UIKitApplication|awk '{print $3}'|xargs launchctl remove
```

### <a name="MT1009"/>MT1009 Could not copy the assembly '\*' to '\*': *

This is a known issue in certain versions of Xamarin.iOS.

If this occurs to you, try the following workaround:

    sudo chmod 0644 /Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/*/*.mdb

However, since this issue has been resolved in the latest version of
Xamarin.iOS, please file a new bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS)
with your full version information and build log output.

### <a name="MT1010"/>MT1010 Could not load the assembly '*': *



### <a name="MT1011"/>MT1011 Could not add missing resource file: '*'



### <a name="MT1012"/>MT1012 Failed to list the apps on the device '*': *



### <a name="MT1013"/>MT1013 Dependency tracking error: no files to compare. Please file a bug report at http://bugzilla.xamarin.com with a test case.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test caes.

### <a name="MT1014"/>MT1014 Failed to re-use cached version of '*': *.



### <a name="MT1015"/>MT1015  Failed to create the executable '*': *

### <a name="MT1015"/>MT1015  Failed to create the executable '*': *

### <a name="MT1016"/>MT1016: Failed to create the NOTICE file because a directory already exists with the same name.

Remove the directory `NOTICE` from the project.

### <a name="MT1017"/>MT1017: Failed to create the NOTICE file: *.

### <a name="MT1018"/>MT1018: Your application failed code-signing checks and could not be installed on the device '*'. Check your certificates, provisioning profiles, and bundle ids. Probably your device is not part of the selected provisioning profile (error: 0xe8008015).
### <a name="MT1019"/>MT1019: Your application has entitlements not supported by your current provisioning profile and could not be installed on the device '*'. Please check the iOS Device Log for more detailed information (error: 0xe8008016).

This can happen if:

* Your application has entitlements that the current provisioning profile does not support.
  Possible solutions:
  - Specify a different provisioning profile that supports the entitlements your application needs.
  - Remove the entitlements not supported in current provisioning profile.
* The device you're trying to deploy to is not included in the provisioning profile you're using.
  Possible solutions:
  - Create a new app from a template in Xcode, select the same provisioning profile, and deploy to same device. Sometimes Xcode can automatically refresh provisioning profiles with new devices (in other cases Xcode will ask you what to do).
  -Go to the iOS Dev Center and update the provisioning profile with the new device, then download the updated provisioning profile to your machine.


In most cases more information about the failure will be printed to the iOS Device Log, which can help diagnosing the issue.

### <a name="MT1020"/>MT1020: Failed to launch the application '\*' on the device '\*': *

### <a name="MT1021"/>MT1021: Could not copy the file '\*' to '\*': {2}

A file could not be copied. The error message from the copy operation has more information about the error.

### <a name="MT1022"/>MT1022: Could not copy the directory '\*' to '\*': {2}

A directory could not be copied. The error message from the copy operation has more information about the error.

### <a name="MT1023"/>MT1023: Could not communicate with the device to find the application '*' : *

An error occurred when trying to lookup an application on device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.

### <a name="MT1024"/>MT1024: The application signature could not be verified on device '*'. Please make sure that the provisioning profile is installed and not expired (error: 0xe8008017).

The device rejected the application install because the signature could not be verified.

Please make sure that the provisioning profile is installed and not expired.

### <a name="MT1025"/>MT1025: Could not list the crash reports on the device *.

An error occurred when trying to list the crash reports on the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

### <a name="MT1026"/>MT1026: Could not download the crash report * from the device *.

An error occurred when trying to download the crash reports from the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

### <a name="MT1027"/>MT1027: Can't use Xcode 7+ to launch applications on devices with iOS * (Xcode 7 only supports iOS 6+).

It is not possible to use Xcode 7+ to launch applications on devices with iOS version below 6.0.

Please use an older version of Xcode, or tap on the app manually to launch it.

### <a name="MT1028"/>MT1028: Invalid device specification: '*'. Expected 'ios', 'watchos' or 'all'.

The device specification passed using --device is invalid. Valid values are: 'ios', 'watchos' or 'all'.

### <a name="MT1029"/>MT1029: Could not find an application at the specified directory: *

The application path passed to --launchdev does not exist. Please specify a valid app bundle.

### <a name="MT1030"/>MT1030: Launching applications on device using a bundle identifier is deprecated. Please pass the full path to the bundle to launch.

It's recommended to pass the path to the app to launch on device instead of just the bundle id.

### <a name="MT1031"/>MT1031: Could not launch the app '\*' on the device '\*' because the device is locked. Please unlock the device and try again.

Please unlock the device and try again.

### <a name="MT1032"/>MT1032: This application executable might be too large (* MB) to execute on device. If bitcode was enabled you might want to disable it for development, it is only required to submit applications to Apple.

### <a name="MT1033"/>MT1033: Could not uninstall the application '\*' from the device '\*': *

<!-- 1034 used by mmp -->

### <a name="MT1035"/>MT1035: Cannot include different versions of the framework '{name}'

It is not possible to link with different versions of the same framework.

This typically happens when an extension references a different version of a framework than the container app (possibly through a third-party binding assembly).

Following this error there will be multiple [MT1036](#MT1036) errors listing the paths for each different framework.

### <a name="MT1036"/>MT1036: Framework '{name}' included from: {path} (Related to previous error)

This error is reported only together with [MT1036](#MT1036). Please see [MT1036](#MT1036) for more information.

### MT11xx: Debug Service

<!--
  MT11xx DebugService.cs
  -->

### <a name="MT1101"/>MT1101 Could not start app



### <a name="MT1102"/>MT1102 Could not attach to the app (to kill it): *



### <a name="MT1103"/>MT1103 Could not detach



### <a name="MT1104"/>MT1104 Failed to send packet: *



### <a name="MT1105"/>MT1105 Unexpected response type



### <a name="MT1106"/>MT1106 Could not get list of applications on the device: Request timed out.



### <a name="MT1107"/>MT1107: Application failed to launch: *

Please check if your device is locked.

If you're deploying an enterprise app or using a free provisioning profile, you might have trust the developer (this is explained <a href="http://stackoverflow.com/a/30726375/183422">here</a>).

### <a name="MT1108"/>MT1108: Could not find developer tools for this XX (YY) device.

A few operations from mtouch require the <tt>DeveloperDiskImage.dmg</tt> file to be present.   This
	file is part of Xcode and is usually located relative to the
	SDK that you are using to build against, in
	the <tt>Xcode.app/Contents/Developer/iPhoneOS.platform/DeviceSupport/VERSION/DeveloperDiskImage.dmg</tt>.

This error can happen either because you do not have a
	DeveloperDiskImage.dmg that matches the device that you have
	connected.


### <a name="MT1109"/>MT1109: Application failed to launch because the device is locked. Please unlock the device and try again.

Please check if your device is locked.

### <a name="MT1110"/>MT1110: Application failed to launch because of iOS security restrictions. Please ensure the developer is trusted.

If you're deploying an enterprise app or using a free provisioning profile, you might have trust the developer (this is explained <a href="http://stackoverflow.com/a/30726375/183422">here</a>).

### <a name="MT1111"/>MT1111: Application launched successfully, but it's not possible to wait for the app to exit as requested because it's not possible to detect app termination when launching using gdbserver.

### MT12xx: Simulator

<!--
  MT12xx simcontroller.cs
  -->

### <a name="MT1201"/>MT1201: Could not load the simulator: *
### <a name="MT1202"/>MT1202: Invalid simulator configuration: *
### <a name="MT1203"/>MT1203: Invalid simulator specification: *
### <a name="MT1204"/>MT1204: Invalid simulator specification '*': runtime not specified.
### <a name="MT1205"/>MT1205: Invalid simulator specification '*': device type not specified.
### <a name="MT1206"/>MT1206: Could not find the simulator runtime '*'.
### <a name="MT1207"/>MT1207: Could not find the simulator device type '*'.
### <a name="MT1208"/>MT1208: Could not find the simulator runtime '*'.
### <a name="MT1209"/>MT1209: Could not find the simulator device type '*'.
### <a name="MT1210"/>MT1210: Invalid simulator specification: \*, unknown key '\*'
### <a name="MT1211"/>MT1211: The simulator version '\*' does not support the simulator type '\*'
### <a name="MT1212"/>MT1212: Failed to create a simulator version where type = * and runtime = *.
### <a name="MT1213"/>MT1213: Invalid simulator specification for Xcode 4: *
### <a name="MT1214"/>MT1214: Invalid simulator specification for Xcode 5: *
### <a name="MT1215"/>MT1215: Invalid SDK specified: *
### <a name="MT1216"/>MT1216: Could not find the simulator UDID '*'.
### <a name="MT1217"/>MT1217: Could not load the app bundle at '*'.
### <a name="MT1218"/>MT1218: No bundle identifier found in the app at '*'.
### <a name="MT1219"/>MT1219: Could not find the simulator for '*'.
### <a name="MT1220"/>MT1220: Could not find the latest simulator runtime for device '*'.

This usually indicates a problem with Xcode.

Things to try to fix this:

* Use the simulator once in Xcode.
* Pass an explicit SDK version using --sdk <version>.
* Reinstall Xcode.

### <a name="MT1221"/>MT1221: Could not find the paired iPhone simulator for the WatchOS simulator '*'.

When launching a WatchOS app in a WatchOS simulator, there must be a paired iOS Simulator as well.

Watch simulators can be paired with iOS Simulators using Xcode's Devices UI (menu Window -> Devices).

### MT13xx: [LinkWith]

<!--
  MT13xx [LinkWith]
  -->


### <a name="MT1301"/>MT1301 Native library `*` (\*) was ignored since it does not match the current build architecture(s) (\*)



### <a name="MT1302"/>MT1302 Could not extract the native library '*' from '+'. Please ensure the native library was properly embedded in the managed assembly (if the assembly was built using a binding project, the native library must be included in the project, and its Build Action must be 'ObjcBindingNativeLibrary').

### <a name="MT1303"/>MT1303: Could not decompress the native framework '\*' from '\*'. Please review the build log for more information from the native 'zip' command.

Could not decompress the specified native framework from the binding library.

Please review the bulid log for more information about this failure from the native 'zip' command.

### <a name="MT1304"/>MT1304: The embedded framework '*' in * is invalid: it does not contain an Info.plist.

The specified embedded framework does not contain an Info.plist, and is therefore not a valid framework.

Please make sure the framework is valid.

### <a name="MT1305"/>MT1305: The binding library '\*' contains a user framework (\*), but embedded user frameworks require iOS 8.0 (the current deployment target is *). Please set the deployment target in the Info.plist file to at least 8.0.

The specified binding library contains an embedded framework, but Xamarin.iOS only supports embedded frameworks on iOS 8.0 or later.

Please set the deployment target in the Info.plist file to at least 8.0 to solve this error (or don't use embedded frameworks).

### MT14xx: Crash Reports

<!--
  MT14xx	CrashReports.cs
  -->

### <a name="MT1400"/>MT1400: Could not open crash report service: AFCConnectionOpen returned *

An error occurred when trying to access crash reports from the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

### <a name="MT1401"/>MT1401: Could not close crash report service: AFCConnectionClose returned *

An error occurred when trying to access crash reports from the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

### <a name="MT1402"/>MT1402: Could not read file info for *: AFCFileInfoOpen returned *

An error occurred when trying to access crash reports from the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

### <a name="MT1403"/>MT1403: Could not read crash report: AFCDirectoryOpen (*) returned: *

An error occurred when trying to access crash reports from the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

### <a name="MT1404"/>MT1404: Could not read crash report: AFCFileRefOpen (*) returned: *

An error occurred when trying to access crash reports from the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

### <a name="MT1405"/>MT1405: Could not read crash report: AFCFileRefRead (*) returned: *

An error occurred when trying to access crash reports from the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

### <a name="MT1406"/>MT1406: Could not list crash reports: AFCDirectoryOpen (*) returned: *

An error occurred when trying to access crash reports from the device.

Things to try to solve this:

* Delete the application from the device and try again.
* Disconnect the device and reconnect it.
* Reboot the device.
* Reboot the Mac.
* Synchronize the device with iTunes (this will remove any crash reports from the device).

<!--- 1407 used by mmp -->

### MT16xx: MachO

<!--
  MT16xx	MachO.cs
  -->

### <a name="MT1600"/>MT1600: Not a Mach-O dynamic library (unknown header '0x*'): *.

An error occurred while processing the dynamic library in question.

Please make sure the dynamic library is a valid Mach-O dynamic library.

The format of a library can be verified using the `file` command from a terminal:

    file -arch all -l /path/to/library.dylib

### <a name="MT1601"/>MT1601: Not a static library (unknown header '*'): *.

An error occurred while processing the static library in question.

Please make sure the static library is a valid Mach-O static library.

The format of a library can be verified using the `file` command from a terminal:

    file -arch all -l /path/to/library.a

### <a name="MT1602"/>MT1602: Not a Mach-O dynamic library (unknown header '0x*'): *.

An error occurred while processing the dynamic library in question.

Please make sure the dynamic library is a valid Mach-O dynamic library.

The format of a library can be verified using the `file` command from a terminal:

    file -arch all -l /path/to/library.dylib

### <a name="MT1603"/>MT1603: Unknown format for fat entry at position * in *.

An error occurred while processing the fat archive in question.

Please make sure the fat archive is valid.

The format of a fat archive can be verified using the `file` command from a terminal:

    file -arch all -l /path/to/file

### <a name="MT1604"/>MT1604: File of type * is not a MachO file (*).

An error occurred while processing the MachO file in question.

Please make sure the file is a valid Mach-O dynamic library.

The format of a file can be verified using the `file` command from a terminal:

    file -arch all -l /path/to/file

# MT2xxx: Linker error messages

<!--
 MT2xxx Linker
  MT20xx Linker (general) errors
  -->

### <a name="MT2001"/>MT2001 Could not link assemblies

This error means that the managed linker encountered an unexpected error, e.g. an exception, and could not complete or save the assembly being processed. More information about the exact error will be part of the build log, e.g.

```
error MT2001: Could not link assemblies.
	Method: `System.Void Todo.TodoListPageCS/<<-ctor>b__1_0>d::SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine)`
	Assembly: `QuickTodo, Version=1.0.6297.28241, Culture=neutral, PublicKeyToken=null`
Reason: Value cannot be null.
Parameter name: instruction
```

It is important to file a bug report for such issues. In most case a workaround can be provided until a proper fix is published. The above information is critical (along with a test case and/or the assembly binairy) to resolve the issue.


### <a name="MT2002"/>MT2002 Can not resolve reference: *



### <a name="MT2003"/>MT2003 Option '*' will be ignored since linking is disabled



### <a name="MT2004"/>MT2004 Extra linker definitions file '*' could not be located.



### <a name="MT2005"/>MT2005 Definitions from '*' could not be parsed.



### <a name="MT2006"/>MT2006: Can not load mscorlib.dll from: *. Please reinstall Xamarin.iOS.

This generally indicates that there is a problem with your Xamarin.iOS installation. Please try reinstalling Xamarin.iOS.

<!--- 2007 used by mmp -->
<!--- 2009 used by mmp -->

### <a name="MT2010"/>MT2010: Unknown HttpMessageHandler `*`. Valid values are HttpClientHandler (default), CFNetworkHandler or NSUrlSessionHandler

### <a name="MT2011"/>MT2011: Unknown TlsProvider `*`.  Valid values are default or appletls.

The value given to `tls-provider=` is not a valid TLS (Transport Layer Security) provider.

The `default` and `appletls` are the only valid values and both represent the same option, which is to provide the SSL/TLS support using the native Apple TLS API.

<!--- 2012 used by mmp -->
<!--- 2013 used by mmp -->
<!--- 2014 used by mmp -->

### <a name="MT2015"/>MT2015: Invalid HttpMessageHandler `*` for watchOS. The only valid value is NSUrlSessionHandler.

This is a warning that occurs because the project file specifies an invalid HttpMessageHandler.

Earlier versions of our preview tools generated by default an invalid value in the project file.

To fix this warning, open the project file in a text editor, and remove all HttpMessageHandler nodes from the XML.

### <a name="MT2016"/>MT2016: Invalid TlsProvider `legacy` option. The only valid value `appletls` will be used.

The `legacy` provider, which was a fully managed SSLv3 / TLSv1 only provider, is not shipped with Xamarin.iOS anymore. Projects that were using this old provider and now build with the newer `appletls` one.

To fix this warning, open the project file in a text editor, and remove all `MtouchTlsProvider`` nodes from the XML.

### <a name="MT2017"/>MT2017: Could not process XML description.

This means there is an error on the [custom XML linker configuration file](https://developer.xamarin.com/guides/cross-platform/advanced/custom_linking/) you provided, please review your file.

### <a name="MT2018"/>MT2018: The assembly '\*' is referenced from two different locations: '\*' and '*'.

The assembly mentioned in the error message is loaded from multiple locations. Make sure to always use the same version of an assembly.

### <a name="MT2019"/>MT2019: Can not load the root assembly '*'

The root assembly could not be loaded. Please verify that the path in the error message refers to an existing file, and that it's a valid .NET assembly.

### <a name="MT202x"/>MT202x: Binding Optimizer failed processing `...`.

Something unexpected occured when trying to optimize generated binding code. The element causing the issue is named in the error message. In order to fix this issue the assembly named (or containing the type or method named) will need to be provided in a [bug report](http://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

The last digit `x` will be:
* `0` for an assembly name;
* `1` for a type name;
* `3` for a method name;

### <a name="MT2030"/>MT2030: Remove User Resources failed processing `...`.

Something unexpected occured when trying to remove user resources. The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](http://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

User resources are files included inside assemblies (as resources) that needs to be extracted, at build time, to create the application bundle. This includes:

* `__monotouch_content_*` and `__monotouch_pages_*` resources; and
* Native libraries embedded inside a binding assembly;

### <a name="MT2040"/>MT2040: Default HttpMessageHandler setter failed processing `...`.

Something unexpected occured when trying to set the default `HttpMessageHandler` for the application. Please file a [bug report](http://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MT2050"/>MT2050: Code Remover failed processing `...`.

Something unexpected occured when trying to remove code from BCL shipping with the application. Please file a [bug report](http://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MT2060"/>MT2060: Sealer failed processing `...`.

Something unexpected occured when trying to seal types or methods (final) or when devirtualizing some methods. The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](http://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MT2070"/>MT2070: Metadata Reducer failed processing `...`.

Something unexpected occured when trying to reduce the metadata from the application. The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](http://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MT2080"/>MT2080: MarkNSObjects failed processing `...`.

Something unexpected occured when trying to mark `NSObject` subclasses from the application. The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](http://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MT2090"/>MT2090: Inliner failed processing `...`.

Something unexpected occured when trying to inline code from the application. The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](https://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MT2100"/>MT2100: Smart Enum Conversion Preserver failed processing `...`.

Something unexpected occured when trying to mark the conversion methods for smart enums from the application. The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](https://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

<!-- MT21xx: more linker errors -->

<!--- 2100 used by mmp -->

### <a name="MT2101"/>MT2101: Can't resolve the reference '\*', referenced from the method '\*' in '*'.

An invalid assembly reference was encountered when processing the method mentioned in the error message.

The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](https://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MT2102"/>MT2102: Error processing the method '\*' in the assembly '\*': *

Something unexpected occured when trying to mark the method mentioned in the error message.

The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](https://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).


### <a name="MT2103"/>MT2103: Error processing assembly '\*': *

An unexpected error occured when processing an assembly.

The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](https://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MT2104"/>MT2104: Unable to link assembly '{0}' as it is mixed-mode.

Mixed-mode assemblies can not be processed by the linker.

See https://msdn.microsoft.com/en-us/library/x0w2664k.aspx for more information on mixed-mode assemblies.

### <a name="MT2105"/>MT2105: The [BindingImpl] attribute on the member * is invalid: *

The `[BindingImpl]` attribute on the mentioned member is invalid. The expected format is `[BindingImpl (BindingImplOptions.ValueA | BindingImplOptions.ValueB)]`.

### <a name="MT2106"/>MT2106: Could not optimize the call to BlockLiteral.SetupBlock[Unsafe] in * at offset * because *.

The linker reports this warning when it can't optimize a call to BlockLiteral.SetupBlock or Block.SetupBlockUnsafe.

The message will point to the method that calls BlockLiteral.SetupBlock[Unsafe], and
it may also give clues as to why the call couldn't be optimized.

Please file an [issue](https://github.com/xamarin/xamarin-macios/issues/new)
along with a complete build log so that we can investigate what went wrong and
possibly enable more scenarios in the future.

### <a name="MT2107"/>MT2107: It's not safe to remove the dynamic registrar because {reasons}

The linker reports this warning when the developer requests removal of the
dynamic registrar (by passing `--optimize:remove-dynamic-registrar` to
mtouch), but the linker determines that it's not safe to do so.

To remove the warning either remove the optimization argument to mtouch, or pass
`--nowarn:2107` to ignore it.

By default this option will be automatically enabled whenever it's possible
and safe to do so.

# MT3xxx: AOT error messages

<!--
 MT3xxx AOT
  MT30xx AOT (general) errors
  -->

### <a name="MT3001"/>MT3001 Could not AOT the assembly '*'

This generally indicates a bug in the AOT compiler. Please file a bug [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a project that can be used to reproduce the error.

Sometimes it's possible to work around this by disabling incremental builds in the project's iOS Build option (but it's still a bug, so please report it anyways).

### <a name="MT3002"/>MT3002 AOT restriction: Method '*' must be static since it is decorated with [MonoPInvokeCallback]. See [https://developer.xamarin.com/guides/ios/advanced_topics/limitations/#Reverse_Callbacks](https://developer.xamarin.com/guides/ios/advanced_topics/limitations/#Reverse_Callbacks)

This error message comes from the AOT compiler.



### <a name="MT3003"/>MT3003 Conflicting --debug and --llvm options. Soft-debugging is disabled.

Debugging is not supported when LLVM is enabled. If you need to debug the app, disable LLVM first.

### <a name="MT3004"/>MT3004 Could not AOT the assembly '*' because it doesn't exist.



### <a name="MT3005"/>MT3005 The dependency '\*' of the assembly '\*' was not found. Please review the project's references.

This typically occurs when an assembly references another version of a platform assembly (usually the .NET 4 version of mscorlib.dll).

This is not supported, and may not build or execute properly (the assembly may use API from the .NET 4 version of mscorlib.dll that the Xamarin.iOS version does not have).

### <a name="MT3006"/>MT3006 Could not compute a complete dependency map for the project. This will result in slower build times because Xamarin.iOS can't properly detect what needs to be rebuilt (and what does not need to be rebuilt). Please review previous warnings for more details.

 build or execute properly (the assembly may use API from the .NET 4 version of mscorlib.dll that the Xamarin.iOS version does not have).

### <a name="MT3007"/>MT3007: Debug info files (*.mdb) will not be loaded when llvm is enabled.

### <a name="MT3008"/>MT3008: Bitcode support requires the use of the LLVM AOT backend (--llvm)

Bitcode support requires the use of the LLVM AOT backend (--llvm).

Either disable Bitcode support or enable LLVM.

<!--- 3009 used by mmp -->
<!--- 3010 used by mmp -->

# MT4xxx: Code generation error messages

### MT40xx: Main

<!--
 MT4xxx code generation
  MT40xx main.m
  -->

### <a name="MT4001"/>MT4001 The main template could not be expanded to `*`.

An error occurred when generating main.m. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT4002"/>MT4002 Failed to compile the generated code for P/Invoke methods. Please file a bug report at http://bugzilla.xamarin.com

Failed to compile the generated code for P/Invoke methods. Please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### MT41xx: Registrar

<!--
  MT41xx registrar.m
  -->

### <a name="MT4101"/>MT4101 The registrar cannot build a signature for type `*`.

A type was found in exported API that the runtime doesn't know how to marshal to/from Objective-C.

If you believe Xamarin.iOS should support the type in question, please file an enhancement request at [http://bugzilla.xamarin.com](http://bugzilla.xamarin.com).

### <a name="MT4102"/>MT4102 The registrar found an invalid type `*` in signature for method `*`. Use `*` instead.

This currently only happens with one type: System.DateTime. Use the Objective-C equivalent (NSDate) instead.

### <a name="MT4103"/>MT4103 The registrar found an invalid type `*` in signature for method `*`: The type implements INativeObject, but does not have a constructor that takes two (IntPtr, bool) arguments

This occurs when the registrar encounter a type in a signature with the mentioned characteristics. The registrar might need to create new instances of the type, and in this case it requires a constructor with the (IntPtr, bool) signature - the first argument (IntPtr) specifies the managed handle, while the second if the caller hands over ownership of the native handle (if this value is false 'retain' will be called on the object).

### <a name="MT4104"/>MT4104 The registrar cannot marshal the return value for type `*` in signature for method `*`.

A type was found in exported API that the runtime doesn't know how to marshal to/from Objective-C.

If you believe Xamarin.iOS should support the type in question, please file an enhancement request at [http://bugzilla.xamarin.com](http://bugzilla.xamarin.com).

### <a name="MT4105"/>MT4105 The registrar cannot marshal the parameter of type `*` in signature for method `*`.

If you believe Xamarin.iOS should support the type in question, please file an enhancement request at [http://bugzilla.xamarin.com](http://bugzilla.xamarin.com).

### <a name="MT4106"/>MT4106 The registrar cannot marshal the return value for structure `*` in signature for method `*`.

A type was found in exported API that the runtime doesn't know how to marshal to/from Objective-C.

If you believe Xamarin.iOS should support the type in question, please file an enhancement request at [http://bugzilla.xamarin.com](http://bugzilla.xamarin.com).

### <a name="MT4107"/>MT4107 The registrar cannot marshal the parameter of type `*` in signature for method `+`.

A type was found in exported API that the runtime doesn't know how to marshal to/from Objective-C.

If you believe Xamarin.iOS should support the type in question, please file an enhancement request at [http://bugzilla.xamarin.com](http://bugzilla.xamarin.com).

### <a name="MT4108"/>MT4108 The registrar cannot get the ObjectiveC type for managed type `*`.

A type was found in exported API that the runtime doesn't know how to marshal to/from Objective-C.

If you believe Xamarin.iOS should support the type in question, please file an enhancement request at [http://bugzilla.xamarin.com](http://bugzilla.xamarin.com).

### <a name="MT4109"/>MT4109 Failed to compile the generated registrar code. Please file a bug report at http://bugzilla.xamarin.com

Failed to compile the generated code for the registrar. The build log will contain the output from the native compiler, explaining why the code isn't compiling.

This is always a bug in Xamarin.iOS; please file a bug report at [http://bugzilla.xamarin.com](http://bugzilla.xamarin.com) with your project or a test case.

### <a name="MT4110"/>MT4110 The registrar cannot marshal the out parameter of type `*` in signature for method `*`.



### <a name="MT4111"/>MT4111 The registrar cannot build a signature for type `*` in method `*`.



### <a name="MT4112"/>MT4112 The registrar found an invalid type `*`. Registering generic types with Objective-C is not supported, and may lead to random behavior and/or crashes (for backwards compatibility with older versions of Xamarin.iOS it is possible to ignore this error by passing `--unsupported--enable-generics-in-registrar` as an additional mtouch argument in the project's iOS Build options page. See [developer.xamarin.com/guides/ios/advanced_topics/registrar](https://developer.xamarin.com/guides/ios/advanced_topics/registrar) for more information).



### <a name="MT4113"/>MT4113 The registrar found a generic method: '\*.\*'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes.



### <a name="MT4114"/>MT4114 Unexpected error in the registrar for the method '\*.\*' - Please file a bug report at http://bugzilla.xamarin.com



### <a name="MT4116"/>MT4116 Could not register the assembly '*': *



### <a name="MT4117"/>MT4117 The registrar found a signature mismatch in the method '*.*' - the selector indicates the method takes * parameters, while the managed method has * parameters.



### <a name="MT4118"/>MT4118 Cannot register two managed types ('\*' and '\*') with the same native name ('*').



### <a name="MT4119"/>MT4119 Could not register the selector '\*' of the member '\*.*' because the selector is already registered on a different member.



### <a name="MT4120"/>MT4120 The registrar found an unknown field type '\*' in field '\*.*'. Please file a bug report at http://bugzilla.xamarin.com

This error indicates a bug in Xamarin.iOS. Please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT4121"/>MT4121 Cannot use GCC/G++ to compile the generated code from the static registrar when using the Accounts framework (the header files provided by Apple used during the compilation require Clang). Either use Clang (--compiler:clang) or the dynamic registrar (--registrar:dynamic).



### <a name="MT4122"/>MT4122 Cannot use the Clang compiler provided in the *.* SDK to compile the generated code from the static registrar when non-ASCII type names ('*') are present in the application. Either use GCC/G++ (--compiler:gcc|g++), the dynamic registrar (--registrar:dynamic) or a newer SDK.



### <a name="MT4123"/>MT4123 The type of the variadic parameter in the variadic function '*' must be System.IntPtr.



### <a name="MT4124"/>MT4124 Invalid * found on '*'. Please file a bug report at http://bugzilla.xamarin.com

This error indicates a bug in Xamarin.iOS. Please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT4125"/>MT4125 The registrar found an invalid type '\*' in signature for method '\*': The interface must have a Protocol attribute specifying its wrapper type.



### <a name="MT4126"/>MT4126 Cannot register two managed protocols ('\*' and '\*') with the same native name ('*').



### <a name="MT4127"/>MT4127 Cannot register more than one interface method for the method '\*' (which is implementing '\*').



### <a name="MT4128"/>MT4128  The registrar found an invalid generic parameter type '\*' in the method '\*'. The generic parameter must have an 'NSObject' constraint.

### <a name="MT4129"/>MT4129  The registrar found an invalid generic return type '\*' in the method '\*'. The generic return type must have an 'NSObject' constraint.

### <a name="MT4130"/>MT4130  The registrar cannot export static methods in generic classes ('*').

### <a name="MT4131"/>MT4131  The registrar cannot export static properties in generic classes ('\*.\*').

### <a name="MT4132"/>MT4132  The registrar found an invalid generic return type '\*' in the property '\*'. The return type must have an 'NSObject' constraint.

### <a name="MT4133"/>MT4133  Cannot construct an instance of the type '*' from Objective-C because the type is generic. [Runtime exception]

### <a name="MT4134"/>MT4134  Your application is using the '*' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS *, while you're building with the iOS * SDK.) Please select a newer SDK in your app's iOS Build options.

### <a name="MT4135"/>MT4135  The member '\*.\*' has an Export attribute that doesn't specify a selector. A selector is required.

### <a name="MT4136"/>MT4136  The registrar cannot marshal the parameter type '\*' of the parameter '\*' in the method '\*.*'

<!-- MT4137 is unused -->

### <a name="MT4138"/>MT4138  The registrar cannot marshal the property type '\*' of the property '\*.*'.

### <a name="MT4139"/>MT4139  The registrar cannot marshal the property type '\*' of the property '\*.*'. Properties with the [Connect] attribute must have a property type of NSObject (or a subclass of NSObject).

### <a name="MT4140"/>MT4140  The registrar found a signature mismatch in the method '*.*' - the selector indicates the variadic method takes * parameters, while the managed method has * parameters.

### <a name="MT4141"/>MT4141  Cannot register the selector '\*' on the member '\*' because Xamarin.iOS implicitly registers this selector.

This occurs when subclassing a framework type, and trying to implement a 'retain', 'release' or 'dealloc' method:

```
class MyNSObject : NSObject
{
	[Export ("retain")]
	new void Retain () {}

	[Export ("release")]
	new void Release () {}

	[Export ("dealloc")]
	new void Dealloc () {}
}
```

It is however possible to override these methods if the class isn't the first subclass of the framework type:

```
class MyNSObject : NSObject
{
}

class MyCustomNSObject : MyNSObject
{
	[Export ("retain")]
	new void Retain () {}

	[Export ("release")]
	new void Release () {}

	[Export ("dealloc")]
	new void Dealloc () {}
}
```

In this case Xamarin.iOS will override `retain`, `release` and `dealloc` on the `MyNSObject` class, and there is no conflict.

### <a name="MT4142"/>MT4142: Failed to register the type '*'.
### <a name="MT4143"/>MT4143: The ObjectiveC class '*' could not be registered, it does not seem to derive from any known ObjectiveC class (including NSObject).
### <a name="MT4144"/>MT4144: Cannot register the method '*' since it does not have an associated trampoline. Please file a bug report at http://bugzilla.xamarin.com.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT4145"/>MT4145: Invalid enum '*': enums with the [Native] attribute must have a underlying enum type of either 'long' or 'ulong'.
### <a name="MT4146"/>MT4146: The Name parameter of the Registrar attribute on the class '\*' ('\*') contains an invalid character: '\*' (\*).

The name of an Objectice-C class can't contain whitespace, which means that the `Register` attribute on the corresponding managed class can't have a `Name` parameter can't contain whitespace either.

Please verify that the `Register` attribute on the managed class mentioned in the error message does not contain any whitespace.

### <a name="MT4147"/>MT4147: Detected a protocol inheriting from the JSExport protocol while using the dynamic registrar. It is not possible to export protocols to JavaScriptCore dynamically; the static registrar must be used (add '--registrar:static to the additional mtouch arguments in the project's iOS Build options to select the static registrar).
### <a name="MT4148"/>MT4148: The registrar found a generic protocol: '*'. Exporting generic protocols is not supported.
### <a name="MT4149"/>MT4149: Cannot register the method '\*.\*' because the type of the first parameter ('\*') does not match the category type ('\*').
### <a name="MT4150"/>MT4150: Cannot register the type '\*' because the Type property ('\*') in its Category attribute does not inherit from NSObject.
### <a name="MT4151"/>MT4151: Cannot register the type '*' because the Type property in its Category attribute isn't set.
### <a name="MT4152"/>MT4152: Cannot register the type '*' as a category because it implements INativeObject or subclasses NSObject.
### <a name="MT4153"/>MT4153: Cannot register the type '\*' as a category because it's generic.
### <a name="MT4154"/>MT4154: Cannot register the method '\*' as a category method because it's generic.
### <a name="MT4155"/>MT4155: Cannot register the method '\*' with the selector '\*' as a category method on '*' because the Objective-C already has an implementation for this selector.
### <a name="MT4156"/>MT4156: Cannot register two categories ('\*' and '\*') with the same native name ('*').
### <a name="MT4157"/>MT4157: Cannot register the category method '\*' because at least one parameter is required (and its type must match the category type '\*')
### <a name="MT4158"/>MT4158: Cannot register the constructor * in the category * because constructors in categories are not supported.
### <a name="MT4159"/>MT4159: Cannot register the method '*' as a category method because category methods must be static.

### <a name="MT4160"/>MT4160: Invalid character '\*' (\*) found in selector '\*' for '\*'.

### <a name="MT4161"/>MT4161: The registrar found an unsupported structure '\*': All fields in a structure must also be structures (field '\*' with type '{2}' is not a structure).

The registrar found a structure with unsupported fields.

All fields in a structure that is exposed to Objective-C must also be structures (not classes).

### <a name="MT4162"/>MT4162: The type '\*' (used as * {2}) is not available in * * (it was introduced in * *)\* Please build with a newer * SDK (usually done by using the most recent version of Xcode.

The registrar found a type which is not included in the current SDK.

Please upgrade Xcode.

### <a name="MT4163"/>MT4163: Internal error in the registrar (*). Please file a bug report at http://bugzilla.xamarin.com

This error indicates a bug in Xamarin.iOS. Please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT4164"/>MT4164: Cannot export the property '\*' because its selector '\*' is an Objective-C keyword. Please use a different name.

The selector for the property in question is not a valid Objective-C identifer.

Please use a valid Objective-C identifier as selectors.

### <a name="MT4165"/>MT4165: The registrar couldn't find the type 'System.Void' in any of the referenced assemblies.

This error most likely indicates a bug in Xamarin.iOS. Please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT4166"/>MT4166: Cannot register the method '\*' because the signature contains a type (\*) that isn't a reference type.

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT4167"/>MT4167: Cannot register the method '\*' because the signature contains a generic type (\*) with a generic argument type that isn't an NSObject subclass (*).

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT4168"/>MT4168: Cannot register the type '{managed\_name}' because its Objective-C name '{exported\_name}' is an Objective-C keyword. Please use a different name.

The Objective-C name for the type in question is not a valid Objective-C identifier.

Please use a valid Objective-C identifier.

### <a name="MT4169"/>MT4169: Failed to generate a P/Invoke wrapper for {method}: {message}

Xamarin.iOS failed to generate a P/Invoke wrapper function for the mentioned.
Please check the reported error message for the underlying cause.

### <a name="MT4170"/>MT4170: The registrar can't convert from '{managed type}' to '{native type}' for the return value in the method {method}.

See the description of error <a href="#MT4172">MT4172</a>.

### <a name="MT4171"/>MT4171: The BindAs attribute on the member {member} is invalid: the BindAs type {type} is different from the property type {type}.

Please make sure the type in the BindAs attribute matches the type of the member it's attached to.

### <a name="MT4172"/>MT4172: The registrar can't convert from '{native type}' to '{managed type}' for the parameter '{parameter name}' in the method {method}.

The registrar does not support converting between the mentioned types.

This is a bug in Xamarin.iOS if the API in question is provided by Xamarin.iOS;
please file a bug at [http://bugzilla.xamarin.com][1].

If you run into this while developing a binding project for a native library,
we're open to adding support for new combinations of types. If this is the
case, please file an enhancement request ([http://bugzilla.xamarin.com][2])
with a test case and we'll evaluate it.

[1]: https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS
[2]: https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS&component=General&bug_severity=enhancement

### <a name="MT4173"/>MT4173: The registrar can't compute the block signature for the delegate of type {delegate-type} in the method {method} because *.

This is a warning indicating that the registrar couldn't inject the block
signature of the specified method into the generated registrar code, because
the registrar couldn't compute it.

This means that the block signature has to be computed at runtime, which is
somewhat slower.

There are currently two possible reasons for this warning:

1. The type of the managed delegate is either a `System.Delegate` or
   `System.MulticastDelegate`. These types don't represent a specific signature,
   which means the registrar can't compute the corresponding native signature
   either. In this case the fix is to use a specific delegate type for the
   block (alternatively the warning can be ignored by adding `--nowarn:4173`
   as an additional mtouch argument in the project's iOS Build options).
2. The registrar can't find the `Invoke` method of the delegate. This
   shouldn't happen, so please file an [issue](https://github.com/xamarin/xamarin-macios/issues/new)
   with a test project so that we can fix it.

# MT5xxx: GCC and toolchain error messages

### MT51xx: Compilation

<!--
 MT5xxx GCC and toolchain
  MT51xx compilation
  -->

### <a name="MT5101"/>MT5101 Missing '*' compiler. Please install Xcode 'Command-Line Tools' component



### <a name="MT5102"/>MT5102 Failed to assemble the file '*'. Please file a bug report at http://bugzilla.xamarin.com



### <a name="MT5103"/>MT5103 Failed to compile the file '*'. Please file a bug report at http://bugzilla.xamarin.com



### <a name="MT5104"/>MT5104 Could not find neither the '\*' nor the '\*' compiler. Please install Xcode 'Command-Line Tools' component

<!-- 5105 is used by mmp -->

### <a name="MT5106"/>MT5106: Could not compile the file(s) '*'. Please file a bug report at http://bugzilla.xamarin.com

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### MT52xx: Linking

<!--
  MT52xx linking
  -->

### <a name="MT5201"/>MT5201 Native linking failed. Please review the build log and the user flags provided to gcc: *

### <a name="MT5202"/>MT5202 Native linking failed. Please review the build log.

### <a name="MT5203"/>MT5203 Native linking warning: *

<!--- 5204-5208 are not used -->

### <a name="MT5209"/>MT5209 Native linking error: *

### <a name="MT5210"/>MT5210: Native linking failed, undefined symbol: *. Please verify that all the necessary frameworks have been referenced and native libraries are properly linked in.

This happens when the native linker cannot find a symbol that is referenced somewhere. There are several reasons this may happen:

* A third-party binding requires a framework, but the binding does not specify this in its `[LinkWith]` attribute. Solutions:
  - If you're the author of the third-party binding, or have access to its source, modify the binding's `[LinkWith]` attribute to include the framework it needs:

            [LinkWith ("mylib.a", Frameworks = "SystemConfiguration")]

  - If you can't modify the third-party binding, you can manually link with the required framework by passing `-gcc_flags '-framework SystemFramework'` to `mtouch` (this is done by modifying the additional mtouch arguments in the project's iOS Build options page. Remember that this must be done for every project configuration).
* In some cases a managed binding is composed of several native libraries, and all must be included in the bindings. It is possible to have more than one native library in each binding project, so the solution is to just add all the required native libraries to the binding project.</li>
* A managed binding refers to native symbols that don't exist in the native library.
    This usually happens when a binding has existed for some time, and the native
    code has been modified during that time so that a particular native class has
    either been removed or renamed, while the binding has not been updated.
* A P/Invoke refers to a native symbol that does not exist. Starting with Xamarin.iOS 7.4 an <a href="#MT5214">MT5214</a> error will be reported for this case (see MT5214 for more information).
* A third-party binding / library was built using C++, but the binding does not specify this in its `[LinkWith]` attribute. This is usually fairly easy to recognize, because the symbols have are mangled C++ symbols (one common example is `__ZNKSt9exception4whatEv`).
  - If you're the author of the third-party binding, or have access to its source, modify the binding's `[LinkWith]` attribute to set the `IsCxx` flag:

            [LinkWith ("mylib.a", IsCxx = true)]

  - If you can't modify the third-party binding, or you're linking manually with a third-party library, you can set the equivalent flag by passing <code>-cxx</code> to mtouch (this is done by modifying the additional mtouch arguments in the project's iOS Build options page. Remember that this must be done for every project configuration).



### <a name="MT5211"/>MT5211: Native linking failed, undefined Objective-C class: \*. The symbol '\*' could not be found in any of the libraries or frameworks linked with your application.

This happens when the native linker cannot find an Objective-C class that is referenced somewhere. There are several reasons this may happen: the same as for [MT5210](#MT5210) and in addition:

* A third-party binding bound an Objective-C protocol, but did not annotate it with the <code>[Protocol]</code> attribute in its api definition. Solutions:
  - Add the missing `[Protocol]` attribute:

              [BaseType (typeof (NSObject))]
              [Protocol] // Add this
              public interface MyProtocol
              {
              }



### <a name="MT5212"/>MT5212: Native linking failed, duplicate symbol: *.

This happens when the native linker encounters duplicated symbols between all the native libraries. Following this error there may be one or more [MT5213](#MT5213) errors with the location for each occurrence of the symbol. Possible reasons for this error:


* The same native library is included twice.
* Two distinct native libraries happen to define the same symbols.
* A native library is not correctly built, and contains the same symbol more than once.
  You can confirm this by using the following set of commands from a terminal (replace i386 with x86_64/armv7/armv7s/arm64 according to which architecture you're building for):

        # Native libraries are usually fat libraries, containing binary code for
        # several architectures in the same file. First we extract the binary
        # code for the architecture we're interested in.
        lipo libNative.a -thin i386 -output libNative.i386.a

        # Now query the native library for the duplicated symbol.
        nm libNative.i386.a | fgrep 'SYMBOL'

        # You can also list the object files inside the native library.
        # In most cases this will reveal duplicated object files.
        ar -t libNative.i386.a

  There are a few possible ways to fix this:

  - Request that the provider of the native library fix it and provide the updated version.
  - Fix it yourself by removing the extra object files (this only works if the problem is in fact duplicated object files)


            # Find out if the library is a fat library, and which
            # architectures it contains.
            lipo -info libNative.a

            # Extract each architecture (i386/x86_64/armv7/armv7s/arm64) to a separate file
            lipo libNative.a -thin ARCH -output libNative.ARCH.a

            # Extract the object files for the offending architecture
            # This will remove the duplicates by overwriting them
            # (since they have the same filename)
            mkdir -p ARCH
            cd ARCH
            ar -x ../libNative.ARCH.a

            # Reassemble the object files in an .a
            ar -r ../libNative.ARCH.a *.o
            cd ..

            # Reassemble the fat library
            lipo *.a -create -output libNative.a


  - Ask the linker to remove unused code. Xamarin.iOS will do this automatically if all of the following conditions are met:
    - All third-party bindings' `[LinkWith]` attributes have enabled SmartLink:

            [assembly: LinkWith ("libNative.a", SmartLink = true)]

    - No `-gcc_flags` is passed to mtouch (in the additional mtouch arguments field of the project's iOS Build options).
    - It's also possible to ask the linker directly to remove unused code by adding `-gcc_flags -dead_strip` to the additional mtouch arguments in the project's iOS Build options.


### <a name="MT5213"/>MT5213: Duplicate symbol in: * (Location related to previous error)

This error is reported only together with [MT5212](#MT5212). Please see [MT5212](#MT5212) for more information.

### <a name="MT5214"/>MT5214  Native linking failed, undefined symbol: *. This symbol was referenced the managed member *. Please verify that all the necessary frameworks have been referenced and native libraries linked.

This error is reported when the managed code contains a P/Invoke to a native method that does not exist. For example:

```
using System.Runtime.InteropServices;
class MyImports {
   [DllImport ("__Internal")]
   public static extern void MyInexistentFunc ();
}
```

There are a few possible solutions:

  -  Remove the P/Invokes in question from the source code.
  -  Enable the managed linker for all assemblies (this is done in the project's iOS Build options, by setting "Linker Behavior" to "All assemblies"). This will effectively remove all the P/Invokes you don't use from the app (automatically, instead of manually like the previous point). The downside is that this will make your simulator builds somewhat slower, and it may break your app if it's using reflection - more information about the linker can be found  [here](/guides/ios/advanced_topics/linker/) )
  -  Create a second native library which contains the missing native symbols. Note that this is merely a workaround (if you happen to try to call those functions, your app will crash).

### <a name="MT5215"/>MT5215: References to '*' might require additional -framework=XXX or -lXXX instructions to the native linker

This is a warning, indicating that a P/Invoke was detected to reference the library in question, but the app is not linking with it.

### <a name="MT5216"/>MT5216: Native linking failed for *. Please file a bug report at http://bugzilla.xamarin.com

This error is reported when linking the output from the AOT compiler.

This error most likely indicates a bug in Xamarin.iOS. Please file a bug report at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT5217"/>MT5217: Native linking possibly failed because the linker command line was too long (* characters).

Native linking failed, and it's possible this occured because the linker
command was too long.

Xamarin.iOS projects will often reference native symbols dynamically, which
means that the native linker might remove such native symbols during the
native linking process, because the native linker does not see that these
symbols are used.

Usually Xamarin.iOS will ask the native linker to keep such symbols using the
`-u symbol` linker flag, but if there are many such symbols, the entire
command-line might exceed the maximum command-line length as specified by the
operating system.

There are a few possible sources for such dynamic symbols:

* P/Invokes to methods in statically linked libraries (where the dll name is
  `__Internal` in the DllImport attribute `[DllImport ("__Internal")]`).
* Field references to memory locations in statically linked libraries from
  binding projects (`[Field]` attributes).
* Objective-C classes referenced in statically linked libraries from binding
  projects (when using incremental builds or when not using the static
  registrar).

Possible solutions:

* Enable the managed linker (if possible for all assemblies instead of only
  SDK assemblies). This might remove enough of the sources for dynamic symbols
  so that the linker's command-line doesn't exceeded the maximum.
* Reduce the number of P/Invokes, field references and/or Objective-C classes.
* Rewrite the dynamic symbols to have shorter names.
* Pass `-dlsym:false` as an additional mtouch argument in the project's iOS
  Build options. With this option, Xamarin.iOS will generate a native
  reference in the AOT-compiled code, and won't need to ask the linker to keep
  this symbol. However, this only works for device builds, and it will cause
  linker errors if there are P/Invokes to functions that don't exist in the
  static library.
* Pass `--dynamic-symbol-mode=code` as an additional mtouch arguments in
  the project's iOS Build options. With this option, Xamarin.iOS will generate
  additional native code that references these symbols instead of asking the
  native linker to keep these symbols using command-line arguments. The
  downside to this approach is that it will increase the size of the
  executable somewhat.
* Enable the static registrar by passing `--registrar:static` as an additional
  mtouch argument in the project's iOS Build options (for simulator builds,
  since the static registrar is already the default for device builds). The
  static registrar will generate code that references Objective-C classes
  statically, so there is no need to ask the native linker to keep such
  classes.
* Disable incremental builds (for device builds). When incremental builds are
  enabled, the code generated by the static registrar won't be considered by
  the native linker, which means that Xamarin.iOS must still ask the linker to
  keep referenced Objective-C classes. Thus disabling incremental builds will
  prevent this need.

### <a name="MT5218"/>MT5218: Can't ignore the dynamic symbol {symbol} (--ignore-dynamic-symbol={symbol}) because it was not detected as a dynamic symbol.

The command-line argument `--ignore-dynamic-symbol=symbol` was passed, but
this symbol is not a symbol that was recognized as a dynamic symbol that must
be manually preserved.

There are two main reasons for this:

* The symbol name is incorrect.
	* Don't prepend an underscore to the symbol name.
	* The symbol for Objective-C classes is `OBJC_CLASS_$_<classname>`.
* The symbol is correct, but it's a symbol that's already preserved by normal
  means (some build options causes the exact list of dynamic symbols to vary).

### MT53xx: Other tools

<!--
  MT53xx other tools
  -->

### <a name="MT5301"/>MT5301: Missing 'strip' tool. Please install Xcode 'Command-Line Tools' component



### <a name="MT5302"/>MT5302: Missing 'dsymutil' tool. Please install Xcode 'Command-Line Tools' component



### <a name="MT5303"/>MT5303: Failed to generate the debug symbols (dSYM directory). Please review the build log.

An error occurred when running dsymutil on the final .app directory to create the debug symbols. Please review the build log to see dsymutil's output and see how it can be fixed.

### <a name="MT5304"/>MT5304: Failed to strip the final binary. Please review the build log.

An error occurred when running the 'strip' tool to remove debugging information from the application.

### <a name="MT5305"/>MT5305: Missing 'lipo' tool. Please install Xcode 'Command-Line Tools' component



### <a name="MT5306"/>MT5306: Failed to create the a fat library. Please review the build log.

An error occurred when running the 'lipo' tool. Please review the build log to see the error reported by 'lipo'.

### <a name="MT5307"/>MT5307: Failed to sign the executable. Please review the build log.

An error occurred when signing the application. Please review the build log to see the error reported by 'codesign'.

<!-- 5308 is used by mmp -->
<!-- 5309 is used by mmp -->
<!-- 5310 is used by mmp -->

# MT6xxx: mtouch internal tools error messages

### MT600x: Stripper

<!--
 MT6xxx mtouch internal tools
  MT600x Stripper
  -->

### <a name="MT6001"/>MT6001: Running version of Cecil doesn't support assembly stripping

### <a name="MT6002"/>MT6002: Could not strip assembly `*`.

An error occurred when stripping managed code(removing the IL code) from the assemblies in the application.

### <a name="MT6003"/>MT6003: [UnauthorizedAccessException message]

A security error ocurred while stripping debugging symbols from the application.

# MT7xxx: MSBuild error messages

<!--
 MT7xxx msbuild errors
  -->

### <a name="MT7001"/>MT7001: Could not resolve host IPs for WiFi debugger settings.

*MSBuild task: DetectDebugNetworkConfigurationTaskBase*

Troubleshooting steps:

- try to run `csharp -e 'System.Net.Dns.GetHostEntry (System.Net.Dns.GetHostName ()).AddressList'` (that should give you an IP address and not an error obviously).
- try to run "ping \`hostname\`" which might give you more information, like: `cannot resolve MyHost.local: Unknown host`

In some cases, it's a "local network" issue and it can be addressed by adding the unknown host `127.0.0.1	MyHost.local` in `/etc/hosts`.

### <a name="MT7002"/>MT7002: This machine does not have any network adapters. This is required when debugging or profiling on device over WiFi.

*MSBuild task: DetectDebugNetworkConfigurationTaskBase*

### <a name="MT7003"/>MT7003: The App Extension '*' does not contain an Info.plist.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7004"/>MT7004: The App Extension '*' does not specify a CFBundleIdentifier.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7005"/>MT7005: The App Extension '*' does not specify a CFBundleExecutable.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7006"/>MT7006: The App Extension '\*' has an invalid CFBundleIdentifier (\*), it does not begin with the main app bundle's CFBundleIdentifier (*).

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7007"/>MT7007: The App Extension '\*' has a CFBundleIdentifier (\*) that ends with the illegal suffix ".key".

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7008"/>MT7008: The App Extension '*' does not specify a CFBundleShortVersionString.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7009"/>MT7009: The App Extension '*' has an invalid Info.plist: it does not contain an NSExtension dictionary.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7010"/>MT7010: The App Extension '*' has an invalid Info.plist: the NSExtension dictionary does not contain an NSExtensionPointIdentifier value.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7011"/>MT7011: The WatchKit Extension '*' has an invalid Info.plist: the NSExtension dictionary does not contain an NSExtensionAttributes dictionary.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7012"/>MT7012: The WatchKit Extension '*' does not have exactly one watch app.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7013"/>MT7013: The WatchKit Extension '*' has an invalid Info.plist: UIRequiredDeviceCapabilities must contain the 'watch-companion' capability.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7014"/>MT7014: The Watch App '*' does not contain an Info.plist.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7015"/>MT7015: The Watch App '*' does not specify a CFBundleIdentifier.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7016"/>MT7016: The Watch App '\*' has an invalid CFBundleIdentifier (\*), it does not begin with the main app bundle's CFBundleIdentifier (*).

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7017"/>MT7017: The Watch App '\*' does not have a valid UIDeviceFamily value. Expected 'Watch (4)' but found '\* (*)'.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7018"/>MT7018: The Watch App '*' does not specify a CFBundleExecutable

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7019"/>MT7019: The Watch App '\*' has an invalid WKCompanionAppBundleIdentifier value ('\*'), it does not match the main app bundle's CFBundleIdentifier ('*').

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7020"/>MT7020: The Watch App '*' has an invalid Info.plist: the WKWatchKitApp key must be present and have a value of 'true'.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7021"/>MT7021: The Watch App '*' has an invalid Info.plist: the LSRequiresIPhoneOS key must not be present.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7022"/>MT7022: The Watch App '*' does not contain a Watch Extension.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7023"/>MT7023: The Watch Extension '*' does not contain an Info.plist.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7024"/>MT7024: The Watch Extension '*' does not specify a CFBundleIdentifier.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7025"/>MT7025: The Watch Extension '*' does not specify a CFBundleExecutable.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7026"/>MT7026: The Watch Extension '\*' has an invalid CFBundleIdentifier (\*), it does not begin with the main app bundle's CFBundleIdentifier (*).

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7027"/>MT7027: The Watch Extension '\*' has a CFBundleIdentifier (\*) that ends with the illegal suffix ".key".

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7028"/>MT7028: The Watch Extension '*' has an invalid Info.plist: it does not contain an NSExtension dictionary.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7029"/>MT7029: The Watch Extension '*' has an invalid Info.plist: the NSExtensionPointIdentifier must be "com.apple.watchkit".

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7030"/>MT7030: The Watch Extension '*' has an invalid Info.plist: the NSExtension dictionary must contain NSExtensionAttributes.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7031"/>MT7031: The Watch Extension '*' has an invalid Info.plist: the NSExtensionAttributes dictionary must contain a WKAppBundleIdentifier.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7032"/>MT7032: The WatchKit Extension '*' has an invalid Info.plist: UIRequiredDeviceCapabilities should not contain the 'watch-companion' capability.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7033"/>MT7033: The Watch App '*' does not contain an Info.plist.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7034"/>MT7034: The Watch App '*' does not specify a CFBundleIdentifier.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7035"/>MT7035: The Watch App '\*' does not have a valid UIDeviceFamily value. Expected '\*' but found '\* (\*)'.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7036"/>MT7036: The Watch App '*' does not specify a CFBundleExecutable.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7037"/>MT7037: The WatchKit Extension '{extensionName}' has an invalid WKAppBundleIdentifier value ('\*'), it does not match the Watch App's CFBundleIdentifier ('\*').

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7038"/>MT7038: The Watch App '*' has an invalid Info.plist: the WKCompanionAppBundleIdentifier must exist and must match the main app bundle's CFBundleIdentifier.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7039"/>MT7039: The Watch App '*' has an invalid Info.plist: the LSRequiresIPhoneOS key must not be present.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7040"/>MT7040: The app bundle {AppBundlePath} does not contain an Info.plist.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7041"/>MT7041: Main Info.plist path does not specify a CFBundleIdentifier.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7042"/>MT7042: Main Info.plist path does not specify a CFBundleExecutable.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7043"/>MT7043: Main Info.plist path does not specify a CFBundleSupportedPlatforms.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7044"/>MT7044: Main Info.plist path does not specify a UIDeviceFamily.

*MSBuild task: ValidateAppBundleTaskBase*

### <a name="MT7045"/>MT7045: Unrecognized Format: *.

*MSBuild task: PropertyListEditorTaskBase*

Where * can be:

- string
- array
- dict
- bool
- real
- integer
- date
- data

### <a name="MT7046"/>MT7046: Add: Entry, *, Incorrectly Specified.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7047"/>MT7047: Add: Entry, *, Contains Invalid Array Index.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7048"/>MT7048: Add: * Entry Already Exists.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7049"/>MT7049: Add: Can't Add Entry, *, to Parent.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7050"/>MT7050: Delete: Can't Delete Entry, *, from Parent.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7051"/>MT7051: Delete: Entry, *, Contains Invalid Array Index.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7052"/>MT7052: Delete: Entry, *, Does Not Exist.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7053"/>MT7053: Import: Entry, *, Incorrectly Specified.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7054"/>MT7054: Import: Entry, *, Contains Invalid Array Index.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7055"/>MT7055: Import: Error Reading File: *.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7056"/>MT7056: Import: Can't Add Entry, *, to Parent.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7057"/>MT7057: Merge: Can't Add array Entries to dict.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7058"/>MT7058: Merge: Specified Entry Must Be a Container.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7059"/>MT7059: Merge: Entry, *, Contains Invalid Array Index.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7060"/>MT7060: Merge: Entry, *, Does Not Exist.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7061"/>MT7061: Merge: Error Reading File: *.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7062"/>MT7062: Set: Entry, *, Incorrectly Specified.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7063"/>MT7063: Set: Entry, *, Contains Invalid Array Index.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7064"/>MT7064: Set: Entry, *, Does Not Exist.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7065"/>MT7065: Unknown PropertyList editor action: *.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7066"/>MT7066: Error loading '*': *.

*MSBuild task: PropertyListEditorTaskBase*

### <a name="MT7067"/>MT7067: Error saving '*': *.

*MSBuild task: PropertyListEditorTaskBase*


# MT8xxx: Runtime error messages

<!--
 MT8xxx runtime
  MT800x misc
  -->

### <a name="MT8001"/>MT8001  Version mismatch between the native Xamarin.iOS runtime and monotouch.dll. Please reinstall Xamarin.iOS.

### <a name="MT8002"/>MT8002  Could not find the method '\*' in the type '\*'.

### <a name="MT8003"/>MT8003  Failed to find the closed generic method '\*' on the type '\*'.

### <a name="MT8004"/>MT8004: Cannot create an instance of * for the native object 0x* (of type '*'), because another instance already exists for this native object (of type *).

### <a name="MT8005"/>MT8005: Wrapper type '\*' is missing its native ObjectiveC class '\*'.

### <a name="MT8006"/>MT8006: Failed to find the selector '\*' on the type '\*'

### <a name="MT8007"/>MT8007: Cannot get the method descriptor for the selector '\*' on the type '\*', because the selector does not correspond to a method

### <a name="MT8008"/>MT8008: The loaded version of Xamarin.iOS.dll was compiled for * bits, while the process is * bits. Please file a bug at http://bugzilla.xamarin.com.

This indicates something is wrong in the build process. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8009"/>MT8009: Unable to locate the block to delegate conversion method for the method *.*'s parameter #*. Please file a bug at http://bugzilla.xamarin.com.

This indicates an API wasn't bound correctly. If this is an API exposed by Xamarin, please file a bug in our bugzilla ([http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS)), if it's a third-party binding, please contact the vendor.

### <a name="MT8010"/>MT8010: Native type size mismatch between Xamarin.[iOS|Mac].dll and the executing architecture. Xamarin.[iOS|Mac].dll was built for *-bit, while the current process is *-bit.

This indicates something is wrong in the build process. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8011"/>MT8011: Unable to locate the delegate to block conversion attribute ([DelegateProxy]) for the return value for the method *.*. Please file a bug at http://bugzilla.xamarin.com.

Xamarin.iOS was unable to locate a required method at runtime (to convert a delegate to a block).

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8012"/>MT8012: Invalid DelegateProxyAttribute for the return value for the method *.*: DelegateType is null. Please file a bug at http://bugzilla.xamarin.com.

The DelegateProxy attribute for the method in question is invalid.

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8013"/>MT8013: Invalid DelegateProxyAttribute for the return value for the method *.*: DelegateType ({2}) specifies a type without a 'Handler' field. Please file a bug at http://bugzilla.xamarin.com.

The DelegateProxy attribute for the method in question is invalid.

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8014"/>MT8014: Invalid DelegateProxyAttribute for the return value for the method *.*: The DelegateType's ({2}) 'Handler' field is null. Please file a bug at http://bugzilla.xamarin.com.

The DelegateProxy attribute for the method in question is invalid.

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8015"/>MT8015: Invalid DelegateProxyAttribute for the return value for the method *.*: The DelegateType's ({2}) 'Handler' field is not a delegate, it's a *. Please file a bug at http://bugzilla.xamarin.com.

The DelegateProxy attribute for the method in question is invalid.

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8016"/>MT8016: Unable to convert delegate to block for the return value for the method *.*, because the input isn't a delegate, it's a *. Please file a bug at http://bugzilla.xamarin.com.

The DelegateProxy attribute for the method in question is invalid.

This usually indicates a bug in Xamarin.iOS; please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

<!-- 8017 is used by mmp -->

### <a name="MT8018"/>MT8018: Internal consistency error. Please file a bug report at http://bugzilla.xamarin.com.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8019"/>MT8019: Could not find the assembly * in the loaded assemblies.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8020"/>MT8020: Could not find the module with MetadataToken * in the assembly *.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8021"/>MT8021: Unknown implicit token type: *.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8022"/>MT8022: Expected the token reference * to be a *, but it's a *. Please file a bug report at http://bugzilla.xamarin.com.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8023"/>MT8023: An instance object is required to construct a closed generic method for the open generic method: * (token reference: *). Please file a bug report at http://bugzilla.xamarin.com.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8024"/>MT8024: Could not find a valid extension type for the smart enum '{smart_type}'. Please file a bug at https://bugzilla.xamarin.com.

This indicates a bug in Xamarin.iOS. Please file a bug at [http://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

### <a name="MT8025"/>MT8025: Failed to compute the token reference for the type '{type.AssemblyQualifiedName}' because {reasons}

This indicates a bug in Xamarin.iOS. Please file a bug at [https://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

A potential workaround would be to disable the `register-protocols`
optimization, by passing `--optimize:-register-protocols` as an additional
mtouch argument in the project's iOS Build options.

### <a name="MT8026"/>MT8026: * is not supported when the dynamic registrar has been linked away.

This usually indicates a bug in Xamarin.iOS, because the dynamic registrar should not be linked away if it's needed. Please file a bug at [https://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS).

It's possible to force the linker to keep the dynamic registrar by adding
`--optimize=-remove-dynamic-registrar` to the additional mtouch arguments in
the project's iOS Build options.
