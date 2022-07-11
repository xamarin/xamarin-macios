# Default complication includes in iOS, tvOS, macOS and Mac Catalyst projects

Default compilation includes for .NET Core projects is explained here:
[Default compilation includes in .NET Core projects][1]

This document explains how default compilation includes is implemented for
iOS, tvOS, macOS, and Mac Catalyst projects.

Default inclusion can be completely disabled by setting
`EnableDefaultItems=false`. It can also be disabled per-platform by setting
the platform-specific variables `EnableDefaultiOSItems=false`,
`EnableDefaulttvOSItems=false`, `EnableDefaultMacCatalystItems=false`, or
`EnableDefaultmacOSItems=false`.

## Property lists

All \*.plist files in the root directory are included by default (as `None`
items).

## SceneKit Assets

All \*.scnassets directories anywhere in the project directory or any
subdirectory are included by default (as `SceneKitAsset` items).

## Storyboards

All \*.storyboard and \*.xib files in the project directory or any
subdirectory are included by default (as `InterfaceDefinition` items).

## Asset catalogs

All \*.pdf, \*.jpg, \*.png and \*.json files inside asset catalogs
(\*.xcassets) in the project directory or any subdirectory are included by
default (as `ImageAsset` items).

## Atlas Textures

All \*.png files inside \*.atlas directories in the project directory or any
subdirectory are included by default (as `AtlasTexture` items).

## Core ML Models

All \*.mlmodel files anywhere in the project directory or any subdirectory are
included by default (as `CoreMLModel` items).

## Metal

All \*.metal files anywhere in the project directory or any subdirectory are
included by default (as `Metal` items).

## All other files in the Resources/ subdirectory

All files in the Resources/ subdirectory, except any items in the `Compile` or
`EmbeddedResource` item groups, and except the ones mentioned above
(\*.scnassets, \*.storyboard, \*.xib, \*.xcassets, \*.atlas, \*.mlmodel,
\*.metal) are included by default (as `BundleResource` items).

[1]: https://docs.microsoft.com/en-us/dotnet/core/tools/csproj#default-compilation-includes-in-net-core-projects

