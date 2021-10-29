# Breaking changes in .NET

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
