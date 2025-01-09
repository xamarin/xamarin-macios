---
title: .NET for iOS, Mac Catalyst, macOS, and tvOS build properties
description: This document lists all the MSBuild properties specific to .NET for iOS, Mac Catalyst, macOS, and tvOS
ms.date: 12/05/2024
---

# Build properties for iOS, Mac Catalyst, macOS and tvOS

MSBuild properties control the behavior of the
[targets](build-targets.md).
They're specified within the project file, for example *MyApp.csproj*, within
an MSBuild PropertyGroup.

## AltoolPath

The full path to the `altool` tool.

The default behavior is to use `xcrun altool`.

## AppBundleDir

The location of the built app bundle.

## AppBundleExtraOptions

Advanced additional arguments for app bundle creation.

The valid set of arguments depend on the platform.

Typically these shouldn't be used unless specified by a Microsoft engineer.

## AppIcon

The `AppIcon` item group can be used to specify an app icon for the app.

The value of the property must point to the filename of an `.appiconset` (for
iOS, macOS and Mac Catalyst) or `.brandassets` (for tvOS) image resource
inside an asset catalog.

Example:

```xml
<PropertyGroup>
    <!-- The value to put in here for the "Resources/MyImages.xcassets/MyAppIcon.appiconset" resource would be "MyAppIcon" -->
    <AppIcon>MyAppIcon</AppIcon>
</PropertyGroup>
```

See also:

* The [AlternateAppIcon](build-items.md#alternateappicon) item group.
* The [IncludeAllAppIcons](#includeallappicons) property.

## ApplicationDisplayVersion

If set, specifies the `CFBundleShortVersionString` value in the app manifest (`Info.plist`).

This is a property that supports .NET "Single Project".

See [OneDotNetSingleProject](https://github.com/dotnet/android/blob/main/Documentation/guides/OneDotNetSingleProject.md) for more information.

## ApplicationId

If set, specifies the `CFBundleIdentifier` value in the app manifest (`Info.plist`).

This is a property that supports .NET "Single Project".

See [OneDotNetSingleProject](https://github.com/dotnet/android/blob/main/Documentation/guides/OneDotNetSingleProject.md) for more information.

## ApplicationTitle

If set, specifies the `CFBundleDisplayName` in the app manifest (`Info.plist`).

This is a property that supports .NET "Single Project".

See [OneDotNetSingleProject](https://github.com/dotnet/android/blob/main/Documentation/guides/OneDotNetSingleProject.md) for more information.

## ApplicationVersion

If set, specifies the `CFBundleVersion` in the app manifest (`Info.plist`).

This is a property that supports .NET "Single Project".

See [OneDotNetSingleProject](https://github.com/dotnet/android/blob/main/Documentation/guides/OneDotNetSingleProject.md) for more information.

## ArchiveBasePath

The location where archives are stored on Windows for a remote build.

The default is: `%LocalAppData%\Xamarin\iOS\Archives`

Only applicable to iOS projects (since only iOS projects can be built remotely from Windows).

## ArchiveOnBuild

If an Xcode archive should be created at the end of the build.

## BGenEmitDebugInformation

Whether the `bgen` tool (the binding generator) should emit debug information or not.

The default behavior is `true` when the `Debug` property is set to `true`.

## BGenExtraArgs

Any extra arguments to the `bgen` tool (the binding generator).

## BGenToolExe

The name of the `bgen` executable (a tool used by binding projects to generate bindings).

The default behavior is to use the `bgen` tool shipped with our workload.

## BGenToolPath

The directory to where the `bgen` ([BGenToolExe](#bgentoolexe)) is located.

The default behavior is to use the `bgen` tool shipped with our workload.

## BuildIpa

If a package (.ipa) should be created for the app bundle at the end of the build.

Only applicable to iOS and tvOS projects.

See [CreatePackage](#createpackage) for macOS and Mac Catalyst projects.

## BundleOriginalResources

This property determines whether resources are compiled before being embedded
into library projects, or if the original (uncompiled) version is embedded.

Historically resources have been compiled before being embedded into library
projects, but this requires having Xcode available, which has a few drawbacks:

* It slows down remote builds on Windows.
* It won't work when building locally on Windows, and neither on any other
  platform except macOS.
* Resources are compiled using the current available Xcode, which may not have
  the same features as a potentially newer Xcode available when the library in
  question is consumed.
* It makes it impossible to have a whole-program view of all the resources
  when building an app, which is necessary to detect clashing resources.

As such, we've added supported for embedding the original resources into
libraries. This will be opt-in in .NET 9, but opt-out starting in .NET 10.

Default value: `false` in .NET 9, `true` in .NET 10+.

Note: please file an issue if you find that you need to disable this feature,
as it's possible we'll remove the option to disable it at some point.

## CodesignAllocate

The path to the `codesign_allocate` tool.

By default this value is auto-detected.

## CodesignDependsOn

This is an extension point for the build: a developer can add any targets to
this property to execute those targets before the app bundle is signed.

Example:

```xml
<PropertyGroup>
  <CodesignDependsOn>$(CodesignDependsOn);DoThisBeforeCodesign</CodesignDependsOn>
</PropertyGroup>
<Target Name="DoThisBeforeCodesign">
  <Exec Command="echo This is executed right before the app is signed." />
</Target>
```

## CodesignEntitlements

The path to the entitlements file that specifies the entitlements the app requires.

Typically "Entitlements.plist".

We'll automatically set this to "Entitlements.plist" if such a file exists in the project root directory.

This can be prevented by setting the [EnableDefaultCodesignEntitlements](#enabledefaultcodesignentitlements) property to `false`.

## CodesignExtraArgs

Extra arguments passed to the 'codesign' tool when signing the app bundle.

## CodesignKey

Specifies the code signing key to use when signing the app bundle.

## CodesignKeychain

The keychain to use during code signing.

## CodeSigningKey

Specifies the code signing key to use when signing the app bundle.

Only applicable to macOS and Mac Catalyst apps, but it's recommended to use the [CodesignKey](#codesignkey) property instead (which works on all platforms).

## CodesignProvision

Specifies the provisioning profile to use when signing the app bundle.

## CodesignResourceRules

The path to the ResourceRules.plist to copy to the app bundle.

## CodesignRequireProvisioningProfile

Specifies whether a provisioning profile is required when signing the app bundle.

By default we require a provisioning profile if:

* macOS, Mac Catalyst: a provisioning profile has been specified (with the [CodesignProvision](#codesignprovision) property).
* iOS, tvOS, watchOS: building for device or an entitlements file has been specified (with the [CodesignEntitlements](#codesignentitlements) property).

Setting this property to `true` or `false` will override the default logic.

## CreateAppBundleDependsOn

This is an extension point for the build: a developer can add any targets to
this property to execute those targets when creating the app bundle.

Example:

```xml
<PropertyGroup>
  <CreateAppBundleDependsOn>$(CreateAppBundleDependsOn);DoThisBeforeCreatingAppBundle</CreateAppBundleDependsOn>
</PropertyGroup>
<Target Name="DoThisBeforeCreatingAppBundle">
  <Exec Command="echo This is executed before the app bundle is created." />
</Target>
```

## CreateIpaDependsOn

This is an extension point for the build: a developer can add any targets to
this property to execute those targets when creating an IPA.

Applicable to all platforms that build IPA archives (currently iOS and tvOS).

Example:

```xml
<PropertyGroup>
  <CreateIpaDependsOn>$(CreateIpaDependsOn);DoThisBeforeCreatingIPA</CreateIpaDependsOn>
</PropertyGroup>
<Target Name="DoThisBeforeCreatingIPA">
  <Exec Command="echo This is executed before the IPA is created." />
</Target>
```

## CreatePackage

If a package (.pkg) should be created for the app bundle at the end of the build.

Only applicable to macOS and Mac Catalyst projects.

See [BuildIpa](#buildipa) for iOS and tvOS projects.

## DeviceSpecificBuild

If the build should be specific to the selected device.

Applicable to all platforms that support device-specific builds (currently iOS and tvOS).

## DeviceSpecificIntermediateOutputPath

The intermediate output path to use when device-specific builds are enabled.

Applicable to all platforms that support device-specific builds (currently iOS and tvOS).

## DeviceSpecificOutputPath

The output path to use when device-specific builds are enabled.

Applicable to all platforms that support device-specific builds (currently iOS and tvOS).

## DittoPath

The full path to the `ditto` executable.

The default behavior is to use `/usr/bin/ditto`.

## EmbedOnDemandResources

If on-demand resources should be embedded in the app bundle.

Default: true

## EnableCodeSigning

If code signing is enabled.

Typically the build will automatically determine whether code signing is
required; this automatic detection can be overridden with this property.

## EnableDefaultCodesignEntitlements

See [CodesignEntitlements](#codesignentitlements).

## EnableOnDemandResources

If on-demand resources are enabled.

Default: false for macOS, true for all other platforms.

## EnablePackageSigning

If the .pkg that was created (if `CreatePackage` was enabled) should be signed.

Only applicable to macOS and Mac Catalyst.

## EnableSGenConc

Enables the concurrent mode for the SGen garbage collector.

Only applicable to iOS, tvOS and Mac Catalyst (when not using NativeAOT).

## GenerateApplicationManifest

If an application manifest (`Info.plist`) should be generated.

Default: true

## GeneratedSourcesDir

Where the generated source from the generator are saved.

## IncludeAllAppIcons

Set the `IncludeAllAppIcons` property to true to automatically include all app
icons from all asset catalogs in the app.

Example:

```xml
<PropertyGroup>
    <IncludeAllAppIcons>true</IncludeAllAppIcons>
</PropertyGroup>
```

See also:

* The [AlternateAppIcon](build-items.md#alternateappicon) item group.
* The [AppIcon](#appicon) property.

## iOSMinimumVersion

Specifies the minimum iOS version the app can run on.

Applicable to iOS; setting this value will set [SupportedOSPlatformVersion](#supportedosplatformversion) for iOS projects (only).

## IPhoneResourcePrefix

The directory where resources are stored (this prefix will be removed when copying resources to the app bundle).

Applicable to iOS, tvOS and Mac Catalyst projects.

See also [MonoMacResourcePrefix](#monomacresourceprefix) and [XamMacResourcePrefix](#xammacresourceprefix).

## IpaIncludeArtwork

If artwork should be included in the IPA.

Only applicable to iOS and tvOS projects.

## IpaPackageName

Specifies the name of the resulting .ipa file (without the path) when creating
an IPA package (see [BuildIpa](#buildipa)). [IpaPackagePath](#ipapackagepath)
will override this value.

Only applicable to iOS and tvOS projects.

## IpaPackageDir

Specifies the directory of the resulting .ipa file when creating an IPA
package (see [BuildIpa](#buildipa)). [IpaPackagePath](#ipapackagepath) will
override this value.

Only applicable to iOS and tvOS projects.

## IpaPackagePath

Specifies the path to the resulting .ipa file when creating an IPA package (see [BuildIpa](#buildipa)).

Only applicable to iOS and tvOS projects.

## IsAppExtension

If a project is an app extension.

## IsBindingProject

If a project is a binding project.

## IsXPCService

If a macOS extension is an xpc service.

Only applicable to macOS projects.

## LinkMode

Specifies the link mode for the project (`None`, `SdkOnly` or `Full`).

Applicable to macOS projects, but this property is deprecated, use
[TrimMode](#trimmode) instead.

See also [MtouchLink](#mtouchlink).

## LinkWithSwiftSystemLibraries

If set to `true`, the build will tell the native linker where to find Swift's system libraries.

This is useful when a native library uses Swift somehow, in which case the
native linker needs to know where to find Swift's system libraries.

Currently this means these arguments will be passed to the native linker:

* -L/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/lib/swift/[platform]
* -L/Applications/Xcode.app/Contents/Developer/Platforms/[platform].platform/Developer/SDKs/[platform].sdk/usr/lib/swift

The exact set of arguments may change in the future.

## MacCatalystMinimumVersion

Specifies the minimum Mac Catalyst (iOS) version the app can run on.

Applicable to Mac Catalyst; setting this value will set [SupportedOSPlatformVersion](#supportedosplatformversion) for Mac Catalyst projects (only).

## MaciOSPrepareForBuildDependsOn

A semi-colon delimited property that can be used to extend the build process.
MSBuild targets added to this property will execute early in the build for both
application and library project types. This property is empty by default.

Example:

```xml
<PropertyGroup>
  <MaciOSPrepareForBuildDependsOn>$(MaciOSPrepareForBuildDependsOn);MyCustomTarget</MaciOSPrepareForBuildDependsOn>
</PropertyGroup>

<Target Name="MyCustomTarget" >
  <Message Text="Running target: 'MyCustomTarget'" Importance="high"  />
</Target>
```

This property was introduced in .NET 9.

## macOSMinimumVersion

Specifies the minimum macOS version the app can run on.

Applicable to macOS; setting this value will set [SupportedOSPlatformVersion](#supportedosplatformversion) for macOS projects (only).

## MacOSXSdkVersion

The macOS SDK version to use for the build.

Default: automatically detected according to the default version shipped with the selected Xcode.

See also [MtouchSdkVersion](#mtouchsdkversion).

## MarshalManagedExceptionMode

Choose how managed exceptions are handled when encountering a native frame
during stack unwinding while processing the managed exception.

Valid values:

* `default`: Currently, this is `throwobjectivecexception`.
* `unwindnativecode`: This is not available when using the CoreCLR runtime.
* `throwobjectivecexception`: Catch the managed exception, and convert it into an Objective-C exception.
* `abort`: Abort the process.
* `disable`: Disable intercepting any managed exceptions. For MonoVM this is equivalent to `unwindnativecode`, for CoreCLR this is equivalent to `abort`.

For more information see the article about [Exception marshaling](todo)

See also [MarshalObjectiveCExceptionMode](#marshalobjectivecexceptionmode)

## MarshalObjectiveCExceptionMode

Choose how Objective-C exceptions are handled when encountering a managed frame
during stack unwinding while processing the Objective-C exception.

Valid values:

* `default`: Currently, this is `throwmanagedexception`.
* `unwindmanagedcode`: This is not available when using the CoreCLR runtime.
* `throwmanagedexception`:  Catch the Objective-C exception, and convert it into a managed exception.
* `abort`: Abort the process.
* `disable`: Disable intercepting any Objective-C exceptions.

For more information see the article about [Exception marshaling](todo)

See also [MarshalManagedExceptionMode](#marshalmanagedexceptionmode)

## MdimportPath

The full path to the `mdimport` tool.

The default behavior is to use `xcrun mdimport`.

## MetalLibPath

The full path to the `metallib` tool (the Metal Linker).

The default behavior is to use `xcrun metallib`.

## MetalPath

The full path to the Metal compiler.

The default behavior is to use `xcrun metal`.

## MmpDebug

Enables debug mode for app bundle creation.

Only applicable to macOS projects.

See also [MtouchDebug](#mtouchdebug).

## MobileAggressiveAttributeTrimming

This property determines whether numerous attributes that are very rarely
needed at runtime should be trimmed away.

This is enabled by default.

Note that while the attributes that are removed are very rarely used, it's
technically possible that the removal can change runtime behavior.

For example, System.Xml.Serialization will behave differently if a constructor
has the `[Obsolete]` attribute (which is one of the attributes that are
removed). This is low enough risk to justify removing these attributes by
default because of the size savings.

The list of attributes that are removed may change in the future, but at the
time of this writing (for .NET 10), these are the attributes:

* Microsoft.CodeAnalysis.EmbeddedAttribute
* System.CLSCompliantAttribute
* System.CodeDom.Compiler.GeneratedCodeAttribute
* System.ComponentModel.EditorBrowsableAttribute
* System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute
* System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute
* System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute
* System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute
* System.Diagnostics.CodeAnalysis.ExperimentalAttribute
* System.Diagnostics.CodeAnalysis.FeatureGuardAttribute
* System.Diagnostics.CodeAnalysis.FeatureSwitchDefinitionAttribute
* System.Diagnostics.CodeAnalysis.MemberNotNullAttribute
* System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute
* System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute
* System.Diagnostics.CodeAnalysis.NotNullWhenAttribute
* System.Diagnostics.CodeAnalysis.RequiresAssemblyFilesAttribute
* System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute
* System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute
* System.Diagnostics.CodeAnalysis.StringSyntaxAttribute
* System.Diagnostics.CodeAnalysis.SuppressMessageAttribute
* System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessageAttribute
* System.Diagnostics.CodeAnalysis.UnscopedRefAttribute
* System.ObsoleteAttribute
* System.Reflection.AssemblyCompanyAttribute
* System.Reflection.AssemblyConfigurationAttribute
* System.Reflection.AssemblyCopyrightAttribute
* System.Reflection.AssemblyDefaultAliasAttribute
* System.Reflection.AssemblyDescriptionAttribute
* System.Reflection.AssemblyMetadataAttribute
* System.Reflection.AssemblyProductAttribute
* System.Reflection.AssemblyTitleAttribute
* System.Runtime.CompilerServices.AsyncMethodBuilderAttribute
* System.Runtime.CompilerServices.CallerArgumentExpressionAttribute
* System.Runtime.CompilerServices.CallerFilePathAttribute
* System.Runtime.CompilerServices.CallerLineNumberAttribute
* System.Runtime.CompilerServices.CallerMemberNameAttribute
* System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute
* System.Runtime.CompilerServices.CompilerGlobalScopeAttribute
* System.Runtime.CompilerServices.EnumeratorCancellationAttribute
* System.Runtime.CompilerServices.ExtensionAttribute
* System.Runtime.CompilerServices.InterpolatedStringHandlerArgumentAttribute
* System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute
* System.Runtime.CompilerServices.IntrinsicAttribute
* System.Runtime.CompilerServices.IsReadOnlyAttribute
* System.Runtime.CompilerServices.IsUnmanagedAttribute
* System.Runtime.CompilerServices.NativeIntegerAttribute
* System.Runtime.CompilerServices.RefSafetyRulesAttribute
* System.Runtime.CompilerServices.ScopedRefAttribute
* System.Runtime.CompilerServices.SkipLocalsInitAttribute
* System.Runtime.CompilerServices.TupleElementNamesAttribute
* System.Runtime.InteropServices.LibraryImportAttribute
* System.Runtime.InteropServices.Marshalling.ContiguousCollectionMarshallerAttribute
* System.Runtime.InteropServices.Marshalling.CustomMarshallerAttribute
* System.Runtime.InteropServices.Marshalling.MarshalUsingAttribute
* System.Runtime.InteropServices.Marshalling.NativeMarshallingAttribute
* System.Runtime.Versioning.NonVersionableAttribute
* System.Runtime.Versioning.ObsoletedOSPlatformAttribute
* System.Runtime.Versioning.RequiresPreviewFeaturesAttribute
* System.Runtime.Versioning.SupportedOSPlatformAttribute
* System.Runtime.Versioning.SupportedOSPlatformGuardAttribute
* System.Runtime.Versioning.TargetPlatformAttribute
* System.Runtime.Versioning.UnsupportedOSPlatformAttribute
* System.Runtime.Versioning.UnsupportedOSPlatformGuardAttribute

This property was introduced in .NET 10.

## MonoBundlingExtraArgs

Additional arguments specifying how to create the app bundle.

Only applicable to macOS projects.

This property is deprecated, use [AppBundleExtraOptions](#appbundleextraoptions) instead.

## MonoMacResourcePrefix

The directory where resources are stored (this prefix will be removed when copying resources to the app bundle).

Only applicable to macOS projects.

See also [IPhoneResourcePrefix](#iphoneresourceprefix) and [XamMacResourcePrefix](#xammacresourceprefix).

## MtouchDebug

Enables debug mode for app bundle creation.

Applicable to iOS, tvOS and Mac Catalyst projects.

See also [MmpDebug](#mmpdebug).

## MtouchEnableSGenConc

Enables the concurrent mode for the SGen garbage collector.

Only applicable to iOS, tvOS and Mac Catalyst when not using NativeAOT.

This property is deprecated, use [EnableSGenConc](#enablesgenconc) instead.

## MtouchExtraArgs

Additional arguments specifying how to create the app bundle.

Only applicable to iOS, tvOS and Mac Catalyst projects.

This property is deprecated, use [AppBundleExtraOptions](#appbundleextraoptions) instead.

## MtouchInterpreter

Enables the interpreter, and optionally takes a comma-separated list of
assemblies to interpret (if prefixed with a minus sign, the assembly will be
AOT-compiled instead). 'all' can be used to specify all assemblies. This
argument can be specified multiple times.

Example:

```xml
<PropertyGroup>
  <!-- interpret all assemblies -->
  <MtouchInterpreter>all</MtouchInterpreter>

  <!-- AOT-compile all assemblies, except System.dll, which will be interpreted. -->
  <MtouchInterpreter>System</MtouchInterpreter>

  <!-- interpret all assemblies, except System.Core.dll, which will be AOT-compiled. -->
  <MtouchInterpreter>all,-System.Core</MtouchInterpreter>
</PropertyGroup>
```

A shorthand for the `MtouchInterpreter` is to set `UseInterpreter=true`, which
is equivalent to `MtouchInterpreter=all`.

If both `UseInterpreter` and `MtouchInterpreter` are set, then
`MtouchInterpreter` takes precedence.

Applicable to iOS, tvOS and Mac Catalyst apps (when not using NativeAOT).

The default behavior is to not enable the interpreter.

> [!NOTE]
> MAUI changes the default by setting `UseInterpreter=true` for the `"Debug"` configuration.

## MtouchLink

Specifies the link mode for the project (`None`, `SdkOnly`, `Full`).

Applicable to iOS, tvOS and Mac Catalyst projects, but this property is
deprecated, use [TrimMode](#trimmode) instead.

See also [LinkMode](#linkmode).

## MtouchSdkVersion

The iOS or tvOS SDK version to use for the build.

Default: automatically detected according to the default version shipped with the selected Xcode.

See also [MacOSXSdkVersion](#macosxsdkversion).

## MtouchUseLlvm

A boolean property that specifies whether AOT compilation should be done using LLVM.

Applicable to iOS, tvOS and Mac Catalyst projects.

Default:

* On iOS and tvOS: enabled for Release builds (where `Configuration="Release"`).
* On Mac Catalyst: never enabled by default.

## NoBindingEmbedding

A boolean property that specifies whether native libraries in binding projects should be embedded
in the managed assembly, or put into a `.resources` directory next to the managed assembly.

The default value is `true` (which means native libraries will _not_ be embeddded in the managed assembly).

> [!NOTE]
> Xcframeworks won't work correctly if embedded inside the managed assembly (if this property is not `true`).

## NoDSymUtil

A boolean property that specifies whether .dSYM generation should be disabled.

Default:

* `true` for iOS and tvOS when building for the simulator.
* `true` for macOS and Mac Catalyst unless creating an archive (`ArchiveOnBuild=true`)

This means the .dSYM archive will be generated in the following cases (by default):

* On iOS and tvOS when building for device.
* On macOS and Mac Catalyst when creating an archive (`ArchiveOnBuild=true`).

## OnDemandResourcesInitialInstallTags

A string property that specifies the initial install tags for on-demand resources.

## OnDemandResourcesPrefetchOrder

A string property that specifies the prefetch order for on-demand resources.

## OnDemandResourcesUrl

A string property that specifies the resource url for on-demand resources.

## OptimizePNGs

A boolean property that specifies whether png images should be optimized.

## OptimizePngImagesDependsOn

This is an extension point for the build: a developer can add any targets to
this property to execute those targets before any png images are optimized.

Example:

```xml
<PropertyGroup>
  <OptimizePngImagesDependsOn>$(OptimizePngImagesDependsOn);MyCustomTarget</OptimizePngImagesDependsOn>
</PropertyGroup>

<Target Name="MyCustomTarget" >
  <Message Text="Running target: 'MyCustomTarget'" Importance="high"  />
</Target>
```

## OptimizePropertyLists

A boolean property that specifies whether property lists (plists) should be optimized.

## OptimizePropertyListsDependsOn

This is an extension point for the build: a developer can add any targets to
this property to execute those targets before any property lists (plists) are
optimized.

Example:

```xml
<PropertyGroup>
  <OptimizePropertyListsDependsOn>$(OptimizePropertyListsDependsOn);MyCustomTarget</OptimizePropertyListsDependsOn>
</PropertyGroup>

<Target Name="MyCustomTarget" >
  <Message Text="Running target: 'MyCustomTarget'" Importance="high"  />
</Target>
```

## PackageSigningKey

Specifies the code signing key to sign the package when creating .pkg for a macOS and Mac Catalyst project.

Only applicable to macOS and Mac Catalyst apps.

## PackagingExtraArgs

Specifies any extra arguments to pass to the 'productbuild' tool when creating .pkg for a macOS and Mac Catalyst project.

Only applicable to macOS and Mac Catalyst apps.

## PkgPackagePath

Specifies the path to the resulting .pkg file when creating a package (see [CreatePackage](#createpackage)).

Only applicable to macOS and Mac Catalyst apps.

## PlutilPath

The full path to the `plutil` command-line tool.

The default behavior is to use `xcrun plutil`.

## PngCrushPath

The full path to the `pngcrush` command-line tool.

The default behavior is to use `xcrun pngcrush`.

## ProcessEnums

A boolean property that specifies whether enums should be processed as an api definition in binding projects.

## ProductBuildPath

The full path to the `productbuild` tool.

The default behavior is to use `xcrun productbuild`.

## ProductDefinition

The product definition template (`.plist`) to be used when creating the product definition to pass to the product build tool when creating packages (.pkg).

Only applicable to macOS and Mac Catalyst apps.

## ReferenceNativeSymbol

The item group `ReferenceNativeSymbol` can be used to specify how we should
handle a given native symbol: either ignore it, or ask the native linker to
keep it (by passing the symbol as `-u ...` or in a symbol file to the native
linker).

There are two supported types of metadata:

* `SymbolType`: either `ObjectiveCClass`, `Function` or `Field`. Used to
  compute the complete native name of a symbol (for instance, the native
  symbol for the Objective-C class `MyClass` is `_OBJC_CLASS_$_MyClass`,
  while for a function `MyFunction` it's just `_MyFunction`.
* `SymbolMode`: either `Ignore` or not set. `Ignore` means to not pass the given
  symbol to the native linker, the default is to do so.

`SymbolType` is required, while `SymbolMode` isn't.

Example symbol to keep:

```xml
<ItemGroup>
    <ReferenceNativeSymbol Include="MyClass" SymbolType="ObjectiveCClass" />
</ItemGroup>
```

Example symbol to ignore:

```xml
<ItemGroup>
    <ReferenceNativeSymbol Include="MyClass" SymbolType="ObjectiveCClass" SymbolMode="Ignore" />
</ItemGroup>
```

## RequireLinkWithAttributeForObjectiveCClassSearch

We will automatically scan all libraries for managed classes that map to
existing Objective-C classes, and then create a native reference at build time
for those Objective-C classes.

This way the native linker won't remove these Objective-C classes, thinking
they're not used.

However, this can cause a problem if a managed class references an Objective-C
class that doesn't exist. The proper fix for this is to remove such managed
classes from the build, but this may be cumbersome, in particular if the
managed class comes from a binary reference (such as NuGet).

In these cases, it's possible to set the property
`RequireLinkWithAttributeForObjectiveCClassSearch` to `true` so that we'll
only scan libraries with the `[LinkWith]` attribute for Objective-C classes:

```xml
<PropertyGroup>
  <RequireLinkWithAttributeForObjectiveCClassSearch>true</RequireLinkWithAttributeForObjectiveCClassSearch>
</PropertyGroup>
```

## SkipStaticLibraryValidation

Hot Restart doesn't support linking with static libraries, so by default we'll
show an error if the project tries to link with any static libraries when
using Hot Restart.

However, in some cases it might be useful to ignore such errors (for instance if testing a code path in the app that doesn't require the static library in question), so it's possible to ignore them.

The valid values are:

* "true", "disable": Completely disable the validation.
* "false", "error", empty string: Enable the validation (this is the default)
* "warn": Validate, but show warnings instead of errors.

Example:

```xml
<PropertyGroup>
  <SkipStaticLibraryValidation>warn</SkipStaticLibraryValidation>
</PropertyGroup>
```

This will show warnings instead of errors if the project tries to link with a static library.

## StripPath

The full path to the `strip` command-line tool.

The default behavior is to use `xcrun strip`.

## SupportedOSPlatformVersion

Specifies the minimum OS version the app can run on.

It's also possible to use a platform-specific property:

* [iOSMinimumVersion](#iosminimumversion)
* [tvOSMinimumVersion](#tvosminimumversion)
* [macOSMinimumVersion](#macosminimumversion)
* [MacCatalystMinimumVersion](#maccatalystminimumversion)

## TrimMode

Specifies the trimming granularity.

The valid options are:

* `full`: Trim every assembly.
* `partial`: Trim assemblies that have opted into trimming.
* `copy`: Trim no assemblies.

See [TrimMode](/dotnet/core/deploying/trimming/trimming-options) for a bit more information about the `TrimMode` property.

> [!NOTE]
> For technical reasons, the trimmer must run for all iOS, tvOS, macOS
> and Mac Catalyst projects, even if no assemblies are to be trimmed. For this
> reason, it's not valid to disable trimming by setting
> [PublishTrimmed](/dotnet/core/deploying/trimming/trimming-options?#enable-trimming)
> to `false` - to disable trimming, set `TrimMode=copy` instead (a build error
> will be raised if `PublishTrimmed` is set to `false`).

The `TrimMode` property is equivalent to the existing
[MtouchLink](#mtouchlink) (for iOS, tvOS and Mac Catalyst) and
[LinkMode](#linkmode) (for macOS) properties, but the valid properties values
are different (even though the semantics are the same):

| MtouchLink/LinkMode | TrimMode |
| --------------------|----------|
| Full                | full     |
| SdkOnly             | partial  |
| None                | copy     |

Going forward, the `MtouchLink` and `LinkMode` properties will be deprecated, please use `TrimMode` instead.

The default trim mode depends on numerous factors, and may also change in the future.

The current (as of .NET 9) default values are:

* iOS and iOS: `partial` when building for device, `copy` when building for the simulator.
* macOS: always `copy`.
* Mac Catalyst: `partial` when building for the `"Release"` configuration, `copy` otherwise.

Exceptions:

* The default value is always `full` when building with NativeAOT.
* MAUI changes the default value to `copy` when building for the `Debug`
  configuration _and_ the interpreter is enabled using
  [UseInterpreter](#useinterpreter) (which MAUI also enables by default when
  using the `"Debug"` configuration).

> [!NOTE]
> The default trim mode may change in the future.

## tvOSMinimumVersion

Specifies the minimum tvOS version the app can run on.

Applicable to tvOS; setting this value will set [SupportedOSPlatformVersion](#supportedosplatformversion) for tvOS projects (only).

## UseHardenedRuntime

A boolean property that specifies if a hardened runtime is enabled.

Applicable to macOS and Mac Catalyst projects.

## UseInterpreter

Enables the interpreter (for all assemblies).

This is equivalent to setting `MtouchInterpreter=all`.

Applicable to iOS, tvOS and Mac Catalyst apps (when not using NativeAOT).

The default behavior is to not enable the interpreter.

> [!NOTE]
> MAUI changes the default by setting `UseInterpreter=true` for the `"Debug"` configuration.

See [MtouchInterpreter](#mtouchinterpreter) for more information.

## UseNativeHttpHandler

Whether the native http handler should be the default http handler or not.

Default: true for all platforms except macOS.

## ValidateEntitlements

Choose whether entitlements the app requests should be validated.

Valid values for this property:

* `disable`: Validation is disabled.
* `warn`: Any validation failures are shown as warnings.
* `error`: Any validation failures are shown as errors. This is the default.

The validation process may not validate every entitlement, nor is it guaranteed to not be overeager.

If the validation fails for entitlements that actually work, please file a new issue.

## XamMacResourcePrefix

The directory where resources are stored (this prefix will be removed when copying resources to the app bundle).

Applicable to macOS projects.

See also [IPhoneResourcePrefix](#iphoneresourceprefix) and [MonoMacResourcePrefix](#monomacresourceprefix).

## ZipPath

The full path to the `zip` command-line tool.

The default behavior is to use `xcrun zip`.
