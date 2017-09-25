Platform assemblies
===================

This directory contains the source code and build logic to build the platform assemblies.

Generator
=========

The generator takes API definition files (most *.cs files in src/) as input,
and generates the required binding code.

There is one generator executable, based on IKVM, that's used to generate the
binding code for all platforms.

The generator relies heavily on binding attributes; all the binding attributes
(that are not in the platform assembly) are compiled into a separate attribute
assembly (Xamarin.[iOS|TVOS|WatchOS|Mac].BindingAttributes.dll).

Since the platform assemblies (and thus all the binding attributes assemblies
as well) reference each platform's BCL, those assemblies can't be loaded
directly into the generator at runtime. In order to not make the generator
code too complicated, all the attributes are also compiled into the generator
executable, and then instantiated as mock-objects of the real attributes.

The solution generator.sln can be used to debug the generator. There are
multiple run configurations (`ios-classic`, `ios-unified`, `tvos`, `watchos`,
`mac-classic`, `mac-unified`, `mac-full`), each configured to execute the
generator with the options for the corresponding profile.

Conditional compilation
=======================

These are the symbols defined for each platform assembly:

| Assembly            | Symbols                                        |
| ------------------  | -----------                                    |
| monotouch.dll       | IPHONE MONOTOUCH IOS                           |
| Xamarin.iOS.dll     | IPHONE MONOTOUCH IOS XAMCORE_2_0               |
| XamMac.dll          | MONOMAC XAMARIN_MAC                            |
| Xamarin.Mac.dll     | MONOMAC XAMARIN_MAC XAMCORE_2_0                |
| Xamarin.WatchOS.dll | IPHONE MONOTOUCH WATCH XAMCORE_2_0 XAMCORE_3_0 |
| Xamarin.TVOS.dll    | IPHONE MONOTOUCH TVOS XAMCORE_2_0 XAMCORE_3_0  |

To build core for only one platform, use the platform unique variables `IOS`, `MONOMAC`, `WATCH` or `TVOS`.

## Core Assemblies ##

Currently 3 variations of the core Xamarin.iOS assembly and 5 variations of
the core Xamarin.Mac assembly are produced:

### Xamarin.iOS ###

* A 32-bit Classic assembly (uses `System.Int32` in place of `NSInteger`, etc.)
* A 32-bit Unified assembly (uses `System.nint` in place of `NSInteger`, etc.)
* A 64-bit Unified assembly (same as 32-bit Unified)

### Xamarin.Mac ###

* A 32-bit Classic assembly (uses `System.Int32` in place of `NSInteger`, etc.)
* A 32-bit Unified assembly (uses `System.nint` in place of `NSInteger`, etc.)
* A 64-bit Unified assembly (same as 32-bit Unified)
* A 32-bit Full assembly (uses `System.nint` in place of `NSInteger`, and references the v4.5 BCL)
* A 64-bit Full assembly (same as 32-bit Full)

The Classic assembly will exist in order to not break customer code. Customers
can choose to continue using this assembly, but we will encourage customers to
move to our Unified assemblies.

The Unified assemblies provides many improvements and support for 64-bit
iOS and OS X APIs.

### Native Types ###

Most native APIs use `NSInteger` (and related) typedefs. On 32-bit systems,
these are 32-bit underlying types; on 64-bit systems, these are 64-bit
underlying types.

Historically Xamarin.iOS and Xamarin.Mac have bound these explicitly as 32-bit
(`System.Int32`, etc). With the move to 64-bit that has been ongoing in OS X
for a few versions (10.6/Snow Leopard) and more recently with the anouncement
of 64-bit support in iOS, we needed a solution to support both worlds.

We have introduced 6 new types to make this possible:

| Native Type   | Legacy (32-bit) CIL Type    | New (32/64-bit) CIL Type |
| ------------- | --------------------------- | ------------------------ |
| `NSInteger`   | `System.Int32`              | `System.nint`            |
| `NSUInteger`  | `System.UInt32`             | `System.nuint`           |
| `CGFloat`     | `System.Single`             | `System.nfloat`          |
| `CGSize`      | `System.Drawing.SizeF`      | `CoreGraphics.CGSize`    |
| `CGPoint`     | `System.Drawing.PointF`     | `CoreGraphics.CGPoint`   |
| `CGRect`      | `System.Drawing.RectangleF` | `CoreGraphics.CGRect`    |

In the Classic assembly, the `System.Drawing` types are backed by the 32-bit
`System.Single` type. In the Unified assemblies, the `CoreGraphics` types
are backed by 32/64-bit `System.nfloat` type.

When binding APIs, it is important to use the *new* types (`nint`, `CGRect`,
etc), *even though they do not exist in the Classic assembly*.

Before compilation, all source code is preprocessed by `pmcs`, an internal
tool for performing C#-aware preprocessing.

For the Classic assembly, instances of the new types are translated to the
legacy types. For the Unified assemblies, these types are not translated,
and the native types are included in the build.

#### Enums ####

Enums are handled specially. Most native enums are backed by `NSInteger` or
`NSUInteger`. Unfortunately in C#, the backing type of an enum may only be
one of the primitive integral C# types. Thus, an enum cannot be backed by
`System.nint` or `System.nuint`.

The convention is to make *all* enums that are backed natively by `NSInteger`
or `NSUInteger` backed by a 64-bit primitive integral C# type (`long` or
`ulong`) and then annotated with the `[Native]` attribute. This ensures that
API is identical between the 32/64-bit assemblies but also hints to the code
generator that Objective-C runtime calls should first cast the enum to a
`System.nint` or `System.nuint`.

However, this also presents a problem of keeping the enum 32-bit on the
Classic assemblies. Therefore, enums should actually be backed in source
code `nuint_compat_int`. This will be preprocessed to `int` on the Classic
assembly and `ulong` on the 32/64-bit assemblies.

**Native Enum Definition**

```c
typedef NS_ENUM(NSUInteger, NSTableViewDropOperation) {
	NSTableViewDropOn,
	NSTableViewDropAbove
};
```

**Managed Enum Definition**

```csharp
[Native]
public enum NSTableViewDropOperation : nuint_compat_int {
	DropOn,
	DropAbove
}
```

When dealing with enums in P/Invokes, one must *never* pass such an enum directly.
The P/Invoke signature should take a `System.nint` or `System.nuint` and a
wrapper API must cast the enum manually (as mentioned above, this is handled
automatically for Objective-C APIs by the generator).

**Objective-C Binding**
```csharp
interface Fooable {
	[Export ("foo:")]
	void Foo (NSTableViewDropOperation dropOp);
}
```

**C Binding**

```csharp
public partial class Fooable {
	[DllImport ("foo")]
	static extern void Foo (nuint dropOp);

	public static void Foo (NSTableViewDropOperation dropOp)
	{
		Foo ((nuint)(ulong)dropOp);
	}
}
```

### pmcs ###

`pmcs` is a wrapper around a regular `mcs` invocation. In addition to any
regular arguments passed to `mcs`, `pmcs` accepts instructions on how to
translate some tokens from one value to another. This is used to translate
instances of native types in source code to legacy types for the 32-bit
Classic assembly.

`pmcs` lives in the `xamarin-macios` repository and is used for both Xamarin.Mac
and Xamarin.iOS. [Explore pmcs documentation](https://github.com/xamarin/xamarin-macios/blob/master/tools/pmcs/README.md).

### `#define` ###

There are a few preprocessor variables that can be used within sources for
conditional compilation:

| Variable  | Description |
| --------- | ------------|
| `MONOMAC` | defined for Xamarin.Mac builds; not defined for Xamarin.iOS |
| `ARCH_32` | defined when the target architecture is 32-bit; this will be defined for Classic and the Unified 32-bit assemblies |
| `ARCH_64` | defined when the target architecture is 64-bit; this will be defined only for the Unified 64-bit assembly |
| `XAMCORE_2_0` | defined for the Unified assemblies; this should be used for most conditions dealing with API differences between Unified and Classic assemblies |
| `COREBUILD` | defined when building the intermediate `core.dll` assembly against which the code generator will produce bindings |

For example, to build an API for all of iOS but only 64-bit OS X (Xamarin.Mac):

```csharp
#if !MONOMAC || (MONOMAC && ARCH_64)
...
#endif
```
