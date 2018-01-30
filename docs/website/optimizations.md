---
id: 84B67E31-B217-443D-89E5-CFE1923CB14E
title: Build optimizations
dateupdated: 2018-01-18
---

[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)
[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)
[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)

Build optimizations
===================

This document explains the various optimizations that are applied at build time for Xamarin.iOS and Xamarin.Mac apps.

Remove UIApplication.EnsureUIThread / NSApplication.EnsureUIThread
------------------------------------------------------------------

Removes calls to [UIApplication.EnsureUIThread][1] (for Xamarin.iOS) or
`NSApplication.EnsureUIThread` (for Xamarin.Mac).

This optimization will change the following type of code:

```csharp
public virtual void AddChildViewController (UIViewController childController)
{
	global::UIKit.UIApplication.EnsureUIThread ();
	// ...
}
```

into the following:

```csharp
public virtual void AddChildViewController (UIViewController childController)
{
	// ...
}
```

This optimization requires the linker to be enabled, and is only applied to
methods with the `[BindingImpl (BindingImplOptions.Optimizable)]` attribute.

By default it's enabled for release builds.

The default behavior can be overridden by passing `--optimize=[+|-]remove-uithread-checks` to mtouch/mmp.

[1]: https://developer.xamarin.com/api/member/UIKit.UIApplication.EnsureUIThread/

Inline IntPtr.Size
------------------

Inlines the constant value of IntPtr.Size according to the target platform.

This optimization will change the following type of code:

```csharp
if (IntPtr.Size == 8) {
	Console.WriteLine ("64-bit platform");
} else {
	Console.WriteLine ("32-bit platform");
}
```

into the following (when building for a 64-bit platform):

```csharp
if (8 == 8) {
	Console.WriteLine ("64-bit platform");
} else {
	Console.WriteLine ("32-bit platform");
}
```

This optimization requires the linker to be enabled, and is only applied to
methods with the `[BindingImpl (BindingImplOptions.Optimizable)]` attribute.

By default it's enabled if targeting a single architecture, or for the
platform assembly (Xamarin.iOS.dll, Xamarin.TVOS.dll, Xamarin.WatchOS.dll or
Xamarin.Mac.dll).

If targeting multiple architecture, this optimization will create different
assemblies for the 32-bit version and the 64-bit version of the app, and both
versions will have to be included in the app, effectively increasing the final
app size instead of decreasing it.

The default behavior can be overridden by passing `--optimize=[+|-]inline-intptr-size` to mtouch/mmp.

Inline NSObject.IsDirectBinding
-------------------------------

NSObject.IsDirectBinding is an instance property that determines whether a
particular instance is of a wrapper type or not (a wrapper type is a managed
type that maps to a native type; for instance the managed `UIKit.UIView` type
maps to the native `UIView` type - the opposite is a user type, in this case
`class MyUIView : UIKit.UIView` would be a user type).

It's necessary to know the value of IsDirectBinding when calling into
Objective-C, because the value determines which version of `objc_msgSend` to
use.

Given only the following code:

```csharp
class UIView : NSObject {
	public virtual string SomeProperty {
		get {
			if (IsDirectBinding) {
				return "true";
			} else {
				return "false"
			}
		}
	}
}

class NSUrl : NSObject {
	public virtual string SomeOtherProperty {
		get {
			if (IsDirectBinding) {
				return "true";
			} else {
				return "false"
			}
		}
	}
}

class MyUIView : UIView {
}
```

we can determine that in `UIView.SomeProperty` the value of `IsDirectBinding` is not a constant and can not be inlined:

```csharp
void uiView = new UIView ();
Console.WriteLine (uiView.SomeProperty); /* prints 'true' */
void myView = new MyUIView ();
Console.WriteLine (myView.SomeProperty); // prints 'false'
```

however it's possible to look at the all the types in the app and determine
that there are no types that inherit from `NSUrl`, and it's thus safe to
inline the `IsDirectBinding` value to a constant `true`:

```csharp
void myURL = new NSUrl ();
Console.WriteLine (myURL.SomeOtherProperty); // prints 'true'
// There's no way to make SomeOtherProperty print anything but 'true', since there are no NSUrl subclasses.
```

In particular, this optimization will change the following type of code (this
is the binding code for NSUrl.AbsoluteUrl):

```csharp
if (IsDirectBinding) {
	return Runtime.GetNSObject<NSUrl> (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.GetHandle ("absoluteURL")));
} else {
	return Runtime.GetNSObject<NSUrl> (global::ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("absoluteURL")));
}
```

into the following (when it can be determined that there are no subclasses of
NSUrl in the app):

```csharp
if (true) {
	return Runtime.GetNSObject<NSUrl> (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.GetHandle ("absoluteURL")));
} else {
	return Runtime.GetNSObject<NSUrl> (global::ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("absoluteURL")));
}
```

This optimization requires the linker to be enabled, and is only applied to
methods with the `[BindingImpl (BindingImplOptions.Optimizable)]` attribute.

It is always enabled by default for Xamarin.iOS, and always disabled by
default for Xamarin.Mac (because it's possible to dynamically load assemblies
in Xamarin.Mac, it's not possible to determine that a particular class is
never subclassed).

The default behavior can be overridden by passing `--optimize=[+|-]inline-isdirectbinding` to mtouch/mmp.

Inline Runtime.Arch
-------------------

This optimization will change the following type of code:

```csharp
if (Runtime.Arch == Arch.DEVICE) {
	Console.WriteLine ("Running on device");
} else {
	Console.WriteLine ("Running in the simulator");
}
```

into the following (when building for device):

```csharp
if (Arch.DEVICE == Arch.DEVICE) {
	Console.WriteLine ("Running on device");
} else {
	Console.WriteLine ("Running in the simulator");
}
```

This optimization requires the linker to be enabled, and is only applied to
methods with the `[BindingImpl (BindingImplOptions.Optimizable)]` attribute.

It is always enabled by default for Xamarin.iOS (it's not available for
Xamarin.Mac).

The default behavior can be overridden by passing `--optimize=[+|-]inline-runtime-arch` to mtouch.

Dead code elimination
---------------------

This optimization will change the following type of code:

```csharp
if (true) {
	Console.WriteLine ("Doing this");
} else {
	Console.WriteLine ("Not doing this");
}
```

into:

```csharp
Console.WriteLine ("Doing this");
```

It will also evaluate constant comparisons, like this:

```csharp
if (8 == 8) {
	Console.WriteLine ("Doing this");
} else {
	Console.WriteLine ("Not doing this");
}
```

and determine that the expression `8 == 8` is a always true, and reduce it to:

```csharp
Console.WriteLine ("Doing this");
```

This is a powerful optimization when used together with the inlining
optimizations, because it can transform the following type of code (this is
the binding code for NFCIso15693ReadMultipleBlocksConfiguration.Range):

```csharp
NSRange ret;
if (IsDirectBinding) {
	if (Runtime.Arch == Arch.DEVICE) {
		if (IntPtr.Size == 8) {
			ret = global::ObjCRuntime.Messaging.NSRange_objc_msgSend (this.Handle, Selector.GetHandle ("range"));
		} else {
			global::ObjCRuntime.Messaging.NSRange_objc_msgSend_stret (out ret, this.Handle, Selector.GetHandle ("range"));
		}
	} else if (IntPtr.Size == 8) {
		ret = global::ObjCRuntime.Messaging.NSRange_objc_msgSend (this.Handle, Selector.GetHandle ("range"));
	} else {
		ret = global::ObjCRuntime.Messaging.NSRange_objc_msgSend (this.Handle, Selector.GetHandle ("range"));
	}
} else {
	if (Runtime.Arch == Arch.DEVICE) {
		if (IntPtr.Size == 8) {
			ret = global::ObjCRuntime.Messaging.NSRange_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("range"));
		} else {
			global::ObjCRuntime.Messaging.NSRange_objc_msgSendSuper_stret (out ret, this.SuperHandle, Selector.GetHandle ("range"));
		}
	} else if (IntPtr.Size == 8) {
		ret = global::ObjCRuntime.Messaging.NSRange_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("range"));
	} else {
		ret = global::ObjCRuntime.Messaging.NSRange_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("range"));
	}
}
return ret;
```

into this (when building for a 64-bit device, and when also able to ensure there are no NFCIso15693ReadMultipleBlocksConfiguration subclasses in the app):

```csharp
NSRange ret;
ret = global::ObjCRuntime.Messaging.NSRange_objc_msgSend (this.Handle, Selector.GetHandle ("range"));
return ret;
```

The AOT compiler is already able to do eliminate dead code like this, but this
optimization is done inside the linker, which means that the linker able to
see that there are multiple methods that are not used anymore, and may thus be
removed (unless used elsewhere):

* global::ObjCRuntime.Messaging.NSRange_objc_msgSend_stret
* global::ObjCRuntime.Messaging.NSRange_objc_msgSendSuper
* global::ObjCRuntime.Messaging.NSRange_objc_msgSendSuper_stret

This optimization requires the linker to be enabled, and is only applied to
methods with the `[BindingImpl (BindingImplOptions.Optimizable)]` attribute.

It is always enabled by default (when the linker is enabled).

The default behavior can be overridden by passing `--optimize=[+|-]dead-code-elimination` to mtouch/mmp.
