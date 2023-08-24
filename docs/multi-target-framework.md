# Multi-targeting

References:

* https://github.com/xamarin/xamarin-macios/issues/18343
* https://github.com/dotnet/sdk/issues/30103#issuecomment-1582571722

## Developer needs

* Consume preview packages for a preview version of Xcode.
* Compile against an earlier version of our bindings.
    * Example: a customer could be producing a NuGet targeting .NET 7, and
      want to support the initial release of .NET 7. Said customer must then
      be able to build against the bindings we shipped at the time.

## Developer viewpoint

### TargetFramework=net7.0-ios16.4

Builds with the bindings we released for iOS 16.4 (Xcode 14.3)

### TargetFramework=net7.0-ios

Builds with the default bindings.

Contrary to other platforms, the default can change in any release. This is
because older OS bindings might not work with newer versions of Xcode, and
since Apple auto-updates Xcode, we might end up in a scenario where the
default bindings wouldn't work if we didn't update them.

This might run into problems where an updated version of either .NET or our
workloads contain unintentional breaking changes. This is still a hypothetical
scenario (it's never happened as of this writing), while the previous scenario
(a new Xcode version breaks older bindings) is very much real (it happens
pretty much every year, and often multiple times per year).

It's also a problem for library projects that want to target the earliest
bindings for a given .NET release - contrary to other platforms these projects
would have to explicitly state the target platform version in their project
files. Since library projects that want to maintain compatibility with the
first .NET release for a given .NET version is much less frequent than
executable projects, we're trying to optimize for the latter.

We considered to instead modify project templates to create projects with a
specific target platform version. For example: `dotnet new ios` might create a
project where TargetFramework=net7.0-ios16.4. The problem with this approach
is that over time every project will contain a specific target platform
version, and we end up in the same scenario where a new (auto-updated) version
of Xcode might not work with the existing bindings, and then everybody would
have to update their project files to use newer bindings. It would also beat
the purpose of having a default (since nobody would be using it anymore).

### TargetFramework=net8.0-ios17.0

Builds with the bindings we've released for iOS 17.0 (on .NET 8).

This might be preview bindings, in which case customers must also set the
following property in their project file to make their intention clear:

```xml
<PropertyGroup>
    <EnablePreviewFeatures>true</EnablePreviewFeatures>
</PropertyGroup>
```

This mirrors how it's done for the Android SDK.

### Implementation details

We'll release a single workload (per platform), with bindings for every OS
version we currently support or plan to support. This means our stable
releases may point to preview packages (but these preview packages have to be
opted in by doing two things: appending the OS version to their target
framework + setting EnablePreviewFeatures=true)

We'll rename our packages. Currently we ship these packages:

* Microsoft.iOS.Sdk
* Microsoft.iOS.Ref
* Microsoft.iOS.Runtime.ios-arm64
* Microsoft.iOS.Runtime.iossimulator-arm64
* Microsoft.iOS.Runtime.iossimulator-x64

and now we'll add the target framework supported by these packages:

* Microsoft.iOS.Sdk.net7.0_16.4
* Microsoft.iOS.Ref.net7.0_16.4
* Microsoft.iOS.Runtime.ios-arm64.net7.0_16.4
* Microsoft.iOS.Runtime.iossimulator-arm64.net7.0_16.4
* Microsoft.iOS.Runtime.iossimulator-x64.net7.0_16.4

The template package (Microsoft.iOS.Templates) keeps the original name.

Potential problems:

* MAX_PATH on Windows (the package names are longer by 10-11 characters).
* Anything else?

We'll select what's loaded at build time by doing something like this in
WorkloadManifest.targets:

```xml
<ImportGroup Condition=" '$(TargetPlatformIdentifier)' == 'iOS' ">
    <Import Project="Sdk.props"
        Sdk="Microsoft.iOS.Sdk.net6"
        Condition=" $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '6.0')) " />
    <Import Project="Sdk.props"
        Sdk="Microsoft.iOS.Sdk.net7.0_16.1"
        Condition=" $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '7.0')) And '$(TargetPlatformVersion)' != '' And $([MSBuild]::VersionEquals($(TargetPlatformVersion), '16.1'))" />
    <Import Project="Sdk.props"
        Sdk="Microsoft.iOS.Sdk.net7.0_16.4"
        Condition=" $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '7.0')) And ('$(TargetPlatformVersion)' == '' Or $([MSBuild]::VersionEquals($(TargetPlatformVersion), '16.4')))" />
    <Import Project="Sdk.props"
        Sdk="Microsoft.iOS.Sdk.net8.0_17.0"
        Condition=" $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '8.0')) And '$(TargetPlatformVersion)' != '' And $([MSBuild]::VersionEquals($(TargetPlatformVersion), '17.0'))" />

    <Import Project="Sdk-error.props"
        Sdk="Microsoft.iOS.Sdk.net7.0_16.4"
        Condition=" $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '7.0')) And ('$(TargetPlatformVersion)' != '' And !$([MSBuild]::VersionEquals($(TargetPlatformVersion), '16.1') And !$([MSBuild]::VersionEquals($(TargetPlatformVersion), '16.4')))" />

    <Import Project="Sdk-error.props"
        Sdk="Microsoft.iOS.Sdk.net8.0_17.0"
        Condition=" $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '8.0')) And ('$(TargetPlatformVersion)' != '' And !$([MSBuild]::VersionEquals($(TargetPlatformVersion), '17.0')" />
</ImportGroup>
```

Note 1: One complication here is that TargetPlatformVersion might not be set
(if the TargetFramework doesn't contain the OS version), so there's one
condition that accepts an empty TargetPlatformVersion. This corresponds with
the default target platform version.

Note 2: we load the preview sdk (Microsoft.iOS.Sdk.net8.0_17.0) even if
`EnablePreviewFeatures!=true` - we show the error requesting
`EnablePreviewFeatures` to be set from the preview sdk instead (this is to get
an actionable error message). Without this, the user would get this rather
unhelpful error message: `error NETSDK1139: The target platform identifier ios
was not recognized.`

Note 3: we load a special error-handling version of the sdk if we don't
support a TargetPlatformVersion for given TargetFrameworkVersion, and show an
error about unsupported TargetPlatformVersion. Without this, the user would
get this rather unhelpful error message: `error NETSDK1139: The target
platform identifier ios was not recognized.`
