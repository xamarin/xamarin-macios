# NativeLibraryInterop

## Overview
Native Library Interop (formerly referred to as the "Slim Binding" approach), refers to a
pattern for accessing native SDKs in .NET for iOS, Mac Catalyst, macOS, and tvOS projects.

Starting in .NET 9, the .NET for iOS, Mac Catalyst, macOS, and tvOS SDKs support building
Xcode framework projects by using the `@(XcodeProjet)` build action. This is declared in
an MSBuild ItemGroup in a project file:

```xml
<ItemGroup>
  <XcodeProject Include="path/to/MyProject.xcodeproj" SchemeName="MyLibrary" />
</ItemGroup>
```

When an `@(XcodeProject)` item is added to a .NET for iOS, Mac Catalyst, macOS, or tvOS binding project,
the build process will attempt to create an XCFramework from the specified Xcode project. The XCFramework
output will be added as a `@(NativeReference)` to the .NET project so that it can be bound and have its
API surfaced via an [API definition][0] file.

Please see the [build-items](build-apps/build-items.md) docs for more information about
the `@(XcodeProjet)` build action.

Additional documentation and references can be found below:

* https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/native-library-interop
* https://github.com/CommunityToolkit/Maui.NativeLibraryInterop

[0]: https://learn.microsoft.com/en-us/previous-versions/xamarin/cross-platform/macios/binding/objective-c-libraries?tabs=macos#The_API_definition_file
