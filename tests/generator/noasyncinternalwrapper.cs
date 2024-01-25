using System;

using Foundation;

using ObjCRuntime;

namespace NoAsyncInternalWrapperTests {
	[Protocol]
	interface MyFooDelegate {

		[Abstract]
		[Async]
		[Export ("requiredMethod:completion:")]
		void RequiredMethod (int arg1, Action<NSError> err);

		[Async]
		[Export ("optional:completion:")]
		void OptionalMethod (int arg1, Action<NSError> err);
	}
}
