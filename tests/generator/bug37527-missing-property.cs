/*
 * Errors when doing users perform a binding but the Delegate property is missing.
 * 
 */
using System;
using Foundation;
using ObjCRuntime;

namespace Test {
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface SharedDelegate {
	}

	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (SharedDelegate) })]
	public interface TestMissingPropertyName {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
	}
}
