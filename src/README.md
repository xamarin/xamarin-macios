Platform assemblies
===================

This directory contains the source code and build logic to build the platform assemblies.

Generator
=========

The generator takes API definition files (most *.cs files in src/) as input,
and generates the required binding code.

There is one generator executable, that's used to generate the binding code
for all platforms.

The generator relies heavily on binding attributes; all the binding attributes
(that are not in the platform assembly) are compiled into a separate attribute
assembly (Xamarin.Apple.BindingAttributes.dll).

Since the platform assemblies (and thus all the binding attributes assemblies
as well) reference each platform's BCL, those assemblies can't be loaded
directly into the generator at runtime. In order to not make the generator
code too complicated, all the attributes are also compiled into the generator
executable, and then instantiated as mock-objects of the real attributes.

The solution generator.sln can be used to debug the generator. There are
multiple run configurations, each configured to execute the generator with the
options for the corresponding profile.

### Generator diff

Two special `make` targets can be used to compare the generated code (.g.cs files) changes between two branches.  
This is **required** when making changes to the generator.

1. Checkout the clean base branch (e.g main's HEAD) the feature (target) branch is based on.
2. Do `make generator-reference` in `xamarin-macios/src`.
3. Checkout the feature branch that requires the diff.
4. Do `make generator-diff`.

*Tip: do `git diff | pbcopy` in `macios/src/generator-reference` and paste that anywhere ([gist](https://gist.github.com) for instance).*

Conditional compilation
=======================

These are the symbols defined for each platform assembly:

| Assembly                  | Symbols                     |
| ------------------------  | ----------------------------|
| Microsoft.iOS.dll         | `__IOS__` `IOS`             |
| Microsoft.macOS.dll       | `__MACOS__` `MONOMAC`       |
| Microsoft.tvOS.dll        | `__TVOS__` `TVOS`           |
| Microsoft.MacCatalyst.dll | `__MACCATALYST__` `__IOS__` |

To build core for only one platform, use the platform unique variables
`__IOS__`, `__MACOS__`, `__MACCATALYST__` or `__TVOS__`. The other variables
can still be used, but will eventually be phased out.

### Binding process ###

The binding process is a multi-step process:

1. We build an assembly with all the attributes used by the generated code
   (`Xamarin.Apple.BindingAttributes.dll`)
2. We build the generator (`bgen`)
3. We compile a `core.dll` with a few important core types (this is any
   `<FRAMEWORK>_CORE_SOURCES` mentions in `frameworks.sources`).
4. We compile all the api definitions into an apidefinition.dll (this is the
   `<framework.cs>.cs` file in this directory + any `<FRAMEWORK>_API_SOURCES`
   mentions in `frameworks.sources`). This references `core.dll`, so that must
   happen first.
5. We pass the `apidefinition.dll` assembly to `bgen`, asking it to generate
   the binding code (but not compile it).
6. We compile the generated binding code + any `<FRAMEWORK>_SOURCES` mentions
   in `frameworks.sources`.
7. We inject a reference to Apple's documentation into the xml documentation
   produced by the C# compiler, using the `adr` command-line tool.

#### Analyzers ####

The platform assemblies (step 6) are compiled with the .NET analyzers
(`Microsoft.CodeAnalysis.NetAnalyzers.dll` and
`Microsoft.CodeAnalysis.CSharp.NetAnalyzers.dll`, both from the .NET SDK) and
the repository's [.editorconfig](../.editorconfig). This means that analyzer
rules configured in `.editorconfig` (for instance
`dotnet_diagnostic.CA1309.severity = error`) also apply to the platform
assemblies.

The platform assemblies are compiled with `-warnaserror+`, so any diagnostic
reported by an analyzer that's enabled by default is a build error too. If
that's not wanted for a particular diagnostic, add it to
`CSC_WARNINGS_TO_FIX` in `src/Makefile` (the availability warnings CA1416 and
CA1422 are already ignored this way).

Changing `.editorconfig` rebuilds the platform assemblies.

`make analyze` builds project files generated from the same response files
using MSBuild, with the availability warnings (CA1416 and CA1422) enabled.

#### Enums ####

Enums are handled specially. Most native enums are backed by `NSInteger` or
`NSUInteger`. Unfortunately in C#, the backing type of an enum may only be
one of the primitive integral C# types. Thus, an enum cannot be backed by
`nint` or `nuint`.

The convention is to make *all* enums that are backed natively by `NSInteger`
or `NSUInteger` backed by a 64-bit primitive integral C# type (`long` or
`ulong`) and then annotated with the `[Native]` attribute. This ensures that
API is identical between the 32/64-bit assemblies but also hints to the code
generator that Objective-C runtime calls should first cast the enum to a
`nint` or `nuint`.

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
public enum NSTableViewDropOperation : nuint {
	DropOn,
	DropAbove
}
```

When dealing with enums in P/Invokes, one must *never* pass such an enum
directly. The P/Invoke signature should take a `nint` or `nuint` and a wrapper
API must cast the enum manually (as mentioned above, this is handled
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

## Source Localization ##

Coming soon!

See [Localization Wiki][Localization-wiki] for more details on our localization process

or the [OneLocBuild Wiki][OneLocBuild-wiki] for information on OneLocBuild.

[Localization-wiki]: https://github.com/xamarin/maccore/wiki/Localization
[OneLocBuild-wiki]: https://ceapex.visualstudio.com/CEINTL/_wiki/wikis/CEINTL.wiki/107/Localization-with-OneLocBuild-Task