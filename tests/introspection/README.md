# Introspection Tests

Introspection tests are executed on target (both simulator and device for
iOS) or a specific version of OSX. The application proceed to analyze itself
using:

* `System.Reflection` for managed code; and
* the ObjectiveC runtime library for native code

and compare the results. E.g. if using .NET reflection it can see a binding
for a `NSBundle` type then it should be able to find a native `NSBundle` 
type using the ObjC runtime functions. Otherwise an error is raised...

Since the application analyze itself it must contains everything we wish
to test. That's why the introspection tests needs to be built with the
managed linker disable, i.e. **"Don't link"**.

Pros

* The tests always tell the truth, which can differ from documentation or header files;

Cons

* Incomplete - Not everything is encoded in the metadata / executable;
* Too complete - Not every truth is good to be known (or published), which requires creating special cases in the tests
