using System;

using CoreFoundation;
using CoreVideo;
using Foundation;
using ObjCRuntime;

namespace NS {
	delegate void NEDatagramAndFlowEndpointsRead ([NullAllowed] CVPixelBuffer [] remoteEndpoints);

	[BaseType (typeof (NSObject))]
	interface INativeObjectInBlocks {
		[Export ("someOtherProperty")]
		NEDatagramAndFlowEndpointsRead SomeOtherProperty { get; set; }
	}
}
