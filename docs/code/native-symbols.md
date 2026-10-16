# Native symbols

Native symbols can be referenced from managed code in several ways:

* P/Invokes (DllImports)
* Calls to `dlsym`, which can happen through:
	* The various APIs in `ObjCRuntime.Dlfcn`
	* The various APIs in `System.Runtime.InteropServices.NativeLibrary`
	* A P/Invoke directly into `dlsym`

It's highly desirable to use a direct native reference to native symbols when building a mobile app, for a few reasons:

* It's faster at runtime, and the app is smaller.
* If the referenced native symbol comes from a third-party static library, the
  native linker can remove it if it's configured to remove unused code
  (because the native linker can't see that the native symbol is in fact used
  at runtime) unless there's a direct native reference to the symbol.

On the other hand there's one scenario when a direct native reference is not desirable: when the native symbol does not exist.

In order to create a direct native reference to native symbols, we need to know the names of those native symbols.

## The `InlineDlfcnMethods` property

This behavior is controlled by the `InlineDlfcnMethods` MSBuild property, which
has two modes:

* `strict`: all calls to `ObjCRuntime.Dlfcn` APIs are inlined.
* `compatibility`: only calls that reference symbols from `[Field]` attributes are inlined.

See the [build properties documentation](../building-apps/build-properties.md) for default values.

## How it works

During the build we try to collect the following:

* Any property or field with the `[Foundation.Field]` attribute: we collect the symbol name.
* Any calls to the `ObjCRuntime.Dlfcn` APIs: we try to collect the symbol name (this might not always succeed, if the symbol name is not a constant).
* We don't process calls to `System.Runtime.InteropServices.NativeLibrary` at the moment (this may change in the future, if there's need).

This is further complicated by the fact that we only want to create native
references for symbols that survive trimming.

So we do the following:

1. Before or during trimming, we run the inlining steps:

	* `ProcessExportedFields`: collects all members with `[Field]` attributes.

	* `InlineDlfcnMethodsStep`: inspects all calls to `ObjCRuntime.Dlfcn`, and
	  inlines them depending on the selected mode. If inlined, the step creates
	  a P/Invoke to a native method that will return the address for that symbol
	  (using a direct native reference), and modifies the code that fetches that
	  symbol to call said P/Invoke.

2. After trimming, we figure out which of those symbols survived:

	* For ILTrim: the `_CollectPostILTrimInformation` MSBuild target inspects
	  the trimmed assemblies and collects all the inlined dlfcn P/Invokes that
	  survived. Per-assembly results are cached to speed up incremental builds.

	* For NativeAOT: the `_CollectPostNativeAOTTrimInformation` MSBuild target
	  inspects the native object file (or static library) produced by NativeAOT,
	  collects all unresolved native references, and filters them against the
	  inlined dlfcn symbols to determine which survived.

3. The `_PostTrimmingProcessing` MSBuild target takes the surviving symbols
   from either path, generates the corresponding native C code, and adds it to
   the list of files to compile and link into the final executable.

## Runtime library P/Invokes

`ListExportedSymbols` also preserves P/Invoke symbols from the runtime libraries
we link with. For MonoVM, this includes `System.Globalization.Native`, whose
symbols are provided by `libmonosgen-2.0` instead of a separate library. This
exception must not apply to NativeAOT: its compiler creates direct native
references, and its build targets omit the globalization library when
`InvariantGlobalization` is `true`.
