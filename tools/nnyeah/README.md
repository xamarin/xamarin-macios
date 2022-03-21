## nnyeah
nnyeah - a tool to rework pre dotnet 6 assemblies to correctly work in a
dotnet 6 runtime.

This tool is not yet done.

Usage:
mono nnyeah.exe /path/to/input/file.dll /path/to/output/file.dll

nnyeah changes the following:

- All references to System.nint and System.nuint will get changed to native int
and native uint.
- Adds the types NativeIntegerAttribute and CompilerGeneratedAttribute

TODO:
- Add support for NFloat
- Add change log
- Add error handling
- Add reference renaming for Xamarin.iOS -> Microsoft.iOS
- Add transformation for .ctor(IntPtr, bool) -> .ctor (NHandle, bool) and
- Add some error checking in the case of method signatures that become synonyms when transformed

Modes of Operation
nnyeah has two modes of operation: signature and code. In signature mode, it changes the types
used by methods, fields, events, and properties. The code searches for type references and changes them.
In code mode, it acts like a peephole optimizer and searches for instructions patterns of
length 1 (although it could be more) and then applies a transformation to the code.