using System;

using Foundation;
using ObjCRuntime;

namespace NoAsyncWarningCS0219Test {
	[BaseType (typeof (NSObject))]
	interface MyBarClass {

		[Async]
		[Export ("barString:completion:")]
		void Bar (int arg1, Action<NSError> err);

		[Async]
		[Export ("barString:completion:")]
		string BarString (int arg1, Action<NSError> err);

		[Async (ResultTypeName = "BarString2Result")]
		[Export ("barString2:completion:")]
		string BarString2 (int arg1, Action<NSError, NSObject> err);
	}
}
