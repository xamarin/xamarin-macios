---
title: .NET for iOS, Mac Catalyst, macOS, and tvOS build items
description: This document lists all the MSBuild items specific to .NET for iOS, Mac Catalyst, macOS, and tvOS
ms.date: 12/05/2024
---

# Build items for iOS, Mac Catalyst, macOS and tvOS

Build items control how .NET for iOS, Mac Catalyst, macOS, and tvOS
application or library projects are built.

## AdditionalAppExtensions

An item group that contains any additional app extensions to copy into the app bundle.

The following metadata can be set:

* Include: The path to the build directory for the Xcode app extension project.
* Name: The name of the extension.
* BuildOutput: This value is appended to the `Include` value to produce the location of the appex bundle. Typically Xcode will place simulator and device builds in different locations, so this can be used to have a single `AdditionalAppExtensions` entry pointing to two different appex bundles, depending on whether building for the simulator or device.
* CodesignEntitlements: Specifies the entitlements to use when signing the app extension. The default value is '%(Name).entitlements' in the 'Include' build directory (if this file exists).
* CodesignWarnIfNoEntitlements: A warning will be produced if no `CodesignEntitlements` value is set. This property can be set to `false` to silence this warning.

Example:

```xml
<ItemGroup>
    <AdditionalAppExtensions Include="path/to/my.appex">
        <Name>MyAppExtensionName</Name>
        <BuildOutput Condition="'$(SdkIsSimulator)' == 'false'">DerivedData/MyAppExtensionName/Build/Products/Debug-iphoneos</BuildOutput>
        <BuildOutput Condition="'$(SdkIsSimulator)' == 'true'">DerivedData/MyAppExtensionName/Build/Products/Debug-iphonesimulator</BuildOutput>
        <CodesignEntitlements>path/to/Entitlements-appextension.plist</CodesignEntitlements>
        <CodesignWarnIfNoEntitlements>false</CodesignWarnIfNoEntitlements>
    </AdditionalAppExtensions>
</ItemGroup>
```

An example solution can be found here: [TestApplication](https://github.com/chamons/xamarin-ios-swift-extension/tree/master/App/net6/TestApplication).

## AlternateAppIcon

The `AlternateAppIcon` item group can be used to specify alternate app icons.

The `Include` metadata must point to the filename of an `.appiconset` (for
iOS, macOS and Mac Catalyst) or `.imagestack` (for tvOS) image resource
inside an asset catalog.

Example:

```xml
<ItemGroup>
    <!-- The value to put in here for the "Resources/MyImages.xcassets/MyAlternateAppIcon.appiconset" resource would be "MyAlternateAppIcon" -->
    <AlternateAppIcon Include="MyAlternateAppIcon" />
</ItemGroup>
```

See also:
* The [AppIcon](build-properties.md#appicon) property.
* The [IncludeAllAppIcons](build-properties.md#includeallappicons) property.

## AtlasTexture

An item group that contains atlas textures.

## AppManifestEntry

An item group that contains entries to add to the app manifest (Info.plist).

These entries are processed last and override entries from the main app
manifest, partial app manifests, and generated values.

The following types are supported:

```xml
<ItemGroup>
    <AppManifestEntry Include="BooleanKey" Type="Boolean" Value="true" />
    <AppManifestEntry Include="StringKey" Type="String" Value="stringvalue" />
    <AppManifestEntry Include="StringArrayKey" Type="StringArray" Value="a;b" />
    <AppManifestEntry Include="StringArrayKeyWithCustomSeparator" Type="StringArray" Value="a|b" ArraySeparator="|" />
    <AppManifestEntry Include="KeyToRemove" Type="Remove" />
</ItemGroup>
```

Boolean values may be `true` or `false` (case-insensitive). String arrays use a
semicolon as the default separator, which can be changed with the
`ArraySeparator` metadata.

## BGenReferencePath

The list of assembly references to pass to the `bgen` tool (binding generator).

Typically this is handled automatically by adding references as
`ProjectReference` or `PackageReference` items instead.

## BundleResource

Files to be copied to the app bundle.

See also:

* https://github.com/dotnet/macios/blob/main/dotnet/BundleContents.md

## CodesignBundle

Additional bundles inside the final app that should be signed.

The purpose is to include in the app signing any other bundles that are copied manually (for instance through custom MSBuild targets during the build) to the app bundle.

The path to include is the path to the app bundle to sign inside the main app bundle, including the app bundle name itself.

Example:

```xml
<ItemGroup>
    <CodesignBundle Include="$(AssemblyName).app/Contents/SharedSupport/MyCustomBundle.app" />
</ItemGroup>
```

There are several pieces of metadata that can be set on the `CodesignBundle` item to direct how signing occurs:

* [CodesignAllocate](build-properties.md#codesignallocate)
* [CodesignEntitlements](build-properties.md#codesignentitlements)
* [CodesignExtraArgs](build-properties.md#codesignextraargs)
* [CodesignKeychain](build-properties.md#codesignkeychain)
* [CodesignResourceRules](build-properties.md#codesignresourcerules)
* [CodesignSigningKey](build-properties.md#codesignkey)
* [CodesignUseHardenedRuntime](build-properties.md#usehardenedruntime)
* [CodesignUseSecureTimestamp](build-properties.md#usehardenedruntime)

Example:

```xml
<ItemGroup>
    <CodesignBundle Include="$(AssemblyName).app/Contents/SharedSupport/MyCustomBundle.app">
        <CodesignEntitlements>path/to/Entitlements.plist</CodesignEntitlements>
    </CodesignBundle>
</ItemGroup>
```

Any metadata not set will use the corresponding property instead (for instance if the `CodesignSigningKey` metadata is not set, the value of the `CodesignSigningKey` property will be used instead.)

## Collada

An item group that contains collada assets.

## Content

Resources (files) to be copied to the app bundle.

They will be placed in the the following directory inside the app bundle:

* /Resources: iOS and tvOS
* /Contents/Resources: macOS and Mac Catalyst

It's possible to set the `Link` metadata to a path relative to the target
directory to change the location in the app bundle.

Example:

```xml
<ItemGroup>
    <Content Include="Readme.txt" Link="Documentation/Readme.txt" />
</ItemGroup>
```

would place the file in the following location:

* /Resources/Documentation/Readme.txt: iOS, tvOS
* /Contents/Resources/Documentation/Readme.txt: macOS, Mac Catalyst

See also:

* https://github.com/dotnet/macios/blob/main/dotnet/BundleContents.md

## CoreMLModel

An item group that contains CoreML models.

## CustomEntitlements

An item group that contains custom entitlements to add to the app.

These entitlements are processed last, and will override any other
entitlements, either from the file specified with the
[CodesignEntitlements](build-properties.md#codesignentitlements) property, or
from the provisioning profile in use (if any).

This is the format:

```xml
<ItemGroup>
    <CustomEntitlements Include="name.of.entitlement" Type="Boolean" Value="true" /> <!-- value can be 'false' too (case doesn't matter) -->
    <CustomEntitlements Include="name.of.entitlement" Type="String" Value="stringvalue" />
    <CustomEntitlements Include="name.of.entitlement" Type="StringArray" Value="a;b" /> <!-- array of strings, separated by semicolon -->
    <CustomEntitlements Include="name.of.entitlement" Type="StringArray" Value="a😁b" ArraySeparator="😁" /> <!-- array of strings, separated by 😁 -->
    <CustomEntitlements Include="name.of.entitlement" Type="Remove" /> <!-- This will remove the corresponding entitlement  -->
</ItemGroup>
```

## ITunesArtwork

An item group that contains iTunes artwork for IPAs.

Only applicable to iOS and tvOS projects.

## ITunesMetadata

Only applicable to iOS and tvOS projects.

## ImageAsset

An item group that contains image assets, including files inside asset catalogs
(\*.xcassets) and Icon Composer directories (\*.icon).

## InterfaceDefinition

An item group that contains interface definitions (\*.xib or \*.storyboard files).

## LinkDescription

Additional xml files to pass to the trimmer.

This is the same as setting [TrimmerRootDescriptor](/dotnet/core/deploying/trimming/trimming-options?#root-descriptors).

## LinkerArgument

Additional arguments to pass to the native linker (`ld`) when compiling the main executable for an app or app extension.

Example 1 (to link with the `AudioToolbox` framework):

```xml
<ItemGroup>
    <LinkerArgument Include="-framework" />
    <LinkerArgument Include="AudioToolbox" />
</ItemGroup>
```

Example 2 (to link with a custom static library):

```xml
<ItemGroup>
    <LinkerArgument Include="$(MSBuildProjectDirectory)/libCustom.a" />
</ItemGroup>
```

Each argument to the linker is a separate `LinkerArgument`, and arguments must not be quoted.

All the arguments will be passed to the native linker in the order they're
added to the `LinkerArgument` item group, but the exact location within all
the arguments passed to the native linker is not defined.

The native executable will be rebuilt automatically if the set of
`LinkerArgument` changes between builds, but if a `LinkerArgument` points to a
file (such as a static library), and that file changes, this change will not
be detected and the native executable will not be rebuilt automatically.

## Metal

An item group that contains metal assets.

## MlaunchAdditionalArguments

An item group that contains extra arguments to the `mlaunch` tool, which is used to launch apps on device and in the simulator. The `mlaunch` tool is considered an internal tool, and behaviour may change at any time.

> [!NOTE]
> This only applies when launching the app from the command line (`dotnet run` or `dotnet build -t:Run`), not when launching from the IDE.

## MlaunchEnvironmentVariables

An item group that contains environment variables that will be set when the app is launched, either on device or in the simulator.

> [!NOTE]
> This only applies when launching the app from the command line (`dotnet run` or `dotnet build -t:Run`), not when launching from the IDE.

## ApplicationArtifact

An item group that contains final application artifacts produced by Apple platform builds and publishes. The item identity is the absolute path to the artifact. This can include:

* `.app` app bundles for iOS, tvOS, macOS, and Mac Catalyst apps.
* `.ipa` packages when [BuildIpa](build-properties.md#buildipa) is enabled.
* `.pkg` installer packages when [CreatePackage](build-properties.md#createpackage) is enabled.
* `.xcarchive` directories when [ArchiveOnBuild](build-properties.md#archiveonbuild) is enabled.

The following metadata is set:

* `PackageFormat`: The artifact format. Possible values are `app`, `ipa`, `pkg`, and `xcarchive`.
* `IsDirectory`: `true` for `.app` and `.xcarchive` outputs; `false` for `.ipa` and `.pkg` outputs.
* `PlatformName`: The Apple platform name, such as `iOS`, `tvOS`, `macOS`, or `MacCatalyst`.
* `BundleIdentifier`: The resolved app bundle identifier.
* `ApplicationId`: The resolved app bundle identifier.
* `ApplicationTitle`: The final `CFBundleDisplayName` value.
* `ApplicationName`: The final `CFBundleDisplayName` value, falling back to `CFBundleName` when `CFBundleDisplayName` isn't set.
* `ApplicationDisplayVersion`: The final `CFBundleShortVersionString` value.
* `ApplicationVersion`: The final `CFBundleVersion` value.

The shared application metadata is read from the compiled app bundle
`Info.plist`, so values supplied by a custom manifest take precedence over
single-project properties. Values that are resolved or localized by Apple at a
later stage are returned as written in the compiled manifest; the build does
not choose a locale.

Example:

```xml
<Target Name="WriteApplicationArtifacts" AfterTargets="Build">
    <WriteLinesToFile
        File="$(OutputPath)application-artifacts.txt"
        Lines="@(ApplicationArtifact->'%(Identity)|%(PackageFormat)')"
        Overwrite="true" />
</Target>
```

See also the [GetApplicationArtifacts](build-targets.md#getapplicationartifacts) target.

## NativeReference

An item group that contains any native references that should be linked into
or linked with when building the native executable.

The `Include` attribute specifies the path to a native library (`.a` or
`.dylib`), framework (`.framework`), or XCFramework (`.xcframework`). Frameworks
and XCFrameworks may also be supplied as `.zip` files.

### Linking metadata

* `Kind`: The type of native reference: `Static`, `Dynamic`, or `Framework`.
  Specify this metadata in binding projects. Use `Framework` for XCFrameworks;
  when an app consumes an XCFramework, the build selects the library for the
  target platform and architectures and determines its kind.
* `ForceLoad`: Set to `true` to pass `-force_load` to the native linker for a
  static library, loading all of its object files instead of only those needed
  to resolve referenced symbols. The default is `false`. In binding projects,
  `SmartLink` may allow the build to omit this flag.
* `SmartLink`: In binding projects, sets the corresponding
  `ObjCRuntime.LinkWithAttribute` property. Set to `true` to allow the build to
  omit `ForceLoad` when it determines that doing so is safe, for example when
  using a static registrar. The default for `NativeReference` items is `true`.
  This is not a managed-code trimming setting.
* `Frameworks`: A space-separated list of additional Apple frameworks required
  by the native reference, for example `CoreGraphics QuartzCore`. These
  frameworks are linked with the app. The default is an empty list.
* `WeakFrameworks`: A space-separated list of additional Apple frameworks to
  weakly link, allowing the app to run on OS versions where those frameworks
  are unavailable. The default is an empty list. Code must check availability
  before using APIs from these frameworks.
* `LinkerFlags`: Additional arguments to pass to the native linker, for example
  `-lz -lsqlite3`. Separate arguments with spaces and quote arguments that
  contain spaces. The default is no additional arguments.
* `IsCxx`: Set to `true` if the native reference requires C++ support, so that
  the app is linked using the C++ compiler. The default is `false`.
* `LinkWithSwiftSystemLibraries`: Set to `true` if the native reference requires
  Swift system libraries. This enables automatic linking with those libraries
  when the app's
  [LinkWithSwiftSystemLibraries](build-properties.md#linkwithswiftsystemlibraries)
  property has not been set explicitly. The default is `false`.
* `NeedsGccExceptionHandling`: Set to `true` to link with the GCC exception
  handling library (`-lgcc_eh`). This is a legacy option and should only be used
  for libraries that require it. The default is `false`.
* `LinkTarget`: A legacy binding-project option that sets the corresponding
  `ObjCRuntime.LinkWithAttribute` property. It accepts names from
  `ObjCRuntime.LinkTarget`, including comma-separated combinations. This
  property is deprecated and ignored: the build inspects the native library
  to determine its architectures. Omit it in new projects.
* `LinkToExecutable`: Set to `false` to avoid linking a native library file
  with the app's executable, for example for a plug-in loaded at runtime.
  By default, the file is linked. This option applies to library files,
  including library files selected from XCFrameworks, not framework references.
  It does not control whether the library is copied into the app bundle.

In binding projects, the `Dlsym` metadata sets the `Dlsym` property of the
generated `ObjCRuntime.LinkWithAttribute`. Supported values (case-insensitive)
are `Default`, `Required`, and `Disabled`. Omitting this metadata preserves
the default behavior.

```xml
<ItemGroup>
    <NativeReference Include="libMyLibrary.a">
        <Kind>Static</Kind>
        <Dlsym>Required</Dlsym>
    </NativeReference>
</ItemGroup>
```

### Bundling and debug-symbol metadata

* `CopyToAppBundle`: Set to `false` to prevent a dynamic library or dynamic
  framework from being copied into the app bundle. By default, these references
  are copied; static libraries and static frameworks are not copied. This
  option does not prevent linking with the reference.
* `NoDSymUtil`: Set to `true` to skip running `dsymutil` for this dynamic library
  or framework, or `false` to generate its dSYM. If omitted, the app's
  [NoDSymUtil](build-properties.md#nodsymutil) property determines the behavior.
* `NoSymbolStrip`: Set to `true` to skip stripping native symbols from this
  dynamic library or framework, or `false` to strip them. If omitted, the app's
  [NoSymbolStrip](build-properties.md#nosymbolstrip) property determines the
  behavior.
* `SymbolFile`: The path to a file containing exported symbol names to preserve
  when stripping this dynamic library or framework (one name per line). The
  file is passed to `strip` with `-s`. There is no symbol file by default, and
  this metadata has no effect when `NoSymbolStrip` is `true`.
* `SymbolFileLocalPath`: For remote builds from Windows, the local path to the
  symbol file to transfer to the Mac. `SymbolFile` specifies its path on the
  Mac. Omit this metadata for local builds or when no transfer is needed.

For example, to include a dynamic framework without regenerating its debug
symbols or stripping it:

```xml
<ItemGroup>
    <NativeReference Include="MyFramework.framework">
        <Kind>Framework</Kind>
        <NoDSymUtil>true</NoDSymUtil>
        <NoSymbolStrip>true</NoSymbolStrip>
    </NativeReference>
</ItemGroup>
```

Binding resource packages preserve custom metadata so that settings such as
`NoDSymUtil` and `NoSymbolStrip` can be applied by the consuming app. Metadata
computed by the build, such as `RelativePath`, `PublishFolderType`,
`SourceDirectory`, and `TargetDirectory`, is not a portable configuration
interface and must not be set in a binding resource package manifest.

## ObjcBindingApiDefinition

An item group that lists all the API definitions for binding projects.

## ObjcBindingCoreSource

An item group that lists all the core source code for binding projects.

## ObjCBindingNativeFramework

An item group that lists all the native frameworks that should be included in a binding project.

This item group is deprecated, use [NativeReference](#nativereference) instead.

## ObjcBindingNativeLibrary

An item group that lists all the native libraries that should be included in a binding project.

This item group is deprecated, use [NativeReference](#nativereference) instead.

## PartialAppManifest

`PartialAppManifest` can be used to add additional partial app manifests that
will be merged with the main app manifest (Info.plist).

Any values in the partial app manifests will override values in the main app
manifest unless the `Overwrite` metadata is set to `false`.

If the same value is specified in multiple partial app manifests, it's
undetermined which one will be the one used.

```xml
<ItemGroup>
    <PartialAppManifest Include="my-partial-manifest.plist" Overwrite="false" />
</ItemGroup>
```

If the developer needs to execute a target to compute what to add to the
`PartialAppManifest` item group, it's possible to make sure this target is
executed before the `PartialAppManifest` items are procesed by adding it to
the `CollectAppManifestsDependsOn` property:

```xml
<PropertyGroup>
    <CollectAppManifestsDependsOn>
        AddPartialAppManifests;
        $(CollectAppManifestsDependsOn);
    </CollectAppManifestsDependsOn>
</PropertyGroup>
<Target Name="AddPartialAppManifests">
    <ItemGroup>
        <PartialAppManifest Include="MyPartialAppManifest.plist" />
    </ItemGroup>
</Target>
```

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

### SkipCodesignItems

An item group that specifies files or directories in the app bundle that should not be signed.

The purpose is to exclude from signing files and directories that are copied
manually (for instance through custom MSBuild targets in the project file) to
the app bundle, and which are already signed.

The path to include is the path to the file or directory relative to the root
of the app bundle.

Example:

```xml
<ItemGroup>
    <SkipCodesignItems Include="Contents/SharedSupport/mysignedlibrary.dylib" />
</ItemGroup>
```

Applicable to all platforms.

## XcodeProject

`<XcodeProject>` can be used to build and consume the outputs
of Xcode framework projects created in Xcode or elsewehere.

The `Include` metadata should point to the path of the XCODEPROJ file to be built.

```xml
<ItemGroup>
  <XcodeProject Include="path/to/MyProject.xcodeproj" SchemeName="MyLibrary" />
</ItemGroup>
```

The following MSBuild metadata are supported:

- `%(SchemeName)`: The name of the build scheme or target that should be used to build the project.

- `%(Configuration)`: The name of the configuration to use to build the project.
    The default value is `Release`.

- `%(CreateNativeReference)`: Output XCFRAMEWORK files will be added as a `@(NativeReference)` to the project.
    Metadata supported by `@(NativeReference)` like `%(Kind)`, `%(Frameworks)`, or `%(SmartLink)` will be forwarded if set.
    The default value is `true`.

- `%(OutputPath)`: Can be set to override the XCARCHIVE and XCFRAMEWORK output path of the Xcode project.
    The default value is `$(IntermediateOutputPath)xcode/{SchemeName}-{Hash}`.

This build action was introduced in .NET 9.
