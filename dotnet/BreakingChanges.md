# Breaking changes in .NET

Reference: https://github.com/xamarin/xamarin-macios/issues/13087

## The SDK assemblies have been renamed.

We've renamed the SDK assemblies like this:

* `Xamarin.iOS.dll` -> `Microsoft.iOS.dll`
* `Xamarin.TVOS.dll` -> `Microsoft.tvOS.dll`
* `Xamarin.MacCatalyst.dll` -> `Microsoft.MacCatalyst.dll`
* `Xamarin.Mac.dll` -> `Microsoft.macOS.dll`

This will affect:

* Code using reflection with hardcoded assembly name.
* [Custom linker configuration files](https://docs.microsoft.com/en-us/xamarin/cross-platform/deploy-test/linker), since they contain the assembly name.

## Removed `System.nint` and `System.nuint`

The two types `System.nint` and `System.nuint` (which despite their `System`
namespace were shipped with Xamarin.iOS/Xamarin.Mac) have been removed in
favor of the C# 9 `nint` and `nuint` types (these map to `System.IntPtr` and
`System.UIntPtr` respectively).

* Code that uses these types with the full namespace (`System.nint` / `System.nuint`) won't compile.

    Fix: remove the namespace, and use `nint` / `nuint` only.

* Code that overloads on `System.IntPtr`/`System.UIntPtr` and `nint`/`nuint`won't compile.

    Example:

    ```csharp
    public void DoSomething (IntPtr value) {}
    public void DoSomething (nint value) {}
    ```

    Fix: one of the overloads would have to be either renamed or removed.

Reference: https://github.com/xamarin/xamarin-macios/issues/10508

## System.nfloat moved to ObjCRuntime.nfloat

The `nfloat` type moved from the `System` namespace to the `ObjCRuntime` namespace.

* Code that references the `nfloat` type might not compile unless the `ObjCRuntime` namespace is imported.

  Fix: add `using ObjCRuntime` to the file in question.

* Code that references the full typename, `System.nfloat` won't compile.

  Fix: use `ObjCRuntime.nfloat` instead.

## System.NMath moved to ObjCRuntime.NMath

The `NMath` type moved from the `System` namespace to the `ObjCRuntime` namespace.

* Code that uses the `NMath` type won't compile unless the `ObjCRuntime` namespace is imported.

  Fix: add `using ObjCRuntime` to the file in question.

## NSObject.Handle and INativeObject.Handle changed type from System.IntPtr to ObjCRuntime.NativeHandle

The `NSObject.Handle` and `INativeObject.Handle` properties changed type from
`System.IntPtr` to `ObjCRuntime.NativeHandle`. This also means that numerous
other parameters and return values change type in the same way; most important
are the constructors that previously took a `System.IntPtr`, or
`System.IntPtr` + `bool`. Both variants now take a `ObjCRuntime.NativeHandle`
instead.

This is so that we can support API that take native-sized integers (`nint` /
`nuint` - which map to `System.[U]IntPtr`) while at the same time have a
different overload that takes a handle.

The most common examples are constructors - all `NSObject` subclasses have a
constructor that (now) take a single `ObjCRuntime.NativeHandle` parameter, and
some types also need to expose a constructor that take a native-sized integer.
For instance `NSMutableString` has a `nint capacity` constructor, which
without this type change would be impossible to expose correctly.

There are implicit conversions between `System.IntPtr` and
`ObjCRuntime.NativeHandle`, so most code should compile without changes.

One major caveat however is that user code with an `IntPtr` constructor like
the following:

```csharp
public class MyViewController : UIViewController {
  public MyViewController (IntPtr handle) : base (handle) {}
}
```

will have to be updated to:


```csharp
public class MyViewController : UIViewController {
  public MyViewController (NativeHandle handle) : base (handle) {}
}
```

Note that code that has the `IntPtr` constructor will compile just fine (no
warnings nor errors), but it will fail at runtime.

## The ObjCRuntime.Arch enum and the Runtime.Arch property have been removed.

These APIs are used to determine whether we're executing in the simulator or
on a device. Neither apply to a Mac Catalyst app, so they've been removed.

Any code that these APIs will have to be ported to not use these APIs.

## The SceneKit.SCNMatrix4 matrix is transposed in memory.

The managed SCNMatrix4 struct used to be a row-major matrix, while the native
SCNMatrix4 struct is a column-major matrix. This difference in the memory
representation meant that matrices would often have to be transposed when
interacting with the platform.

In .NET, we've changed the managed SCNMatrix4 to be a column-major matrix, to
match the native version. This means that any transposing that's currently
done when accessing Apple APIs has to be undone.

## Some types were moved from the CoreServices namespace to the CFNetwork namespace.

The following types:

* CFHTTPStream
* CFHTTPMessage
* CFHTTPAuthentication

were moved from the CoreServices namespace to the CFNetwork namespace.

This requires adding a `using CFNetwork;` statement to any files that uses these types.

## Numerous types in ModelIO have corrected their API

When we originally implemented ModelIO, we didn't notice at first that some of
the matrix types Apple used had a column-major layout, so we accidentally
bound many API with the wrong matrix type (with a row-major layout). This was
troublesome, because many matrices had to be transposed for code to work
correctly. We re-implemented all the API with the correct matrix type, but
named differently (and worse). In .NET we've removed all the incorrectly bound
API, and we've renamed the correctly bound API to use the best name (usually
reflecting how Apple named these APIs).

This affects methods and properties on the following classes:

* MDLCamera
* MDLMaterialProperty
* MDLStereoscopicCamera
* MDLTransform
* MDLTransformComponent

## Removal of `Runtime.UseAutoreleasePoolInThreadPool`

Enabling or disabling this feature is not supported at runtime and must
be done using the MSBuild [`AutoreleasePoolSupport`](https://docs.microsoft.com/en-us/dotnet/core/run-time-config/threading#autoreleasepool-for-managed-threads)
property.

You can query if the build-time feature is enabled with the following code:

```csharp
AppContext.TryGetSwitch ("System.Threading.Thread.EnableAutoreleasePool", out var enabled);
```

## The types `NSFileProviderExtension` and `NSFileProviderExtensionFetchThumbnailsHandler` moved

The types `NSFileProviderExtension` and
`NSFileProviderExtensionFetchThumbnailsHandler` moved from the `UIKit`
namespace to the `NSFileProvider` namespace (this is reflecting that Apple
originally added these types to `UIKit`, but then moved them to their own
namespace, `NSFileExtension`).

## The 'Foundation.MonoTouchException' and 'Foundation.ObjCException' types have been renamed/moved to 'ObjCRuntime.ObjCException'.

The type `Foundation.MonoTouchException` (for iOS, tvOS and Mac Catalyst) and
the type `Foundation.ObjCException` (for macOS) have been renamed/moved to
`ObjCRuntime.ObjCException`. Both types had the exact same functionality: they
were wrapping a native NSException, and were renamed so that we have identical
API and behavior on all platforms.

## The type 'CFNetwork.MessageHandler' has been removed.

The type 'CFNetwork.MessageHandler' has been removed. Please use
'System.Net.Http.CFNetworkHandler' or the more recent
'Foundation.NSUrlSessionHandler' instead.
