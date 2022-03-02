using System;
using CoreGraphics;
using Foundation;
using Metal;
using MetalPerformanceShaders;
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
		[Static, Export ("new")]
		MPSGraph Create ();

		[Export ("options")]
		MPSGraphOptions Options { get; set; }

		[Export ("placeholderTensors")]
		MPSGraphTensor[] PlaceholderTensors { get; }

		[Export ("runWithFeeds:targetTensors:targetOperations:")]
		MPSGraphTensorDataDictionary Run (MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations);

		[Export ("runWithMTLCommandQueue:feeds:targetTensors:targetOperations:")]
		MPSGraphTensorDataDictionary Run (IMTLCommandQueue commandQueue, MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations);

		[Export ("runWithMTLCommandQueue:feeds:targetOperations:resultsDictionary:")]
		void Run (IMTLCommandQueue commandQueue, MPSGraphTensorDataDictionary feeds, [NullAllowed] MPSGraphOperation[] targetOperations, MPSGraphTensorDataDictionary resultsDictionary);

		[Export ("runAsyncWithFeeds:targetTensors:targetOperations:executionDescriptor:")]
		MPSGraphTensorDataDictionary RunAsync (MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

		[Export ("runAsyncWithMTLCommandQueue:feeds:targetTensors:targetOperations:executionDescriptor:")]
		MPSGraphTensorDataDictionary RunAsync (IMTLCommandQueue commandQueue, MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

		[Export ("runAsyncWithMTLCommandQueue:feeds:targetOperations:resultsDictionary:executionDescriptor:")]
		void RunAsync (IMTLCommandQueue commandQueue, MPSGraphTensorDataDictionary feeds, [NullAllowed] MPSGraphOperation[] targetOperations, MPSGraphTensorDataDictionary resultsDictionary, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

		[Export ("encodeToCommandBuffer:feeds:targetTensors:targetOperations:executionDescriptor:")]
		MPSGraphTensorDataDictionary Encode (MPSCommandBuffer commandBuffer, MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

		[Export ("encodeToCommandBuffer:feeds:targetOperations:resultsDictionary:executionDescriptor:")]
		void Encode (MPSCommandBuffer commandBuffer, MPSGraphTensorDataDictionary feeds, [NullAllowed] MPSGraphOperation[] targetOperations, MPSGraphTensorDataDictionary resultsDictionary, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);
	}

	delegate void MPSGraphCompletionHandler (MPSGraphTensorDataDictionary resultsDictionary, [NullAllowed] NSError error);
	delegate void MPSGraphScheduledHandler (MPSGraphTensorDataDictionary resultsDictionary, [NullAllowed] NSError error);

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	interface MPSGraphExecutionDescriptor {
		[NullAllowed, Export ("scheduledHandler")]
		MPSGraphScheduledHandler ScheduledHandler { get; set; }

		[NullAllowed, Export ("completionHandler")]
		MPSGraphCompletionHandler CompletionHandler { get; set; }

		[Export ("waitUntilCompleted")]
		bool WaitUntilCompleted { get; set; }
	}

	// MPSGraphCore.h

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphShapedType : NSCopying {
		// TODO: fak: MPSGraphShapedType
	}

	// MPSGraphOperation.h

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphOperation : NSCopying {
		// TODO: fak: MPSGraphOperation
	}

	// MPSGraphTensor.h

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphTensor : NSCopying {
		// TODO: fak: MPSGraphTensor
	}

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphTensorData {
		// TODO: fak: MPSGraphTensorData
	}
}
