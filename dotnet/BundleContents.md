# Content bundled in the app bundle

There are a few items that are automatically included during the build, and
that we're supposed to copy to the app bundle somehow:

* `@(None)` and `@(EmbeddedResource)` items with the `CopyToOutputDirectory` or the
  `CopyToPublishDirectory` metadata set (to either `Always` or
  `PreserveNewest`).
    * `CopyToOutputDirectory` doesn't work with directories (for frameworks),
      in that case `CopyToPublishDirectory` must be used.
* `@(Content)` and `@(BundleResource)` items (the `CopyToOutputDirectory` or
  `CopyToPublishDirectory` metadata has no effect on these items).
* Runtime packs (our own, or the runtime itself (CoreCLR/MonoVM)). We have
  some logic to detect this.
* The output from referenced projects (transitively).
* NuGets with a runtimes/RID/native directory. An aggravating issue here is
  that NuGet will strip the relative path at some point ([dotnet publish with
  a rid flattens Nuget package
  files](https://github.com/dotnet/sdk/issues/9643)).
* The ResolvePackageAssets adds to the NativeCopyLocalItems group
* Then ResolveLockFileCopyLocalFiles adds it to ReferenceCopyLocalPaths
* And of course files added directly to `@(ResolvedFileToPublish)`.

The problem is that we have to decide where to place these files in the app
bundle.

For this purpose, we support the `PublishFolderType` metadata on items in the
`ResolvedFileToPublish` item group, and will place these files accordingly.
Below is a list of known/valid `PublishFolderType` values and the
corresponding action taken for each.

In all cases the relative directory is preserved (i.e. we don't follow the
behavior in [dotnet/sdk#9643](https://github.com/dotnet/sdk/issues/9643).

If the `PublishFolderType` metadata isn't set, we'll try to guess. If we guess
wrong, then developers can override the target location by:

* Setting `PublishFolderType=None` on items.
* Setting the `TargetPath` metadata on items to specify a different location
  in the app bundle (if `PublishFolderType` is also set, the `TargetPath` path
  is relative to the folder of the specified `PublishFolderType` value).
* The `Link` metadata can be used just like `TargetPath`.
* For some item groups it's also possible to set `CopyToOutputDirectory=Never`
  on items that shouldn't be copied.

## The guessing

* `@(Content)` or `@(EmbeddedResource)` items: `PublishFolderType=Resource`
* `@(BundleResource)` items: `PublishFolderType=Resource`
* Assemblies and their related files (\*.dll, \*.exe, \*.pdb, \*.mdb,
  \*.config):
    * If the `PackageDebugSymbols` property is set to `true': `PublishFolderType=Assembly`.
    * If the `PackageDebugSymbols` is set to something else: `PublishFolderType=None`.
    * If the `PackageDebugSymbols` is not set: `PublishFolderType=None` for
      release builds, `PublishFolderType=Assembly` otherwise.
* \*.xml: if there's an assembly with the same name (\*.exe or \*.dll), then `PublishFolderType=None`
* A \*.resources directory or a \*.resources.zip file next to an assembly with
  the same name is treated as a third-party binding
  (`PublishFolderType=AppleBindingResourcePackage`), and we handle it as such
  (the exact details are not relevant for this discussion).
* Native frameworks (\*.framework/\*): `PublishFolderType=AppleFramework`
* Native xc frameworks (\*.xcframework/\*): `PublishFolderType=AppleFramework`
* Resources (*.jpg, *.png, ...?): `PublishFolderType=Resource`
* \*.framework.zip and \*.xcframework.zip:
  `PublishFolderType=CompressedAppleFramework`
* \*.dylib: `PublishFolderType=DynamicLibrary`
* \*.so: `PublishFolderType=PluginLibrary`
* \*.a: `PublishFolderType=StaticLibrary`
* No other files are copied. We show a warning if we find any such files.

## Known/valid PublishFolderType values

### None

The item won't be copied to the app bundle.

### RootDirectory

The item will be copied to the root directory of the app bundle. The `Link`
metadata can be used to place an item in a subdirectory relative to the root
directory.

### Assembly

The item is copied to where the managed assemblies are located in the app
bundle.

The assembly will not be AOT-compiled (only assemblies that are reachable by
iterating recursively over all assembly references starting with the
executable assembly are AOT-compiled), and won't be executable on platforms
where AOT-compilation is required.

The target directory is:
* iOS, tvOS: the root directory of the app bundle
* macOS, Mac Catalyst: the `Contents/MonoBundle/` subdirectory (the 
  `MonoBundle` name can be customized if desired).

### Resource

Items are copied to where resources are located in the app bundle.

The target directory is:
* iOS, tvOS: the root directory of the app bundle
* macOS, Mac Catalyst: the `Contents/Resources/` subdirectory.

### AppleBindingResourcePackage

This is a third-party binding resource package, and the actual action
performed depends on the contents of the package (we'll link with static
libraries, link with and embed dynamic libraries and frameworks).

Setting the `TargetPath` or `Link` metadata has no effect these items.

### CompressedAppleBindingResourcePackaged

Treated as a zipped third-party binding resource (first unzipped, and then
treated as `AppleBindingResourcePackage`).

### AppleFramework

* If the item is a \*.framework or \*.xcframework directory, these directories
  will be copied to the app bundle's Frameworks directory.
* If any of the item's containing directories is an \*.xcframework directory,
  then select that directory instead.
* Otherwise, if any of the item's containing directories is a \*.framework
  directory, then select that directory instead.
    * This means that if a MyFramework.framework/MyFramework file is listed,
      any other files in the MyFramework.framework directory will also be
      copied to the app bundle.
    * The order is important here: we're checking for \*.xcframework before
      \*.framework, because the former will often contain the latter, and we
      need to link with the former.
* Otherwise an error is shown.

We'll also link the native executable with the framework.

Setting the `TargetPath` or `Link` metadata has no effect these items.

### CompressedAppleFramework

The item is assumed to be a zip file containing one or more \*.framework or
\*.xcframework directories. The zip file will be decompressed, and the
\*.framework and \*.xcframework directories treated as `AppleFramework` items.

Setting the `TargetPath` or `Link` metadata has no effect these items.

### PlugIns

The target directory is:

* iOS, tvOS: the `PlugIns/` subdirectory.
* macOS, Mac Catalyst: the `Contents/PlugIns/` subdirectory.

### CompressedPlugIns

The item must be a zip file, which is decompressed, and then treated as
`PlugIns` (the contents of the zip file will be copied to the corresponding
`PlugIns` directory).

Setting the `TargetPath` or `Link` metadata has no effect these items.

If a plugin needs to be in a custom subdirectory, then put it in that
directory in the zip file.

### DynamicLibrary

These are dynamic libraries (\*.dylib) files.

We will link with these libraries when linking the native executable.

The target directory is the same as for `Assembly`:

* iOS, tvOS: the root directory of the app bundle
* macOS, Mac Catalyst: the `Contents/MonoBundle/` subdirectory.

*Warning*: The App Store will reject any apps with \*.dylib files (for iOS and
tvOS, not for macOS or Mac Catalyst).

### PluginLibrary

These are plugins provided as un-versioned dynamic library (\*.so or \*.dylib) files.

An example are GStreamer plugins: `libgstogg.dylib`

We will _not_ link with these libraries when linking the native executable since
this type of plugins are loaded on demand at runtime.

The target directory is the same as for `DynamicLibrary`

*Warning*: The App Store will reject any apps with dynamic library files, for iOS and
tvOS plugins must be provided as static libraries.

### StaticLibrary

These are static libraries (\*.a) files.

We will link with these libraries when linking the native executable.

Static libraries are not copied to the app bundle.

### Unknown

We show a warning, and we don't copy the item to the app bundle (i.e. treat it
as `None`).

## Examples

### Example 1

```xml
<Content Update="MyImage.png" PublishFolderType="PlugIns" />
```

would put MyImage.png in MyApp.app/PlugIns/MyImage.png on iOS and tvOS, and
MyApp.app/Contents/PlugIns/MyImage.png on macOS and Mac Catalyst.

### Example 2

```xml
<Content Update="MyImage.png" PublishFolderType="PlugIns" Link="Subfolder/YourImage.png" />
```

would put MyImage.png in MyApp.app/PlugIns/Subfolder/YourImage.png on iOS and
tvOS, and MyApp.app/Contents/PlugIns/Subfolder/YourImage.png on macOS and Mac
Catalyst.

### Example 3

```xml
<Content Update="MyImage.png" Link="Resources/YourImage.png" />
```

would put MyImage.png in MyApp.app/Resources/YourImage.png on all platforms
(and that would be wrong for macOS and Mac Catalyst).

## FAQ

### I have a file I want to place in the app bundle. How do I do that?

If it doesn't fit any of the existing `PublishFolderType` values, you can add
it to the `None` items like this:

```xml
<None Include="MyFile.bin" CopyToPublishDirectory="Always" PublishFolderType="RootDirectory" />
```

### I want to put a file in a specific subdirectory, not related to any other content type

```xml
<None Include="MyFile.bin" CopyToOutputDirectory="PreserveNewest" PublishFolderType="RootDirectory" Link="Subfolder/MyFile.bin" />
```

### I don't want a file to be copied to the app bundle

The easiest way is to set `PublishFolderType` to `None`:

```xml
<EmbeddedResource Include="MyFile.bin" CopyToOutputDirectory="PreserveNewest" PublishFolderType="None" />
```

## References

* [.NET: What to do about files in ResolvedFileToPublish](https://github.com/xamarin/xamarin-macios/issues/12572)
* [[maccatalyst] NativeReference results in "bundle format is ambiguous (could be app or framework)"](https://github.com/xamarin/xamarin-macios/issues/12369)
* [[.NET 6] Files copies into MonoBundle get folder structure flattened](https://github.com/xamarin/xamarin-macios/issues/12386)
* [.NET: build fails when referencing BenchmarkDotNet](https://github.com/xamarin/xamarin-macios/issues/12418)
* [Having multiple .frameworks in a nuget package fails to build due to multiple info.plist files](https://github.com/xamarin/xamarin-macios/issues/12440)
* [Automatically include .framework or .a files in the NuGet's runtimes/ios/native folder](https://github.com/xamarin/xamarin-macios/issues/11667)
