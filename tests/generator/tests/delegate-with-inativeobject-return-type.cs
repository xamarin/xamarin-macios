using System;

using CoreGraphics;

using Foundation;

namespace DelegateWithINativeObjectReturnType {

	delegate CGPDFDocument MyHandler ();
	delegate NSObject MyHandler2 ();

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Export ("createWithBlock2:")]
		void CreateWithBlock2 (MyHandler2 handler);

		[Export ("createWithBlock:")]
		void CreateWithBlock (MyHandler handler);

	}
}
