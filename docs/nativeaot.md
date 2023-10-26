# NativeAOT

We've added *experimental* support for using [NativeAOT][1] when publishing iOS,
tvOS, Mac Catalyst and macOS apps in .NET 8.

NativeAOT may produce smaller and/or faster apps - or it may not. It's very
important test and profile to determine the results of enabling NativeAOT.

NativeAOT is enabled by setting the `PublishAot` property to `true` in the project file:

```xml
<PropertyGroup>
	<PublishAot>true</PublishAot>
</PropertyGroup>
```

## Notes

NativeAOT is only used when publishing (`dotnet publish`). In particular
`dotnet build -t:Publish` is _not_ equivalent to `dotnet publish`, only
`dotnet publish` will enable NativeAOT.

**Unsupported** workaround: set the `_IsPublishing` property to `true` to make
`dotnet build` think it's `dotnet publish`:

```xml
<PropertyGroup>
	<PublishAot>true</PublishAot>
	<_IsPublishing>true</_IsPublishing>
</PropertyGroup>
```

This can be useful to install and run apps with `NativeAOT` from the IDE,
because it's not possible to use `dotnet publish` when running from the IDE.

Note that NativeAOT does not support managed debugging.

There's no interpreter when using NativeAOT, and as such the `UseInterpreter`
and `MtouchInterpreter` properties have no effect.

NativeAOT requires trimming, and `MAUI` isn't trimmer-safe, and thus
unfortunately `MAUI` projects don't typically work with NativeAOT (we hope to
rectify this situation for .NET 9).

There are no known issues specific to our platforms with NativeAOT; but the
[limitations][2] are exactly the same as for other platforms.

[1]: https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot
[2]: https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=net8plus%2Cwindows#limitations-of-native-aot-deployment
