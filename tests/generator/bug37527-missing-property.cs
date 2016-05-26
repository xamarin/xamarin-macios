/*
 * Errors when doing users perform a binding but the Delegate property is missing.
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
	public interface TestMissingPropertyName{
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
	}
}
