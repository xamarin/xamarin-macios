# .NET "Single Project"

In order to improve the cross-platform experience between Android and our
Apple platforms, there are certain properties that can be set in the project
file that will be added to the app in a platform-specific way.

More a more detailed description see this document: [OneDotNetSingleProject.md][1]

For our Apple platforms this means we're mapping the following MSBuild
properties to Info.plist keys (this mapping will only take place if the
Info.plist in the project doesn't already contain entries for these keys):

| MSBuild Property          | Info.plist key             | Notes                                     |
| --------------------------|----------------------------|-------------------------------------------|
| ApplicationId             | CFBundleIdentifier         |                                           |
| ApplicationTitle          | CFBundleDisplayName        |                                           |
| ApplicationVersion        | CFBundleVersion            |                                           |
| ApplicationDisplayVersion | CFBundleShortVersionString | Defaults to ApplicationVersion when blank |

This is only enabled if the `GenerateApplicationManifest` is set to `true`
(which is the default for `.NET 6`, and not for "legacy"
Xamarin.iOS/Xamarin.Mac)

Additionally, `$(ApplicationDisplayVersion)` will overwrite the value for `$(Version)`,
so the following properties will be set with the same value:

* `$(AssemblyVersion)`
* `$(FileVersion)`
* `$(InformationalVersion)`

Ref: [Issue #10473][2]

[1]: https://github.com/xamarin/xamarin-android/blob/40cedfa89c2660479fcb5e2482d2463fbcad1d04/Documentation/guides/OneDotNetSingleProject.md
[2]: https://github.com/xamarin/xamarin-macios/issues/10473
