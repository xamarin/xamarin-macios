# Trimming-friendly Registrar

Author: Simon Rozsival
Last update: Oct 23, 2023


## Introduction

When I first started looking into how to remove the dependency on Custom Linker Steps in the build process of
iOS apps using NativeAOT, I tried coming up with ideas how to migrate the code that generates the Managed Static
Registrar (MSR). This effort failed because the MSR needs to generate code _between_ the
marking/dependency analysis phase of a linker and the native codegen phase and implementing this logic directly into
ILCompiler wasn't an option.

After becoming more familiar with the Xamarin codebase and learning how it works on low levels, I think that in order
to remove custom linker steps, we cannot just migrate the code that generates any of the existing registrars out of
the custom linker steps. We need a _new registrar_ that can be generated independently of any linker.

In this document, I describe my idea of transforming the existing MSR to make it fully trimmable.

---

## Why is the current registrar not trimmable?

### TL;DR

The current registrar requires several lookup tables (both in .NET and in Objective-C) which we can generate only once we
know which types survived trimming. We use these lookup tables to create managed object instances for existing native
objects and to create instances of native objects for existing managed objects.

These methods can't be generated _before_ trimming as they would root all NSObject subclasses.

I propose removing the lookup tables and instead use Objective-C virtual method dispatch and Objective-C category methods
with specific naming conventions.

### The long version

The current managed static registrar has quite complicated logic to create an instance of a .NET object for an existing
native object. It is built as an extension of the existing _static_ registrar and it still uses the same logic to
find which .NET type maps to which Objective-C type.

Consider the following example:

```c#
class NSDictionary : NSObject { /* ... */ }
class NSMutableDictionary : NSDictionary { /* ... */ }

// app code:
[Export("getDictionaryItem:atIndex:")]
public void GetDictionaryItem(NSDictionary dictionary, int i) { /* ... */ }

// generated code:
[UnmanagedCallersOnly(EntryPoint = "_registrar__...")]
public static void GetDictionaryItem(IntPtr pobj, IntPtr pDictionary, int i)
{
  // ...
  NSDictionary dictionary = Runtime.GetNSObject<NSDictionary>(pDictionary);
  // ...
}
```

__Note:__ All code examples in this document are simplified and aren't production-ready. For example, I omitted
exception handling in UCO methods and I removed the `__Registrar_Callbacks__` class to reduce nesting.

In the generated method, we need to call the instance method on a managed object corresponding to the native object
based on the handle of its peer native object. The `Runtime.GetNSObject<T>(IntPtr handle)` method resolves the managed
instance using the following logic:

1. If there's an existing .NET object instance for the native object and it's of type `T`, _return it_.
2. Otherwise, find the .NET type that corresponds to the native object (note that we cannot simply use the type from the
   managed method signature).
3. Create an instance of the .NET type and pass the native handle to the constructor.

If the native object corresponding to `pDictionary` is an instance of Objective-C `NSMutableDictionary` class and there
isn't an existing managed object mapped to the handle, the `GetNSObject` method should return a new instance of the .NET
`Foundation.NSMutableDictionary` class.

The internal implementation of the `GetNSObject` method currently works as follows:
1. It determines the Objective-C class of the native object (using `objc_getClass`)
2. It finds the "type ID" of the .NET type corresponding to the native class using the generated `MTRegistrationMap`
   native lookup table
    - _Note_: We use generated "type IDs" to emulate type metadata tokens which aren't available in NativeAOT.
3. It finds the .NET `RuntimeTypeHandle` of the type that corresponds to the "type ID" using a generated lookup
   function `RuntimeTypeHandle LookupType(uint typeId)`
4. It creates an instance of the .NET object using the generated lookup function
   `NSObject ConstructNSObject(RuntimeTypeHandle typeHandle, IntPtr nativeInstanceHandle)`

There is also an inverse mechanism which is simpler. We have one more lookup function `uint LookupTypeId(RuntimeTypeHandle runtimeHandle)`
that's used in combination with the already mentioned `MTRegistrationMap` to create instances of native objects for a given
.NET object instance.

## Proposed Solution

### TL;DR

Instead of asking a native object what its native class is and looking up the corresponding .NET type and instantiating it,
we can ask the native object to create an instance of its managed counterpart directly.

We are in control of the generated Objective-C code for the classes we fully generate and we can attach instance methods
to existing classes using Objective-C categories.

### The long version

We can generate an Objective-C instance method `createManagedInstance` when we generate Objective-C classes for NSObject
subclasses defined in the .NET code. We can generate the same method as a category method (Objective-C "extension"
method) for existing Objective-C classes that just have .NET bindings.

For existing types which we can't extend with category methods (C-style structs) and for protocols, we'll need to generate
"fallback" factory methods on a custom `__dotnet` Objective-C class with a specifically named selector.

These Objective-C methods will call to an `[UnmanagedCallersOnly]` method that will create the managed object instance
and return a GCHandle back to Objective-C. This "P/Invoke + reverse P/Invoke" combo will resolve the correct type using
Objective-C virtual dispatch mechanism. None of the three lookup tables mentioned before will be necessary anymore.

The generated code for our example could look something like this (all error handling is omitted):

```c#
partial class NSDictionary {
  [UnmanagedCallersOnly(EntryPoint = "_registrar__Foundation_NSDictionary__createManagedInstance")]
  public static IntPtr CreateManagedInstance(IntPtr handle)
    => GCHandle.Alloc(new NSDictionary(handle)).ToIntPtr();
}

partial class NSMutableDictionary {
  [UnmanagedCallersOnly(EntryPoint = "_registrar__Foundation_NSMutableDictionary__createManagedInstance")]
  public static IntPtr CreateManagedInstance(IntPtr handle)
    => GCHandle.Alloc(new NSMutableDictionary(handle)).ToIntPtr();
}
```
```obj-c
@class __dotnet;

@implementation NSDictionary (__dotnet)
id _registrar__Foundation_NSDictionary__createManagedInstance(id handle);
-(id) createManagedInstance {
  return _registrar__Foundation_NSDictionary__createManagedInstance(self);
}
@end

@implementation NSMutableDictionary (__dotnet)
id _registrar__Foundation_NSMutableDictionary__createManagedInstance(id handle);
-(id) createManagedInstance {
  return _registrar__Foundation_NSMutableDictionary__createManagedInstance(self);
}
@end
```

For the C-style structs and protocols with protocol wrappers we could generate this code:

```c#
partial class CGRect {
  [UnmanagedCallersOnly(EntryPoint = "_registrar__CoreGraphics_CGRect__CreateManagedInstance")]
  public static IntPtr CreateManagedInstance(IntPtr handle)
    => GCHandle.Alloc (new CGRect (handle, owns: false)).ToIntPtr ();
}

[Protocol(WrapperType = typeof(MyProtocolWrapper))]
interface MyProtocol { /* ... */ }

partial class MyProtocolWrapper {
  [UnmanagedCallersOnly(EntryPoint = "_registrar__MyProtocol__CreateManagedInstance")]
  public static IntPtr CreateManagedInstance(IntPtr handle)
    => GCHandle.Alloc (new MyProtocolWrapper (handle)).ToIntPtr ();
}
```
```obj-c
@implementation __dotnet (CGRect)
id _registrar__CoreGraphics_CGRect__CreateManagedInstance (id handle);
+(id) createManagedInstance_CoreGraphics_CGRect: (id) handle {
  return _registrar__CoreGraphics_CGRect__CreateManagedInstance (handle);
}
@end

@implementation __dotnet (MyProtocol)
id _registrar__MyProtocol__CreateManagedInstance (id handle);
+(id) createManagedInstance_MyProtocol: (id) handle {
  return _registrar__MyProtocol__CreateManagedInstance (handle);
}
@end
```

The `GetNSObject` method can then take advantage of the generated methods like this:

```c#
partial static class Runtime {
  private static Selector respondsToSelector = Selector.GetHandle("respondsToSelector:");
  private static Selector createManagedInstance = Selector.GetHandle("createManagedInstance");
  private static Class dotnetClass = Class.GetHandle("__dotnet");

  public static T? GetNSObject<T> (NativeHandle handle)
  {
    if (handle == NativeHandle.Zero) {
      return default;
    }

    var instance = TryGetManagedInstance<T> (handle) ?? TryCreateManagedInstance<T> (handle);

    if (instance is null) {
      // ... in some cases we want to throw an exception ...
    }

    return instance;
  }

  public static T? TryGetManagedInstance<T>(NativeHandle handle)
    where T : INativeObject
  {
    if (handle == IntPtr.Zero) {
      return null;
    }

    if (object_map.TryGetValue(handle, out INativeObject someNativeObject) && someNativeObject is T nativeObject) {
      // ... handle the case when an nsobject is in finalizer queue ...
      return nativeObject;
    }

    return null;
  }

  public static T? TryCreateManagedInstance<T>(NativeHandle handle)
    where T : INativeObject
  {
    if (handle == IntPtr.Zero) {
      return null;
    }

    var canCreateManagedInstance = Messaging.bool_objc_msgSend(handle, respondsToSelector, createManagedInstance);
    if (canCreateManagedInstance) {
      var managedHandle = Messaging.IntPtr_objc_msgSend(handle, createManagedInstance);
      return GCHandle.FromIntPtr(managedHandle).Target as T;
    }

    // fallback for C-style structs and protocols
    var fallbackSelector = Selector.GetHandle ($"createManagedInstance_{MangleName (typeof (T))}:");
    var canCreateManagedInstanceUsingFallback = class_respondsToSelector (dotnetClass.Handle, fallbackSelector) == 1;
    if (canCreateManagedInstanceUsingFallback) {
      var managedHandle = Messaging.IntPtr_objc_msgSend(dotnetClass.Handle, fallbackSelector, handle);
      return GCHandle.FromIntPtr(managedHandle).Target as T;
    }

    return null;
  }
}
```

There is a similar solution for the inverse direction in which we want to create a new instance of a native object
for an existing .NET object (which boils down to the implementation of `Class.FindClass(Type type, out bool isCustomType)`):

```c#
[Register("MyNativeClass")]
public class MyClass : NSObject {}
```
```obj-c
@implementation __dotnet (MyNativeClass)
+(id) getNativeClass_MyNamespace_MyClass: (BOOL*) isCustomType {
  *isCustomType = YES;
  return [MyNativeClass class];
}
@end
```
```c#
partial class Class {
  private static IntPtr s_dotnetTypesClass = Class.GetHandle("__dotnet");

  public unsafe static IntPtr FindClass(Type type, out bool isCustomType) {
    byte _isCustomType = 0;
    var selector = Selector.GetHandle($"getNativeClass_{MangleName(type)}:");
    var classHandle = Messaging.IntPtr_objc_msgSend_ref_byte(s_dotnetTypesClass, selector, &_isCustomType);
    isCustomType = _isCustomType == 1;
    return classHandle;
  }
}
```

## Dependencies between native and managed code - considerations for guiding IL Compiler

The registrar allows native Objective-C code to create instances of managed classes and call methods on them. In fact,
some managed objects might only be instantiated from Objective-C and some their methods might only ever be called from
Objective-C. In these cases, we need to guide the trimmer to correctly preserve the code we need. Some of the scenarios
we need to express are these:

- "Keep this class and all its members. It is only ever created from Objective-C and only called from Objective-C."
- "Keep all the members of this class if this class survives trimming. Some of the members will only be called through UCOs."

In Xamarin, it was a common practice to use `[Preserve(AllMembers = true)]`  and `[Preserve(Conditional = true)]`
attributes to express these scenarios. In NativeAOT, we can "strategically" place `[DynamicDependency]` attributes
on module initializers or class and instance constructors to emulate similar behaviour today.

Unfortunately, `DynamicDependency` implies use of reflection. Therefore, the IL Compiler emits unnecessary metadata.
Ideally, we would have some additional attribute to express "static" or "implicit" dependency.  It would also help if we
could express "inverse" dependency as discussed in https://github.com/dotnet/runtime/issues/50333.

The current .NET 8 release of NativeAOT on iOS doesn't need the inverse dependency because it uses Mono.Cecil to place
the attributes wherever needed and generate any missing constructors.

### Trimmable `[UnmanagedCallersOnly]` methods with explicit entry point names?

Unlike ILLink, the ILCompiler currently doesn't support trimming UCOs with an EntryPoint. The UCO entry point is considered
public API surface of the binary and so it's also a root of dependency-analysis the same way the `Main` method is.
If we wanted to pre-generate all the UCO entry points for all NSObject subclasses in .NET MAUI or some
other UI control library, we couldn't trim any of those controls even though they aren't used anywhere in the app.

In order to implement the fully trimmable registrar, we need to be able to tell ILCompiler to behave the same way as ILLink
and allow trimming UCOs with explicit entry point names and also a way to reliably express the "conditionally preserve
all members" semantic.

```c#
public partial class MyClass : NSObject {
  [Export("myMethod")]
  public void MyMethod() {}
}

// generated code
partial class MyClass {
  [DynamicDependency(DynamicallyAccessedMembers.PublicMethods, typeof(MyClass.__Registrar_Callbacks__))] // MyClass is preserved => preserve all methods of __Registrar_Callbacks__
  static MyClass() {} // this would be a problem for roslyn source generators -> if the class already has a static constructor, this can't be generated 🤔

  // [Preserve(Conditional = true, AllMembers = true)] // -- we need to express the semantic of this Xamarin concept
  private static class __Registrar_Callbacks__ {
    [UnmanagedCallersOnly(EntryPoint = "_registrar_MyNamespace_MyClass_myMethod")]
    // if we had "inverse dependency" attribute what would be its target in this case?
    // - it couldn't be MyMethod because that method might only be called from the UCO and we can't guarantee that it
    //   won't be trimmed
    // - the #cctor of MyClass? (we could source-generate a static cctor if there isn't one)
    //   - this approach works in .NET 8, but is it a reliable method going forward?
    // - DynamicallyAccessedMemberTypes.All?
    //   - would that generate way too much metadata?
    public void MyMethod(IntPtr pobj, IntPtr sel) {
      MyClass obj = Runtime.TryGetManagedInstance(pobj) ?? Runtime.TryCreateManagedInstance(pobj) ?? throw new Exception("...");
      obj.MyMethod(); // this method is preserved => preserve the exported method
    }
  }
}
```

---
 
## Cherry-picking the native side of the registrar

To make .NET classes available to the Objective-C world, we must generate Objective-C proxy classes or categories which
forward all calls to the exported selectors to the original .NET class via reverse P/Invokes.

After trimming, we need to detect which .NET classes survived trimming and which Objective-C classes and categories
we need to include in the app (and avoid using the rest of the pre-generated Objective-C code).

I propose the following process:
- Generate all the Objective-C classes while we generate the C#/IL code before trimming
- Put each generated Objective-C class or category into its separate `<objc-name>.h` + `<objc-name>.mm` files
- Store additional information about the native dependencies of the class based on the parent class, implemented protocols,
  and method parameters and return values
  - Frameworks
  - Third-party static libraries
  - Other generated Objective-C classes
- After ILC produces the object file, collect the information about the Objective-C classes we need to include
  - Scan the binary for undefined symbols which follow a specific naming convention which would include the Objective-C class name:
  ```bash
  $ nm -u aoted-app.o | grep '^_registrar__'
  ```
- Collect the Objective-C source files from the required Objective-C classes and their dependencies
- Build the Objective-C source files into a second object file
- Link the AOTed object file together with the second object file + all the frameworks and third-party libraries

### Registrar endpoint naming convention

The names of the registrar callbacks should contain the Objective-C class or category name we should include and it should
be easy to reliably parse the endpoint name: `_registrar__<L>_<objc-name>_<N>_<sanitized-selector>`.
  - The `L` would be the length of the `objc-name` string (could be encoded as dec, hex, base64)
  - The `sanitized-selector` would be the selector where `:` symbols are replaced with `_`.
  - The `N` is the index of the selector in an array of sorted selectors of the class/category
    - this is a form of de-duplication to avoid name clashes caused by the sanitization (for example `some:_selector` vs
      `some_:selector`)
    - the `sanitized-selector` is there mostly for debugging purposes to appear in stack traces, the selector would be
      unique even without it

---

## How to generate the registrar?

I see three ways of generating the registrar:
- Keep using custom linker steps.
- Use a pre-trimming MSBuild task that's based on Mono.Cecil (independent of ILLink).
- Use roslyn source generators.
  - the generated and pre-built code will have to be distributed with the libraries (the same way the bgen output is
    pre-built and distributed with libraries)

### Option A: Custom linker steps

We can modify the existing code that generates the managed static registrar to generate the trimmable variant. This could
be useful to validate the idea and iron out the design before we put any effort into a proper implementation.

PoC: https://github.com/xamarin/xamarin-macios/compare/main...simonrozsival:xamarin-macios:net8.0-poc-trimmable-msr

### Option B: Pre-trimming + post-trimming MSBuild tasks

We would modify the DLLs directly before the code is AOTed. Instead of ILLink and custom linker steps, we would create
an independent MSBuild task to perform the weaving. This would include generating the `__Registrar_Callback__` nested
classes with UCO endpoints, proxy classes for generic types, and generating any `[DynamicDependency]` or other attributes
to express all inverse dependencies. At the same time, we would generate the Objective-C code and any information about
native dependencies (frameworks, static libraries, other generated Objective-C source files).

It should only be necessary to pre-process any third-party libraries and the runtimepack DLLs once after restore and not
with every publish.

The post-trimming task would analyze the object file produced by ILC and collect all the Objective-C source files
based on the registrar callbacks that survived trimming and their dependencies. These source files will be compiled
and linked with the AOTed binary.

- **Pros**:
  - generated code isn't distributed with runtimepack/nupkg
    - it would be an invisible change for libraries authors - they won't need to rebuild their apps for .NET 9
  - when we generate IL directly, we have control over visibility of certain members and nested types if needed
  - we can reuse some code from the current codebase if we implement the pre-trimming task using Mono.Cecil
- **Cons**:
  - slower than source generators - all code is generated when publishing

### Option C: Source generator + post-trimming MSBuild task

The code for the UCO callbacks should be expressible in C#. The only problem that we might have to work around is the
absence of the "inverse dependency" attributes. It might be a good use case to drive the implementation of this feature.

The post-trimming task would be exactly the same as in Option B.

- **Pros**:
  - likely faster than an IL-weaving MSBuild task
    - code is generated while editing the source files
    - the code for the xamarin runtime and for all libraries (e.g, MAUI) would already be pre-built in managed DLLs
  - code generator can produce errors and warnings while editing the source files for better DX
  - source generators are more future-proof than Mono.Cecil
- **Cons**:
  - the generated code has to be distributed with runtimepack/nupkg
    - all libraries authors will have to publish their code specifically for .NET 9 and any future releases
      which change the registrar
  - generating Objective-C code in roslyn source generators is a _hack_ that isn't officially supported
    - source generators are _synchronous_ and doing many IO operations will slow them down, causing lagging in the editor
  - its's not yet clear how to correctly generate the `[DynamicDependency]` attributes and if the inverse dependency
    attributes could be added to .NET

### Notes

- I suggest implementing the change with the Mono.Cecil-based approach since it should be much faster to implement and
  overall less risky.
- The C# source generator is a more tempting option and probably more future-proof.
  - I'm worried it could take a lot longer to implement and there might have problems with third party libraries.
- This registrar relies on static linking and doesn't support the non-AOT scenario (using `xamarin_registrar_dlsym` to
  load function pointers to the registrar callbacks).
    - This is not a problem in the context of NativeAOT but it might mean that the registrar could not completely replace
      the existing MSR and static registrar for Mono-based (CoreCLR-based?) scenarios and there will still be need
      to keep the custom linker steps around to build those registrars.
- There are certainly edge-cases that aren't covered in this document but they should all be possible to implement
  using the same ideas that are outlined in this document.
  - This claim needs validation though. Are there some obvious cases which would be hard or obviously inefficient
    to implement this way?
- It is unclear what would be the file size and startup or runtime performance implications of these changes.
  - We will need to generate less managed code but on the other hand we'll generate more Objective-C code.
  - There will be more transitions between managed and native code. Does it matter too much in the context of NativeAOT
    though? Will it be worse than performing linear search in the lookup tables that we do today?

---

## Risks and Benefits

### Benefits

- It should be straightforward to modify the existing runtime code and custom linker steps to validate the proposed approach.
- We would be able to completely drop IL Linker from the build process when publishing with NativeAOT.
  - The new design will be trimmable using any IL linking tool.
- There is potential for file size and performance improvements, although this needs validating.
- It should be possible to generate the code of the registrar using roslyn source generator instead of IL weaving.

### Risks

- We still need the current setup for all the cases which are unsupported by this scenario (inner dev loop, support
  for existing codebases that rely on the static registrar). The maintenance cost might not decrease, but instead
  increase because now there are two very different scenarios we need to support. We would probably need to deprecate
  the static registrar completely for this change to make sense from maintenance perspective.
  - On the other hand, we won't be able to deprecate the current setup unless we introduce an alternative and support
    both of these scenarios until all customers are able to migrate to the new setup.
- There is some missing functionality in the IL Compiler that would be required to implement the fully-trimmable registrar.
- If we go the source-generator route:
  - it will have implications for library authors who will have to rebuild and re-release
    their codebases for the new version of .NET,
    - This is similar to how the code generated by bgen needs to be re-released. Is this really a problem?
  - implementation won't be trivial and we won't be able to reuse much of the code from the existing codebase.