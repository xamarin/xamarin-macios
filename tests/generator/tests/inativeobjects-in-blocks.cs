using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace NS {

	[Partial]
	interface DispatchData2 {
	}

	delegate void DispatchB (DispatchData2 data);
	delegate void DispatchA (DispatchData2 data, [BlockCallback] DispatchB dispatch);

	[BaseType (typeof (NSObject))]
	interface INativeObjectInBlocks {
		[Export ("someProperty")]
		DispatchA SomeProperty { get; set; }
	}
}
