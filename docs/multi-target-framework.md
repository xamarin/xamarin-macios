# Multi-targetting

Ref: https://github.com/xamarin/xamarin-macios/issues/18343
Ref: https://github.com/dotnet/sdk/issues/30103#issuecomment-1582571722

## Customer needs

* Consume preview packages for a preview version of Xcode.
* Compile against an earlier version of our bindings.
    * Example: a customer could be producing a NuGet targetting .NET 7, and want to support the initial release of .NET 7. Said customer must then be able to build against the bindings we shipped at the time.

## Customer viewpoint

### TargetFramework=net7.0-ios16.4

Builds with the bindings we released for iOS 16.4 (Xcode 14.3)

### TargetFramework=net7.0-ios:

Builds with the default bindings. This can change in any release (this is contrary to what other platforms do - specifically because older OS bindings might not work with newer Xcodes, and Apple auto-updates people's Xcodes, so it's rather frequent that customers use newer OS bindings. Having a lot of people specify the OS version in the target framework is undesirable).

### TargetFramework=net8.0-ios17.0

Builds with the bindings we've released for iOS 17.0

This might be preview bindings, in which case customers must also set the following property in their project file to make their intention clear:

```xml
<PropertyGroup>
    <EnablePreviewFeatures>true</EnablePreviewFeatures>
</PropertyGroup>
```

This mirrors how it's done for the Android SDK.

### Implementation details

We'll release a single workload (per platform), with bindings for every OS version we currently support or plan to support. This means our stable releases may point to preview packages (but these preview packages have to be opted in by doing two things: appending the OS version to their target framework + setting EnablePreviewFeatures=true)

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

Undecided: what do do about the template package (Microsoft.iOS.Templates)?

Potential problems:

* MAX_PATH on Windows (the package names are longer by 10-11 characters).
* Default item inclusion (see below).
* Anything else?

We'll select what's loaded at build time by doing this:

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

Note 1: One complication here is that TargetPlatformVersion might not be set (if the TargetFramework doesn't contain the OS version), so there's one condition that accepts an empty TargetPlatformVersion. This corresponds with the default target platform version.

Note 2: we load the preview sdk (Microsoft.iOS.Sdk.net8.0_17.0) even if `EnablePreviewFeatures!=true` - we show the error requesting `EnablePreviewFeatures` to be set from the preview sdk instead (this is to get an actionable error message).

Note 3: we load a special error-handling version of the sdk if we don't support a TargetPlatformVersion for given TargetFrameworkVersion, and show an error about unsupported TargetPlatformVersion. Without this, the user would get this rather unhelpful error message: `error NETSDK1139: The target platform identifier ios was not recognized.`







