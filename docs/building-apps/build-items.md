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

## BGenReferencePath

The list of assembly references to pass to the `bgen` tool (binding generator).

Typically this is handled automatically by adding references as
`ProjectReference` or `PackageReference` items instead.

## BundleResource

Files to be copied to the app bundle.

See also:

* https://github.com/xamarin/xamarin-macios/blob/main/dotnet/BundleContents.md

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

* /Resources/Documentation/Readme.txt: iOS, tvOS, watchOS
* /Contents/Resources/Documentation/Readme.txt: macOS, Mac Catalyst

See also:

* https://github.com/xamarin/xamarin-macios/blob/main/dotnet/BundleContents.md

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

An item group that contains image assets.

## InterfaceDefinition

An item group that contains interface definitions (\*.xib or \*.storyboard files).

## LinkDescription

Additional xml files to pass to the trimmer.

This is the same as setting [TrimmerRootDescriptor](/dotnet/core/deploying/trimming/trimming-options?#root-descriptors).

## Metal

An item group that contains metal assets.

## NativeReference

An item group that contains any native references that should be linked into
or linked with when building the native executable.

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
