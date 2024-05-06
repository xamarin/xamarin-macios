# Objective-C protocols

This document describes how we bind Objective-C protocols in C#, and in
particular improvements we've done in .NET 9.

## Can Objective-C protocols be modeled as C# interfaces?

Objective-C protocols are quite similar to C# interfaces, except when they're
not, and that makes binding them somewhat complicated.

### Optional/required members

Objective-C protocols can have both optional and required members. It's always
been possible to represent required members in a C# interface (any interface
member would be required), but optional members were not possible until C#
added support for default interface members in C# 8.

We represent optional members in two ways:

* As an extension method on the interface (useful when calling the optional member).
* As an IDE feature that would show any optional members from an interface by
  typing 'override ...' in the text editor (useful when implementing an optional member).

This has a few drawbacks:

* There are no extension properties, so optional properties would have to be
  bound as a pair of GetProperty/SetProperty methods.

* The IDE feature was obscure, few people knew about it, it broke on pretty
  much every major release of Visual Studio for Mac, and it was never
  implemented for Visual Studio on Windows. This made it quite hard to
  implement optional members in a managed class extending an Objective-C
  protocol, since developers would have to figure out the correct Export
  attribute as well as the signature (which is quite complicated for more
  complex signatures, especially if blocks are involved).

### Changing requiredness

It's entirely possible to change a member from being required to being optional
in Objective-C. Technically it's also a breaking change to do the opposite (make
an optional member required), but Apple does it all the time.

We've handled this by just not updating the binding until we're able to do
breaking changes (which happens very rarely).

### Static members

Objective-C protocols can have static members. C# didn't allow for static
members in interfaces until C# 11, so until recently there hasn't been any
good way to bind static protocol members on a protocol.

Our workaround is to manually inline every static member in all classes that
implemented a given protocol.

### Initializers

Objective-C protocols can have initializers (constructors). C# still doesn't
allow for constructors in interfaces.

In the past we haven't bound any protocol initializer at all, we've completely
ignored them.

## Binding in C#

### Initializers

Given the following API definition:

```cs
[Protocol]
public interface Protocol {
    [Abstract]
    [Export ("init")]
    IntPtr Constructor ();

    [Export ("initWithValue:")]
    IntPtr Constructor (IntPtr value);

    [BindAs ("Create")]
    [Export ("initWithPlanet:")]
    IntPtr Constructor ();
}
```

we're binding it like this:

```cs
[Protocol ("Protocol")]
public interface IProtocol : INativeObject {
    [Export ("init")]
    public static T CreateInstance<T> () where T: NSObject, IProtocol { /* default implementation */ }

    [Export ("initWithValue:")]
    public static T CreateInstance<T> () where T: NSObject, IProtocol { /* default implementation */ }

    [Export ("initWithPlanet:")]
    public static T Create<T> () where T: NSObject, IProtocol { /* default implementation */ }
}
```

In other words: we bind initializers as a static C# factory method that takes
a generic type argument specifying the type to instantiate.

Notes:

1. Constructors are currently not inlined in any implementing classes, like
   other members are. This is something we could look into if there's enough
   interest.
2. If a managed class implements a protocol with a constructor, the class has
   to implement the constructor manually using the `[Export]` attribute in
   order to conform to the protocol:

```cs
[Protocol]
interface IMyProtocol {
    [Export ("initWithValue:")]
    IntPtr Constructor (string value);
}
```

```cs
class MyClass : NSObject, IMyProtocol {
    public string Value { get; private set; }

    [Export ("initWithValue:")]
    public MyClass (string value)
    {
        this.Value = value;
    }
}
```
