/*
 * Errors when doing users perform a binding but the Delegate property does not use the correct name.
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
	public interface TestWrongPropertyName {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SharedDelegate TestProperty { get; set; }
	}
}
