using System;
using CoreGraphics;
using Foundation;
using Metal;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MetalPerformanceShadersGraph {

	// MPSGraph.h

	[TV (14,0), Mac (11,0), iOS (14,0)]
	[BaseType (typeof(NSObject))]
	interface MPSGraph {
	}
}
