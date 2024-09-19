---
title: .NET for iOS, Mac Catalyst, macOS, and tvOS Build Items
description: .NET for iOS, Mac Catalyst, macOS, and tvOS Build Items
ms.date: 09/19/2024
---

# Build Items

Build items control how .NET for iOS, Mac Catalyst, macOS, and tvOS
application or library projects are built.

## XcodeProject

`<XcodeProject>` can be used to build and consume the outputs
of Xcode library or framework projects created in Xcode or elsewehere.

The `Include` metadata should point to the path of the XCODEPROJ file to be built.

```xml
<ItemGroup>
  <XcodeProject Include="path/to/MyProject.xcodeproj" SchemeName="MyLibrary" />
</ItemGroup>
```

The following MSBuild metadata are supported:

- `%(SchemeName)`: The name of the build scheme that should be used to build the project.

- `%(Configuration)`: The name of the configuration to use to build the project.
    The default value is `Release`.

- `%(CreateNativeReference)`: Output XCFRAMEWORK files will be added as a `@(NativeReference)` to the project.
    Metadata supported by `@(NativeReference)` like `%(Kind)`, `%(Frameworks)`, or `%(SmartLink)` will be forwarded if set.
    The default value is `true`.

- `%(OutputPath)`: Can be set to override the XCARCHIVE and XCFRAMEWORK output path of the Xcode project.
    The default value is `$(IntermediateOutputPath)xcode/{SchemeName}-{Hash}`.

This build action was introduced in .NET 9.
