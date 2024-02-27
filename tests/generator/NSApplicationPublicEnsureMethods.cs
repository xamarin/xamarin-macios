/*
 * Errors when doing non-internal (Xamarin.Mac.dll) bindings with a weak delegate:
 * 
 * /var/folders/d5/0m2657ws4b339b0ryh0brkhw0000gn/T/etr9ustz.vgc/Test/TestClass.g.cs(105,19): error CS0122: `AppKit.NSApplication.EnsureDelegateAssignIsNotOverwritingInternalDelegate(object, object, System.Type)' is inaccessible due to its protection level
 * /var/folders/d5/0m2657ws4b339b0ryh0brkhw0000gn/T/etr9ustz.vgc/Test/TestClass.g.cs(133,19): error CS0122: `AppKit.NSApplication.EnsureEventAndDelegateAreNotMismatched(object, System.Type)' is inaccessible due to its protection level
 *  
 * EnsureDelegateAssignIsNotOverwritingInternalDelegate should be marked as public, not internal
 * EnsureEventAndDelegateAreNotMismatched should be marked as public, not internal
 *
 */
using System;
using Foundation;
using ObjCRuntime;

namespace Test {
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (SharedDelegate) })]
	public interface TestClass {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		ISharedDelegate Delegate { get; set; }
	}

	public interface ISharedDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface SharedDelegate {
	}
}
