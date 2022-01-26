# Breaking changes in .NET

Reference: https://github.com/xamarin/xamarin-macios/issues/13087

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
