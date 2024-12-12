using System;

using CoreVideo;
using Foundation;
using ObjCRuntime;

namespace NS {
	[BaseType (typeof (NSObject))]
	interface ReleaseAttributeTest {
		[Export ("getNSObject")]
		[return: Release]
		NSObject GetNSObject ();

		[Export ("getNativeObject")]
		[return: Release]
		CVPixelBuffer GetNativeObject ();

		[Export ("getINativeObject")]
		[return: Release]
		Selector GetINativeObject ();
	}
}
