using System;

using Foundation;

using ObjCRuntime;

namespace NS {
	[BaseType (typeof (NSObject))]
	interface MyObject {
		[Export ("something:")]
		nfloat Something (nfloat something);

		[Export ("someProperty")]
		nfloat SomeProperty { get; set; }

		[Export ("exceptionyProperty")]
		nfloat ExceptionyProperty { [MarshalNativeExceptions] get; [MarshalNativeExceptions] set; }
	}
}
