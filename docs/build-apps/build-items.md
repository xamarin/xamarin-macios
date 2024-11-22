---
title: .NET for iOS, Mac Catalyst, macOS, and tvOS Build Items
description: .NET for iOS, Mac Catalyst, macOS, and tvOS Build Items
ms.date: 09/19/2024
---

# Build Items

Build items control how .NET for iOS, Mac Catalyst, macOS, and tvOS
application or library projects are built.

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
* The [AppIcon](build-properties.md#AppIcon) property.
* The [IncludeAllAppIcons](build-properties.md#IncludeAllAppIcons) property.

## BGenReferencePath

The list of assembly references to pass to the `bgen` tool (binding generator).

Typically this is handled automatically by adding references as
`ProjectReference` or `PackageReference` items instead.

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
