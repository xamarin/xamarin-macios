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

> [!IMPORTANT]
> Runtime-specific Mono properties on this page apply to supported builds targeting
> .NET 10 or earlier that use the Mono runtime. Starting with .NET 11, CoreCLR
> is the default runtime for iOS, tvOS, and Mac Catalyst (NativeAOT remains a
> separate option). Setting `UseMonoRuntime=true` on those platforms produces
> `NETSDK1242`. On macOS, setting `UseMonoRuntime=true` instead produces the
> existing `Only CoreCLR is supported on macOS` workload error.

## AltoolPath

The full path to the `altool` tool.

The default behavior is to use `xcrun altool`.

## ACToolExtraArgs

Additional arguments to pass to `actool`.

## ACToolPath

The full path to the `actool` tool.

The default behavior is to use `xcrun actool`.

## AppBundleResourcePrefix

The directory where resources are stored (this prefix will be removed when copying resources to the app bundle).

If not explicitly set, this property will inherit its value from the platform-specific resource prefix properties ([IPhoneResourcePrefix](#iphoneresourceprefix), [MonoMacResourcePrefix](#monomacresourceprefix), or [XamMacResourcePrefix](#xammacresourceprefix) depending on the platform).

Default: "Resources"

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

## ArchiveDir

An optional path for the archive directory. If set, the archive will be created
in this directory instead of computing a unique path under
`~/Library/Developer/Xcode/Archives`.

## ArchiveOnBuild

If an Xcode archive should be created at the end of the build.

Created archives are exposed in the [ApplicationArtifact](build-items.md#applicationartifact) item group.

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

Created IPA packages are exposed in the [ApplicationArtifact](build-items.md#applicationartifact) item group.

## BundleCreateDump

CoreCLR has a command-line utility called [`createdump`][createdump] to create
core dumps if the process crashes. macOS will automatically create crash
reports for any App Store apps and make them available to the app developer,
so the `createdump` tool is not useful for many macOS apps, and as such, it's
not included in apps by default.

This can be overriden by setting the `BundleCreateDump` property:

```xml
<PropertyGroup>
  <BundleCreateDump>true</BundleCreateDump>
</PropertyGroup>
```

Note: the `createdump` tool does currently not work for sandboxed apps ([#18961](https://github.com/dotnet/macios/issues/18961));

Note: an alternative option is to enable the in-process crash reporter (see [EnableCrashReport](#enablecrashreport)). The in-process crash reporter also works for sandboxed apps.

Only applicable to macOS projects.

[createdump]: https://github.com/dotnet/runtime/blob/3b63eb1346f1ddbc921374a5108d025662fb5ffd/docs/design/coreclr/botr/xplat-minidump-generation.md

## BundleOriginalResources

This property determines whether resources are compiled before being embedded
into library projects, or if the original (uncompiled) version is embedded.

Historically, resources have been compiled before being embedded into library
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

The default value of this property `false` in .NET 9, and `true` in .NET 10+.

> [!NOTE]
> File an issue if you find that you need to disable this feature, as it's possible that the option to disable it will be removed in future.

## CheckForIllegalCrossThreadCalls

Controls whether the UI thread checks (the `[NS|UI]Application.EnsureUIThread`
calls the generated bindings emit for UI code) are performed.

When set to `true`, the checks are enabled: accessing UI API off the UI thread
throws an exception. When set to `false`, the checks are disabled.

This property is emitted as the `ObjCRuntime.Runtime.CheckForIllegalCrossThreadCalls`
runtime feature switch, so it takes effect even when trimming is disabled: the
`[NS|UI]Application.CheckForIllegalCrossThreadCalls` field reflects the property
value at runtime, and the checks are enabled or disabled accordingly.

When trimming is enabled and the checks are disabled, ILLink additionally stubs
the `[NS|UI]Application.EnsureUIThread` method body and trims away the
`CheckForIllegalCrossThreadCalls` field, making the app slightly smaller and
faster.

If this value is not specified, the checks are kept in debug builds and removed
in release builds.

## CodesignAllocate

The path to the `codesign_allocate` tool.

By default this value is auto-detected.

## CodesignConfigureDependsOn

This is an extension point for the build: a developer can add any targets to
this property to execute those targets before the build looks at any of the
codesigning properties.

This can for instance be used to disable code signing for simulator builds:

```xml
<PropertyGroup>
  <CodesignConfigureDependsOn>$(CodesignConfigureDependsOn);DisableCodesignInSimulator</CodesignConfigureDependsOn>
</PropertyGroup>
<Target Name="DisableCodesignInSimulator" Condition="'$(SdkIsSimulator)' == 'true'">
  <PropertyGroup>
    <EnableCodeSigning>false</EnableCodeSigning>
  </PropertyGroup>
</Target>
```

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
* iOS, tvOS: building for device or an entitlements file has been specified (with the [CodesignEntitlements](#codesignentitlements) property).

Setting this property to `true` or `false` will override the default logic.

## ComputeInstructionSetForReadyToRun

Controls whether to automatically compute and pass the instruction set to the ReadyToRun (R2R) compiler based on the deployment target.

When `PublishReadyToRun` is `true`, the build system automatically computes the minimum CPU instruction set required based on:
* The `SupportedOSPlatformVersion` (minimum OS version the app supports)
* The `RuntimeIdentifier` (target architecture and platform)

This computed instruction set is then passed to crossgen2 via the `--instruction-set` argument, enabling the R2R compiler to generate optimized native code using appropriate CPU instructions.

Set this property to `false` to disable automatic instruction set computation and use crossgen2's default behavior.

Default: `true`

Example:
```xml
<PropertyGroup>
  <ComputeInstructionSetForReadyToRun>false</ComputeInstructionSetForReadyToRun>
</PropertyGroup>
```

## CompressBindingResourcePackage

The native references in a binding projects are copied to the output directory during the build process, next to the binding assembly (into something we call a "binding resource package").

These native references can either be stored compressed inside a zip file (named `$(AssemblyName).resources.zip`, or as-is, inside a directory named `$(AssemblyName).resources`.

The `CompressBindingResourcePackage` property specifies whether to create a zip file or a directory.

The possible values are:

* `auto`: automatically decide the best option (currently a zip file is always created, but once Visual Studio supports long paths on Windows, this may change to only zip binding resource packages with symlinks).
* `true`: create a zip file
* `false`: create a directory

The default is `auto`.

This also applies to how native references are stored inside NuGets.

> [!NOTE]
> In some cases it can be beneficial to force a zip file on iOS as well, especially when there's a framework with files that have long names, because the zip file can sometimes work around MAX_PATH issues on Windows.

## CopyDSYMToPublishDirectory

A boolean property that specifies whether any generated `*.dSYM` directories should be
copied to the publish directory when publishing (`dotnet publish`).

The `*.dSYM` directories are generated next to the app bundle (see [NoDSymUtil](#nodsymutil)),
and when this property is `true` they'll also be copied to the publish directory (next to the
generated `.ipa`/`.pkg`).

The default value is `true`.

## CopySceneKitAssetsPath

The full path to the `copySceneKitAssets` tool.

The default behavior is to use `xcrun copySceneKitAssets`.

## CoreMLCompilerPath

The full path to the `coremlc` tool.

The default behavior is to use `xcrun coremlc`.

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

Created PKG packages are exposed in the [ApplicationArtifact](build-items.md#applicationartifact) item group.

## Device

Specifies which mobile device or simulator to target when using `dotnet run --device <Device>` or MSBuild targets that interact with devices (such as `Run`, `Install`, or `Uninstall`).

The value can be anything the command-line tools `simctl` or `devicectl`
accept for the device name; this is typically either the UDID or the name of
the device. For example, for the device `My iOS Device` with UDID `00001111-012301230123ABCD`, use
either `-p:Device="My iOS Device"` or `-p:Device=00001111-012301230123ABCD`.

For more information about device selection, see the
[.NET SDK device selection specification](https://github.com/dotnet/sdk/blob/2b9fc02a265c735f2132e4e3626e94962e48bdf5/documentation/specs/dotnet-run-for-maui.md).

## DeviceSpecificBuild

If the build should be specific to the selected device.

Applicable to all platforms that support device-specific builds (currently iOS and tvOS).

## DeviceSpecificIntermediateOutputPath

The intermediate output path to use when device-specific builds are enabled.

Applicable to all platforms that support device-specific builds (currently iOS and tvOS).

## DeviceSpecificOutputPath

The output path to use when device-specific builds are enabled.

Applicable to all platforms that support device-specific builds (currently iOS and tvOS).

## DiagnosticAddress

The IP address where `dotnet-dsrouter` is executing. This is typcially
`127.0.0.1` when profiling on the simulator, and the IP address of the machine
where `dotnet-dsrouter` when profiling on a device.

This is the IP address component of [DiagnosticConfiguration](#diagnosticconfiguration)`.

Implicitly sets [EnableDiagnostics](#enablediagnostics) to `true`.

Defaults to `127.0.0.1`.

## DiagnosticConfiguration

A value provided by `dotnet-dsrouter` for `DOTNET_DiagnosticPorts` such as:

* `127.0.0.1:9000,suspend,connect`
* `127.0.0.1:9000,nosuspend,connect`

Note that the `,` character will need to be escaped with `%2c` if
passed in command-line to `dotnet build`:

```dotnetcli
dotnet build -c Release -p:DiagnosticConfiguration=127.0.0.1:9000%2csuspend%2cconnect
```

This will automatically set the `DOTNET_DiagnosticPorts` environment variable
packaged inside the application, so that the environment variable is set when
the app launches.

Implicitly sets [EnableDiagnostics](#enablediagnostics) to `true`.

The default behavior is to compute this value from the other diagnostics
properties ([DiagnosticAddress](#diagnosticaddress),
[DiagnosticPort](#diagnosticport),
[DiagnosticListenMode](#diagnosticlistenmode), and
[DiagnosticSuspend](#diagnosticsuspend)).

If set, any of the other diagnostic properties will be ignored.

## DiagnosticListenMode

A value provided by `dotnet-dsrouter` such as `connect` or `listen`, the
listening mode component of
[DiagnosticConfiguration](#diagnosticconfiguration)`.

Implicitly sets [EnableDiagnostics](#enablediagnostics) to `true`.

Defaults to `listen`.

## DiagnosticPort

A value provided by `dotnet-dsrouter` such as `9000`, the port
component of [DiagnosticConfiguration](#diagnosticconfiguration)`.

Implicitly sets [EnableDiagnostics](#enablediagnostics) to `true`.

Defaults to `9000`.

## DiagnosticSuspend

A value that specifies the startup behavior when profiling an application.

Set to `true` to suspend the app at startup (waiting for the diagnostics
server to connect to the app) or `false` to launch the app as usual (and
connect the diagnostics server to the app later).

This corresponds with the `suspend/nosuspend` value in
[DiagnosticConfiguration](#diagnosticconfiguration)`.

Implicitly sets [EnableDiagnostics](#enablediagnostics) to `true`.

Defaults to `false`.

## DittoPath

The full path to the `ditto` executable.

The default behavior is to use `/usr/bin/ditto`.

## DynamicRegistrationSupported

Controls whether the dynamic registrar is available at runtime (as reported by
`ObjCRuntime.Runtime.DynamicRegistrationSupported`).

If this value is not specified, the build will compute a default value based
on whether the app needs the dynamic registrar, and enables the
`ObjCRuntime.Runtime.DynamicRegistrationSupported` trimmer feature switch
accordingly (so the trimmer can remove the dynamic registrar when it's not
needed, making the app smaller).

Set this property to `true` or `false` to override the computed default.

Removing the dynamic registrar requires a static registrar (`Registrar=static` or
`Registrar=managed-static`) and trimming, so setting this property has no effect (and the
build warns) when those conditions aren't met.

## TrimExportAttributes

Controls whether `Foundation.ExportAttribute`, `Foundation.ActionAttribute`,
and `Foundation.OutletAttribute` instances are removed during trimming.

If this property is not specified, the build automatically removes these
attributes when assembly preparation and post-processing are enabled, the
trimmable static registrar is selected, dynamic registration is not required,
and no runtime fallback needs the attributes.

Set this property to `false` to preserve the attributes. Set it to `true` to
require their removal; the build will fail if it detects that the attributes
are needed at runtime.

## EmbedOnDemandResources

Controls where on-demand resource asset packs are placed, so that the on-demand
resources APIs can find them at runtime. This property does **not** enable
on-demand resources (use [EnableOnDemandResources](#enableondemandresources) for
that) — it only affects how already-tagged asset packs are packaged.

This is the property set by the "Embed on-demand resources in the app bundle"
option in the IDE.

When building for the **simulator**, the asset packs can't be hosted anywhere
(there's no App Store nor a local hosting server), so they must be embedded in
the app bundle for on-demand resources to work at all:

* `true`: the asset packs are embedded in the `.app` bundle and served locally
  by the app.
* `false`: the asset packs are not embedded, so the on-demand resources APIs
  won't find them on the simulator.

When packaging an **IPA** for `AdHoc` distribution:

* `true`: the asset packs are embedded in the `.app` bundle inside the IPA and
  served locally by the app.
* `false`: the asset packs are packaged separately (outside the app bundle) so
  they can be streamed/hosted (for example on a web server).

For `AppStore` distribution the asset packs are always placed outside the `.app`
bundle (to be hosted by the App Store), regardless of this property.

This property has no effect on a device debug build.

Default: true

## EnableCodeSigning

If code signing is enabled.

Code signing is enabled by default for all platforms; this can be overridden with this property.

## EnableCrashReport

Enables crash reports for the app. When enabled, the `DOTNET_EnableCrashReport`
environment variable is set to `1` at startup, which makes the .NET runtime's
in-process crash reporter write a JSON crash report when the app crashes.

This setting is disabled by default, but it can be enabled like this:

```xml
<PropertyGroup>
    <EnableCrashReport>true</EnableCrashReport>
</PropertyGroup>
```

The crash reports are written to a subdirectory of the app's caches directory.

See also: [Collect crash dumps](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/collect-dumps-crash).

## EnableDefaultCodesignEntitlements

See [CodesignEntitlements](#codesignentitlements).

## EnableOnDemandResources

If on-demand resources are enabled.

When enabled, bundle resources that are tagged with `ResourceTags` metadata are
placed into on-demand resource asset packs instead of being copied directly into
the app bundle. A resource is tagged like this:

```xml
<ItemGroup>
  <BundleResource Update="Resources\MyResource.dat" ResourceTags="MyTag" />
</ItemGroup>
```

Use `Update` (not `Include`) when the file is already part of the project's
default resources (for example anything under the `Resources` folder on iOS,
which is automatically included as a `BundleResource`), otherwise the resource
would be added twice and the untagged copy would win. In a .NET MAUI project the
same `ResourceTags` metadata can be set on the corresponding `MauiAsset` item.

This property only enables on-demand resources; it does not control how the asset
packs are packaged for distribution — see
[EmbedOnDemandResources](#embedondemandresources) for that.

Default: false for macOS, true for all other platforms.

## EnablePackageSigning

If the .pkg that was created (if `CreatePackage` was enabled) should be signed.

Only applicable to macOS and Mac Catalyst.

## EnableDiagnostics

Enable components that are required for diagnostics (such as profiling) to work.

It's enabled by default for debug builds (when [MtouchDebug](#mtouchdebug) or
[MmpDebug](#mmpdebug) is enabled), but needs to be enabled manually before
profiling release builds:

```xml
<PropertyGroup>
  <EnableDiagnostics>true</EnableDiagnostics>
</PropertyGroup>
```

This will increase the app size slightly.

Only applicable when using the Mono runtime (CoreCLR always supports
diagnostics, while NativeAOT never does).

## EnableSGenConc

Enables the concurrent mode for the SGen garbage collector.

Only applicable to iOS, tvOS and Mac Catalyst (when not using NativeAOT).

This property only has an effect when using the Mono runtime, and a warning
will be shown if it's set when not using the Mono runtime (for instance when
using CoreCLR).

## EventSourceSupport

When set to `false`, disables .NET's [EventSource][eventsource] support from
trimmed applications. Disabling this feature would prevent .NET diagnostic
tools like `dotnet-counters` from functioning, but at the benefit of reduced
application size.

Default: set to `false` when `Optimize` is set to `true` (which is the default
for `Release` builds), unless `$(EnableDiagnostics)` is enabled.

[eventsource]: /dotnet/core/diagnostics/eventsource

## GenerateApplicationManifest

If an application manifest (`Info.plist`) should be generated.

Default: true

## GeneratedSourcesDir

Where the generated source from the generator are saved.

## GenerateTrustedPlatformAssemblies

A boolean property that specifies whether the trusted platform assembly list is
generated at build time instead of discovered when the app launches.

The default value is `false` for debug builds and `true` for other builds.

## GetApplicationArtifactsDependsOn

A semi-colon delimited property that can be used to extend the
[GetApplicationArtifacts](build-targets.md#getapplicationartifacts) and
`Publish` targets. `Build` is a mandatory dependency of
`GetApplicationArtifacts`; MSBuild targets added to this property execute after
the platform build has collected `@(ApplicationArtifact)` items and before
`GetApplicationArtifacts` or `Publish` returns them.

Apple platform builds populate the common application metadata documented for
[ApplicationArtifact](build-items.md#applicationartifact) before targets in
this property execute. Extension targets can update or override that metadata,
and should only add new items when introducing additional artifacts.

Example:

```xml
<PropertyGroup>
  <GetApplicationArtifactsDependsOn>$(GetApplicationArtifactsDependsOn);AddApplicationArtifactMetadata</GetApplicationArtifactsDependsOn>
</PropertyGroup>

<Target Name="AddApplicationArtifactMetadata">
  <ItemGroup>
    <ApplicationArtifact Update="@(ApplicationArtifact)">
      <ApplicationTitle>$(ApplicationTitle)</ApplicationTitle>
    </ApplicationArtifact>
  </ItemGroup>
</Target>
```

## HotReloadCompatibleBuild

A boolean property that indicates whether the build must remain compatible
with Hot Reload. When set to `true`, the build avoids modifying user
assemblies so they stay byte-for-byte unchanged (a requirement for Hot
Reload). This will disable a few minor optimizations, but will otherwies not
affect anything.

The default value is `true` for non-NativeAOT debug builds that don't use the
managed static registrar, and `false` otherwise (the managed static registrar
modifies user assemblies, which is incompatible with Hot Reload).

## IBToolExtraArgs

Additional arguments to pass to `ibtool`.

For example, to use the simulator-based Interface Builder compilation mode:

```xml
<PropertyGroup>
  <IBToolExtraArgs>--cocoatouch-compiler-mode simulator</IBToolExtraArgs>
</PropertyGroup>
```

## IBToolPath

The full path to the `ibtool` tool.

The default behavior is to use `xcrun ibtool`.

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

## InlineClassGetHandle

Controls whether the build system replaces runtime calls to `Class.GetHandle` /
`Class.GetHandleIntrinsic` with direct native references to Objective-C classes
at build time.

See [docs/code/class-handles.md](../code/class-handles.md) for an overview.

The valid options are:

* `compatibility`: Inlines `Class.GetHandle` calls only for types whose declaring
  type matches the requested Objective-C class name.
* `strict`: Inlines all `Class.GetHandle` calls unconditionally. Requires using
  the static registrar (not the dynamic registrar).
* (empty): Disables inlining of `Class.GetHandle` calls.

Default value:
* .NET 11+: `strict` when using NativeAOT (`PublishAot=true`), `compatibility` otherwise.
* .NET 10 and earlier: not set (disabled).

Example:

```xml
<PropertyGroup>
    <InlineClassGetHandle>compatibility</InlineClassGetHandle>
</PropertyGroup>
```

Custom behavior for specific Objective-C classes can be set using the [ReferenceNativeSymbol](build-items.md#referencenativesymbol) item group:

```xml
<ItemGroup>
    <ReferenceNativeSymbol SymbolMode="Ignore" SymbolType="ObjectiveCClass" Include="SomeClassName" />
</ItemGroup>
```

## InlineDlfcnMethods

Controls whether the build system replaces runtime calls to `ObjCRuntime.Dlfcn` methods with direct native symbol lookups at build time, eliminating the overhead of `dlsym` at runtime.

The valid options are:

* `compatibility`: Only inlines symbol usages backed by `[Field]` attributes. This is more conservative and avoids link errors for symbols that don't exist at build time.
* `strict`: Inlines dlfcn method calls and creates native references for all symbols. This is more aggressive and may cause link errors if referenced native symbols don't exist.
* (empty): Disables inlining of dlfcn method calls.

Default value:
* .NET 11+: `strict` when using NativeAOT (`PublishAot=true`), `compatibility` otherwise.
* .NET 10 and earlier: not set (disabled).

Example:

```xml
<PropertyGroup>
    <InlineDlfcnMethods>compatibility</InlineDlfcnMethods>
</PropertyGroup>
```

Custom behavior for specific symbols can be set using the [ReferenceNativeSymbol](build-items.md#referencenativesymbol) item group:

```xml
<ItemGroup>
    <ReferenceNativeSymbol SymbolMode="Ignore" SymbolType="Field" Include="InexistentSymbol" />
</ItemGroup>
```

## iOSMinimumVersion

Specifies the minimum iOS version the app can run on.

Applicable to iOS; setting this value will set [SupportedOSPlatformVersion](#supportedosplatformversion) for iOS projects (only).

## IPhoneResourcePrefix

The directory where resources are stored (this prefix will be removed when copying resources to the app bundle).

Applicable to iOS, tvOS and Mac Catalyst projects.

Consider using the unified [AppBundleResourcePrefix](#appbundleresourceprefix) property instead.

See also [MonoMacResourcePrefix](#monomacresourceprefix) and [XamMacResourcePrefix](#xammacresourceprefix).

## IpaIncludeArtwork

If artwork should be included in the IPA.

Only applicable to iOS and tvOS projects.

## IpaIncludeSymbols

If the app's symbols should be included in the IPA, in the `Symbols`
directory Apple expects. This makes App Store Connect (and Xcode's Organizer)
symbolicate crash reports for the app automatically.

The symbols are the Apple `*.symbols` files generated from the build's dSYM
directories using `xcrun symbols` (the dSYM directories themselves are not
embedded in the IPA).

The default value is `true`. Set it to `false` to opt out. This property has
no effect unless an IPA is being created (see [BuildIpa](#buildipa)).

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

The resulting IPA is exposed in the [ApplicationArtifact](build-items.md#applicationartifact) item group.

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

For more information, see [Exception marshaling](/dotnet/ios/advanced-concepts/exception-marshaling) and [MarshalObjectiveCExceptionMode](#marshalobjectivecexceptionmode).

## MarshalObjectiveCExceptionMode

Choose how Objective-C exceptions are handled when encountering a managed frame
during stack unwinding while processing the Objective-C exception.

Valid values:

* `default`: Currently, this is `throwmanagedexception`.
* `unwindmanagedcode`: This is not available when using the CoreCLR runtime.
* `throwmanagedexception`:  Catch the Objective-C exception, and convert it into a managed exception.
* `abort`: Abort the process.
* `disable`: Disable intercepting any Objective-C exceptions.

For more information, see [Exception marshaling](/dotnet/ios/advanced-concepts/exception-marshaling) and [MarshalManagedExceptionMode](#marshalmanagedexceptionmode).

## MdimportPath

The full path to the `mdimport` tool.

The default behavior is to use `xcrun mdimport`.

## MetalLibPath

The full path to the `metallib` tool (the Metal Linker).

The default behavior is to use `xcrun metallib`.

## MetalPath

The full path to the Metal compiler.

The default behavior is to use `xcrun metal`.

## MetricsSupport

When set to `false`, disables .NET's [Metrics][dotnetmetrics] support from
trimmed applications. Disabling this feature would prevent APIs such as
`System.Diagnostics.Metrics` from functioning, but at the benefit of reduced
application size.

Default: set to `false` when `Optimize` is set to `true` (which is the default
for `Release` builds), unless `$(EnableDiagnostics)` is enabled.

[dotnetmetrics]: /dotnet/core/diagnostics/metrics

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

Consider using the unified [AppBundleResourcePrefix](#appbundleresourceprefix) property instead.

See also [IPhoneResourcePrefix](#iphoneresourceprefix) and [XamMacResourcePrefix](#xammacresourceprefix).

## MonoUseCompressedInterfaceBitmap

This directs the Mono runtime to use a compressed version of interface bitmaps
(interface bitmaps are used to determine whether a certain types implements a
given interface).

These bitmaps can use a significant amount of memory at runtime, in particular
for apps that have a substantial amount of interfaces.

This setting is disabled by default, but it can be enabled like this, which
will decrease the amount of memory used at runtime:

```xml
<PropertyGroup>
    <MonoUseCompressedInterfaceBitmap>true</MonoUseCompressedInterfaceBitmap>
</PropertyGroup>
```

The downside is that type checks (`obj is SomeInterface`) will be slower.

Only applicable when using the Mono runtime. A warning will be shown if it's
set when not using the Mono runtime (for instance when using CoreCLR).

## MtouchDebug

Enables debug mode for app bundle creation.

Applicable to iOS, tvOS and Mac Catalyst projects.

See also [MmpDebug](#mmpdebug).

## MtouchEnableSGenConc

Enables the concurrent mode for the SGen garbage collector.

Only applicable to iOS, tvOS and Mac Catalyst when not using NativeAOT.

This property is deprecated, use [EnableSGenConc](#enablesgenconc) instead.

This property only has an effect when using the Mono runtime, and a warning
will be shown if it's set when not using the Mono runtime (for instance when
using CoreCLR).

## MtouchExtraArgs

Additional arguments specifying how to create the app bundle.

Only applicable to iOS, tvOS and Mac Catalyst projects.

This property is deprecated, use [AppBundleExtraOptions](#appbundleextraoptions) instead.

## MtouchInterpreter

Enables the Mono interpreter, and optionally takes a comma-separated list of
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
> For .NET 10 and earlier, MAUI sets `UseInterpreter=true` for iOS and Mac
> Catalyst Debug builds that use Mono. MAUI doesn't set this property for
> supported .NET 11 iOS and Mac Catalyst Debug builds, which use CoreCLR.

This property configures the Mono interpreter and doesn't configure the CoreCLR
interpreter. It only has an effect when using Mono. A warning is shown and the
property has no effect when using CoreCLR, including in .NET 11 iOS, tvOS, and
Mac Catalyst projects.

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

This property only has an effect when using the Mono runtime, and a warning
will be shown if it's set when not using the Mono runtime (for instance when
using CoreCLR).

## NoBindingEmbedding

> [!WARNING]
> Setting this property to `false` is currently deprecated and will produce a build error in .NET 12+.

A boolean property that specifies whether native libraries in binding projects should be embedded
in the managed assembly, or put into a `.resources` directory next to the managed assembly.

The default value is `true` (which means native libraries will _not_ be embedded in the managed assembly).

> [!NOTE]
> Xcframeworks won't work correctly if embedded inside the managed assembly (if this property is not `true`).

## NoDSymUtil

A boolean property that specifies whether .dSYM generation should be disabled.

Default:

* `true` for iOS and tvOS when building for the simulator.
* `true` for macOS and Mac Catalyst unless creating an archive (`ArchiveOnBuild=true`) or using Native AOT.

This means the .dSYM archive will be generated in the following cases (by default):

* On iOS and tvOS when building for device.
* On macOS and Mac Catalyst when creating an archive (`ArchiveOnBuild=true`) or using Native AOT.

## NoSymbolStrip

A boolean property that specifies whether debug symbols are removed from the app at build time.

The default behavior is to keep debug symbols for:

* `Debug` builds for desktop platforms.
* Simulator builds for mobile platforms.

Example to keep debug symbols:

```xml
<PropertyGroup>
  <NoSymbolStrip>true</NoSymbolStrip>
</PropertyGroup>
```

## OnDemandResourcesInitialInstallTags

A string property that specifies the initial install tags for on-demand resources.

## OnDemandResourcesPrefetchOrder

A string property that specifies the prefetch order for on-demand resources.

## OnDemandResourcesUrl

A string property that specifies the resource url for on-demand resources.

## OptimizePNGs

A boolean property that specifies whether PNG bundle resources should be optimized using Apple's `pngcrush` tool.

In .NET 11 and later, this defaults to `true` for release builds on iOS, tvOS, and Mac Catalyst. It defaults to `false` for debug builds, macOS builds, and projects targeting earlier .NET versions. Set this property explicitly to opt in or out.

The `Optimize` metadata on individual `BundleResource` items overrides this property.

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

A boolean property that specifies whether property list (`.plist`) and localization (`.strings`) bundle resources should be converted to binary property lists.

In .NET 11 and later, this defaults to `true` for release builds on iOS, tvOS, and Mac Catalyst. It defaults to `false` for debug builds, macOS builds, and projects targeting earlier .NET versions. Set this property explicitly to opt in or out.

The `Optimize` metadata on individual `BundleResource` items overrides this property.

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

The resulting PKG is exposed in the [ApplicationArtifact](build-items.md#applicationartifact) item group.

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

## PublishReadyToRunComposite

Specifies whether ReadyToRun (R2R) compilation produces a single composite image containing all the assemblies, or one image per assembly.

Only composite ReadyToRun compilation is supported for iOS, tvOS and Mac Catalyst apps, because the ReadyToRun code is embedded in the app bundle as native Mach-O code, and the runtime only knows how to locate such code for a composite image. Setting this property to `false` will produce a build error; set [PublishReadyToRun](https://learn.microsoft.com/dotnet/core/deploying/ready-to-run) to `false` to turn off ReadyToRun compilation completely instead.

Default: `true` (when `PublishReadyToRun` is `true`).

## RecommendedXcodeVersion

The version of Xcode recommended for use with this version of .NET for iOS, tvOS, macOS and Mac Catalyst.

This is the Xcode version the build validates against (see [ValidateXcodeVersion](#validatexcodeversion)); using a different version is likely to produce problems later on in the build process.

This property is read-only: it's computed by the SDK and shouldn't be set in project files.

You can get the recommended Xcode version for a project by running:

```shell
$ dotnet build -getProperty:RecommendedXcodeVersion myProject.csproj
26.6
```

Note: the version number may contain more than 2 components ("26.6.1" for instance). Only the first two components (major and minor) are taken into account when validating the installed Xcode version.

## ReferenceNativeSymbol

See [ReferenceNativeSymbol](build-items.md#referencenativesymbol)

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

## ResolveResourceItemsRelativeToProject

This property determines whether Content and BundleResource items have their
logical names computed relative to the project file or relative to the file
that declared them.

When set to `true`, item paths are always computed relative to the project
file. This fixes issues where SDKs (such as the Razor SDK) add Content items
from a directory far from the project, producing incorrect paths that get
rejected at build time.

When set to `false` (the default for versions before .NET 12), the legacy behavior
is preserved: paths are computed relative to the file that declared the item.

Starting with .NET 13, this property is permanently enabled and setting it to
`false` will produce a build error.

| .NET version | Default                       |
|--------------|-------------------------------|
| < .NET 12    | `false` (opt-in)              |
| .NET 12      | `true` (opt-out)              |
| .NET 13+     | `true` (permanently enabled)  |

```xml
<PropertyGroup>
  <ResolveResourceItemsRelativeToProject>true</ResolveResourceItemsRelativeToProject>
</PropertyGroup>
```

## RunWithOpen

This property determines whether apps are launched using the `open` command on
macOS, or if the app's executable is executed directly.

This only applies to macOS and Mac Catalyst apps.

The default value is `true`. In this mode, the app will be launched by macOS as any other UI application, any stdout/stderr output will be swallowed by macOS, and the `dotnet run` command will finish as soon as the app has launched.

If set to `false`, any stdout/stderr output will be printed to the current terminal, and the `dotnet run` command won't finish until the app has exited.

The following properties can be used to configure the behavior when set to `true` (i.e. using the `open` command):

### OpenNewInstance

If a new instance will be opened if the app is already running (defaults to `false`).

This will pass `-n` to `open` if set to `true`.

Example:

```shell
$ dotnet run -p:OpenNewInstance=false
```

### OpenWaitForExit

If 'dotnet run' should wait for the app to exit (defaults to `false`).

This will pass `-W` to `open` if set to `true`.

Note: the [WaitForExit](#waitforexit) property takes precedence over this property.

Example:

```shell
$ dotnet run -p:OpenWaitForExit=true
```

### OpenArguments

This property can be used to pass additional arguments to the `open` command.

Example (to set environment variables):

```shell
$ dotnet run -p:OpenArguments="--env VARIABLE1=VALUE1 --env VARIABLE2=value2"
```

Example (to redirect stdout and stderr to a file):

```shell
$ dotnet run -p:OpenArguments="--stdout /tmp/stdout.txt --stderr /tmp/stderr.txt"
```

Run `man open` to see a list of all the options `open` accepts.

### StandardOutputPath

This property can be used to redirect the stdout output from the app to a file.

This applies to all platforms (macOS, Mac Catalyst, iOS, and tvOS).

Example writing to a file:

```shell
$ dotnet run -p:StandardOutputPath=stdout.txt
```

Example writing to the current terminal:

```shell
$ dotnet run -p:StandardOutputPath=$(tty)
[... Console.WriteLine output from app ...]
```

Note: for macOS and Mac Catalyst apps using the `open` command, this can also be accomplished by passing `--stdout ...` using the [OpenArguments](#openarguments) property.

### StandardErrorPath

This property can be used to redirect the stderr output from the app to a file.

This applies to all platforms (macOS, Mac Catalyst, iOS, and tvOS).

Example writing to a file:

```shell
$ dotnet run -p:StandardErrorPath=stderr.txt
```

Example writing to the current terminal:

```shell
$ dotnet run -p:StandardErrorPath=$(tty)
[... Console.Error.WriteLine output from app ...]
```

Note: for macOS and Mac Catalyst apps using the `open` command, this can also be accomplished by passing `--stderr ...` using the [OpenArguments](#openarguments) property.

### StandardInputPath

This property can be used to redirect the stdin input to the app from a file.

Example:

```shell
$ dotnet run -p:StandardInputPath=stdin.txt
```

Note: this can also be accomplished by passing `--stdin ...` using the [OpenArguments](#openarguments) property.

## WaitForExit

If 'dotnet run' should wait for the app to exit before returning (defaults to
`true` for iOS and tvOS apps, and `false` for macOS and Mac Catalyst apps).

This applies to all platforms (macOS, Mac Catalyst, iOS, and tvOS).

For macOS and Mac Catalyst apps, this property takes precedence over the
[OpenWaitForExit](#openwaitforexit) property.

Example:

```shell
$ dotnet run -p:WaitForExit=true
```

## SdkIsDesktop

This property is a read-only property (setting it will have no effect) that
specifies whether we're building for a desktop platform (macOS or Mac Catalyst).

This property is `true` when the target platform is macOS or Mac Catalyst,
and is not set for iOS or tvOS builds.

Like `SdkIsSimulator`, this property is only set after [imports and
properties](/visualstudio/msbuild/build-process-overview#evaluate-imports-and-properties)
have been evaluated.

## SdkIsDevice

This property is a read-only property (setting it will have no effect) that
specifies whether we're building for a device or not.

This property is only `true` when building for an iOS or tvOS device (i.e.,
when `SdkIsSimulator` is not `true` and the platform is iOS or tvOS). It is
not set for macOS or Mac Catalyst builds.

Like `SdkIsSimulator`, this property is only set after [imports and
properties](/visualstudio/msbuild/build-process-overview#evaluate-imports-and-properties)
have been evaluated.

## SdkIsMobile

This property is a read-only property (setting it will have no effect) that
specifies whether we're building for a mobile platform (iOS or tvOS).

This property is `true` when the target platform is iOS or tvOS, and is not
set for macOS or Mac Catalyst builds.

Like `SdkIsSimulator`, this property is only set after [imports and
properties](/visualstudio/msbuild/build-process-overview#evaluate-imports-and-properties)
have been evaluated.

## SdkIsSimulator

This property is a read-only property (setting it will have no effect) that
specifies whether we're building for a simulator or not.

It is only set after [imports and
properties](/visualstudio/msbuild/build-process-overview#evaluate-imports-and-properties)
have been evaluated. This means the property is not set while evaluating the
properties in the project file, so this will _not_ work:

```xml
<PropertyGroup>
  <EnableCodeSigning Condition="'$(SdkIsSimulator)' == 'true'">false</EnableCodeSigning>
</PropertyGroup>
```

However, the either of the following works:

```xml
<ItemGroup>
  <!-- item groups (and their conditions) are evaluated after properties have been evaluated -->
  <CustomEntitlements Condition="'$(SdkIsSimulator)' == 'true'" Include="com.apple.simulator-entitlement" Type="Boolean" Value="true" />
  <CodesignConfigureDependsOn>$(CodesignConfigureDependsOn);ConfigureSimulatorSigning</CodesignConfigureDependsOn>
</ItemGroup>
<!-- targets are executed after properties have been evaluated -->
<Target Name="ConfigureSimulatorSigning">
  <PropertyGroup>
    <EnableCodeSigning Condition="'$(SdkIsSimulator) == 'true'">false</EnableCodeSigning>
  </PropertyGroup>
</Target>
```

Note: this property will always be `false` on macOS and Mac Catalyst.

## StripFrameworkHeaders

A boolean property that specifies whether the `Headers`, `PrivateHeaders`, and
`Modules` directories are removed from embedded frameworks before code signing.

The default value is `true`. Set it to `false` to preserve these directories.

## StripPath

The full path to the `strip` command-line tool.

The default behavior is to use `xcrun strip`.

## StripMergeableLibraries

A boolean property that specifies whether static linking metadata (`LC_ATOM_INFO`)
is removed from mergeable libraries embedded in the app bundle.

Mergeable libraries are dynamic libraries that also contain metadata for static
linking. This metadata can roughly double the size of the library. When this
property is `true`, the metadata is stripped to reduce app size.

The default value is the value of the `Optimize` property, which means `Release`
builds strip mergeable library metadata by default, while `Debug` builds preserve
it.

```xml
<PropertyGroup>
  <StripMergeableLibraries>true</StripMergeableLibraries>
</PropertyGroup>
```

## SupportedOSPlatformVersion

Specifies the minimum OS version the app can run on.

It's also possible to use a platform-specific property:

* [iOSMinimumVersion](#iosminimumversion)
* [tvOSMinimumVersion](#tvosminimumversion)
* [macOSMinimumVersion](#macosminimumversion)
* [MacCatalystMinimumVersion](#maccatalystminimumversion)

## TextureAtlasPath

The full path to the `TextureAtlas` tool.

The default behavior is to use `xcrun TextureAtlas`.

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

> [!NOTE]
> Due to [a known issue](https://github.com/dotnet/runtime/issues/108269), setting `PublishTrimmed`
> to `true` may cause confusing problems, so the build will report an error if this
> is detected (the solution is to not set `PublishTrimmed` at all).


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

* iOS and tvOS: `partial` when building for device, `copy` when building for the simulator.
* macOS: always `copy`.
* Mac Catalyst: `partial` when building for the `"Release"` configuration, `copy` otherwise.

Exceptions:

* The default value is always `full` when building with NativeAOT.
* For iOS and Mac Catalyst Debug builds using Mono, MAUI changes the default
  trim mode to `copy` when [UseInterpreter](#useinterpreter) is enabled. MAUI
  enables `UseInterpreter` by default for these builds in .NET 10 and earlier,
  but not for supported .NET 11 CoreCLR builds.

> [!NOTE]
> The default trim mode may change in the future.

## tvOSMinimumVersion

Specifies the minimum tvOS version the app can run on.

Applicable to tvOS; setting this value will set [SupportedOSPlatformVersion](#supportedosplatformversion) for tvOS projects (only).

## UseFloatingTargetPlatformVersion

A boolean property that controls whether library projects should use a floating target platform version or the oldest available platform version.

By default (starting in .NET 10), library projects without an explicit `TargetPlatformVersion` will use the oldest available reference assemblies for the current .NET version. This ensures maximum compatibility and allows library code to compile against the minimum API surface available for the target framework.

However, this default behavior means that library projects are built differently than executable projects (which use the latest platform version). Code that works in an executable project may not compile when moved to a library project if it uses APIs only available in newer platform versions.

Setting this property to `true` disables the automatic selection of the oldest platform version, allowing the library project to use the default (latest) platform version like executable projects do.

Example:

```xml
<PropertyGroup>
  <!-- Use the latest platform version instead of the oldest -->
  <UseFloatingTargetPlatformVersion>true</UseFloatingTargetPlatformVersion>
</PropertyGroup>
```

Default: `false` (use oldest platform version for library projects in .NET 10+).

This property only applies to library projects (`OutputType=Library`) that are
not app extensions and have not specified an explicit target platform version
(the target platform version is the optional version number at the end of the
`TargetFramework` property, for example for the TargetFramework
`net10.0-ios26.0` the target platform version is explicitly `26.0`).

This property was introduced in .NET 10.

## UseHardenedRuntime

A boolean property that specifies if a hardened runtime is enabled.

Applicable to macOS and Mac Catalyst projects.

## UseInterpreter

Enables the Mono interpreter (for all assemblies).

This is equivalent to setting `MtouchInterpreter=all`.

Applicable to iOS, tvOS and Mac Catalyst apps (when not using NativeAOT).

The default behavior is to not enable the interpreter.

> [!NOTE]
> For .NET 10 and earlier, MAUI sets `UseInterpreter=true` for iOS and Mac
> Catalyst Debug builds that use Mono. MAUI doesn't set this property for
> supported .NET 11 iOS and Mac Catalyst Debug builds, which use CoreCLR.

See [MtouchInterpreter](#mtouchinterpreter) for more information.

This property configures the Mono interpreter and doesn't configure the CoreCLR
interpreter. It only has an effect when using Mono. A warning is shown and the
property has no effect when using CoreCLR, including in .NET 11 iOS, tvOS, and
Mac Catalyst projects.

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

## ValidateXcodeVersion

Choose whether the current Xcode version should be validated.

The default value is to validate; set to `false` to disable.

> [!NOTE]
> Using a different than the recommended version is likely to produce problems later on in the build process.

## XamMacResourcePrefix

The directory where resources are stored (this prefix will be removed when copying resources to the app bundle).

Applicable to macOS projects.

Consider using the unified [AppBundleResourcePrefix](#appbundleresourceprefix) property instead.

See also [IPhoneResourcePrefix](#iphoneresourceprefix) and [MonoMacResourcePrefix](#monomacresourceprefix).

## XcodeLocation

Specifies the location of Xcode.

When the build searches for Xcode, it's done in this order:

1. If the `XcodeLocation` property is set, use that. Note that since all environment variables are automatically MSBuild properties as well, it's also possible to set the `XcodeLocation` environment variable for the same effect.
2. If the `MD_APPLE_SDK_ROOT` environment variable is set, use that.
3. If either of the files `~/Library/Preferences/maui/Settings.plist` or `~/Library/Preferences/Xamarin/Settings.plist` exist, and has the property list value `AppleSdkRoot`, use that.
4. Use the system version of Xcode (as determined by executing `xcode-select --print-path`).

> [!WARNING]
> Support for the `MD_APPLE_SDK_ROOT` environment variable, and the `~/Library/Preferences/maui/Settings.plist` and `~/Library/Preferences/Xamarin/Settings.plist` files, is deprecated and will be removed in the future.
> Going forward, choose which Xcode to use by either making it the system's version of Xcode (either using `xcode-select --switch ...` on the command line, or in Xcode's settings), or by setting the `XcodeLocation` MSBuild property / environment variable.

## ZipPath

The full path to the `zip` command-line tool.

The default behavior is to use `xcrun zip`.
