# Managed static registrar

The managed static registrar is a variation of the static registrar where we
don't use features the NativeAOT compiler doesn't support (most notably
metadata tokens).

It also takes advantage of new features in C# and managed code since the
original static registrar code was written - in particular it tries to do as
much as possible in managed code instead of native code, as well as various
other performance improvements. The actual performance characteristics
compared to the original static registrar will vary between the specific
exported method signatures, but in general it's expected that method calls
from native code to managed code will be faster.

In order to make the managed static registrar easily testable and debuggable,
it's also implemented for the other runtimes as well (Mono and CoreCLR), even
when not using AOT in any form.

## Design

### Exported methods

For each method exported to Objective-C, the managed static registrar will
generate a managed method we'll call directly from native code, and which does
all the marshalling.

This method will have the `[UnmanagedCallersOnly]` attribute, so that it doesn't
need any additional marshalling from the managed runtime - which makes it
possible to obtain a native function pointer for it. It will also have a
native entry point, which means that for AOT we can just directly call it from
the generated Objective-C code.

Given the following method:

```csharp
class AppDelegate : NSObject, IUIApplicationDelegate {
    // this method is written by the app developer
    public override bool FinishedLaunching (UIApplication app, NSDictionary options)
    {
        // ...
    }
}
```

The managed static registrar will add the following method to the `AppDelegate` class:

```csharp
class AppDelegate {
    [UnmanagedCallersOnly (EntryPoint = "__registrar__uiapplicationdelegate_didFinishLaunching")]
    static byte __registrar__DidFinishLaunchingWithOptions (IntPtr handle, IntPtr selector, IntPtr p0, IntPtr p1)
    {
        var obj = Runtime.GetNSObject (handle);
        var p0Obj = (UIApplication) Runtime.GetNSObject (p0);
        var p1Obj = (NSDictionary) Runtime.GetNSObject (p1);
        var rv = obj.DidFinishLaunchingWithOptions (p0Obj, p1Obj);
        return rv ? (byte) 1 : (byte) 0;
    }
}
```

and the generated Objective-C code will look something like this:

```objective-c
extern BOOL __registrar__uiapplicationdelegate_init (AppDelegate self, SEL _cmd, UIApplication* p0, NSDictionary* p1);

@interface AppDelegate : NSObject<UIApplicationDelegate, UIApplicationDelegate> {
}
    -(BOOL) application:(UIApplication *)p0 didFinishLaunchingWithOptions:(NSDictionary *)p1;
@end
@implementation AppDelegate {
}
    -(BOOL) application:(UIApplication *)p0 didFinishLaunchingWithOptions:(NSDictionary *)p1
    {
        return __registrar__uiapplicationdelegate_didFinishLaunching (self, _cmd, p0, p1);
    }
@end
```

Note: the actual code is somewhat more complex in order to properly support
managed exceptions and a few other corner cases.

### Type mapping

The runtime needs to quickly and efficiently do lookups between an Objective-C
type and the corresponding managed type. In order to support this, the managed
static registrar will add lookup tables in each assembly. The managed static
registrar will create a numeric ID for each managed type, which is then
emitted into the generated Objective-C code, and which we can use to look up
the corresponding managed type. There is also a table in Objective-C that maps
between the numeric ID and the corresponding Objective-C type.

We also need to be able to find the wrapper type for interfaces representing
Objective-C protocols - this is accomplished by generating a table in
unmanaged code that maps the ID for the interface to the ID for the wrapper
type.

This is all supported by the `ObjCRuntime.IManagedRegistrar.LookupTypeId` and
`ObjCRuntime.IManagedRegistrar.LookupType` methods.

Note that in many ways the type ID is similar to the metadata token for a type
(and is sometimes referred to as such in the code, especially code that
already existed before the managed static registrar was implemented).

### Method mapping

When AOT-compiling code, the generated Objective-C code can call the entry
point for the UnmanagedCallersOnly trampoline directly (the AOT compiler will
emit a native symbol with the name of the entry point).

However, when not AOT-compiling code, the generated Objective-C code needs to
find the function pointer for the UnmanagedCallersOnly methods. This is
implemented using another lookup table in managed code.

For technical reasons, this is implemented using multiple levels of functions if
there is a significant number of UnmanagedCallersOnly methods. As it seems
that the JIT will compile the target for every function pointer in a method,
even if the function pointer isn't loaded at runtime. This means that if
there are 1.000 methods in the lookup table and if the lookup was
implemented in a single function, the JIT will have to compile all
the 1.000 methods the first time the lookup method is called, even 
if the lookup method will eventually just find a single callback.

This might be easier to describe with some code.

Instead of this:

```csharp
class __Registrar_Callbacks__ {
    IntPtr LookupUnmanagedFunction (int id)
    {
        switch (id) {
        case 0: return (IntPtr) (delegate* unmanaged<void>) &Callback0;
        case 1: return (IntPtr) (delegate* unmanaged<void>) &Callback1;
        ...
        case 999: return (IntPtr) (delegate* unmanaged<void>) &Callback999;
        }
        return (IntPtr) -1);
    }
}
```

we do this instead:

```csharp
class __Registrar_Callbacks__ {
    IntPtr LookupUnmanagedFunction (int id)
    {
        if (id < 100)
            return LookupUnmanagedFunction_0 (id);
        if (id < 200)
            return LookupUnmanagedFunction_1 (id);
        ...
        if (id < 1000)
            LookupUnmanagedFunction_9 (id);
        return (IntPtr) -1;
    }

    IntPtr LookupUnmanagedFunction_0 (int id)
    {
        switch (id) {
        case 0: return (IntPtr) (delegate* unmanaged<void>) &Callback0;
        case 1: return (IntPtr) (delegate* unmanaged<void>) &Callback1;
        /// ...
        case 9: return (IntPtr) (delegate* unmanaged<void>) &Callback9;
        }
        return (IntPtr) -1;
    }


    IntPtr LookupUnmanagedFunction_1 (int id)
    {
        switch (id) {
        case 10: return (IntPtr) (delegate* unmanaged<void>) &Callback10;
        case 11: return (IntPtr) (delegate* unmanaged<void>) &Callback11;
        /// ...
        case 19: return (IntPtr) (delegate* unmanaged<void>) &Callback19;
        }
        return (IntPtr) -1;
    }
}
```


### Generation

All the generated IL is done in two separate custom linker steps. The first
one, ManagedRegistrarStep, will generate the UnmanagedCallersOnly trampolines
for every method exported to Objective-C. This happens before the trimmer has
done any work (i.e. before marking), because the generated code will cause
more code to be marked (and this way we don't have to replicate what the
trimmer does when it traverses IL and metadata to figure out what else to
mark).

The trimmer will then trim away any UnmanagedCallersOnly trampoline that's no
longer needed because the target method has been trimmed away.

On the other hand, the lookup tables for the type mapping are generated after
trimming, because we only want to add types that aren't trimmed away to the
lookup tables (otherwise we'd end up causing all those types to be kept).

## Interpreter / JIT

When not using the AOT compiler, we need to look up the native entry points
for UnmanagedCallersOnly methods at runtime. In order to support this, the
managed static registrar will add lookup tables in each assembly. The managed
static registrar will create a numeric ID for each UnmanagedCallersOnly
method, which is then emitted into the generated Objective-C code, and which
we can use to look up the managed UnmanagedCallersOnly method at runtime (in
the lookup table).

This is the `ObjCRuntime.IManagedRegistrar.LookupUnmanagedFunction` method.

## Performance

Preliminary testing shows the following:

### macOS

Calling an exported managed method from Objective-C is 3-6x faster for simple method signatures.

### Mac Catalyst

Calling an exported managed method from Objective-C is 30-50% faster for simple method signatures.

## References

* https://github.com/dotnet/runtime/issues/80912
