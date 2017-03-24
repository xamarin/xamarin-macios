using System;
using Foundation;

namespace Bug53103AllowNonVoidReturnTypeTests {

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Export ("fooMethodWithCompletion:")]
		[Async (allowNonVoidReturnType: true)]
		NSObject FooMethod (Action<NSError, NSString> completion);

		[Export ("fooMethodWithCompletion2:")]
		[Async (AllowNonVoidReturnType = true)]
		NSObject FooMethod2 (Action<NSError, NSString> completion);
	}
}