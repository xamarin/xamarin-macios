# Preview APIs

The APIs listed here are currently marked as preview APIs, and as such may
change in the future (we don't guarante binary or source compatibility between
releases for these APIs).

We've marked these APIs using the [Experimental][1] attribute, which means
that compilation error will be shown if they're used:

> error APL0001: 'PreviewAPI' is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

This means that it's not possible to use these preview APIs by accident, the diagnostic has to be explicitly ignored.

Example program consuming preview API:

```cs
using System.Diagnostics.CodeAnalysis;

class App
{
    public static void Main ()
    {
        Do.Something ();
    }
}

[Experimental ("APL0001")]
class Do {
    public static void Something () {}
}
```

this will show:

> Program.cs(8,9): error APL0001: 'Do' is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Then ignore the warning in order to make the code compile:

```cs
using System.Diagnostics.CodeAnalysis;

class App
{
    public unsafe static void Main ()
    {
        #pragma warning disable APL0001
        Do.Something ();
        #pragma warning restore APL0001
    }
}

[Experimental ("APL0001")]
class Do {
    public static void Something () {}
}

```

Our diagnostic IDs will be of the format `APL####` - for instance `APL0001` -
where the number is just monotonically increasing since the previous number,
without any specific meaning.

References:

* https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.experimentalattribute?view=net-8.0
* https://learn.microsoft.com/en-us/dotnet/fundamentals/apicompat/preview-apis#experimentalattribute

## Previws of new API bindings (XCODE_x_y_PREVIEW)

We sometimes release previews of new API bindings, and until these are deemed
stable (by both Apple and us), we mark all our API bindings as preview of the
new Xcode version.

The diagnostic id is `XCODE_x_y_PREVIEW`, where `x` and `y` are Xcode's major and
minor versions.

## CryptoTokenKit (APL0001)

CryptoTokenKit requires special hardware to test, so it's not trivial for us to do so.
Thus we need customer input in order to validate the API, and as such we mark every
type in this framework as preview API for a while.

We've tentatively set .NET 10 as the release when we'll stop marking CryptoTokenKit as preview API.

The diagnostic id for CryptoTokenKit is APL0001.

## FSKit (APL0002)

FSKit is a new framework in macOS 15.0 to implement a file system in user space.

At the moment implementing a file system is out of our reach, so we haven't
been able to validate the API, and as such we mark every type in this
framework as preview API for a while.

We've tentatively set .NET 11 as the release when we'll stop marking FSKit as preview API.

The diagnostic id for FSKit is APL0002.

## StoreKit.AppStore.RequestReview (APL0004)

The
[AppStore.RequestReview](https://developer.apple.com/documentation/storekit/appstore/3954432-requestreview/)
method is Swift API we've bound manually, and as such it's marked as experimental until .NET 10.

[1]: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.experimentalattribute?view=net-8.0

## Rgen (APL0003)

Rgen is the new Roslyn codegenerator based binding tool. The tool is underdevelopment and its API is open to change until
a stable release is announced.

The diagnostic id for Rgen is APL0003.
