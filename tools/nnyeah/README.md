## nnyeah
nnyeah - a tool to rework pre dotnet 6 assemblies to correctly work in a
.NET 6 or later runtime.

**Important**
nnyeah is not meant to be a long-term solution to the problem of getting older assemblies to run under .NET 7 and later. It is
a stop gap that can be used to help you get your app running in the near term. In the long term, the older dependencies should be
ported to .NET 6.

Usage:
```
dotnet run --project /path/to/nnyeah.csproj \
           --input /path/to/fileToBeConverted.dll \
           --output /path/to/finaloutput.dll \
           [--xamarin-assembly /path/to/Xamarin.platform.dll \]
           --microsoft-assembly /path/to/Microsoft.platform.dll \
           [--force-overwrite]
           [--verbose]
```

The `--input` file is the name of the file to be converted. It should be a .NET dll compiled prior to .NET 6.
The `--output` output file is the name of the final output file.
The `--xamarin-assembly` is a path to the platform assembly used prior to .NET 6. It will be either `Xamarin.iOS.dll` or `Xamarin.macOS.dll`.
The `--microsoft-assembly` is a path to the platform assembly used by the `dotnet` program when building a platform assembly. It wil be either `Microsoft.iOS.dll` or `Microsoft.macOS.dll`.
If `--force-overwrite` is supplied, then `nnyeah` will overwrite the output file if it exists. If it is not supplied then nnyeah will stop with an error.
If `--verbose` is set then `nnyeah` will print out messages journaling every code change that it makes.

If you don't speficy `--xamarin-assembly` nnyeah will try to find the right file for you. It will look at the supplied `--microsoft-assembly` and
will look for a matching legacy Xamarin assembly (ie, iOS or macOS).

## Finding the Microsoft assembly
You can find the Microsoft platform assembly with either of these shell commands:
```
find $(dirname $(which dotnet)) -name Microsoft.iOS.dll -print | grep ref
find $(dirname $(which dotnet)) -name Microsoft.macOS.dll -print | grep ref
```
These may print a number of matches. The best one to use will look something like:
```
/usr/local/share/dotnet/packs/Microsoft.PLATFORM-NAME.Ref/THE-VERSION-YOU-WANT/ref/net6.0/Microsoft.PLATFORM-NAME.dll
```

nnyeah changes the following:

- All references to `System.nint`, `System.nuint` and `System.nfloat` will get changed to `native int`, `native uint` and `System.NFloat`
- Nearly all methods on `System.nint` and `System.nuint` will be changed to either equivalent calls on `native int` and `native uint` or will be replaced with equivalent inline code.
- Adds the types NativeIntegerAttribute and CompilerGeneratedAttribute
- All references to types and methods in Xamarin.platform.dll that exist in Microsoft.platform.dll will get renamed
- All objects that inherit from either `Foundation.NSObject` or `ObjCRuntime.DisposableObject` that have constructors of the form `.ctor (IntPtr) or .ctor (IntPtr, bool)` will change the `IntPtr` references to `ObjCRuntime.NativeHandle`.
- All objects that inherit from `Foundation.NSObject` that have `ClassHandle` or `Handle` properties of type `IntPtr` will get changed to type `ObjCRuntime.NativeHandle`

## What Happens When It Runs

When nnyeah runs, there are three classes of output:
1. successful with code changes (generates the output file)
2. successful with no code changes needed (does not generate the output file)
3. unsuccessful - nnyeah generates an error. 

In the last case, not every assembly can be changed to run in .NET 6. The most common reason for this is that the assembly uses
an API that does not exist in .NET 6. Typically this will be a type of API that was already obsolete and has now been removed. Less commonly,
some types have constructors that call public constructors that took `IntPtr` arguments but are now internal. In either of these two cases,
there is nothing that nnyeah can do for the old assembly and it will need to be hand-ported.

## How It Works

1. nnyeah builds a database of translations from the Xamarin platform dll onto the Microsoft platform dll. All types or entrypoints
in the Xamarin platform dll that are not found are recorded as well.
2. the input file gets loaded and is examined to see if there is any necessary work to be done. If there is no work, nnyeah exits, generating no output.
3. nnyeah retrieves types that might be needed from Microsoft.platform.dll.
4. nnyeah adds private attributes to the output assembly that are needed for `native int` and `native uint`
5. all types and methods are visited and any type references and method references get changed to the Microsoft platform assembly


## Internals

### ModuleVisitor
ModuleVisitor is a class that follows the visitor pattern and fires events when it encounters the following entities:
- Types (and inner types)
- Methods
- Fields
- Events
- Properties

ModuleVisitor is used by the ModuleElements type to aggregate all the types and members of an assembly.

### ComparingVisitor
ComparingVisitor uses a ModuleElements type for both the earlier and later assemblies and generates events to create a map from the string signature
of the earlier type/member to the Mono.cecil type for the later type/member.

### Transformation
Transformation is a class that represents a change to be made to IL instructions in a method. This allows an instruction to
be removed or replaced with a list of instructions or for a list of instructions to be inserted before or after an instruction.

### MethodTransformations
MethodTransformations is a set of static and dynamic Transformation objects to apply to IL instructions based on a signature of a method found in an existing IL instruction.
Some transformations are static because they have no outside references. If a transformation has outside references, the transformation needs to get built at runtime.

### FieldTransformations
FieldTransformations is a set of dynamic Transformation objects to apply to IL instructions based on a signature of a field reference

### ConstructorTransforms
ConstructorTransforms is code that is used to find and patch constructors that take `IntPtr`

### ModuleContainer
ModuleContainer is a class used to hold modules that are needed for the transformation.

### Reworker
The Reworker class does most of the work in terms of seeking out the places that need transformations and applying
them.

### Attributes
The Reworker class creates the following attributes for the output assembly:
- EmbeddedAttribute
- NativeIntegerAttribute

## Platform Assemblies
nnyeah uses the platform assemblies to build the mapping from pre-.NET 6 to post .NET 6. Initially the code generated the mapping once
and imported the mapping at runtime. The problem with this is that the mapping file was larger than the actual assemblies themselves and
the Microsoft platform assembly would still be necessary. Because of this it made more sense to build the mapping on the fly.