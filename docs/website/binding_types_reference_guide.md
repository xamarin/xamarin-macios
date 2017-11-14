---
id: C6618E9D-07FA-4C84-D014-10DAC989E48D
title: Binding Types Reference Guide
dateupdated: 2017-06-26
---

[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)
[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)
[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)

This document describes the list of attributes that you can use to annotate
your API contract files to drive the binding and the code generated

Xamarin.iOS and Xamarin.Mac API contracts are written in C# mostly as interface
definitions that define the way that Objective-C code is surfaced to C#. The
process involves a mix of interface declarations plus some basic type
definitions that the API contract might require. For an introduction to binding
types, see our companion guide [Binding Objective-C Libraries](/guides/cross-platform/macios/binding/objective-c-libraries/).


# Type Definitions

Syntax:

```
[BaseType (typeof (BTYPE))
interface MyType [: Protocol1, Protocol2] {
     IntPtr Constructor (string foo);
}
```

Every interface in your contract definition that has the `[BaseType]` attribute
that declares the base type for the generated object. In the above declaration a
`MyType` class C# type will be generated that binds to an Objective-C type called
**MyType**.

If you specify any types after the typename (in the sample above `Protocol1`
and `Protocol2`) using the interface inheritance syntax the contents of those
interfaces will be inlined as if they had been part of the contract for `MyType`.
The way that Xamarin.iOS surfaces that a type adopts a protocol is by inlining all
of the methods and properties that were declared in the protocol into the type
itself.

The following shows how the Objective-C declaration for `UITextField` would be
defined in a Xamarin.iOS contract:

```
@interface UITextField : UIControl <UITextInput> {

}
```

Would be written like this as a C# API contract:

```
[BaseType (typeof (UIControl))]
interface UITextField : UITextInput {
}
```

You can control many other aspects of the code generation by applying other
attributes to the interface as well as configuring the BaseType attribute.

 <a name="Generating_Events" />


## Generating Events

One feature of the Xamarin.iOS and Xamarin.Mac API design is that we map
Objective-C delegate classes as C# events and callbacks. Users can choose in a
per-instance basis whether they want to adopt the Objective-C programming
pattern, by assigning to properties like **Delegate** an instance of a class that
implements the various methods that the Objective-C runtime would call, or by
choosing the C#-style events and properties.

Let us see one example of how to use the Objective-C model:

```
bool MakeDecision ()
{
    return true;
}

void Setup ()
{
     var scrollView = new UIScrollView (myRect);
     scrollView.Delegate = new MyScrollViewDelegate ();
     ...
}

class MyScrollViewDelegate : UIScrollViewDelegate {
    public override void Scrolled (UIScrollView scrollView)
    {
        Console.WriteLine ("Scrolled");
    }

    public override bool ShouldScrollToTop (UIScrollView scrollView)
    {
        return MakeDecision ();
    }
}
```

In the above example, you can see that we have chosen to overwrite two
methods, one a notification that a scrolling event has taken place, and the
second that is a callback that should return a boolean value instructing the
scrollView whether it should scroll to the top or not.

The C# model allows the user of your library to listen to notifications using
the C# event syntax or the property syntax to hook up callbacks that are
expected to return values.

This is how the C# code for the same feature looks like using lambdas:

```
void Setup ()
{
    var scrollview = new UIScrollView (myRect);
    // Event connection, use += and multiple events can be connected
    scrollView.Scrolled += (sender, eventArgs) { Console.WriteLine ("Scrolled"); }

    // Property connection, use = only a single callback can be used
    scrollView.ShouldScrollToTop = (sv) => MakeDecision ();
}
```

Since events do not return values (they have a void return type) you can
connect multiple copies. The `ShouldScrollToTop` is not an event, it is instead a
property with the type `UIScrollViewCondition` which has this
signature:

```
public delegate bool UIScrollViewCondition (UIScrollView scrollView);
```

It returns a bool value, in this case the lambda syntax allows us to just
return the value from the `MakeDecision` function.

The binding generator supports generating events and properties that link a
class like `UIScrollView` with its `UIScrollViewDelegate` (well call these the Model
class), this is done by annotating your `BaseType` definition with the `Events` and
`Delegates` parameters (described below). In addition to annotating the `BaseType`
with those parameters it is necessary to inform the generator of a few more
components.

For events that take more than one parameter (in Objective-C the convention
is that the first parameter in a delegate class is the instance of the sender
object) you must provide the name that you would like for the generated
EventArgs class to be. This is done with the `EventArgs` attribute on the method
declaration in your Model class. For example:

```
[BaseType (typeof (UINavigationControllerDelegate))]
[Model][Protocol]
public interface UIImagePickerControllerDelegate {
    [Export ("imagePickerController:didFinishPickingImage:editingInfo:"), EventArgs ("UIImagePickerImagePicked")]
    void FinishedPickingImage (UIImagePickerController picker, UIImage image, NSDictionary editingInfo);
}
```

The above declaration will generate a `UIImagePickerImagePickedEventArgs` class that derives from `EventArgs` and packs both parameters, the `UIImage` and
the `NSDictionary`. The generator produces this:

```
public partial class UIImagePickerImagePickedEventArgs : EventArgs {
    public UIImagePickerImagePickedEventArgs (UIImage image, NSDictionary editingInfo);
    public UIImage Image { get; set; }
    public NSDictionary EditingInfo { get; set; }
}
```

It then exposes the following in the UIImagePickerController class:

```
public event EventHandler<UIImagePickerImagePickedEventArgs> FinishedPickingImage { add; remove; }
```

Model methods that return a value are bound differently. Those require both a
name for the generated C# delegate (the signature for the method) and also a
default value to return in case the user does not provide an implementation
himself. For example, the `ShouldScrollToTop` definition is this:

```
[BaseType (typeof (NSObject))]
[Model][Protocol]
public interface UIScrollViewDelegate {
    [Export ("scrollViewShouldScrollToTop:"), DelegateName ("UIScrollViewCondition"), DefaultValue ("true")]
    bool ShouldScrollToTop (UIScrollView scrollView);
}
```

The above will create a `UIScrollViewCondition` delegate with the signature
that was shown above, and if the user does not provide an implementation, the
return value will be true.

In addition to the `DefaultValue` attribute, you can also use
the `DefaultValueFromArgument` that
directs the generator to return the value of the specified
parameter in the call or the `NoDefaultValue` parameter that
instructs the generator that there is no default value.

 <a name="BaseTypeAttribute" />


## BaseTypeAttribute

Syntax:

```
public class BaseTypeAttribute : Attribute {
        public BaseTypeAttribute (Type t);

        // Properties
        public Type BaseType { get; set; }
        public string Name { get; set; }
        public Type [] Events { get; set; }
        public string [] Delegates { get; set; }
        public string KeepRefUntil { get; set; }
}
```

### BaseType.Name

You use the `Name` property to control the name that this type will bind to in
the Objective-C world. This is typically used to give the C# type a name that is
compliant with the .NET Framework Design Guidelines, but which maps to a name in
Objective-C that does not follow that convention.

Example, in the following case we map the Objective-C `NSURLConnection` type to
`NSUrlConnection`, as the .NET Framework Design Guidelines use "Url" instead of
"URL":

```
[BaseType (typeof (NSObject), Name="NSURLConnection")]
interface NSUrlConnection {
}
```

The specified name is specified is used as the value for the generated
`[Register]` attribute in the binding. If `Name` is not specified, the type's short
name is used as the value for the `Register` attribute in the generated
output.


### BaseType.Events and BaseType.Delegates

These properties are used to drive the generation of C#-style Events in the
generated classes. They are used to link a given class with its Objective-C
delegate class. You will encounter many cases where a class uses a delegate
class to send notifications and events. For example a `BarcodeScanner` would have
a companion `BardodeScannerDelegate` class. The `BarcodeScanner` class would
typically have a "delegate" property that you would assign an instance of
`BarcodeScannerDelegate` to, while this works, you might want to expose to your
users a C#-like style event interface, and in those cases you would use the
`Events` and `Delegates` properties of the `BaseType` attribute.

These properties are always set together and must have the same number of
elements and be kept in sync. The `Delegates` array contains one string for each
weakly-typed delegate that you want to wrap, and the Events array contains one
type for each type that you want to associate with it.

```
[BaseType (typeof (NSObject),
           Delegates=new string [] { "WeakDelegate" },
           Events=new Type [] {typeof(UIAccelerometerDelegate)})]
public interface UIAccelerometer {
}

[BaseType (typeof (NSObject))]
[Model][Protocol]
public interface UIAccelerometerDelegate {
}
```

 <a name="BaseType.KeepRefUntil" />


### BaseType.KeepRefUntil

If you apply this attribute when new instances of this class are created, the
instance of that object will be kept around until the method referenced by the
`KeepRefUntil` has been invoked. This is useful to improve the usability of your
APIs, when you do not want your user to keep a reference to an object around to
use your code. The value of this property is the name of a method in the
`Delegate` class, so you must use this in combination with the Events and
`Delegates` properties as well.

The following example show how this is used by `UIActionSheet` in
Xamarin.iOS:

```
[BaseType (typeof (NSObject), KeepRefUntil="Dismissed")]
[BaseType (typeof (UIView),
           KeepRefUntil="Dismissed",
           Delegates=new string [] { "WeakDelegate" },
           Events=new Type [] {typeof(UIActionSheetDelegate)})]
public interface UIActionSheet {
}

[BaseType (typeof (NSObject))]
[Model][Protocol]
public interface UIActionSheetDelegate {
    [Export ("actionSheet:didDismissWithButtonIndex:"), EventArgs ("UIButton")]
    void Dismissed (UIActionSheet actionSheet, nint buttonIndex);
}
```

 <a name="DisableDefaultCtorAttribute" />


## DisableDefaultCtorAttribute

When this attribute is applied to the interface definition it will prevent
the generator from producing the default constructor.

Use this attribute when you need the object to be initialized with one of the
other constructors in the class.

 <a name="PrivateDefaultCtorAttribute" />


## PrivateDefaultCtorAttribute

When this attribute is applied to the interface definition it will flag the
default constructor as private. This means that you can still instantiate object
of this class internally from your extension file, but it just wont be
accessible to users of your class.

<a name="CategoryAttribute" />
## CategoryAttribute

Use this attribute on a type definition to bind Objective-C
categories and to expose those as C# extension methods to mirror the
way Objective-C exposes the functionality.

Categories are an Objective-C mechanism used to extend the set of
methods and properties available in a class.   In practice, they are
used to either extend the functionality of a base class (for example
`NSObject`) when a specific framework is linked in (for example `UIKit`),
making their methods available, but only if the new framework is
linked in.   In some other cases, they are used to organize features
in a class by functionality.   They are similar in spirit to C#
extension methods.

This is what a category would look like in Objective-C:

```
@interface UIView (MyUIViewExtension)
-(void) makeBackgroundRed;
@end
```

The above example if found on a library would extend instances of
`UIView` with the method `makeBackgroundRed`.

To bind those, you can use the `[Category]` attribute on
an interface definition.   When using the `Category` attribute, the
meaning of the `[BaseType]` attribute changes from being
used to specify the base class to extend, to be the type to extend.

The following shows how the `UIView` extensions are bound and turned
into C# extension methods:

```
[BaseType (typeof (UIView))]
[Category]
interface MyUIViewExtension {
    [Export ("makeBackgroundRed")]
    void MakeBackgroundRed ();
}
```

The above will create a `MyUIViewExtension` a class
that contains the `MakeBackgroundRed` extension method.   This means
that you can now call "MakeBackgroundRed" on any `UIView` subclass,
giving you the same functionality you would get on Objective-C.

In some cases you will find **static** members inside categories like in the following example:

```objc
@interface FooObject (MyFooObjectExtension)
+ (BOOL)boolMethod:(NSRange *)range;
@end
```

This will lead to an **incorrect** Category C# interface definition:

```csharp
[Category]
[BaseType (typeof (FooObject))]
interface FooObject_Extensions {

	// Incorrect Interface definition
	[Static]
	[Export ("boolMethod:")]
	bool BoolMethod (NSRange range);
}
```

This is incorrect because in order to use the `BoolMethod` extension you need an instance of `FooObject` but you are binding an ObjC **static** extension, this is a side effect due to the fact of how C# extension methods are implemented.

The only way to use the above definitions is by the following ugly code:

```csharp
(null as FooObject).BoolMethod (range);
```

The recommendation to avoid this is to inline the `BoolMethod` definition inside the `FooObject` interface definition itself, this will allow you to call this extension like it is intended `FooObject.BoolMethod (range)`.

```csharp
[BaseType (typeof (NSObject))]
interface FooObject {

	[Static]
	[Export ("boolMethod:")]
	bool BoolMethod (NSRange range);
}
```

We will issue a warning (BI1117) whenever we find a `[Static]` member inside a `[Category]` definition. If you really want to have `[Static]` members inside your `[Category]` definitions you can silence the warning by using `[Category (allowStaticMembers: true)]` or by decorating either your member or `[Category]` interface definition with `[Internal]`.

<a name="StaticAttribute" />
## StaticAttribute

When this attribute is applied to a class it will just generate a static
class, one that does not derive from `NSObject` so the `[BaseType]` attribute is
ignored. Static classes are used to host C public variables that you want to
expose.

For example:

```
[Static]
interface CBAdvertisement {
    [Field ("CBAdvertisementDataServiceUUIDsKey")]
    NSString DataServiceUUIDsKey { get; }
```

Will generate a C# class with the following API:

```
public partial class CBAdvertisement  {
    public static NSString DataServiceUUIDsKey { get; }
}
```

<a name="Model_Definitions" />
# Model Definitions

###Protocol definitions/Model

Models are typically used by protocol implementation.
They differ in that the runtime will only register with
Objective-C the methods that actually have been  overwritten.
Otherwise, the method will not be registered.

This in general means that when you subclass a class that
has been flagged with the `ModelAttribute`, you should not call
the base method.   Calling that method will throw an
exception, you are supposed to implement the entire behavior
on your subclass for any methods you override.

<a name="AbstractAttribute" />
## AbstractAttribute

By default, members that are part of a protocol are not mandatory. This
allows users to create a subclass of the `Model` object by merely deriving from
the class in C# and overriding only the methods they care about. Sometimes the
Objective-C contract requires that the user provides an implementation for this
method (those are flagged with the @required directive in Objective-C). In those
cases, you should flag those methods with the `Abstract` attribute.

The `Abstract` attribute can be applied to either methods or properties and
causes the generator to flag the generated member as "abstract" and the class to
be an abstract class.

The following is taken from Xamarin.iOS:

```
[BaseType (typeof (NSObject))]
[Model][Protocol]
public interface UITableViewDataSource {
    [Export ("tableView:numberOfRowsInSection:")]
    [Abstract]
    nint RowsInSection (UITableView tableView, nint section);
}
```

 <a name="DefaultValueAttribute" />


## DefaultValueAttribute

Specifies the default value to be returned by a model method if the user does
not provide a method for this particular method in the Model object

Syntax:

```
public class DefaultValueAttribute : Attribute {
        public DefaultValueAttribute (object o);
        public object Default { get; set; }
}
```

For example, in the following imaginary delegate class for a `Camera` class, we
provide a `ShouldUploadToServer` which would be exposed as a property on the
`Camera` class. If the user of the `Camera` class does not explicitly set a the
value to a lambda that can respond true or false, the default value return in
this case would be false, the value that we specified in the `DefaultValue`
attribute:

```
[BaseType (typeof (NSObject))]
[Model][Protocol]
interface CameraDelegate {
    [Export ("camera:shouldPromptForAction:"), DefaultValue (false)]
    bool ShouldUploadToServer (Camera camera, CameraAction action);
}
```

If the user sets a handler in the imaginary class, then this value would be
ignored:

```
var camera = new Camera ();
camera.ShouldUploadToServer = (camera, action) => return SomeDecision ();
```



See
also: [NoDefaultValueAttribute](#NoDefaultValueAttribute), [DefaultValueFromArgumentAttribute](#DefaultValueFromArgumentAttribute).


## DefaultValueFromArgumentAttribute

Syntax:

```
public class DefaultValueFromArgumentAttribute : Attribute {
    public DefaultValueFromArgumentAttribute (string argument);
    public string Argument { get; }
}
```

This attribute when provided on a method that returns a value on a model
class will instruct the generator to return the value of the specified parameter
if the user did not provide his own method or lambda.

Example:

```
[BaseType (typeof (NSObject))]
[Model][Protocol]
public interface NSAnimationDelegate {
    [Export ("animation:valueForProgress:"), DelegateName ("NSAnimationProgress"), DefaultValueFromArgumentAttribute ("progress")]
    float ComputeAnimationCurve (NSAnimation animation, nfloat progress);
}
```

In the above case if the user of the `NSAnimation` class chose to use any of
the C# events/properties, and did not set `NSAnimation.ComputeAnimationCurve` to a
method or lambda, the return value would be the value passed in the progress
parameter.



See also: [NoDefaultValueAttribute](#NoDefaultValueAttribute), [DefaultValueAttribute](#DefaultValueAttribute)

## IgnoredInDelegateAttribute

Sometimes it makes sense not to expose an event or delegate property from a
Model class into the host class so adding this attribute will instruct the
generator to avoid the generation of any method decorated with it.

```
[BaseType (typeof (UINavigationControllerDelegate))]
[Model][Protocol]
public interface UIImagePickerControllerDelegate {
    [Export ("imagePickerController:didFinishPickingImage:editingInfo:"), EventArgs ("UIImagePickerImagePicked")]
    void FinishedPickingImage (UIImagePickerController picker, UIImage image, NSDictionary editingInfo);

    [Export ("imagePickerController:didFinishPickingImage:"), IgnoredInDelegate)] // No event generated for this method
    void FinishedPickingImage (UIImagePickerController picker, UIImage image);
}
```


## DelegateNameAttribute

This attribute is used in Model methods that return values to set the name of
the delegate signature to use.

Example:

```
[BaseType (typeof (NSObject))]
[Model][Protocol]
public interface NSAnimationDelegate {
    [Export ("animation:valueForProgress:"), DelegateName ("NSAnimationProgress"), DefaultValueFromArgumentAttribute ("progress")]
    float ComputeAnimationCurve (NSAnimation animation, float progress);
}
```

With the above definition, the generator will produce the following public
declaration:

```
public delegate float NSAnimationProgress (MonoMac.AppKit.NSAnimation animation, float progress);
```

## DelegateApiNameAttribute

This attribute is used to allow the generator to change the name of the property generated
in the host class. Sometimes it is useful when the name of the FooDelegate class method
makes sense for the Delegate class, but would look odd in the host class as a property.

Also this is really useful (and needed) when you have two or more overload methods that makes
sense to keep them named as is in the FooDelegate class but you want to expose them in the host
class with a better given name.

Example:

```
[BaseType (typeof (NSObject))]
[Model][Protocol]
public interface NSAnimationDelegate {
    [Export ("animation:valueForProgress:"), DelegateApiName ("ComputeAnimationCurve"), DelegateName ("Func<NSAnimation, float, float>"), DefaultValueFromArgument ("progress")]
    float GetValueForProgress (NSAnimation animation, float progress);
}
```

With the above definition, the generator will produce the following public
declaration in the host class:

```
public Func<NSAnimation, float, float> ComputeAnimationCurve { get; set; }
```


## EventArgsAttribute

For events that take more than one parameter (in Objective-C the convention
is that the first parameter in a delegate class is the instance of the sender
object) you must provide the name that you would like for the generated
EventArgs class to be. This is done with the `EventArgs` attribute on the method
declaration in your `Model` class.

For example:

```
[BaseType (typeof (UINavigationControllerDelegate))]
[Model][Protocol]
public interface UIImagePickerControllerDelegate {
    [Export ("imagePickerController:didFinishPickingImage:editingInfo:"), EventArgs ("UIImagePickerImagePicked")]
    void FinishedPickingImage (UIImagePickerController picker, UIImage image, NSDictionary editingInfo);
}
```

The above declaration will generate a `UIImagePickerImagePickedEventArgs` class that derives from EventArgs
and packs both parameters, the `UIImage` and the `NSDictionary`. The generator produces this:

```
public partial class UIImagePickerImagePickedEventArgs : EventArgs {
    public UIImagePickerImagePickedEventArgs (UIImage image, NSDictionary editingInfo);
    public UIImage Image { get; set; }
    public NSDictionary EditingInfo { get; set; }
}
```

It then exposes the following in the UIImagePickerController class:

```
public event EventHandler<UIImagePickerImagePickedEventArgs> FinishedPickingImage { add; remove; }
```

<a name="EventNameAttribute" />
## EventNameAttribute

This attribute is used to allow the generator to change the name of an event
or property generated in the class. Sometimes it is useful when the name of the
`Model` class method makes sense for the model class, but would look odd in the
originating class as an event or property.

For example, the `UIWebView` uses the following bit from the
`UIWebViewDelegate`:

```
[Export ("webViewDidFinishLoad:"), EventArgs ("UIWebView"), EventName ("LoadFinished")]
void LoadingFinished (UIWebView webView);
```

The above exposes `LoadingFinished` as the method in the `UIWebViewDelegate`, but
`LoadFinished` as the event to hook up to in a `UIWebView`:

```
var webView = new UIWebView (...);
webView.LoadFinished += delegate { Console.WriteLine ("done!"); }
```

 <a name="ModelAttribute" />


## ModelAttribute

When you apply the `Model` attribute to a type definition in your contract API,
the runtime will generate special code that will only surface invocations to
methods in the class if the user has overwritten a method in the class. This
attribute is typically applied to all APIs that wrap an Objective-C delegate
class.

 <a name="NoDefaultValueAttribute"></a>


## NoDefaultValueAttribute



Specifies that the method on the model does not provide a
default return value.

This works with the Objective-C runtime by responding
"false" to the Objective-C runtime request to determine if the
specified selector is implemented in this class.

```
[BaseType (typeof (NSObject))]
[Model][Protocol]
interface CameraDelegate {
    [Export ("shouldDisplayPopup"), NoDefaultValue]
    bool ShouldUploadToServer ();
}
```

See also: [DefaultValueAttribute](#DefaultValueAttribute) and
[DefaultValueAttribute](#DefaultValueAttribute).


# Protocols

The Objective-C protocol concept does not really exist in C#. Protocols are
similar to C# interfaces but they differ in that not all of the methods and
properties declared in a protocol must be implemented by the class that adopts
it. Instead some of the methods and properties are optional.

Some protocols are generally used as Model classes, those should be bound
using the Model attribute.

```
[BaseType (typeof (NSObject))]
[Model, Protocol]
interface MyProtocol {
    // Use [Abstract] when the method is defined in the @required section
    // of the protocol definition in Objective-C
    [Abstract]
    [Export ("say:")]
    void Say (string msg);

    [Export ("listen")]
    void Listen ();
}
```

Starting with MonoTouch 7.0 a new and improved protocol
binding functionality has been incorporated.  Any definition
that contains the `[Protocol]` attribute will actually generate
three supporting classes that vastly improve the way that you
consume protocols:

```
// Full method implementation, contains all methods
class MyProtocol : IMyProtocol {
    public void Say (string msg);
    public void Listen (string msg);
}

// Interface that contains only the required methods
interface IMyProtocol: INativeObject, IDisposable {
    [Export ("say:")]
    void Say (string msg);
}

// Extension methods
static class IMyProtocol_Extensions {
    public static void Optional (this IMyProtocol this, string msg);
    }
}
```

The **class implementation** provides a complete abstract class that you can override individual methods of and get full type safety. But due to C# not supporting multiple inheritance, there are scenarios where you might require a different base class, but still want to implement an interface.

This is where the generated **interface definition** comes in.  It is an interface that has all the required methods from the protocol.  This allows developers that want to implement your protocol to merely implement the interface.  The runtime will automatically register the type as adopting the protocol.

Notice that the interface only lists the required methods
and does expose the optional methods.   This means that
classes that adopt the protocol will get full type checking
for the required methods, but will have to resort to weak
typing (manually using Export attributes and matching the
signature) for the optional protocol methods.

To make it convenient to consume an API that uses
protocols, the binding tool also will produce an extensions
method class that exposes all of the optional methods.   This
means that as long as you are consuming an API, you will be
able to treat protocols as having all the methods.

If you want to use the protocol definitions in your API,
you will need to write skeleton empty interfaces in your API
definition.   If you want to use the MyProtocol in an API, you
would need to do this:

```
[BaseType (typeof (NSObject))]
[Model, Protocol]
interface MyProtocol {
    // Use [Abstract] when the method is defined in the @required section
    // of the protocol definition in Objective-C
    [Abstract]
    [Export ("say:")]
    void Say (string msg);

    [Export ("listen")]
    void Listen ();
}

interface IMyProtocol {}

[BaseType (typeof(NSObject))]
interface MyTool {
    [Export ("getProtocol")]
    IMyProtocol GetProtocol ();
}
```

The above is needed because at binding time the `IMyProtocol`
would not exist, that is why you need to provide an empty
interface.

## Adopting Protocol Generated Interfaces

Whenever you implement one of the interfaces generated for
the protocols, like this:

```
class MyDelegate : NSObject, IUITableViewDelegate {
    nint IUITableViewDelegate.GetRowHeight (nint row) {
        return 1;
    }
}
```

The implementation for the interface methods automatically gets
exported with the proper name, so it is equivalent to this:

```
class MyDelegate : NSObject, IUITableViewDelegate {
    [Export ("getRowHeight:")]
    nint IUITableViewDelegate.GetRowHeight (nint row) {
        return 1;
    }
}
```

It does not matter if the interface is implemented
implicitly or explicitly.

## Protocol Inlining

While you bind existing Objective-C types that have been declared as adopting
a protocol, you will want to inline the protocol directly. To do this, merely
declare your protocol as an interface without any `[BaseType]` attribute and list
the protocol in the list of base interfaces for your interface.

Example:

```
interface SpeakProtocol {
    [Export ("say:")]
    void Say (string msg);
}

[BaseType (typeof (NSObject))]
interface Robot : SpeakProtocol {
    [Export ("awake")]
    bool Awake { get; set; }
}
```

 <a name="Member_Definitions" />


# Member Definitions

The attributes in this section are applied to individual members of a type:
properties and method declarations.

 <a name="AlignAttribute" />


## AlignAttribute

Used to specify the alignment value for property return types. Certain
properties take pointers to addresses that must be aligned at certain boundaries
(in Xamarin.iOS this happens for example with some `GLKBaseEffect` properties that
must be 16-byte aligned). You can use this property to decorate the getter, and
use the alignment value. This is typically used with the `OpenTK.Vector4` and
`OpenTK.Matrix4` types when integrated with Objective-C APIs.

Example:

```
public interface GLKBaseEffect {
    [Export ("constantColor")]
    Vector4 ConstantColor { [Align (16)] get; set;  }
}
```

 <a name="AppearanceAttribute" />


## AppearanceAttribute

The `Appearance` attribute is limited to iOS5 where the Appearance manager was
introduced.

The `Appearance` attribute can be applied to any method or property that
participate in the `UIAppearance` framework. When this attribute is applied to a
method or property in a class, it will direct the binding generator to create a
strongly-typed appearance class that is used to style all the instances of this
class, or the instances that match certain criteria.

Example:

```
public interface UIToolbar {
    [Since (5,0)]
    [Export ("setBackgroundImage:forToolbarPosition:barMetrics:")]
    [Appearance]
    void SetBackgroundImage (UIImage backgroundImage, UIToolbarPosition position, UIBarMetrics barMetrics);

    [Since (5,0)]
    [Export ("backgroundImageForToolbarPosition:barMetrics:")]
    [Appearance]
    UIImage GetBackgroundImage (UIToolbarPosition position, UIBarMetrics barMetrics);
}
```

The above would generate the following code in UIToolbar:

```
public partial class UIToolbar {
    public partial class UIToolbarAppearance : UIView.UIViewAppearance {
        public virtual void SetBackgroundImage (UIImage backgroundImage, UIToolbarPosition position, UIBarMetrics barMetrics);
        public virtual UIImage GetBackgroundImage (UIToolbarPosition position, UIBarMetrics barMetrics)
    }
    public static new UIToolbarAppearance Appearance { get; }
    public static new UIToolbarAppearance AppearanceWhenContainedIn (params Type [] containers);
}
```


## AutoReleaseAttribute (Xamarin.iOS 5.4)

Use the `AutoReleaseAttribute` on methods and properties to wrap the method
invocation to the method in an `NSAutoReleasePool`.

In Objective-C there are some methods that return values that are added to
the default `NSAutoReleasePool`. By default, these would go to your thread
`NSAutoReleasePool`, but since Xamarin.iOS also keeps a reference to your objects as
long as the managed object lives, you might not want to keep an extra reference
in the `NSAutoReleasePool` which will only get drained until your thread returns
control to the next thread, or you go back to the main loop.

This attribute is applied for example on heavy properties (for example
`UIImage.FromFile`) that returns objects that have been added to the default
`NSAutoReleasePool`. Without this attribute, the images would be retained as long
as your thread did not return control to the main loop. Uf your thread was some
sort of background downloader that is always alive and waiting for work, the
images would never be released.


## ForcedTypeAttribute

The `ForcedTypeAttribute` is used to enforce the creation of a managed type even
if the returned unmanaged object does not match the type described in the binding
definition.

This is useful when the type described in a header does not match the returned type
of the native method, for example take the following Objective-C definition from `NSURLSession`:

`- (NSURLSessionDownloadTask *)downloadTaskWithRequest:(NSURLRequest *)request`

It clearly states that it will return an `NSURLSessionDownloadTask` instance, but yet it
**returns** a `NSURLSessionTask`, which is a superclass and thus not convertible to
`NSURLSessionDownloadTask`. Since we are in a type-safe context an `InvalidCastException`
will happen.

In order to comply with the header description and avoid the `InvalidCastException`, the
`ForcedTypeAttribute` is used.

```
[BaseType (typeof (NSObject), Name="NSURLSession")]
interface NSUrlSession {

	[Export ("downloadTaskWithRequest:")]
	[return: ForcedType]
	NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request);
}
```

The `ForcedTypeAttribute` also accepts a boolean value named `Owns` that is `false`
by default `[ForcedType (owns: true)]`. The owns parameter is used to follow
the [Ownership Policy](https://developer.apple.com/library/content/documentation/CoreFoundation/Conceptual/CFMemoryMgmt/Concepts/Ownership.html)
for **Core Foundation** objects.

The `ForcedTypeAttribute` is only valid on `parameters`, `properties` and `return value`.



## BindAsAttribute

The `BindAsAttribute` allows binding `NSNumber`, `NSValue` and `NSString`(enums) into more accurate C# types. The attribute can be used to create better, more accurate, .NET API over the native API.

You can decorate methods (on return value), parameters and properties with `BindAs`. The only restriction is that your member **must not** be inside a `[Protocol]` or `[Model]` interface.

For example:

```
[return: BindAs (typeof (bool?))]
[Export ("shouldDrawAt:")]
NSNumber ShouldDraw ([BindAs (typeof (CGRect))] NSValue rect);
```

Would output:

```
[Export ("shouldDrawAt:")]
bool? ShouldDraw (CGRect rect) { ... }
```

Internally we will do the `bool?` <-> `NSNumber` and `CGRect` <-> `NSValue` conversions.

The current supported encapsulation types are:

* `NSValue`
* `NSNumber`
* `NSString`

### NSValue

The following C# data types are supported to be encapsulated from/into `NSValue`:

* CGAffineTransform
* NSRange
* CGVector
* SCNMatrix4
* CLLocationCoordinate2D
* SCNVector3
* SCNVector4
* CGPoint / PointF
* CGRect / RectangleF
* CGSize / SizeF
* UIEdgeInsets
* UIOffset
* MKCoordinateSpan
* CMTimeRange
* CMTime
* CMTimeMapping
* CATransform3D

### NSNumber

The following C# data types are supported to be encapsulated from/into `NSNumber`:

* bool
* byte
* double
* float
* short
* int
* long
* sbyte
* ushort
* uint
* ulong
* nfloat
* nint
* nuint
* Enums

### NSString

`[BindAs]` works in conjuntion with [enums backed by a NSString constant](#enum-attributes) so you can create better .NET API, for example:

```
[BindAs (typeof (CAScroll))]
[Export ("supportedScrollMode")]
NSString SupportedScrollMode { get; set; }
```

Would output:

```
[Export ("supportedScrollMode")]
CAScroll SupportedScrollMode { get; set; }
```

We will handle the `enum` <-> `NSString` conversion only if the provided enum type to `[BindAs]` is [backed by a NSString constant](#enum-attributes).

### Arrays

`[BindAs]` also supports arrays of any of the supported types, you can have the following API definition as an example:

```
[return: BindAs (typeof (CAScroll []))]
[Export ("getScrollModesAt:")]
NSString [] GetScrollModes ([BindAs (typeof (CGRect []))] NSValue [] rects);
```

Would output:

```
[Export ("getScrollModesAt:")]
CAScroll? [] GetScrollModes (CGRect [] rects) { ... }
```

The `rects` parameter will be encapsulated into a `NSArray` that contains an `NSValue` for each `CGRect` and in return you will get an array of `CAScroll?` which has been created using the values of the returned `NSArray` containing `NSStrings`.


## BindAttribute

The `Bind` attribute has two uses one when applied to a method or property
declaration, and another one when applied to the individual getter or setter in
a property.

When used for a method or property, the effect of the Bind attribute is to
generate a method that invokes the specified selector. But the resulting
generated method is not decorated with the `[Export]` attribute, which means that
it can not participate in method overriding. This is typically used in
combination with the `Target` attribute for implementing Objective-C extension
methods.

For example:

```
public interface UIView {
    [Bind ("drawAtPoint:withFont:")]
    SizeF DrawString ([Target] string str, CGPoint point, UIFont font);
}
```

When used in a getter or setter, the `Bind` attribute is used to alter the
defaults inferred by the code generator when generating the getter and setter
Objective-C selector names for a property. By default when you flag a property
with the name "fooBar", the generator would generate a "fooBar" export for the
getter and "setFooBar:" for the setter. In a few cases, Objective-C does not
follow this convention, usually they change the getter name to be "isFooBar".
You would use this attribute to inform the generator of this.

For example:

```
// Default behavior
[Export ("active")]
bool Active { get; set; }

// Custom naming with the Bind attribute
[Export ("visible")]
bool Visible { [Bind ("isVisible")] get; set; }
```

 <a name="AsyncAttribute"></a>


## AsyncAttribute

Only available on Xamarin.iOS 6.3 and newer.

This attribute can be applied to methods that take a
completion handler as their last argument.

You can use the `[Async]` attribute on methods whose
last argument is a callback.  When you apply
this to a method, the binding generator will generate a
version of that method with the suffix `Async`.  If the callback
takes no parameters, the return value will be a `Task`, if the
callback takes a parameter, the result will be a
Task&lt;T&gt;.

```
[Export ("upload:complete:")]
[Async]
void LoadFile (string file, NSAction complete)
```

The following will generate this async method:

```
Task LoadFileAsync (string file);
```

If the callback takes multiple parameters, you
should set the `ResultType` or `ResultTypeName` to specify the
desired name of the generated type which will hold all the
properties.

```
delegate void OnComplete (string [] files, nint byteCount);

[Export ("upload:complete:")]
[Async (ResultTypeName="FileLoading")]
void LoadFiles (string file, OnComplete complete)
```

The following will generate this async method, where
`FileLoading` contains properties to access both "files" and
"byteCount":

```
Task<FileLoading> LoadFile (string file);
```

If the last parameter of the callback is an `NSError`, then
the generated `Async` method will check if the value is not
null, and if that is the case, the generated async method will
set the task exception.

```
[Export ("upload:onComplete:")]
[Async]
void Upload (string file, Action<string,NSError> onComplete);
```

The above generates the following async method:

```
Task<string> UploadAsync (string file);
```

And on error, the resulting Task will have the exception
set to an `NSErrorException` that wraps the resulting `NSError`.

### AsyncAttribute.ResultType



Use this property to specify the value for the returning
`Task` object.   This parameter takes an existing type, thus it
needs to be defined in one of your core api definitions.


### AsyncAttribute.ResultTypeName

Use this property to specify the value for the returning
`Task` object.   This parameter takes the name of your desired
type name, the generator will produce a series of properties,
one for each parameter that the callback takes.


### AsyncAttribute.MethodName



Use this property to customize the name of the generated
async methods.   The default is to use the name of the method
and append the text "Async", you can use this to change this default.


## DisableZeroCopyAttribute

This attribute is applied to string parameters or string properties and
instructs the code generator to not use the zero-copy string marshaling for
this parameter, and instead create a new NSString instance from the C# string.
This attribute is only required on strings if you instruct the generator to use
zero-copy string marshaling using either the `--zero-copy` command
line option or setting the assembly-level attribute `ZeroCopyStringsAttribute`.

This is necessary in cases where the property is declared in Objective-C to
be a "retain" or "assign" property instead of a "copy" property. These typically
happen in third-party libraries that have been wrongly "optimized" by
developers. In general, "retain" or "assign" `NSString` properties are incorrect
since `NSMutableString` or user-derived classes of `NSString` might alter the
contents of the strings without the knowledge of the library code, subtly
breaking the application. Typically this happens due to premature
optimization.

The following shows two such properties in Objective-C:

```
@property(nonatomic,retain) NSString *name;
@property(nonatomic,assign) NSString *name2;
```

 <a name="DisposeAttribute" />


## DisposeAttribute

When you apply the `DisposeAttribute` to a class, you provide a code snippet
that will be added to the `Dispose()` method implementation of the class.

Since the `Dispose` method is automatically generated by the `bmac-native` and `btouch-native`
tools, you need to use the `Dispose` attribute to inject some code in the
generated `Dispose` method implementation.

For example:

```
[BaseType (typeof (NSObject))]
[Dispose ("if (OpenConnections > 0) CloseAllConnections ();")]
interface DatabaseConnection {
}
```

 <a name="ExportAttribute" />


## ExportAttribute

The `Export` attribute is used to flag a method or property to be exposed to
the Objective-C runtime. This attribute is shared between the binding tool and
the actual Xamarin.iOS and Xamarin.Mac runtimes. For methods, the parameter is passed
verbatim to the generated code, for properties, a getter and setter Exports are
generated based on the base declaration (see the section on the `BindAttribute`
for information on how to alter the behavior of the binding tool).

Syntax:

```
public enum ArgumentSemantic {
    None, Assign, Copy, Retain.
}

[AttributeUsage (AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
public class ExportAttribute : Attribute {
    public ExportAttribute();
    public ExportAttribute (string selector);
    public ExportAttribute (string selector, ArgumentSemantic semantic);
    public string Selector { get; set; }
    public ArgumentSemantic ArgumentSemantic { get; set; }
}
```

The [selector](http://developer.apple.com/library/ios/#documentation/cocoa/conceptual/objectivec/Chapters/ocSelectors.html) and it represents the underlying Objective-C name
of the method or property that is being bound.

 <a name="ExportAttribute.ArgumentSemantic" />


### ExportAttribute.ArgumentSemantic

 <a name="FieldAttribute" />


## FieldAttribute

This attribute is used to expose a C global variable as a field that is
loaded on demand and exposed to C# code. Usually this is required to get the
values of constants that are defined in C or Objective-C and that could be
either tokens used in some APIs, or whose values are opaque and must be used
as-is by user code.

Syntax:

```
public class FieldAttribute : Attribute {
    public FieldAttribute (string symbolName);
    public FieldAttribute (string symbolName, string libraryName);
    public string SymbolName { get; set; }
    public string LibraryName { get; set; }
}
```

The `symbolName` is the C symbol to link with. By default this
will be loaded from a library whose name is inferred from the namespace where
the type is defined. If this is not the library where the symbol is looked up,
you should pass the `libraryName` parameter. If you're linking a
static library, use "__Internal" as the `libraryName` parameter.

The generated properties are always static.

Properties flagged with the Field attribute can be of the following types:

* `NSString`
* `NSArray`
* `nint` / `int` / `long`
* `nuint` / `uint` / `ulong`
* `nfloat` / `float`
* `double`
* `CGSize`
* `System.IntPtr`
* Enums

Setters are not supported for [enums backed by NSString constants](#enum-attributes), but they can be manually bound if needed.

Example:

```
[Static]
interface CameraEffects {
     [Field ("kCameraEffectsZoomFactorKey", "CameraLibrary")]
     NSString ZoomFactorKey { get; }
}
```




## InternalAttribute

The `Internal` attribute can be applied to methods or properties and it has the
effect of flagging the generated code with the "internal" C# keyword making the
code only accessible to code in the generated assembly. This is typically used
to hide APIs that are too low-level or provide a suboptimal public API that you
want to improve upon or for APIs that are not supported by the generator and
require some hand-coding.

When you design the binding, you would typically hide the method or property
using this attribute and provide a different name for the method or property,
and then on your C# complementary support file, you would add a strongly typed
wrapper that exposes the underlying functionality.

For example:

```
[Internal]
[Export ("setValue:forKey:");
void _SetValueForKey (NSObject value, NSObject key);

[Internal]
[Export ("getValueForKey:")]
NSObject _GetValueForKey (NSObject key);
```

Then, in your supporting file, you could have some code like this:

```
public NSObject this [NSObject idx] {
    get {
        return _GetValueForKey (idx);
    }
    set {
        _SetValueForKey (value, idx);
    }
}
```


## IsThreadStaticAttribute

This attribute flags the backing field for a property to be annotated with
the .NET `[ThreadStatic]` attribute. This is useful if the field is a thread
static variable.

 <a name="MarshalNativeExceptions_(Xamarin.iOS_6.0.6)" />


## MarshalNativeExceptions (Xamarin.iOS 6.0.6)

This attribute will make a method support native (ObjectiveC) exceptions.
Instead of calling `objc_msgSend` directly, the invocation will go through a
custom trampoline which catches ObjectiveC exceptions and marshals them into
managed exceptions.

Currently only a few `objc_msgSend` signatures are supported (you will find out
if a signature isn't supported when native linking of an app that uses the
binding fails with a missing monotouch_*_objc_msgSend* symbol), but more can be
added at request.

 <a name="NewAttribute" />


## NewAttribute

This attribute is applied to methods and properties to have the generator
generate the "new" keyword in front of the declaration.

It is used to avoid compiler warnings when the same method or property name
is introduced in a subclass that already existed in a base class.

 <a name="NotificationAttribute" />


## NotificationAttribute

You can apply this attribute to fields to have the generator produce a
strongly typed helper Notifications class.

This attribute can be used without arguments for notifications that carry no
payload, or you can specify a `System.Type` that references another interface in
the API definition, typically with the name ending with "EventArgs". The
generator will turn the interface into a class that subclasses `EventArgs` and
will include all of the properties listed there. The `[Export]` attribute should
be used in the `EventArgs` class to list the name of the key used to look up the
Objective-C dictionary to fetch the value.

For example:

```
interface MyClass {
    [Notification]
    [Field ("MyClassDidStartNotification")]
    NSString DidStartNotification { get; }
}
```

The above code will generate a nested class `MyClass.Notifications` with the
following methods:

```
public class MyClass {
   [..]
   public Notifications {
      public static NSObject ObserveDidStart (EventHandler<NSNotificationEventArgs> handler)
      public static NSObject ObserveDidStart (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
   }
}
```

Users of your code can then easily subscribe to notifications posted
to the
[NSDefaultCenter](/api/property/Foundation.NSNotificationCenter.DefaultCenter/)
by using code like this:

```
var token = MyClass.Notifications.ObserverDidStart ((notification) => {
    Console.WriteLine ("Observed the 'DidStart' event!");
});
```

Or to set a specific object to observe. If you pass `null` to `objectToObserve` this method will behave just like its other peer.

```
var token = MyClass.Notifications.ObserverDidStart (objectToObserve, (notification) => {
    Console.WriteLine ("Observed the 'DidStart' event on objectToObserve!");
});
```

The returned value from `ObserveDidStart` can be used to easily stop receiving
notifications, like this:

```
token.Dispose ();
```


Or you can call [NSNotification.DefaultCenter.RemoveObserver](/api/member/Foundation.NSNotificationCenter.RemoveObserver/p/Foundation.NSObject//)
and pass the token. If your notification contains parameters, you
should specify a helper `EventArgs` interface, like this:

```
interface MyClass {
    [Notification (typeof (MyScreenChangedEventArgs)]
    [Field ("MyClassScreenChangedNotification")]
    NSString ScreenChangedNotification { get; }
}

// The helper EventArgs declaration
interface MyScreenChangedEventArgs {
    [Export ("ScreenXKey")]
    nint ScreenX { get; set; }

    [Export ("ScreenYKey")]
    nint ScreenY { get; set; }

    [Export ("DidGoOffKey")]
    [ProbePresence]
    bool DidGoOff { get; }
}
```

The above will generate a `MyScreenChangedEventArgs` class with the
`ScreenX` and `ScreenY` properties that will fetch the data from the
[NSNotification.UserInfo](/api/property/Foundation.NSNotification.UserInfo/)
dictionary using the keys **ScreenXKey** and **ScreenYKey**
respectively and apply the proper conversions. The `[ProbePresence]`
attribute is used for the generator to probe if the key is set in the
`UserInfo`, instead of trying to extract the value. This is used for
cases where the presence of the key is the value (typically for
boolean values).

This allows you to write code like this:

```
var token = MyClass.NotificationsObserveScreenChanged ((notification) => {
    Console.WriteLine ("The new screen dimensions are {0},{1}", notification.ScreenX, notification.ScreenY);
});
```

In some cases, there is no constant associated with the value passed
on the dictionary.  Apple sometimes uses public symbol constants and
sometimes uses string constants.  By default the `[Export]` attribute
in your provided `EventArgs` class will use the specified name as a
public symbol to be looked up at runtime.  If this is not the case,
and instead it is supposed to be looked up as a string constant then
pass the `ArgumentSemantic.Assign` value to the Export attribute.

**New in Xamarin.iOS 8.4**

Sometimes, notifications will begin life without any arguments, so the
use of `[Notification]` without arguments is acceptable.  But
sometimes, parameters to the notification will be introduced.  To
support this scenario, the attribute can be applied more than once.

If you are developing a binding, and you want to avoid breaking
existing user code, you would turn an existing notification from:

```
interface MyClass {
    [Notification]
    [Field ("MyClassScreenChangedNotification")]
    NSString ScreenChangedNotification { get; }
}
```

Into a version that lists the notification attribute twice, like this:

```
interface MyClass {
    [Notification]
    [Notification (typeof (MyScreenChangedEventArgs)]
    [Field ("MyClassScreenChangedNotification")]
    NSString ScreenChangedNotification { get; }
}
```


## NullAllowedAttribute

When this is applied to a property it flags the property as allowing the
value null to be assigned to it. This is only valid for reference types.

When this is applied to a parameter in a method signature it indicates that
the specified parameter can be null and that no check should be performed for
passing null values.

If the reference type does not have this attribute, the binding tool will
generate a check for the value being assigned before passing it to Objective-C
and will generate a check that will throw an `ArgumentNullException` if the value
assigned is null.

For example:

```
// In properties

[NullAllowed]
UIImage IconFile { get; set; }

// In methods
void SetImage ([NullAllowed] UIImage image, State forState);
```

## OverrideAttribute

Use this attribute to instruct the binding generator that the binding for
this particular method should be flagged with an "override" keyword.

 <a name="PreSnippetAttribute" />


## PreSnippetAttribute

You can use this attribute to inject some code to be inserted after the input
parameters have been validated, but before the code calls into Objective-C

Example:

```
[Export ("demo")]
[PreSnippet ("var old = ViewController;")]
void Demo ();
```

 <a name="PrologueSnippetAttribute" />


## PrologueSnippetAttribute

You can use this attribute to inject some code to be inserted before any of
the parameters are validated in the generated method.

Example:

```
[Export ("demo")]
[Prologue ("Trace.Entry ();")]
void Demo ();
```

 <a name="PostGetAttribute" />


## PostGetAttribute

Instructs the binding generator to invoke the specified property from this
class to fetch a value from it.

This property is typically used to refresh the cache that points to reference
objects that keep the object graph referenced. Usually it shows up in code that
has operations like Add/Remove. This method is used so that after elements are
added or removed that the internal cache be updated to ensure that we are
keeping managed references to objects that are actually in use. This is possible
because the binding tool generates a backing field for all reference objects in
a given binding.

Example:

```
[BaseType (typeof (NSObject))]
[Since (4,0)]
public interface NSOperation {
    [Export ("addDependency:")][PostGet ("Dependencies")]
    void AddDependency (NSOperation op);

    [Export ("removeDependency:")][PostGet ("Dependencies")]
    void RemoveDependency (NSOperation op);

    [Export ("dependencies")]
    NSOperation [] Dependencies { get; }
}
```

In this case, the `Dependencies` property will be invoked after
adding or removing dependencies from the `NSOperation` object, ensuring that we
have a graph that represents the actual loaded objects, preventing both memory
leaks as well as memory corruption.

 <a name="PostSnippetAttribute" />


## PostSnippetAttribute

You can use this attribute to inject some C# source code to be inserted after
the code has invoked the underlying Objective-C method

Example:

```
[Export ("demo")]
[PostSnippet ("if (old != null) old.DemoComplete ();")]
void Demo ();
```

 <a name="ProxyAttribute" />


## ProxyAttribute

This attribute is applied to return values to flag them as being proxy
objects. Some Objective-C APIs return proxy objects that can not be
differentiated from user bindings. The effect of this attribute is to flag the
object as being a `DirectBinding` object. For a scenario in Xamarin.Mac, you can see
the [discussion on this bug](https://bugzilla.novell.com/show_bug.cgi?id=670844).

 <a name="RetainListAttribute" />


## RetainListAttribute

Instructs the generator to keep a managed reference to the parameter or
remove an internal reference to the parameter. This is used to keep objects
referenced.

Syntax:

```
public class RetainListAttribute: Attribute {
     public RetainListAttribute (bool doAdd, string listName);
}
```

If the value of "doAdd" is true, then the parameter is added to the
`__mt_{0}_var List<NSObject>;`. Where `{0}` is replaced with the given
`listName`. You must declare this backing field in your complementary partial
class to the API.

For an example see [foundation.cs](https://github.com/mono/maccore/blob/master/src/foundation.cs) and [NSNotificationCenter.cs](https://github.com/mono/maccore/blob/master/src/Foundation/NSNotificationCenter.cs)

 <a name="ReleaseAttribute_(Xamarin.iOS_6.0)" />


## ReleaseAttribute (Xamarin.iOS 6.0)

This can be applied to return types to indicate that the generator should
call `Release` on the object before returning it. This is only needed when a
method gives you a retained object (as opposed to an autoreleased object, which
is the most common scenario)

Example:

```
[Export ("getAndRetainObject")]
[return: Release ()]
NSObject GetAndRetainObject ();
```

Additionally this attribute is propagated to the generated code, so that
the Xamarin.iOS runtime knows it must retain the object upon returning to
Objective-C from such a function.

 <a name="SealedAttribute" />


## SealedAttribute

Instructs the generator to flag the generated method as sealed. If this
attribute is not specified, the default is to generate a virtual method (either
a virtual method, an abstract method or an override depending on how other
attributes are used).

 <a name="StaticAttribute" />


## StaticAttribute

When the `Static` attribute is applied to a method or property this generates a
static method or property. If this attribute is not specified, then the
generator produces an instance method or property.

 <a name="TransientAttribute" />


## TransientAttribute

Use this attribute to flag properties whose values are transient, that is,
objects that are created temporarily by iOS but are not long lived. When this
attribute is applied to a property, the generator does not create a backing
field for this property, which means that the managed class does not keep a
reference to the object.

 <a name="WrapAttribute" />


## WrapAttribute

In the design of the Xamarin.iOS/Xamarin.Mac bindings, the `Wrap` attribute is used
to wrap a weakly typed object with a strongly typed object. This comes into play
mostly with Objective-C "delegate" objects which are typically declared as being
of type `id` or `NSObject`. The convention used by
Xamarin.iOS and Xamarin.Mac is to expose those delegates or data sources as being of
type `NSObject` and are named using the convention "Weak" + the name being
exposed. An "id delegate" property from Objective-C would be exposed as an
`NSObject WeakDelegate { get; set; }` property in the API contract file.

But typically the value that is assigned to this delegate is of a strong
type, so we surface the strong type and apply the `Wrap` attribute, this means
that users can choose to use weak types if they need some fine-control or if
they need to resort to low-level tricks, or they can use the strongly typed
property for most of their work.

Example:

```
[BaseType (typeof (NSObject))]
interface Demo {
     [Export ("delegate"), NullAllowed]
     NSObject WeakDelegate { get; set; }

     [Wrap ("WeakDelegate")]
     DemoDelegate Delegate { get; set; }
}

[BaseType (typeof (NSObject))]
[Model][Protocol]
interface DemoDelegate {
    [Export ("doDemo")]
    void DoDemo ();
}
```

This is how the user would use the weakly-typed version of the Delegate:

```
// The weak case, user has to roll his own
class SomeObject : NSObject {
    [Export ("doDemo")]
    void CallbackForDoDemo () {}

}

var demo = new Demo ();
demo.WeakDelegate = new SomeObject ();
```

And this is how the user would use the strongly typed version, notice that
the user takes advantage of C#'s type system and is using the override keyword
to declare his intent and that he does not have to manually decorate the method
with `Export`, since we did that work in the binding for the user:

```
// This is the strong case,
class MyDelegate : DemoDelegate {
   override void Demo DoDemo () {}
}


var strongDemo = new Demo ();
demo.Delegate = new MyDelegate ();
```

 <a name="Parameter_Attributes" />

Another use of the `Wrap` attribute is to support strongly typed version
of methods.   For example:

```
[BaseType (typeof (NSObject))]
interface XyzPanel {
    [Export ("playback:withOptions:")]
    void Playback (string fileName, [NullAllowed] NSDictionary options);

    [Wrap ("Playback (fileName, options == null ? null : options.Dictionary")]
    void Playback (string fileName, XyzOptions options);
}
```

The members generated by `[Wrap]` are not `virtual` by default, if you need a `virtual` member you can set to `true` the optional `isVirtual` parameter.

```
[BaseType (typeof (NSObject))]
interface FooExplorer {
	[Export ("fooWithContentsOfURL:")]
	void FromUrl (NSUrl url);

	[Wrap ("FromUrl (NSUrl.FromString (url))", isVirtual: true)]
	void FromUrl (string url);
}
```

# Parameter Attributes

This section describes the attributes that you can apply to the parameters in
a method definition as well as the `NullAttribute` that applies to a property as a
whole.


## BlockCallback



This attribute is applied to parameter types in C#
delegate declarations to notify the binder that the parameter
in question conforms to the Objective-C block calling
convention and should marshal it in this way.

This is typically used for callbacks that are defined like
this in Objective-C:

```
typedef returnType (^SomeTypeDefinition) (int parameter1, NSString *parameter2);
```

See also: [CCallback](#CCallback).


## CCallback



This attribute is applied to parameter types in C#
delegate declarations to notify the binder that the parameter
in question conforms to the C ABI function pointer calling
convention and should marshal it in this way.

This is typically used for callbacks that are defined like
this in Objective-C:

    typedef returnType (*SomeTypeDefinition) (int parameter1, NSString *parameter2);

See also: [BlockCallback](#BlockCallback).


## Params



You can use the `[Params]` attribute on the last array parameter of
a method definition to have the generator inject a "params" in
the definition.   This allows the binding to easily allow for
optional parameters.

For example, the following definition:

    [Export ("loadFiles:")]
    void LoadFiles ([Params]NSUrl [] files);

Allows the following code to be written:

    foo.LoadFiles (new NSUrl (url));
    foo.LoadFiles (new NSUrl (url1), new NSUrl (url2), new NSUrl (url3));

This has the added advantage that it does not require users
to create an array purely for passing elements.


## PlainString

You can use the `[PlainString]` attribute in front of string parameters to
instruct the binding generator to pass the string as a C string, instead of
passing the parameter as an `NSString`.

Most Objective-C APIs consume `NSString` parameters, but a handful of APIs
expose a `char *` API for passing strings, instead of the `NSString` variation.
Use `[PlainString]` in those cases.

For example, the following Objective-C declarations:

```
- (void) setText: (NSString *) theText;
- (void) logMessage: (char *) message;
```

Should be bound like this:

```
[Export ("setText:")]
void SetText (string theText);

[Export ("logMessage:")]
void LogMessage ([PlainString] string theText);
```

 <a name="RetainAttribute" />


## RetainAttribute

Instructs the generator to keep a reference to the specified parameter. The
generator will provide the backing store for this field or you can specify a
name (the `WrapName`) to store the value at. This is useful to hold a reference to
a managed object that is passed as a parameter to Objective-C and when you know
that Objective-C will only keep this copy of the object. For instance, an API
like `SetDisplay (SomeObject)` would use this attribute as it is likely that the
SetDisplay could only display one object at a time. If you need to keep track of
more than one object (for example, for a Stack-like API) you would use the
`RetainList` attribute.

Syntax:

```
public class RetainAttribute {
    public RetainAttribute ();
    public RetainAttribute (string wrapName);
    public string WrapName { get; }
}
```

 <a name="RetainListAttribute" />


## RetainListAttribute

Instructs the generator to keep a managed reference to the parameter or
remove an internal reference to the parameter. This is used to keep objects
referenced.

Syntax:

```
public class RetainListAttribute: Attribute {
     public RetainListAttribute (bool doAdd, string listName);
}
```

If the value of "doAdd" is true, then the parameter is added to the
`__mt_{0}_var List<NSObject>`. Where `{0}` is replaced with the given
`listName`. You must declare this backing field in your complementary partial
class to the API.

For an example see [foundation.cs](https://github.com/mono/maccore/blob/master/src/foundation.cs) and [NSNotificationCenter.cs](https://github.com/mono/maccore/blob/master/src/Foundation/NSNotificationCenter.cs)

 <a name="TransientAttribute"></a>


## TransientAttribute



This attribute is applied to parameters and is only used
when transitioning from Objective-C to C#.  During those
transitions the various Objective-C NSObjects parameters are
wrapped into a managed representation of the object.

The runtime will take a reference to the native object and
keep the reference until the last managed reference to the
object is gone, and the GC has a chance to run.

In a few cases, it is important for the C# runtime to not
keep a reference to the native object.  This sometimes happens
when the underlying native code has attached a special
behavior to the lifecycle of the parameter.  For example: the
destructor for the parameter will perform some cleanup action,
or dispose some precious resource.

This attribute informs the runtime that you desire the
object to be disposed if possible when returning back to
Objective-C from your overwritten method.

The rule is simple: if the runtime had to create a new
managed representation from the native object, then at the end
of the function, the retain count for the native object will
be dropped, and the Handle property of the managed object will be
cleared.   This means that if you kept a reference to the
managed object, that reference will become useless (invoking
methods on it will throw an exception).

If the object passed was not created, or if there was
already an outstanding managed representation of the object,
the forced dispose does not take place. <a name="Global_Attributes" />

# Property Attributes

## NotImplementedAttribute

This attribute is used to support an Objective-C idiom where a
property with a getter is introduced in a base class, and a mutable
subclass introduces a setter.

Since C# does not support this model, the base class needs to have
both the setter and the getter, and a subclass can use the
[OverrideAttribute](#OverrideAttribute).

This attribute is only used in property setters, and is used to
support the mutable idiom in Objective-C.

Example:

```
[BaseType (typeof (NSObject))]
interface MyString {
    [Export ("initWithValue:")]
    IntPtr Constructor (string value);

    [Export ("value")]
    string Value {
        get;

	[NotImplemented ("Not available on MyString, use MyMutableString to set")]
        set;
    }
}

[BaseType (typeof (MyString))]
interface MyMutableString {
    [Export ("value")]
    [Override]
    string Value { get; set; }
}
```

# Enum Attributes

Mapping `NSString` constants to enum values is a easy way to create better
.NET API. It:

* allows code completion to be more useful, by showing **only** the correct values for the API;
* adds type safety, you cannot use another `NSString` constant in a incorrect context; and
* allows to hide some constants, making code completion show shorter API list without losing functionality.

Example:

```
enum NSRunLoopMode {

	[DefaultEnumValue]
	[Field ("NSDefaultRunLoopMode")]
	Default,

	[Field ("NSRunLoopCommonModes")]
	Common,

	[Field (null)]
	Other = 1000
}
```

From the above binding definition the generator will create the `enum` itself and will
also create a `*Extensions` static type that includes two-ways conversion methods
between the enum values and the `NSString` constants. This means the constants remains
available to developers even if they are not part of the API.

Examples:

```
// using the NSString constant in a different API / framework / 3rd party code
CallApiRequiringAnNSString (NSRunLoopMode.Default.GetConstant ());
```

```
// converting the constants from a different API / framework / 3rd party code
var constant = CallApiReturningAnNSString ();
// back into an enum value
CallApiWithEnum (NSRunLoopModeExtensions.GetValue (constant));
```

## DefaultEnumValueAttribute

You can decorate **one** enum value with this attribute. This will become the constant
being returned if the enum value is not known.

From the example above:

```
var x = (NSRunLoopMode) 99;
Call (x.GetConstant ()); // NSDefaultRunLoopMode will be used
```

If no enum value is decorated then a `NotSupportedException` will be thrown.

## ErrorDomainAttribute

Error codes are bound as an enum values. There's generally an error domain for them
and it's not always easy to find which one applies (or if one even exists).

You can use this attribute to associate the error domain with the enum itself.

Example:

```
	[Native]
	[ErrorDomain ("AVKitErrorDomain")]
	public enum AVKitError : nint {
		None = 0,
		Unknown = -1000,
		PictureInPictureStartFailed = -1001
	}
```

You can then call the extension method `GetDomain` to get the domain constant of
any error.


## FieldAttribute

This is the same `[Field]` attribute used for constants inside type. It can also
be used inside enums to map a value with a specific constant.

A `null` value can be used to specify which enum value should be returned if a
`null` `NSString` constant is specified.

From the example above:

```
var constant = NSRunLoopMode.NewInWatchOS3; // will be null in watchOS 2.x
Call (NSRunLoopModeExtensions.GetValue (constant)); // will return 1000
```

If no `null` value is present then an `ArgumentNullException` will be thrown.

# Global Attributes

Global attributes are either applied using the `[assembly:]` attribute modifier
like the `LinkWithAttribute` or can be used anywhere, like the `Lion` and `Since`
attributes.

 <a name="LinkWithAttribute" />


## LinkWithAttribute

This is an assembly-level attribute which allows developers to specify the
linking flags required to reuse a bound library without forcing the consumer
of the library to manually configure the gcc_flags and extra mtouch arguments
passed to a library.

Syntax:

```
// In properties
[Flags]
public enum LinkTarget {
    Simulator    = 1,
    ArmV6    = 2,
    ArmV7    = 4,
    Thumb    = 8,
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
public class LinkWithAttribute : Attribute {
    public LinkWithAttribute ();
    public LinkWithAttribute (string libraryName);
    public LinkWithAttribute (string libraryName, LinkTarget target);
    public LinkWithAttribute (string libraryName, LinkTarget target, string linkerFlags);
    public bool ForceLoad { get; set; }
    public string Frameworks { get; set; }
    public bool IsCxx { get; set;  }
    public string LibraryName { get; }
    public string LinkerFlags { get; set; }
    public LinkTarget LinkTarget { get; set; }
    public bool NeedsGccExceptionHandling { get; set; }
    public bool SmartLink { get; set; }
    public string WeakFrameworks { get; set; }
}
```

This attribute is applied at the assembly level, for example, this is what
the [CorePlot bindings](https://github.com/mono/monotouch-bindings/tree/master/CorePlot) use:

```
[assembly: LinkWith ("libCorePlot-CocoaTouch.a", LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Simulator, Frameworks = "CoreGraphics QuartzCore", ForceLoad = true)]
```

When you use the `LinkWith` attribute, the specified `libraryName` is embedded
into the resulting assembly, allowing users to ship a single DLL that contains
both the unmanaged dependencies as well as the command line flags necessary to
properly consume the library from Xamarin.iOS.

It's also possible to not provide a `libraryName`, in which case the
`LinkWith` attribute can be used to only specify additional linker flags:

 ``` csharp
[assembly: LinkWith (LinkerFlags = "-lsqlite3")]
 ```

 <a name="LinkWithAttribute_Constructors" />


### LinkWithAttribute Constructors

These constructors allow you to specify the library to link with and embed into
your resulting assembly, the supported targets that the library supports and any
optional library flags that are necessary to link with the library.

Note that the LinkTarget argument is inferred by Xamarin.iOS and does not need to be set.

Examples:

```
// Specify additional linker:
[assembly: LinkWith (LinkerFlags = "-sqlite3")]

// Specify library name for the constructor:
[assembly: LinkWith ("libDemo.a");

// Specify library name, and link target for the constructor:
[assembly: LinkWith ("libDemo.a", LinkTarget.Thumb | LinkTarget.Simulator);

// Specify only the library name, link target and linker flags for the constructor:
[assembly: LinkWith ("libDemo.a", LinkTarget.Thumb | LinkTarget.Simulator, SmartLink = true, ForceLoad = true, IsCxx = true);
```

 <a name="LinkWithAttribute.ForceLoad" />


### LinkWithAttribute.ForceLoad

The `ForceLoad` property is used to decide whether or not the `-force_load` link
flag is used for linking the native library. For now, this should always be
true.

 <a name="LinkWithAttribute.Frameworks" />


### LinkWithAttribute.Frameworks

If the library being bound has a hard requirement on any frameworks (other
than `Foundation` and `UIKit`), you should set the `Frameworks` property to a string
containing a space-delimited list of the required platform frameworks. For
example, if you are binding a library that requires `CoreGraphics` and `CoreText`,
you would set the `Frameworks` property to `"CoreGraphics CoreText"`.

 <a name="LinkWithAttribute.IsCxx" />


### LinkWithAttribute.IsCxx

Set this property to true if the resulting executable needs to be compiled
using a C++ compiler instead of the default, which is a C compiler. Use this if
the library that you are binding was written in C++.

 <a name="LinkWithAttribute.LibraryName" />


### LinkWithAttribute.LibraryName

The name of the unmanaged library to bundle. This is a file with the
extension ".a" and it can contain object code for multiple platforms (for
example, ARM and x86 for the simulator).

Earlier versions of Xamarin.iOS checked the `LinkTarget` property to determine
the platform your library supported, but this is now auto-detected, and the
`LinkTarget` property is ignored.

 <a name="LinkWithAttribute.LinkerFlags" />


### LinkWithAttribute.LinkerFlags

The `LinkerFlags` string provides a way for binding authors to specify any
additional linker flags needed when linking the native library into the
application.

For example, if the native library requires libxml2 and zlib, you would set
the `LinkerFlags` string to `"-lxml2 -lz"`.

 <a name="LinkWithAttribute.LinkTarget" />


### LinkWithAttribute.LinkTarget

Earlier versions of Xamarin.iOS checked the `LinkTarget` property to determine
the platform your library supported, but this is now auto-detected, and the
`LinkTarget` property is ignored.



### LinkWithAttribute.NeedsGccExceptionHandling

Set this property to true if the library that you are linking requires the
GCC Exception Handling library (gcc_eh)

 <a name="LinkWithAttribute.SmartLink" />


### LinkWithAttribute.SmartLink

The `SmartLink` property should be set to true to let Xamarin.iOS determine
whether `ForceLoad` is required or not.

 <a name="LinkWithAttribute.WeakFrameworks" />


### LinkWithAttribute.WeakFrameworks

The `WeakFrameworks` property works the same way as the `Frameworks` property,
except that at link-time, the `-weak_framework` specifier is passed
to gcc for each of the listed frameworks.

`WeakFrameworks` makes it possible for libraries and applications to weakly
link against platform frameworks so that they can optionally use them if they
are available but do not take a hard dependency on them which is useful if your
library is meant to add extra features on newer versions of iOS. For more
information on weak linking, see Apple's documentation on [Weak Linking](http://developer.apple.com/library/mac/#documentation/MacOSX/Conceptual/BPFrameworks/Concepts/WeakLinking.html).

Good candidates for weak linking would be `Frameworks` like Accounts,
`CoreBluetooth`, `CoreImage`, `GLKit`, `NewsstandKit` and `Twitter` since they are only
available in iOS 5.

 <a name="SinceAttribute_(iOS)_and_LionAttribute_(MacOS_X)" />


## SinceAttribute (iOS) and LionAttribute (MacOS X)

You use the `Since` Attribute to flag APIs as having being introduced at a
certain point in time. The attribute should only be used to flag types and
methods that could cause a runtime problem if the underlying class, method or
property is not available.

Syntax:

```
public SinceAttribute : Attribute {
     public SinceAttribute (byte major, byte minor);
     public byte Major, Minor;
}
```

It should in general not be applied to enumerations, constraints or new
structures as those would not cause a runtime error if they are executed on a
device with an older version of the operating system.

Example when applied to a type:

```
// Type introduced with iOS 4.2
[Since (4,2)]
[BaseType (typeof (UIPrintFormatter))]
interface UIViewPrintFormatter {
    [Export ("view")]
    UIView View { get; }
}
```

Example when applied to a new member:

```
[BaseType (typeof (UIViewController))]
public interface UITableViewController {
    [Export ("tableView", ArgumentSemantic.Retain)]
    UITableView TableView { get; set; }

    [Since (3,2)]
    [Export ("clearsSelectionOnViewWillAppear")]
    bool ClearsSelectionOnViewWillAppear { get; set; }
```

The `Lion` attribute is applied in the same way but for types introduced with
Lion. The reason to use `Lion` versus the more specific version number that is
used in iOS is that iOS is revised very often, while major OS X releases happen
rarely and it is easier to remember the operating system by their codename than
by their version number

 <a name="AdviceAttribute" />


## AdviceAttribute



Use this attribute to give developers a hint about other APIs that
might be more convenient for them to use.   For example, if you
provide a strongly typed version of an API, you could use this
attribute on the weakly typed attribute to direct the developer to the
better API.

The information from this attribute is shown in the documentation
and tools can be developed to give user suggestions on how to improve
his code. <a name="ZeroCopyStringsAttribute" />


## ZeroCopyStringsAttribute

Only available in Xamarin.iOS 5.4 and newer.

This attribute instructs the generator that the binding for this specific
library (if applied with `[assembly:]`) or type should use the fast
zero-copy string marshaling. This attribute is equivalent to passing the
command line option `--zero-copy` to the generator.

When using zero-copy for strings, the generator effectively uses the same C#
string as the string that Objective-C consumes without incurring the creation of
a new `NSString` object and avoiding copying the data from the C# strings to the
Objective-C string. The only drawback of using Zero Copy strings is that you
must ensure that any string property that you wrap that happens to be flagged as
"retain" or "copy" has the `DisableZeroCopy` attribute set. This is
require because the handle for zero-copy strings is allocated on the stack and
is invalid upon the function return.

Example:

```
[ZeroCopyStrings]
[BaseType (typeof (NSObject))]
interface MyBinding {
    [Export ("name")]
    string Name { get; set; }

    [Export ("domain"), NullAllowed]
    string Domain { get; set; }

    [DisablZeroCopy]
    [Export ("someRetainedNSString")]
    string RetainedProperty { get; set; }
}

```

You can also apply the attribute at the assembly level, and it will apply to
all the types of the assembly:

    [assembly:ZeroCopyStrings]

# Strongly Typed Dictionaries

With Xamarin.iOS 8.0 we introduced support for easily creating
Strongly Typed classes that wrap `NSDictionaries`.

While it has always been possible to use the
[DictionaryContainer](/api/type/Foundation.DictionaryContainer/)
data type together with a manual API, it is now a lot simpler to do
this.  For more information, see [Surfacing Strong
Types](/guides/cross-platform/macios/binding/objective-c-libraries/#Surfacing_Strong_Types_for_weak_NSDictionary_parameters).

<a name="StrongDictionar"/>

## StrongDictionary

When this attribute is applied to an interface, the generator will
produce a class with the same name as the interface that derives from
[DictionaryContainer](/api/type/Foundation.DictionaryContainer/)
and turns each property defined in the interface into a strongly typed
getter and setter for the dictionary.

This automatically generates a class that can be instantiated from an
existing `NSDictionary` or that has been created new.

This attribute takes one parameter, the name of the class containing
the keys that are used to access the elements on the dictionary.   By
default each property in the interface with the attribute will lookup
a member in the specified type for a name with the suffix "Key".

For example:

```
[StrongDictionary ("MyOptionKeys")]
interface MyOption {
    string Name { get; set; }
    nint    Age  { get; set; }
}

[Static]
interface MyOptionKeys {
    // In Objective-C this is "NSString *MYOptionNameKey;"
    [Field ("MYOptionNameKey")]
    NSString NameKey { get; }

    // In Objective-C this is "NSString *MYOptionAgeKey;"
    [Field ("MYOptionAgeKey")]
    NSString AgeKey { get; }
}

```

In the above case, the `MyOption` class will produce a string property
for `Name` that will use the `MyOptionKeys.NameKey` as the key into the
dictionary to retrieve a string.   And will use the
`MyOptionKeys.AgeKey` as the key into the dictionary to retrieve an
`NSNumber` which contains an int.

If you want to use a different key, you can use the export attribute
on the property, for example:

```
[StrongDictionary ("MyColoringKeys")]
interface MyColoringOptions {
    [Export ("TheName")]  // Override the default which would be NameKey
    string Name { get; set; }

    [Export ("TheAge")] // Override the default which would be AgeKey
    nint    Age  { get; set; }
}

[Static]
interface MyColoringKeys {
    // In Objective-C this is "NSString *MYColoringNameKey"
    [Field ("MYColoringNameKey")]
    NSString TheName { get; }

    // In Objective-C this is "NSString *MYColoringAgeKey"
    [Field ("MYColoringAgeKey")]
    NSString TheAge { get; }
}
```

### Strong Dictionary Types

The following data types are supported in the `StrongDictionary`
definition:

<table border="1" cellpadding="1" cellspacing="1" width="80%">
<tbody>
  <tr>
    <td>C# Interface Type</td>
    <td>NSDictionary Storage Type</td>
  </tr>
  <tr>
    <td>bool</td>
    <td>Boolean stored in an NSNumber</td>
  </tr>
  <tr>
    <td>Enumeration values</td>
    <td>integer stored in an NSNumber</td>
  </tr>
  <tr>
    <td>int</td>
    <td>32-bit integer stored in an NSNumber</td>
  </tr>
  <tr>
    <td>uint</td>
    <td>32-bit unsigned integer stored in an NSNumber</td>
  </tr>
  <tr>
    <td>nint</td>
    <td>NSInteger stored in an NSNumber</td>
  </tr>
  <tr>
    <td>nuint</td>
    <td>NSUInteger stored in an NSNumber</td>
  </tr>
  <tr>
    <td>long</td>
    <td>64-bit integer stored in an NSNumber</td>
  </tr>
  <tr>
    <td>float</td>
    <td>32-bit integer stored as an NSNumber</td>
  </tr>
  <tr>
    <td>double</td>
    <td>64-bit integer stored as an NSNumber</td>
  </tr>
  <tr>
    <td>NSObject and subclasses</td>
    <td>NSObject</td>
  </tr>
  <tr>
    <td>NSDictionary</td>
    <td>NSDictionary</td>
  </tr>
  <tr>
    <td>string</td>
    <td>NSString</td>
  </tr>
  <tr>
    <td>NSString</td>
    <td>NSString</td>
  </tr>
  <tr>
    <td>C# Array of NSObject</td>
    <td>NSArray</td>
  </tr>
  <tr>
    <td>C# Array of enumerations</td>
    <td>NSArray containing NSNumbers with the value</td>
  </tr>
</tbody>
