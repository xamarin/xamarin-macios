---
title: .NET for iOS, Mac Catalyst, macOS, and tvOS Build Properties
description: .NET for iOS, Mac Catalyst, macOS, and tvOS Build Properties
ms.date: 09/19/2024
---

# Build Properties

MSBuild properties control the behavior of the
[targets](build-targets.md).
They're specified within the project file, for example **MyApp.csproj**, within
an MSBuild PropertyGroup.

## AltoolPath

The full path to the `altool` tool.

The default behavior is to use `xcrun altool`.

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

* The [AlternateAppIcon](build-items.md#AlternateAppIcon) item group.
* The [IncludeAllAppIcons](#IncludeAllAppIcons) property.

### BGenEmitDebugInformation

Whether the `bgen` tool (the binding generator) should emit debug information or not.

The default behavior is `true` when the `Debug` property is set to `true`.

## BGenExtraArgs

Any extra arguments to the `bgen` tool (the binding generator).

## BGenToolExe

The name of the `bgen` executable (a tool used by binding projects to generate bindings).

The default behavior is to use the `bgen` tool shipped with our workload.

## BGenToolPath

The directory to where the `bgen` ([BGenToolExe](#BGenToolExe)) is located.

The default behavior is to use the `bgen` tool shipped with our workload.

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

## DittoPath

The full path to the `ditto` executable.

The default behavior is to use `/usr/bin/ditto`.

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

* The [AlternateAppIcon](build-items.md#AlternateAppIcon) item group.
* The [AppIcon](#AppIcon) property.

## MaciOSPrepareForBuildDependsOn

A semi-colon delimited property that can be used to extend the build process.
MSBuild targets added to this property will execute early in the build for both
application and library project types. This property is empty by default.

Example:

```xml
<PropertyGroup>
  <MaciOSPrepareForBuildDependsOn>MyCustomTarget</MaciOSPrepareForBuildDependsOn>
</PropertyGroup>

<Target Name="MyCustomTarget" >
  <Message Text="Running target: 'MyCustomTarget'" Importance="high"  />
</Target>
```

This property was introduced in .NET 9.

## MdimportPath

The full path to the `mdimport` tool.

The default behavior is to use `xcrun mdimport`.

## MetalLibPath

The full path to the `metallib` tool (the Metal Linker).

The default behavior is to use `xcrun metallib`.

## MetalPath

The full path to the Metal compiler.

The default behavior is to use `xcrun metal`.

## PngCrushPath

The full path to the `pngcrush` command-line tool.

The default behavior is to use `xcrun pngcrush`.

## StripPath

The full path to the `strip` command-line tool.

The default behavior is to use `xcrun strip`.

## ZipPath

The full path to the `zip` command-line tool.

The default behavior is to use `xcrun zip`.
