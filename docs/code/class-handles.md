# Objective-C classes

Objective-C classes can be referenced from managed code in several ways:

* Calls to Class.GetHandle / GetHandleIntrinsic

It's highly desirable to use a direct native reference to Objective-C classes when building a mobile app, for a few reasons:

* It's faster at runtime, and the app is smaller.
* If the referenced Objective-C class comes from a third-party static library, the
  native linker can remove it if it's configured to remove unused code
  (because the native linker can't see that the class is in fact used
  at runtime) unless there's a direct native reference to the class.

On the other hand there's one scenario when a direct native reference is not desirable: when the native Objective-C class does not exist.

This can happen for third-party bindings that declare a `[BaseType (typeof (NSObject))]`
type for something that is a protocol - and not a class - natively (bgen generates a
`[Protocol]` attribute on such a type). There's no native Objective-C class for these types
(the static registrar doesn't even register them), so a direct native reference would turn
into a hard link error (`Undefined symbols: _OBJC_CLASS_$_TheClass`), even if the class is
never actually used at runtime.

To avoid this, we don't inline `Class.GetHandle` when the target class has a `[Protocol]`
attribute: the call falls back to a runtime lookup instead, which returns a zero handle
(`IntPtr.Zero`) for a missing native class - the same behavior these bindings had before
`Class.GetHandle` inlining existed.

In order to create a direct native reference to Objective-C classes, we need to know the names of those Objective-C classes.

## The `InlineClassGetHandle` property

This behavior is controlled by the `InlineClassGetHandle` MSBuild property, which
can either be enabled or disabled.

See the [build properties documentation](../building-apps/build-properties.md) for default values.

## How it works

During the build we try to collect the following:

* Any calls to `Class.GetHandle[Intrinsic]` APIs: we try to collect the class name (this might not always succeed, if the class name is not a constant).

This is further complicated by the fact that we only want to create native
references for managed references that survive trimming.

So we do the following:

1. During assembly preparation:

	* `InlineClassGetHandleStep`: for every call `Class.GetHandle` we've
	  collected, this step creates a P/Invoke to a native method that will
	  return the Objective-C class for that symbol (using a direct native
	  reference), and modifies the code that fetches that symbol to call said
	  P/Invoke.
	  The native static registrar registers types before this step collects
	  class metadata, rather than waiting until postprocessing. This preserves
	  Swift stub-class annotations required by the generated native lookups.

2. After trimming, we figure out which of those symbols survived:

	* For ILTrim: the `_CollectPostILTrimInformation` MSBuild target inspects
	  the trimmed assemblies and collects all the inlined P/Invokes that
	  survived. Per-assembly results are cached to speed up incremental builds.
	* For NativeAOT: the `_CollectPostNativeAOTTrimInformation` MSBuild target
	  inspects the native object file (or static library) produced by NativeAOT,
	  collects all unresolved native references, and filters them against the
	  Objective-C classes to determine which survived.

3. The `_PostTrimmingProcessing` MSBuild target takes the surviving symbols
   from either path, generates the corresponding native Objective-C code, and
   adds it to the list of files to compile and link into the final executable.

Note that the generated native code contains a direct reference to each surviving
Objective-C class, so the native registrar must register any such class, even when
ILC trimmed away all the trampolines for the corresponding managed type (otherwise
the reference would turn into a hard link error). This is why the surviving class
names are also passed to the registrar when the native registrar code is generated
after ILC (see `Application.ClassesReferencedByInlinedClassGetHandle`).
