---
id: 5B26339F-A202-4E41-9229-D0BC9E77868E
title: Xamarin.Mac errors
dateupdated: 2017-06-26
---

[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)
[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)
[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)

# MM0xxx: mmp error messages

E.g. parameters, environment, missing tools.

### <a name="MM0000">MM0000: Unexpected error - Please file a bug report at http://bugzilla.xamarin.com

An unexpected error condition occurred. Please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=Xamarin.Mac) with as much information as possible, including:

* Full build logs, with maximum verbosity (e.g. `-v -v -v -v` in the **Additional mmp arguments**);
* A minimal test case that reproduce the error; and
* All version informations

The easiest way to get exact version information is to use the **Xamarin Studio** menu, **About Xamarin Studio** item, **Show Details** button and copy/paste the version informations (you can use the **Copy Information** button).

### <a name="MM0001">MM0001: This version of Xamarin.Mac requires Mono {0} (the current Mono version is {1}). Please update the Mono.framework from http://mono-project.com/Downloads

### <a name="MM0003">MM0003: Application name '{0}.exe' conflicts with an SDK or product assembly (.dll) name.

### <a name="MM0007">MM0007: The root assembly '{0}' does not exist

### <a name="MM0008">MM0008: You should provide one root assembly only, found {0} assemblies: '{1}'

### <a name="MM0009"/>MM0009: Error while loading assemblies: *.

An error occurred while loading the assemblies from the root assembly references. More information may be provided in the build output.

### <a name="MM0010">MM0010: Could not parse the command line arguments: {0}

<!-- 0013 is unused -->

### <a name="MM0016">MM0016: The option '{0}' has been deprecated.

### <a name="MM0017">MM0017: You should provide a root assembly

### <a name="MM0018">MM0018: Unknown command line argument: '{0}'

### <a name="MM0020">MM0020: The valid options for '{0}' are '{1}'.

### <a name="MM0023">MM0023: Application name '{0}.exe' conflicts with another user assembly.

### <a name="MM0026">MM0026: Could not parse the command line argument '{0}': {1}

### <a name="MM0043">MM0043: The Boehm garbage collector is not supported. The SGen garbage collector has been selected instead.

### <a name="MM0050">MM0050: You cannot provide a root assembly if --no-root-assembly is passed.

### <a name="MM0051">MM0051: An output directory (--output) is required if --no-root-assembly is passed.

### <a name="MM0053">MM0053: Cannot disable new refcount with the Unified API.

### <a name="MM0056">MM0056: Cannot find Xcode in any of our default locations. Please install Xcode, or pass a custom path using --sdkroot=<path>

### <a name="MM0059">MM0059: Could not find the currently selected Xcode on the system: {0};

### <a name="MM0060">MM0060: Could not find the currently selected Xcode on the system. 'xcode-select --print-path' returned '{0}', but that directory does not exist.

### <a name="MM0068">MM0068: Invalid value for target framework: {0}.

### <a name="MM0071">MM0071: Unknown platform: *. This usually indicates a bug in Xamarin.Mac; please file a bug report at https://bugzilla.xamarin.com with a test case.

This usually indicates a bug in Xamarin.Mac; please file a bug report at [https://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=Xamarin.Mac) with a test case.

### <a name="MM0073"/>MM0073: Xamarin.Mac * does not support a deployment target of * (the minimum is *). Please select a newer deployment target in your project's Info.plist.

The minimum deployment target is the one specified in the error message; please select a newer deployment target in the project's Info.plist.

If updating the deployment target is not possible, then please use an older version of Xamarin.Mac.

### <a name="MM0074"/>MM0074: Xamarin.Mac * does not support a deployment target of * (the maximum is *). Please select an older deployment target in your project's Info.plist or upgrade to a newer version of Xamarin.Mac.

Xamarin.Mac does not support setting the minimum deployment target to a higher version than the version this particular version of Xamarin.Mac was built for.

Please select an older minimum deployment target in the project's Info.plist, or upgrade to a newer version of Xamarin.Mac.

### <a name="MM0079">MM0079: Internal Error - No executable was copied into the app bundle. Please contact 'support@xamarin.com'

### <a name="MM0080">MM0080: Disabling NewRefCount, --new-refcount:false, is deprecated.

<!-- 0088 used by mtouch -->
<!-- 0089 used by mtouch -->

### <a name="MM0091"/>MM0091: This version of Xamarin.Mac requires the * SDK (shipped with Xcode *). Either upgrade Xcode to get the required header files or use the dynamic registrar or set the managed linker behaviour to Link Platform or Link Framework SDKs Only (to try to avoid the new APIs).

Xamarin.Mac requires the header files, from the SDK version specified in the error message, to build your application with the static registrar.. The recommended way to fix this error is to upgrade Xcode to get the required SDK, this will include all the required header files. If you have multiple versions of Xcode installed, or want to use an Xcode in a non-default location, make sure to set the correct Xcode location in your IDE's preferences.

One potential, alternative solution, is to enable the managed linker. This will remove unused API including, in most cases, the new API where the header files are missing (or incomplete). However this will not work if your project uses API that was introduced in a newer SDK than the one your Xcode provides.

A second potential, alternative solution, is use the dynamic registrar instead. This will impose a startup cost by dynamically registering types but remove the header file requirement. 

A last-straw solution would be to use an older version of Xamarin.Mac, one that supports the SDK your project requires.


### <a name="MM0097">MM0097: machine.config file '{0}' can not be found.

### <a name="MM0098">MM0098: AOT compilation is only available on Unified

### <a name="MM0099">MM0099: Internal error {0}. Please file a bug report with a test case (http://bugzilla.xamarin.com).

### <a name="MM0114">MM0114: Hybrid AOT compilation requires all assemblies to be AOT compiled.

### <a name="MM0129"/>MM0129: Debugging symbol file for '*' does not match the assembly and is ignored.

The debugging symbols, either a .pdb (portable pdb only) or a .mdb file, for the specified assembly could not be loaded.

This generally means the assembly is newer or older than the symbols. Since they do not match they cannot be used and the symbols are ignored.

This warning won't affect the application being built, however you might not be able to debug it entirely (in particular the code from specified assembly). Also exceptions, stack traces and crash reports might be missing some information.

Please report this issue to the publisher of the assembly package (e.g. nuget author) so this can be fixed in their future releases.

### <a name="MM0130"/>MM0130: No root assemblies found. You should provide at least one root assembly.

When running --runregistrar, at least one root assembly should be provided.

### <a name="MM0131"/>MM0131: Product assembly '{0}' not found in assembly list: '{1}'

When running --runregistrar, the assembly list should include the product assembly, Xamarin.Mac, XamMac.

### <a name="MM0132/>MM0132: Unknown optimization: *. Valid values are: *

The specified optimization was not recognized.

The accepted format is `[+|-]optimization-name`, where `optimization-name` is one of the values listed in the error message.

See [Build optimizations](https://developer.xamarin.com/guides/cross-platform/macios/build-optimizations) for a complete description of each optimization.

### <a name="MM0133"/>MM0133: Found more than 1 assembly matching '{0}' choosing first: '{1}'

### <a name="MM0134"/>MM0134: 32-bit applications should be migrated to 64-bit.

Apple has announced that it will not allow macOS App Store submissions of 32-bit apps (starting January 2018). 

In addition 32-bit applications will not run on the version of macOS after High Sierra "without compromises". 

For more details: https://developer.apple.com/news/?id=06282017a

Consider updating your application and any dependencies to 64-bit.

### <a name="MM0135"/>MM0135: Did not link system framework '{0}' (referenced by assembly '{1}') because it was introduced in {2} {3}, and we're using the {2} {4} SDK.

To build your application, Xamarin.Mac must link against system libraries, some of which depend upon the SDK version specified in the error message. Since you are using an older version of the SDK, invocations to those APIs may fail at runtime.

The recommended way to fix this error is to upgrade Xcode to get the needed SDK. If you have multiple versions of Xcode installed or want to use an Xcode in a non-default location, make sure to set the correct Xcode location in your IDE's preferences.

Alternatively, enable the managed [linker](https://docs.microsoft.com/xamarin/mac/deploy-test/linker) to remove unused APIs, including (in most cases) the new ones which require the specified library. However, this will not work if your project requires APIs introduced in a newer SDK than the one your Xcode provides.

As a last-straw solution, use an older version of Xamarin.Mac that does not require these new SDKs to be present during the build process.

# MM1xxx: file copy / symlinks (project related)

### <a name="MM1034">MM1034: Could not create symlink '{file}' -> '{target}': error {number}

## MM14xx: Product assemblies

### <a name="MM1401">MM1401: The required '{0}' assembly is missing from the references

### <a name="MM1402">MM1402: The assembly '{0}' is not compatible with this tool

### <a name="MM1403">MM1403: {0} '{1}' could not be found. Target framework '{0}' is unusable to package the application.

### <a name="MM1404">MM1404: Target framework '{0}' is invalid.

### <a name="MM1405">MM1405: useFullXamMacFramework must always target framework .NET 4.5, not '{0}' which is invalid

### <a name="MM1406">MM1406: Target framework '{0}' is invalid when targetting Xamarin.Mac 4.5 .NET framwork.

### <a name="MM1407">MM1407: Mismatch between Xamarin.Mac reference '{0}' and target framework selected '{1}'.

## MM15xx: Assembly gathering (not requiring linker) errors

### <a name="MM1501">MM1501: Can not resolve reference: {0}

## MachO.cs

### <a name="MM1600">MM1600: Not a Mach-O dynamic library (unknown header '0x{0}'): {1}.

### <a name="MM1601">MM1601: Not a static library (unknown header '{0}'): {1}.

### <a name="MM1602">MM1602: Not a Mach-O dynamic library (unknown header '0x{0}'): {1}.

### <a name="MM1603">MM1603: Unknown format for fat entry at position {0} in {1}.

### <a name="MM1604">MM1604: File of type {0} is not a MachO file ({1}).

# MM2xxx: Linker

## MM20xx: Linker (general) errors

### <a name="MM2001">MM2001: Could not link assemblies

### <a name="MM2002">MM2002: Can not resolve reference: {0}

### <a name="MM2003">MM2003: Option '{0}' will be ignored since linking is disabled

### <a name="MM2004">MM2004: Extra linker definitions file '{0}' could not be located.

### <a name="MM2005">MM2005: Definitions from '{0}' could not be parsed.

### <a name="MM2006">MM2006: Native library '{0}' was referenced but could not be found.

### <a name="MM2007">MM2007: Xamarin.Mac Unified API against a full .NET profile does not support linking. Pass the -nolink flag.

### <a name="MM2009">MM2009: Referenced by {0}.{1}     ** This message is related to MM2006 **

### <a name="MM2010">MM2010: Unknown HttpMessageHandler `{0}`. Valid values are HttpClientHandler (default), CFNetworkHandler or NSUrlSessionHandler

### <a name="MM2011">MM2011: Unknown TLSProvider `{0}.  Valid values are default or appletls

### <a name="MM2012">MM2012: Only first {0} of {1} "Referenced by" warnings shown. ** This message related to 2009 **

### <a name="MM2013">MM2013: Failed to resolve the reference to "{0}", referenced in "{1}". The app will not include the referenced assembly, and may fail at runtime.

### <a name="MM2014">MM2014: Xamarin.Mac Extensions do not support linking. Request for linking will be ignored. ** This message is obsolete in XM 3.6+ **

<!-- 2015 used by mtouch -->

### <a name="MM2016">MM2016: Invalid TlsProvider `{0}` option. The only valid value `{1}` will be used.

### <a name="MM2017">MM2017: Could not process XML description: {0}

### <a name="MM202x"/>MM202x: Binding Optimizer failed processing `...`.

### <a name="MM2103"/>MM2103: Error processing assembly '\*': *

An unexpected error occured when processing an assembly.

The assembly causing the issue is named in the error message. In order to fix this issue the assembly will need to be provided in a [bug report](https://bugzilla.xamarin.com) along with a complete build log with verbosity enabled (i.e. `-v -v -v -v` in the **Additional mtouch arguments**).

### <a name="MM2104"/>MM2104: Unable to link assembly '{0}' as it is mixed-mode.

Mixed-mode assemblies can not be processed by the linker.

See https://msdn.microsoft.com/en-us/library/x0w2664k.aspx for more information on mixed-mode assemblies.

<!-- 2015 used by mtouch -->

### <a name="MM2106"/>MM2106: Could not optimize the call to BlockLiteral.SetupBlock[Unsafe] in * at offset * because *.

The linker reports this warning when it can't optimize a call to BlockLiteral.SetupBlock or Block.SetupBlockUnsafe.

The message will point to the method that calls BlockLiteral.SetupBlock[Unsafe], and
it may also give clues as to why the call couldn't be optimized.

Please file an [issue](https://github.com/xamarin/xamarin-macios/issues/new)
along with a complete build log so that we can investigate what went wrong and
possibly enable more scenarios in the future.

### <a name="MM2107"/>MM2107: It's not safe to remove the dynamic registrar because {reasons}

The linker reports this warning when the developer requests removal of the
dynamic registrar (by passing `--optimize:remove-dynamic-registrar` to
mmp), but the linker determines that it's not safe to do so.

To remove the warning either remove the optimization argument to mmp, or pass
`--nowarn:2107` to ignore it.

By default this option will be automatically enabled whenever it's possible
and safe to do so.

### <a name="MM2108"/>MM2108: '{0}' was stripped of architectures except '{1}' to comply with App Store restrictions. This could break exisiting codesigning signatures. Consider stripping the library with lipo or disabling with --optimize=-trim-architectures");

The App Store now rejects applications which contain libraries and frameworks which contain 32-bit variants. The library was stripped of unused archtectures when copied into the final application bundle.

This is in general safe, and will reduce application bundle size as an added benefit. However, any bundled framework that is code signed will have its signature invalidated (and resigned later if the application is signed).

Consider using `lipo` to remove the unnecessary archtectures permanently from the source library. If the application is not being published to the App Store, this removal can be disabled by passing --optimize=-trim-architectures as Additional MMP Arguments.

### <a name="MM2109"/>MM2109: Xamarin.Mac Classic API does not support Platform Linking.


# MM3xxx: AOT

## MM30xx: AOT (general) errors

### <a name="MM3001">MM3001: Could not AOT the assembly '{0}'

<!-- 3002 used by mtouch -->
<!-- 3003 used by mtouch -->
<!-- 3004 used by mtouch -->
<!-- 3005 used by mtouch -->
<!-- 3006 used by mtouch -->
<!-- 3007 used by mtouch -->
<!-- 3008 used by mtouch -->
### <a name="MM3009">MM3009: AOT of '{0}' was requested but was not found

### <a name="MM3010">MM3010: Exclusion of AOT of '{0}' was requested but was not found

# MM4xxx: code generation

## MM40xx: driver.m

### <a name="MM4001">MM4001: The main template could not be expanded to `{0}`.

## MM41xx: registrar

### <a name="MM4134">MM4134: Your application is using the '{0}' framework, which isn't included in the MacOS SDK you're using to build your app (this framework was introduced in OSX {2}, while you're building with the MacOS {1} SDK.) This configuration is not supported with the static registrar (pass --registrar:dynamic as an additional mmp argument in your project's Mac Build option to select). Alternatively select a newer SDK in your app's Mac Build options.

### <a name="MM4173"/>MM4173: The registrar can't compute the block signature for the delegate of type {delegate-type} in the method {method} because *.

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
   as an additional mmp argument in the project's Mac Build options).
2. The registrar can't find the `Invoke` method of the delegate. This
   shouldn't happen, so please file an [issue](https://github.com/xamarin/xamarin-macios/issues/new)
   with a test project so that we can fix it.

### <a name="MM4174"/>MM4174: Unable to locate the block to delegate conversion method for the method {method}'s parameter #{parameter}.

This is a warning indicating that the static registrar couldn't find the
method to create a delegate for an Objective-C block. An attempt will be made
at runtime to find the method, but it will likely fail as well (with an MM8009
exception).

One possible reason for this warning is when manually writing bindings for API
that uses blocks. It's recommended to use a binding project to bind
Objective-C code, in particular when it involves blocks, since it's quite
complicated to get it right when doing it manually.

If this is not the case, please file a bug at [https://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=Xamarin.Mac) with a test case.

### <a name="MM4175"/>MM4175: The parameter '{parameter}' in the method '{method}' has an invalid BlockProxy attribute (the type passed to the attribute does not have a 'Create' method).

This is a warning indicating that the parameter in the error message has an
invalid `[BlockProxy]` attribute, where the type passed to the attribute is
unexpected:

```csharp
public override NSUrlSessionDataTask CreateDataTask (NSUrl url, [BlockProxy (typeof (UnexpectedType))] NSUrlSessionResponse completionHandler)
{
}
```

Any `[BlockProxy]` attributes with invalid types will be ignored.

This also means that removing the attribute will fix the warning:

```csharp
public override NSUrlSessionDataTask CreateDataTask (NSUrl url, NSUrlSessionResponse completionHandler)
{
}
```

Xamarin.Mac will instead look for the attribute in the following locations:

* For method overrides (like the example above), on the base method.
* For methods implementing protocol members, on the method in the
  corresponding managed interface.

In all cases the attribute is generated by our binding generator, and should
be present in those locations.

If no attribute can be found, then an <a href="#MM4174">MM4174</a> warning
will be shown.

Reference: https://github.com/xamarin/xamarin-macios/issues/4072

# MM5xxx: GCC and toolchain

## MM51xx: compilation

### <a name="MM5101">MM5101: Missing '{0}' compiler. Please install Xcode 'Command-Line Tools' component.

<!-- 5102 used by mtouch -->

### <a name="MM5103">MM5103: Failed to compile. Error code - {0}. Please file a bug report at http://bugzilla.xamarin.com

<!-- 5104 used by mtouch -->

## MM52xx: linking

### <a name="MM5202">MM5202: Mono.framework MDK is missing. Please install the MDK for your Mono.framework version from http://mono-project.com/Downloads

### <a name="MM5203">MM5203: Can't find libxammac.a, likely because of a corrupted Xamarin.Mac installation. Please reinstall Xamarin.Mac.

### <a name="MM5204">MM5204: Invalid architecture. x86_64 is only supported on non-Classic profiles.

### <a name="MM5205">MM5205: Invalid architecture '{0}'. Valid architectures are i386 and x86_64 (when --profile=mobile).

### <a name="MM5218"/>MM5218: Can't ignore the dynamic symbol {symbol} (--ignore-dynamic-symbol={symbol}) because it was not detected as a dynamic symbol.

See the [equivalent mtouch warning](mtouch-errors.md#MT5218).

<!-- 5206 used by mtouch -->
<!-- 5207 used by mtouch -->
<!-- 5208 used by mtouch -->
<!-- 5209 used by mtouch -->
<!-- 5210 used by mtouch -->
<!-- 5211 used by mtouch -->
<!-- 5212 used by mtouch -->
<!-- 5213 used by mtouch -->
<!-- 5214 used by mtouch -->
<!-- 5215 used by mtouch -->
<!-- 5216 used by mtouch -->
<!-- 5217 used by mtouch -->

## MM53xx: other tools

### <a name="MM5301">MM5301: pkg-config could not be found. Please install the Mono.framework from http://mono-project.com/Downloads

<!-- 5302 used by mtouch -->
<!-- 5303 used by mtouch -->
<!-- 5304 used by mtouch -->

### <a name="MM5305">MM5305: Missing 'otool' tool. Please install Xcode 'Command-Line Tools' component

### <a name="MM5306">MM5306: Missing dependencies. Please install Xcode 'Command-Line Tools' component

### <a name="MM5308">MM5308: Xcode license agreement may not have been accepted.  Please launch Xcode.

### <a name="MM5309">MM5309: Native linking failed with error code 1.  Check build log for details.

### <a name="MM5310">MM5310: install_name_tool failed with an error code '{0}'. Check build log for details.

### <a name="MM5311">MM5311: lipo failed with an error code '{0}'. Check build log for details.

<!-- MM6xxx: mmp internal tools -->
<!-- MM7xxx: reserved -->

# MM8xxx: runtime

## MM800x: misc

<!-- 8000 used by mtouch -->
<!-- 8001 used by mtouch -->
<!-- 8002 used by mtouch -->
<!-- 8003 used by mtouch -->
<!-- 8004 used by mtouch -->
<!-- 8005 used by mtouch -->
<!-- 8006 used by mtouch -->
<!-- 8007 used by mtouch -->
<!-- 8008 used by mtouch -->
<!-- 8009 used by mtouch -->
<!-- 8010 used by mtouch -->
<!-- 8011 used by mtouch -->
<!-- 8012 used by mtouch -->
<!-- 8013 used by mtouch -->
<!-- 8014 used by mtouch -->
<!-- 8015 used by mtouch -->
<!-- 8016 used by mtouch -->

### <a name="MM8017">MM8017: The Boehm garbage collector is not supported. Please use SGen instead.

### <a name="MM8025"/>MM8025: Failed to compute the token reference for the type '{type.AssemblyQualifiedName}' because {reasons}

This indicates a bug in Xamarin.Mac. Please file a bug at [https://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=Xamarin.Mac).

A potential workaround would be to disable the `register-protocols`
optimization, by passing `--optimize:-register-protocols` as an additional mmp
argument in the project's Mac Build options.

### <a name="MM8026"/>MM8026: * is not supported when the dynamic registrar has been linked away.

This usually indicates a bug in Xamarin.Mac, because the dynamic registrar should not be linked away if it's needed. Please file a bug at [https://bugzilla.xamarin.com](https://bugzilla.xamarin.com/enter_bug.cgi?product=Xamarin.Mac).

It's possible to force the linker to keep the dynamic registrar by adding
`--optimize=-remove-dynamic-registrar` to the additional mmp arguments in
the project's Mac Build options.
