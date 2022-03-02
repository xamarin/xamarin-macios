using System;
using CoreGraphics;
using Foundation;
using Metal;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

using MPSGraphTensorDataDictionary = Foundation.NSDictionary<MetalPerformanceShadersGraph.MPSGraphTensor, MetalPerformanceShadersGraph.MPSGraphTensorData>;
using MPSGraphTensorShapedTypeDictionary = Foundation.NSDictionary<MetalPerformanceShadersGraph.MPSGraphTensor, MetalPerformanceShadersGraph.MPSGraphShapedType>;

namespace MetalPerformanceShadersGraph
{
	// MPSGraph.h

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	interface MPSGraph {
		[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
		[Static]
		[Export ("new")]
		MPSGraph Create ();

		[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
		[Export ("options")]
		MPSGraphOptions Options { get; set; }

		[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
		[Export ("placeholderTensors")]
		NSArray<MPSGraphTensor> PlaceholderTensors { get; }
	}

	// MPSGraphCore.h

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphShapedType : NSCopying {
	}

	// MPSGraphTensor.h

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphTensor : NSCopying {
	}

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphTensorData {
	}
}
