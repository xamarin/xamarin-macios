/*
 * Errors when doing users perform a binding but the Delegate property does not use the correct name.
 *
 */
using System;
#if !XAMCORE_2_0
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#else
using Foundation;
using ObjCRuntime;
#endif

namespace Test {
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface SharedDelegate {
	}

	[BaseType (typeof (NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (SharedDelegate)})]
	public interface TestWrongPropertyName{
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SharedDelegate TestProperty { get; set; }
	}
}
