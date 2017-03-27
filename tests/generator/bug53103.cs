using System;
using Foundation;

namespace Bug53103Tests {

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Export ("fooMethodWithCompletion:")]
		[Async]
		NSObject FooMethod (Action<NSError, NSString> completion);
	}
}