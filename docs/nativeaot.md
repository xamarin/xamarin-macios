# NativeAOT

We've added *experimental* support for using [NativeAOT][1] when publishing iOS,
tvOS, Mac Catalyst and macOS apps in .NET 8.

NativeAOT may produce smaller and/or faster apps - or it may not. It's very
important to test and profile to determine the results of enabling NativeAOT.

Our initial testing shows significant improvements both in size (up
to 50% smaller) and startup (up to 50% faster). For more information about
performance see [.NET 8 Performance Improvements in .NET MAUI][3].

However, it is important to point out that NativeAOT requires trimming whole
applications (using `<MtouchLink>Full</MtouchLink>`, which is enabled by
default) and is much more restrictive and aggressive in removing code which is
not AOT compatible. As a result, if the applications is using some AOT
incompatible constructs (e.g. dynamically referencing code for which a
dependency cannot be statically determined), NativeAOT compiler might remove
them during build time, which can result in crashes at runtime. To warn users
about when this can happen, trimming and AOT warnings are produced and they
should not be neglected. Instead, all such warnings should be addressed, code
adapted accordingly, and the application must be tested and profiled in this
configuration. Only then it is safe to assume that the application is
compatible with NativeAOT and should not cause any unexpected behaviour.

Finally, there could be cases when fixing mentioned trimming and AOT warnings
is not possible, i.e. warnings coming from referenced frameworks or
third-party libraries. In such cases, `<MtouchLink>SdkOnly</MtouchLink>` can
be specified on the project level and could help as a workaround. This affects
the application size, but even in this mode NativeAOT tends to produce up to
28% smaller applications compared to Mono showing a great potential of this
new experimental feature.

## How to enable NativeAOT?

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

## Compatibility and limitations

There are no known issues specific to our platforms with NativeAOT; but the
[limitations][2] are exactly the same as for other supported platforms.

Nevertheless, we would like to point out a few features that are not available
with NativeAOT, that are with Mono, when targeting Apple platforms:

- NativeAOT does not support managed debugging.

- There's no interpreter when using NativeAOT, and as such the
  `UseInterpreter` and `MtouchInterpreter` properties have no effect.

- NativeAOT requires trimming, and `MAUI` isn't trimmer-safe, and thus
  unfortunately `MAUI` projects don't typically work with NativeAOT (we hope
  to rectify this situation for .NET 9).

[1]: https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot
[2]: https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=net8plus%2Cwindows#limitations-of-native-aot-deployment
[3]: https://devblogs.microsoft.com/dotnet/dotnet-8-performance-improvements-in-dotnet-maui/
