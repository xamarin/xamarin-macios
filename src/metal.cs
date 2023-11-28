//
// API for the Metal framework
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2014-2015, Xamarin Inc.
//
// TODO:
//    * Provide friendly accessors instead of those IntPtrs that point to arrays
//    * MTLRenderPipelineReflection: the two arrays are NSObject
//    * Make the *array classes implement all the ICollection methods.
//

using System;
using System.ComponentModel;

using CoreAnimation;
using CoreData;
using CoreGraphics;
using CoreImage;
using CoreLocation;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if TVOS
using MTLAccelerationStructureSizes = Foundation.NSObject;
#endif

namespace Metal {

	delegate void MTLDeallocator (IntPtr pointer, nuint length);

	delegate void MTLNewComputePipelineStateWithReflectionCompletionHandler (IMTLComputePipelineState computePipelineState, MTLComputePipelineReflection reflection, NSError error);

	delegate void MTLDrawablePresentedHandler (IMTLDrawable drawable);

	delegate void MTLNewRenderPipelineStateWithReflectionCompletionHandler (IMTLRenderPipelineState renderPipelineState, MTLRenderPipelineReflection reflection, NSError error);

	interface IMTLCommandEncoder { }

	[Deprecated (PlatformName.MacOSX, 13, 0)]
	[Deprecated (PlatformName.iOS, 16, 0)]
	[Deprecated (PlatformName.TvOS, 16, 0)]
	[Deprecated (PlatformName.MacCatalyst, 16, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLArgument {
		[Export ("name")]
		string Name { get; }

		[Export ("type")]
		MTLArgumentType Type { get; }

		[Export ("access")]
		MTLArgumentAccess Access { get; }

		[Export ("index")]
		nuint Index { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }

		[Export ("bufferAlignment")]
		nuint BufferAlignment { get; }

		[Export ("bufferDataSize")]
		nuint BufferDataSize { get; }

		[Export ("bufferDataType")]
		MTLDataType BufferDataType { get; }

		[NullAllowed]
		[Export ("bufferStructType")]
		MTLStructType BufferStructType { get; }

		[Export ("threadgroupMemoryAlignment")]
		nuint ThreadgroupMemoryAlignment { get; }

		[Export ("threadgroupMemoryDataSize")]
		nuint ThreadgroupMemoryDataSize { get; }

		[Export ("textureType")]
		MTLTextureType TextureType { get; }

		[Export ("textureDataType")]
		MTLDataType TextureDataType { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("isDepthTexture")]
		bool IsDepthTexture { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("arrayLength")]
		nuint ArrayLength { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("bufferPointerType")]
		MTLPointerType BufferPointerType { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (MTLType))]
	interface MTLArrayType {
		[Export ("arrayLength")]
		nuint Length { get; }

		[Export ("elementType")]
		MTLDataType ElementType { get; }

		[Export ("stride")]
		nuint Stride { get; }

		[Export ("elementStructType")]
		[return: NullAllowed]
		MTLStructType ElementStructType ();

		[Export ("elementArrayType")]
		[return: NullAllowed]
		MTLArrayType ElementArrayType ();

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("argumentIndexStride")]
		nuint ArgumentIndexStride { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("elementTextureReferenceType")]
		MTLTextureReferenceType ElementTextureReferenceType { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("elementPointerType")]
		MTLPointerType ElementPointerType { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLCommandEncoder {
		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("endEncoding")]
		void EndEncoding ();

		[Abstract, Export ("insertDebugSignpost:")]
		void InsertDebugSignpost (string signpost);

		[Abstract, Export ("pushDebugGroup:")]
		void PushDebugGroup (string debugGroup);

		[Abstract, Export ("popDebugGroup")]
		void PopDebugGroup ();
	}

	interface IMTLBuffer { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLBuffer : MTLResource {
		[Abstract, Export ("length")]
		nuint Length { get; }

		[Abstract, Export ("contents")]
		IntPtr Contents { get; }

		[NoiOS, NoTV, MacCatalyst (15, 0)]
		[Abstract, Export ("didModifyRange:")]
		void DidModify (NSRange range);

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
#if NET || !MONOMAC
		[Abstract]
#endif
		[Export ("newTextureWithDescriptor:offset:bytesPerRow:")]
		[return: Release]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor, nuint offset, nuint bytesPerRow);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("addDebugMarker:range:")]
		void AddDebugMarker (string marker, NSRange range);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("removeAllDebugMarkers")]
		void RemoveAllDebugMarkers ();

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[NullAllowed, Export ("remoteStorageBuffer")]
		IMTLBuffer RemoteStorageBuffer { get; }

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("newRemoteBufferViewForDevice:")]
		[return: NullAllowed]
		[return: Release]
		IMTLBuffer CreateRemoteBuffer (IMTLDevice device);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("gpuAddress")]
		ulong GpuAddress { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLBufferLayoutDescriptor : NSCopying {
		[Export ("stride")]
		nuint Stride { get; set; }

		[Export ("stepFunction", ArgumentSemantic.Assign)]
		MTLStepFunction StepFunction { get; set; }

		[Export ("stepRate")]
		nuint StepRate { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLBufferLayoutDescriptorArray {
		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		MTLBufferLayoutDescriptor ObjectAtIndexedSubscript (nuint index);

		[Internal]
		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLBufferLayoutDescriptor bufferDesc, nuint index);
	}


	interface IMTLCommandBuffer { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLCommandBuffer {

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("commandQueue")]
		IMTLCommandQueue CommandQueue { get; }

		[Abstract, Export ("retainedReferences")]
		bool RetainedReferences { get; }

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("status")]
		MTLCommandBufferStatus Status { get; }

		[Abstract, Export ("error")]
		NSError Error { get; }

		[Abstract, Export ("enqueue")]
		void Enqueue ();

		[Abstract, Export ("commit")]
		void Commit ();

		[Abstract, Export ("addScheduledHandler:")]
		void AddScheduledHandler (Action<IMTLCommandBuffer> block);

		[Abstract, Export ("waitUntilScheduled")]
		void WaitUntilScheduled ();

		[Abstract, Export ("addCompletedHandler:")]
		void AddCompletedHandler (Action<IMTLCommandBuffer> block);

		[Abstract, Export ("waitUntilCompleted")]
		void WaitUntilCompleted ();

		[Abstract, Export ("blitCommandEncoder")]
		IMTLBlitCommandEncoder BlitCommandEncoder { get; }

		[Abstract, Export ("computeCommandEncoder")]
		IMTLComputeCommandEncoder ComputeCommandEncoder { get; }

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("computeCommandEncoderWithDispatchType:")]
		[return: NullAllowed]
		IMTLComputeCommandEncoder ComputeCommandEncoderDispatch (MTLDispatchType dispatchType);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("encodeWaitForEvent:value:")]
		void EncodeWait (IMTLEvent @event, ulong value);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("encodeSignalEvent:value:")]
		void EncodeSignal (IMTLEvent @event, ulong value);

		[Field ("MTLCommandBufferErrorDomain")]
		NSString ErrorDomain { get; }

		[Abstract]
		[Export ("parallelRenderCommandEncoderWithDescriptor:")]
		[return: NullAllowed]
		IMTLParallelRenderCommandEncoder CreateParallelRenderCommandEncoder (MTLRenderPassDescriptor renderPassDescriptor);

		[Abstract]
		[Export ("presentDrawable:")]
		void PresentDrawable (IMTLDrawable drawable);

		[Abstract]
		[Export ("presentDrawable:atTime:")]
		void PresentDrawable (IMTLDrawable drawable, double presentationTime);

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Export ("presentDrawable:afterMinimumDuration:")]
		void PresentDrawableAfter (IMTLDrawable drawable, double duration);

		[Abstract]
		[Export ("renderCommandEncoderWithDescriptor:")]
		IMTLRenderCommandEncoder CreateRenderCommandEncoder (MTLRenderPassDescriptor renderPassDescriptor);

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[MacCatalyst (13, 1)]
		[Export ("kernelStartTime")]
		double /* CFTimeInterval */ KernelStartTime { get; }

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[MacCatalyst (13, 1)]
		[Export ("kernelEndTime")]
		double /* CFTimeInterval */ KernelEndTime { get; }

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[MacCatalyst (13, 1)]
		[Export ("GPUStartTime")]
		double /* CFTimeInterval */ GpuStartTime { get; }

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[MacCatalyst (13, 1)]
		[Export ("GPUEndTime")]
		double /* CFTimeInterval */ GpuEndTime { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Export ("pushDebugGroup:")]
		void PushDebugGroup (string @string);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Export ("popDebugGroup")]
		void PopDebugGroup ();

#if NET
#if XAMCORE_5_0
		[MacCatalyst (14, 0), Mac (11, 0), iOS (13, 0), TV (16,0)]
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#elif !TVOS
		[MacCatalyst (14, 0), Mac (11, 0), iOS (13, 0), NoTV]
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#else
		[NoMacCatalyst, NoMac, NoiOS, TV (16,0)]
#endif

#else
		[MacCatalyst (14, 0), Mac (11, 0), iOS (13, 0), TV (16, 0)]
#endif
		[NullAllowed, Export ("resourceStateCommandEncoder")]
		IMTLResourceStateCommandEncoder ResourceStateCommandEncoder { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("errorOptions")]
		MTLCommandBufferErrorOption ErrorOptions { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("logs")]
		IMTLLogContainer Logs { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("computeCommandEncoderWithDescriptor:")]
		IMTLComputeCommandEncoder CreateComputeCommandEncoder (MTLComputePassDescriptor computePassDescriptor);

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("blitCommandEncoderWithDescriptor:")]
		IMTLBlitCommandEncoder CreateBlitCommandEncoder (MTLBlitPassDescriptor blitPassDescriptor);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("resourceStateCommandEncoderWithDescriptor:")]
		IMTLResourceStateCommandEncoder CreateResourceStateCommandEncoder (MTLResourceStatePassDescriptor resourceStatePassDescriptor);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("accelerationStructureCommandEncoder")]
		IMTLAccelerationStructureCommandEncoder CreateAccelerationStructureCommandEncoder ();

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("accelerationStructureCommandEncoderWithDescriptor:")]
		IMTLAccelerationStructureCommandEncoder CreateAccelerationStructureCommandEncoder (MTLAccelerationStructurePassDescriptor descriptor);
	}

	interface IMTLCommandQueue { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLCommandQueue {

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("commandBuffer")]
		[Autorelease]
		[return: NullAllowed]
		IMTLCommandBuffer CommandBuffer ();

		[Abstract, Export ("commandBufferWithUnretainedReferences")]
		[Autorelease]
		[return: NullAllowed]
		IMTLCommandBuffer CommandBufferWithUnretainedReferences ();

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'MTLCaptureScope' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'MTLCaptureScope' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'MTLCaptureScope' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MTLCaptureScope' instead.")]
		[Abstract, Export ("insertDebugCaptureBoundary")]
		void InsertDebugCaptureBoundary ();

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("commandBufferWithDescriptor:")]
		[return: NullAllowed]
		IMTLCommandBuffer CreateCommandBuffer (MTLCommandBufferDescriptor descriptor);
	}

	interface IMTLComputeCommandEncoder { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLComputeCommandEncoder : MTLCommandEncoder {

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("dispatchType")]
		MTLDispatchType DispatchType { get; }

		[Abstract, Export ("setComputePipelineState:")]
		void SetComputePipelineState (IMTLComputePipelineState state);

		[Abstract, Export ("setBuffer:offset:atIndex:")]
		void SetBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract, Export ("setTexture:atIndex:")]
		void SetTexture (IMTLTexture texture, nuint index);

		[Abstract, Export ("setSamplerState:atIndex:")]
		void SetSamplerState (IMTLSamplerState sampler, nuint index);

		[Abstract, Export ("setSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetSamplerState (IMTLSamplerState sampler, float /* float, not CGFloat */ lodMinClamp, float /* float, not CGFloat */ lodMaxClamp, nuint index);

		[Abstract, Export ("setThreadgroupMemoryLength:atIndex:")]
		void SetThreadgroupMemoryLength (nuint length, nuint index);

		[Abstract, Export ("dispatchThreadgroups:threadsPerThreadgroup:")]
		void DispatchThreadgroups (MTLSize threadgroupsPerGrid, MTLSize threadsPerThreadgroup);

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("dispatchThreadgroupsWithIndirectBuffer:indirectBufferOffset:threadsPerThreadgroup:")]
		void DispatchThreadgroups (IMTLBuffer indirectBuffer, nuint indirectBufferOffset, MTLSize threadsPerThreadgroup);

#if NET
		[Abstract]
		[Export ("setBuffers:offsets:withRange:")]
		void SetBuffers (IntPtr buffers, IntPtr offsets, NSRange range);
#else
		[Abstract]
		[Export ("setBuffers:offsets:withRange:")]
		void SetBuffers (IMTLBuffer [] buffers, IntPtr offsets, NSRange range);
#endif


		[Abstract]
		[Export ("setSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetSamplerStates (IMTLSamplerState [] samplers, IntPtr floatArrayPtrLodMinClamps, IntPtr floatArrayPtrLodMaxClamps, NSRange range);

		[Abstract]
		[Export ("setSamplerStates:withRange:")]
		void SetSamplerStates (IMTLSamplerState [] samplers, NSRange range);

		[Abstract]
		[Export ("setTextures:withRange:")]
		void SetTextures (IMTLTexture [] textures, NSRange range);

		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("setBufferOffset:atIndex:")]
		void SetBufferOffset (nuint offset, nuint index);

		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("setBytes:length:atIndex:")]
		void SetBytes (IntPtr bytes, nuint length, nuint index);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setStageInRegion:")]
		void SetStage (MTLRegion region);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setStageInRegionWithIndirectBuffer:indirectBufferOffset:")]
		void SetStageInRegion (IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("updateFence:")]
		void Update (IMTLFence fence);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("waitForFence:")]
		void Wait (IMTLFence fence);

		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("dispatchThreads:threadsPerThreadgroup:")]
		void DispatchThreads (MTLSize threadsPerGrid, MTLSize threadsPerThreadgroup);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("useResource:usage:")]
		void UseResource (IMTLResource resource, MTLResourceUsage usage);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("useResources:count:usage:")]
		void UseResources (IMTLResource [] resources, nuint count, MTLResourceUsage usage);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("useHeap:")]
		void UseHeap (IMTLHeap heap);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("useHeaps:count:")]
		void UseHeaps (IMTLHeap [] heaps, nuint count);

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
#if NET
		[Abstract]
#endif
		[Export ("setImageblockWidth:height:")]
		void SetImageblock (nuint width, nuint height);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("memoryBarrierWithScope:")]
		void MemoryBarrier (MTLBarrierScope scope);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("memoryBarrierWithResources:count:")]
		void MemoryBarrier (IMTLResource [] resources, nuint count);

		[TV (13, 0), Mac (11, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("executeCommandsInBuffer:withRange:")]
		void ExecuteCommands (IMTLIndirectCommandBuffer indirectCommandBuffer, NSRange executionRange);

		[TV (13, 0), Mac (11, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("executeCommandsInBuffer:indirectBuffer:indirectBufferOffset:")]
		void ExecuteCommands (IMTLIndirectCommandBuffer indirectCommandbuffer, IMTLBuffer indirectRangeBuffer, nuint indirectBufferOffset);

#if NET
		[Abstract]
#endif
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sampleCountersInBuffer:atSampleIndex:withBarrier:")]
#if NET
		void SampleCounters (IMTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#else
		[Obsolete ("Use the overload that takes an IMTLCounterSampleBuffer instead.")]
		void SampleCounters (MTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#endif

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setVisibleFunctionTable:atBufferIndex:")]
		void SetVisibleFunctionTable ([NullAllowed] IMTLVisibleFunctionTable visibleFunctionTable, nuint bufferIndex);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setVisibleFunctionTables:withBufferRange:")]
		void SetVisibleFunctionTables (IMTLVisibleFunctionTable [] visibleFunctionTables, NSRange range);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setIntersectionFunctionTable:atBufferIndex:")]
		void SetIntersectionFunctionTable ([NullAllowed] IMTLIntersectionFunctionTable intersectionFunctionTable, nuint bufferIndex);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setIntersectionFunctionTables:withBufferRange:")]
		void SetIntersectionFunctionTables (IMTLIntersectionFunctionTable [] intersectionFunctionTables, NSRange range);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setAccelerationStructure:atBufferIndex:")]
		void SetAccelerationStructure ([NullAllowed] IMTLAccelerationStructure accelerationStructure, nuint bufferIndex);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setBuffer:offset:attributeStride:atIndex:")]
		void SetBuffer (IMTLBuffer buffer, nuint offset, nuint stride, nuint index);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setBuffers:offsets:attributeStrides:withRange:")]
		void SetBuffers (IntPtr /* IMTLBuffer[] */ buffers, IntPtr /* nuint[] */ offsets, IntPtr /* nuint[] */ strides, NSRange range);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setBufferOffset:attributeStride:atIndex:")]
		void SetBufferOffset (nuint offset, nuint stride, nuint index);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setBytes:length:attributeStride:atIndex:")]
		void SetBytes (IntPtr bytes, nuint length, nuint stride, nuint index);

	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLComputePipelineReflection {

		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("arguments")]
#if NET
		MTLArgument [] Arguments { get; }
#else
		NSObject [] Arguments { get; }
#endif

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("bindings")]
		IMTLBinding [] Bindings { get; }
	}

	interface IMTLComputePipelineState { }
	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLComputePipelineState {
		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; }

		[Abstract, Export ("threadExecutionWidth")]
		nuint ThreadExecutionWidth { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("label")]
		string Label { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("staticThreadgroupMemoryLength")]
		nuint StaticThreadgroupMemoryLength { get; }

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("imageblockMemoryLengthForDimensions:")]
		nuint GetImageblockMemoryLength (MTLSize imageblockDimensions);

		[TV (13, 0), Mac (11, 0), iOS (13, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; }

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("functionHandleWithFunction:")]
		IMTLFunctionHandle CreateFunctionHandle (IMTLFunction function);

#if NET

#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("newComputePipelineStateWithAdditionalBinaryFunctions:error:")]
		[return: Release]
		IMTLComputePipelineState CreateComputePipelineState (IMTLFunction [] functions, [NullAllowed] out NSError error);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("newVisibleFunctionTableWithDescriptor:")]
		[return: Release]
		IMTLVisibleFunctionTable CreateVisibleFunctionTable (MTLVisibleFunctionTableDescriptor descriptor);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("newIntersectionFunctionTableWithDescriptor:")]
		[return: Release]
		IMTLIntersectionFunctionTable CreateIntersectionFunctionTable (MTLIntersectionFunctionTableDescriptor descriptor);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("gpuResourceID")]
		MTLResourceId GpuResourceId { get; }
	}

	interface IMTLBlitCommandEncoder { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLBlitCommandEncoder : MTLCommandEncoder {

		[NoiOS, NoTV, MacCatalyst (15, 0)]
		[Abstract, Export ("synchronizeResource:")]
		void Synchronize (IMTLResource resource);

		[NoiOS, NoTV, MacCatalyst (15, 0)]
		[Abstract, Export ("synchronizeTexture:slice:level:")]
		void Synchronize (IMTLTexture texture, nuint slice, nuint level);

		[Abstract, Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, MTLOrigin sourceOrigin, MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel, MTLOrigin destinationOrigin);

		[Abstract, Export ("copyFromBuffer:sourceOffset:sourceBytesPerRow:sourceBytesPerImage:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, nuint sourceBytesPerRow, nuint sourceBytesPerImage, MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel, MTLOrigin destinationOrigin);

		[MacCatalyst (13, 1)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("copyFromBuffer:sourceOffset:sourceBytesPerRow:sourceBytesPerImage:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:options:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, nuint sourceBytesPerRow, nuint sourceBytesPerImage, MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel, MTLOrigin destinationOrigin, MTLBlitOption options);

		[Abstract, Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toBuffer:destinationOffset:destinationBytesPerRow:destinationBytesPerImage:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, MTLOrigin sourceOrigin, MTLSize sourceSize, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint destinatinBytesPerRow, nuint destinationBytesPerImage);

		[MacCatalyst (13, 1)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toBuffer:destinationOffset:destinationBytesPerRow:destinationBytesPerImage:options:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, MTLOrigin sourceOrigin, MTLSize sourceSize, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint destinatinBytesPerRow, nuint destinationBytesPerImage, MTLBlitOption options);

		[Abstract, Export ("generateMipmapsForTexture:")]
		void GenerateMipmapsForTexture (IMTLTexture texture);

		[Abstract, Export ("fillBuffer:range:value:")]
		void FillBuffer (IMTLBuffer buffer, NSRange range, byte value);

		[Abstract, Export ("copyFromBuffer:sourceOffset:toBuffer:destinationOffset:size:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint size);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("updateFence:")]
		void Update (IMTLFence fence);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("waitForFence:")]
		void Wait (IMTLFence fence);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("optimizeContentsForGPUAccess:")]
		void OptimizeContentsForGpuAccess (IMTLTexture texture);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("optimizeContentsForGPUAccess:slice:level:")]
		void OptimizeContentsForGpuAccess (IMTLTexture texture, nuint slice, nuint level);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("optimizeContentsForCPUAccess:")]
		void OptimizeContentsForCpuAccess (IMTLTexture texture);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("optimizeContentsForCPUAccess:slice:level:")]
		void OptimizeContentsForCpuAccess (IMTLTexture texture, nuint slice, nuint level);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("resetCommandsInBuffer:withRange:")]
		void ResetCommands (IMTLIndirectCommandBuffer buffer, NSRange range);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("copyIndirectCommandBuffer:sourceRange:destination:destinationIndex:")]
		void Copy (IMTLIndirectCommandBuffer source, NSRange sourceRange, IMTLIndirectCommandBuffer destination, nuint destinationIndex);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("optimizeIndirectCommandBuffer:withRange:")]
		void Optimize (IMTLIndirectCommandBuffer indirectCommandBuffer, NSRange range);

#if NET && !__MACOS__ && !__MACCATALYST__ && !TVOS
		[Abstract]
#endif
		[Mac (11, 0), TV (16, 0), iOS (13, 0), MacCatalyst (15, 0)]
		[Export ("getTextureAccessCounters:region:mipLevel:slice:resetCounters:countersBuffer:countersBufferOffset:")]
		void GetTextureAccessCounters (IMTLTexture texture, MTLRegion region, nuint mipLevel, nuint slice, bool resetCounters, IMTLBuffer countersBuffer, nuint countersBufferOffset);

#if NET && !__MACOS__ && !__MACCATALYST__ && !TVOS
		[Abstract]
#endif
		[Mac (11, 0), TV (16, 0), iOS (13, 0), MacCatalyst (15, 0)]
		[Export ("resetTextureAccessCounters:region:mipLevel:slice:")]
		void ResetTextureAccessCounters (IMTLTexture texture, MTLRegion region, nuint mipLevel, nuint slice);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("copyFromTexture:sourceSlice:sourceLevel:toTexture:destinationSlice:destinationLevel:sliceCount:levelCount:")]
		void Copy (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel, nuint sliceCount, nuint levelCount);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("copyFromTexture:toTexture:")]
		void Copy (IMTLTexture sourceTexture, IMTLTexture destinationTexture);

#if NET
		[Abstract]
#endif
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sampleCountersInBuffer:atSampleIndex:withBarrier:")]
#if NET
		void SampleCounters (IMTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#else
		void SampleCounters (MTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#endif

#if NET
		[Abstract]
#endif
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("resolveCounters:inRange:destinationBuffer:destinationOffset:")]
#if NET
		void ResolveCounters (IMTLCounterSampleBuffer sampleBuffer, NSRange range, IMTLBuffer destinationBuffer, nuint destinationOffset);
#else
		void ResolveCounters (MTLCounterSampleBuffer sampleBuffer, NSRange range, IMTLBuffer destinationBuffer, nuint destinationOffset);
#endif
	}

	interface IMTLFence { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	interface MTLFence {
		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	interface IMTLDevice { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLDevice {

		[Abstract, Export ("name")]
		string Name { get; }

#if NET
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[MacCatalyst (13, 1)]
		[Export ("maxThreadsPerThreadgroup")]
		MTLSize MaxThreadsPerThreadgroup { get; }

#if NET
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[MacCatalyst (15, 0)]
		[NoiOS]
		[NoTV]
		[Export ("lowPower")]
		bool LowPower { [Bind ("isLowPower")] get; }

#if NET
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[MacCatalyst (15, 0)]
		[NoiOS]
		[NoTV]
		[Export ("headless")]
		bool Headless { [Bind ("isHeadless")] get; }

		[iOS (17, 0), TV (17, 0), NoWatch, MacCatalyst (15, 0)]
#if NET
		[Abstract]
#endif
		[Export ("recommendedMaxWorkingSetSize")]
		ulong RecommendedMaxWorkingSetSize { get; }

#if NET
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[MacCatalyst (15, 0)]
		[NoiOS]
		[NoTV]
		[Export ("depth24Stencil8PixelFormatSupported")]
		bool Depth24Stencil8PixelFormatSupported { [Bind ("isDepth24Stencil8PixelFormatSupported")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("heapTextureSizeAndAlignWithDescriptor:")]
		MTLSizeAndAlign GetHeapTextureSizeAndAlign (MTLTextureDescriptor desc);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("heapBufferSizeAndAlignWithLength:options:")]
		MTLSizeAndAlign GetHeapBufferSizeAndAlignWithLength (nuint length, MTLResourceOptions options);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newHeapWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLHeap CreateHeap (MTLHeapDescriptor descriptor);

		[Abstract, Export ("newCommandQueue")]
		[return: NullAllowed]
		[return: Release]
		IMTLCommandQueue CreateCommandQueue ();

		[Abstract, Export ("newCommandQueueWithMaxCommandBufferCount:")]
		[return: NullAllowed]
		[return: Release]
		IMTLCommandQueue CreateCommandQueue (nuint maxCommandBufferCount);

		[Abstract, Export ("newBufferWithLength:options:")]
		[return: NullAllowed]
		[return: Release]
		IMTLBuffer CreateBuffer (nuint length, MTLResourceOptions options);

		[Abstract, Export ("newBufferWithBytes:length:options:")]
		[return: NullAllowed]
		[return: Release]
		IMTLBuffer CreateBuffer (IntPtr pointer, nuint length, MTLResourceOptions options);

		[Abstract, Export ("newBufferWithBytesNoCopy:length:options:deallocator:")]
		[return: NullAllowed]
		[return: Release]
		IMTLBuffer CreateBufferNoCopy (IntPtr pointer, nuint length, MTLResourceOptions options, MTLDeallocator deallocator);

		[Abstract, Export ("newDepthStencilStateWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLDepthStencilState CreateDepthStencilState (MTLDepthStencilDescriptor descriptor);

		[Abstract, Export ("newTextureWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor);

#if NET
		[Abstract]
#endif
		[NoWatch]
		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[return: Release]
		[Export ("newTextureWithDescriptor:iosurface:plane:")]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor, IOSurface.IOSurface iosurface, nuint plane);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newSharedTextureWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateSharedTexture (MTLTextureDescriptor descriptor);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newSharedTextureWithHandle:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateSharedTexture (MTLSharedTextureHandle sharedHandle);

		[Abstract, Export ("newSamplerStateWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLSamplerState CreateSamplerState (MTLSamplerDescriptor descriptor);

		[Abstract, Export ("newDefaultLibrary")]
		[return: Release]
		IMTLLibrary CreateDefaultLibrary ();

		[Abstract, Export ("newLibraryWithFile:error:")]
		[return: Release]
		IMTLLibrary CreateLibrary (string filepath, out NSError error);

#if !NET
		[Abstract, Export ("newLibraryWithData:error:")]
		[return: Release]
		[Obsolete ("Use the overload that take a 'DispatchData' instead.")]
		IMTLLibrary CreateLibrary (NSObject data, out NSError error);
#endif

#if NET
		[Abstract]
		[Export ("newLibraryWithData:error:")]
		[return: Release]
		IMTLLibrary CreateLibrary (DispatchData data, out NSError error);
#endif

		[Abstract, Export ("newLibraryWithSource:options:error:")]
		[return: Release]
		IMTLLibrary CreateLibrary (string source, MTLCompileOptions options, out NSError error);

		[Abstract, Export ("newLibraryWithSource:options:completionHandler:")]
		[Async]
		void CreateLibrary (string source, MTLCompileOptions options, Action<IMTLLibrary, NSError> completionHandler);

#if NET
		[Abstract]
#endif
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("newDefaultLibraryWithBundle:error:")]
		[return: Release]
		[return: NullAllowed]
#if NET
		IMTLLibrary CreateDefaultLibrary (NSBundle bundle, out NSError error);
#else
		[Obsolete ("Use 'CreateDefaultLibrary' instead.")]
		IMTLLibrary CreateLibrary (NSBundle bundle, out NSError error);
#endif

		[Abstract, Export ("newRenderPipelineStateWithDescriptor:error:")]
		[return: Release]
		IMTLRenderPipelineState CreateRenderPipelineState (MTLRenderPipelineDescriptor descriptor, out NSError error);

		[Abstract, Export ("newRenderPipelineStateWithDescriptor:completionHandler:")]
		void CreateRenderPipelineState (MTLRenderPipelineDescriptor descriptor, Action<IMTLRenderPipelineState, NSError> completionHandler);

		[Abstract]
		[Export ("newRenderPipelineStateWithDescriptor:options:reflection:error:")]
		[return: Release]
		IMTLRenderPipelineState CreateRenderPipelineState (MTLRenderPipelineDescriptor descriptor, MTLPipelineOption options, out MTLRenderPipelineReflection reflection, out NSError error);

		[Abstract]
		[Export ("newRenderPipelineStateWithDescriptor:options:completionHandler:")]
		void CreateRenderPipelineState (MTLRenderPipelineDescriptor descriptor, MTLPipelineOption options, Action<IMTLRenderPipelineState, MTLRenderPipelineReflection, NSError> completionHandler);

		[Abstract]
		[Export ("newComputePipelineStateWithFunction:options:reflection:error:")]
		[return: Release]
		IMTLComputePipelineState CreateComputePipelineState (IMTLFunction computeFunction, MTLPipelineOption options, out MTLComputePipelineReflection reflection, out NSError error);

		[Abstract]
		[Export ("newComputePipelineStateWithFunction:completionHandler:")]
		void CreateComputePipelineState (IMTLFunction computeFunction, Action<IMTLComputePipelineState, NSError> completionHandler);

		[Abstract, Export ("newComputePipelineStateWithFunction:error:")]
		[return: Release]
		IMTLComputePipelineState CreateComputePipelineState (IMTLFunction computeFunction, out NSError error);

		[Abstract, Export ("newComputePipelineStateWithFunction:options:completionHandler:")]
		void CreateComputePipelineState (IMTLFunction computeFunction, MTLPipelineOption options, Action<IMTLComputePipelineState, MTLComputePipelineReflection, NSError> completionHandler);

		[MacCatalyst (13, 1)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("newComputePipelineStateWithDescriptor:options:reflection:error:")]
		[return: Release]
		IMTLComputePipelineState CreateComputePipelineState (MTLComputePipelineDescriptor descriptor, MTLPipelineOption options, out MTLComputePipelineReflection reflection, out NSError error);

		[MacCatalyst (13, 1)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("newComputePipelineStateWithDescriptor:options:completionHandler:")]
		void CreateComputePipelineState (MTLComputePipelineDescriptor descriptor, MTLPipelineOption options, MTLNewComputePipelineStateWithReflectionCompletionHandler completionHandler);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newFence")]
		[return: Release]
		IMTLFence CreateFence ();

		[Abstract, Export ("supportsFeatureSet:")]
		bool SupportsFeatureSet (MTLFeatureSet featureSet);

		[MacCatalyst (13, 1)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("supportsTextureSampleCount:")]
		bool SupportsTextureSampleCount (nuint sampleCount);

		[NoiOS, NoWatch, NoTV, MacCatalyst (15, 0)]
#if NET
		[Abstract]
#endif
		[Export ("removable")]
		bool Removable { [Bind ("isRemovable")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("readWriteTextureSupport")]
		MTLReadWriteTextureTier ReadWriteTextureSupport { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("argumentBuffersSupport")]
		MTLArgumentBuffersTier ArgumentBuffersSupport { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("rasterOrderGroupsSupported")]
		bool RasterOrderGroupsSupported { [Bind ("areRasterOrderGroupsSupported")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newLibraryWithURL:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLLibrary CreateLibrary (NSUrl url, [NullAllowed] out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("minimumLinearTextureAlignmentForPixelFormat:")]
		nuint GetMinimumLinearTextureAlignment (MTLPixelFormat format);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("minimumTextureBufferAlignmentForPixelFormat:")]
		nuint GetMinimumTextureBufferAlignment (MTLPixelFormat format);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("maxThreadgroupMemoryLength")]
		nuint MaxThreadgroupMemoryLength { get; }

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("maxArgumentBufferSamplerCount")]
		nuint MaxArgumentBufferSamplerCount { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("programmableSamplePositionsSupported")]
		bool ProgrammableSamplePositionsSupported { [Bind ("areProgrammableSamplePositionsSupported")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("getDefaultSamplePositions:count:")]
		void GetDefaultSamplePositions (IntPtr positions, nuint count);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newArgumentEncoderWithArguments:")]
		[return: NullAllowed]
		[return: Release]
		IMTLArgumentEncoder CreateArgumentEncoder (MTLArgumentDescriptor [] arguments);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newIndirectCommandBufferWithDescriptor:maxCommandCount:options:")]
		[return: NullAllowed]
		[return: Release]
		IMTLIndirectCommandBuffer CreateIndirectCommandBuffer (MTLIndirectCommandBufferDescriptor descriptor, nuint maxCount, MTLResourceOptions options);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[return: NullAllowed]
		[return: Release]
		[Export ("newEvent")]
		IMTLEvent CreateEvent ();

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[return: NullAllowed]
		[return: Release]
		[Export ("newSharedEvent")]
		IMTLSharedEvent CreateSharedEvent ();

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newSharedEventWithHandle:")]
		[return: NullAllowed]
		[return: Release]
		IMTLSharedEvent CreateSharedEvent (MTLSharedEventHandle sharedEventHandle);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("maxBufferLength")]
		nuint MaxBufferLength { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("registryID")]
		ulong RegistryId { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("currentAllocatedSize")]
		nuint CurrentAllocatedSize { get; }

#if false // https://bugzilla.xamarin.com/show_bug.cgi?id=59342
		[NoiOS, NoTV, NoWatch]
		[Notification]
		[Field ("MTLDeviceWasAddedNotification")]
		NSString DeviceWasAdded { get; }

		[NoiOS, NoTV, NoWatch]
		[Notification]
		[Field ("MTLDeviceRemovalRequestedNotification")]
		NSString DeviceRemovalRequested { get; }

		[NoiOS, NoTV, NoWatch]
		[Notification]
		[Field ("MTLDeviceWasRemovedNotification")]
		NSString DeviceWasRemoved { get; }
#endif

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
#if NET
		[Abstract]
#endif
		[Export ("newRenderPipelineStateWithTileDescriptor:options:reflection:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLRenderPipelineState CreateRenderPipelineState (MTLTileRenderPipelineDescriptor descriptor, MTLPipelineOption options, [NullAllowed] out MTLRenderPipelineReflection reflection, [NullAllowed] out NSError error);

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
#if NET
		[Abstract]
#endif
		[Export ("newRenderPipelineStateWithTileDescriptor:options:completionHandler:")]
		void CreateRenderPipelineState (MTLTileRenderPipelineDescriptor descriptor, MTLPipelineOption options, MTLNewRenderPipelineStateWithReflectionCompletionHandler completionHandler);

#if NET
#if XAMCORE_5_0
		[MacCatalyst (13, 4), TV (16,0), iOS (13, 0)]
		[Abstract]
#elif !TVOS
		[MacCatalyst (13, 4), NoTV, iOS (13, 0)]
		[Abstract]
#else
		[NoMac, NoMacCatalyst, TV (16,0), NoiOS]
#endif

#else
		[MacCatalyst (13, 4), TV (16, 0), iOS (13, 0)]
#endif
		[Export ("supportsVertexAmplificationCount:")]
		bool SupportsVertexAmplification (nuint count);

#if NET
#if XAMCORE_5_0
		[MacCatalyst (13, 4), TV (16,0), iOS (13, 0)]
		[Abstract]
#elif !TVOS
		[MacCatalyst (13, 4), NoTV, iOS (13, 0)]
		[Abstract]
#else
		[NoMac, NoMacCatalyst, TV (16,0), NoiOS]
#endif

#else
		[MacCatalyst (13, 4), TV (16, 0), iOS (13, 0)]
#endif
		[Export ("supportsRasterizationRateMapWithLayerCount:")]
		bool SupportsRasterizationRateMap (nuint layerCount);

#if NET
#if XAMCORE_5_0
		[MacCatalyst (14, 0), Mac (11, 0), TV (16,0), iOS (13, 0)]
		[Abstract]
#elif !TVOS
		[MacCatalyst (14, 0), Mac (11, 0), NoTV, iOS (13, 0)]
		[Abstract]
#else
		[NoMacCatalyst, NoMac, TV (16,0), NoiOS]
#endif

#else
		[MacCatalyst (14, 0), Mac (11, 0), TV (16, 0), iOS (13, 0)]
#endif
		[Export ("sparseTileSizeWithTextureType:pixelFormat:sampleCount:")]
		MTLSize GetSparseTileSize (MTLTextureType textureType, MTLPixelFormat pixelFormat, nuint sampleCount);

#if NET
#if XAMCORE_5_0
		[MacCatalyst (14, 0), Mac (11, 0), TV (16,0), iOS (13, 0)]
		[Abstract]
#elif !TVOS
		[MacCatalyst (14, 0), Mac (11, 0), NoTV, iOS (13, 0)]
		[Abstract]
#else
		[NoMacCatalyst, NoMac, TV (16,0), NoiOS]
#endif

#else
		[MacCatalyst (14, 0), Mac (11, 0), TV (16, 0), iOS (13, 0)]
#endif
		[Export ("sparseTileSizeInBytes")]
		nuint SparseTileSizeInBytes { get; }

#if NET
#if XAMCORE_5_0
		[MacCatalyst (13, 4), TV (16,0), iOS (13, 0)]
		[Abstract]
#elif !TVOS
		[MacCatalyst (13, 4), NoTV, iOS (13, 0)]
		[Abstract]
#else
		[NoMac, NoMacCatalyst, TV (16,0), NoiOS]
#endif

#else
		[MacCatalyst (13, 4), TV (16, 0), iOS (13, 0)]
#endif
		[Export ("newRasterizationRateMapWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLRasterizationRateMap CreateRasterizationRateMap (MTLRasterizationRateMapDescriptor descriptor);

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Mac (11, 0), TV (16, 0), iOS (13, 0)]
		[Export ("convertSparseTileRegions:toPixelRegions:withTileSize:numRegions:")]
		void ConvertSparseTileRegions (IntPtr tileRegions, IntPtr pixelRegions, MTLSize tileSize, nuint numRegions);

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Mac (11, 0), TV (16, 0), iOS (13, 0)]
		[Export ("convertSparsePixelRegions:toTileRegions:withTileSize:alignmentMode:numRegions:")]
		void ConvertSparsePixelRegions (IntPtr pixelRegions, IntPtr tileRegions, MTLSize tileSize, MTLSparseTextureRegionAlignmentMode mode, nuint numRegions);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("hasUnifiedMemory")]
		bool HasUnifiedMemory { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("supportsFamily:")]
		bool SupportsFamily (MTLGpuFamily gpuFamily);

#if NET
		[Abstract]
#endif
		[iOS (14, 0), NoTV, NoMacCatalyst]
		[Export ("barycentricCoordsSupported")]
		bool BarycentricCoordsSupported { [Bind ("areBarycentricCoordsSupported")] get; }

#if NET
#if XAMCORE_5_0
		[iOS (14, 0), TV (16,0), NoMacCatalyst]
		[Abstract]
#elif !TVOS
		[iOS (14, 0), NoTV, NoMacCatalyst]
		[Abstract]
#else
		[NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[iOS (14, 0), TV (16, 0), NoMacCatalyst]
#endif
		[Export ("supportsShaderBarycentricCoordinates")]
		bool SupportsShaderBarycentricCoordinates { get; }

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("peerIndex")]
		uint PeerIndex { get; }

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("peerCount")]
		uint PeerCount { get; }

#if NET
		[Abstract]
#endif
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("counterSets")]
#if NET
		IMTLCounterSet[] CounterSets { get; }
#else
		[Obsolete ("Use 'GetIMTLCounterSets' instead.")]
		MTLCounterSet [] CounterSets { get; }
#endif

#if NET
		[Abstract]
#endif
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("newCounterSampleBufferWithDescriptor:error:")]
		[return: NullAllowed]
		[return: Release]
#if NET
		IMTLCounterSampleBuffer CreateCounterSampleBuffer (MTLCounterSampleBufferDescriptor descriptor, [NullAllowed] out NSError error);
#else
		[Obsolete ("Use 'CreateIMTLCounterSampleBuffer' instead.")]
		MTLCounterSampleBuffer CreateCounterSampleBuffer (MTLCounterSampleBufferDescriptor descriptor, [NullAllowed] out NSError error);
#endif

#if NET
		[Abstract]
#endif
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sampleTimestamps:gpuTimestamp:")]
		void GetSampleTimestamps (nuint cpuTimestamp, nuint gpuTimestamp);

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("peerGroupID")]
		ulong PeerGroupId { get; }

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("maxTransferRate")]
		ulong MaxTransferRate { get; }

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("location")]
		MTLDeviceLocation Location { get; }

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("locationNumber")]
		nuint LocationNumber { get; }

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), TV (16,0), iOS (14, 5), MacCatalyst (14, 5)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), NoTV, iOS (14, 5), MacCatalyst (14, 5)]
		[Abstract]
#else
		[NoMac, TV (16,0), NoiOS, NoMacCatalyst]
#endif

#else
		[Mac (11, 0), TV (16, 0), iOS (14, 5), MacCatalyst (14, 5)]
#endif
		[Export ("supports32BitFloatFiltering")]
		bool Supports32BitFloatFiltering { get; }

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), TV (16,0), iOS (14, 5), MacCatalyst (14, 5)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), NoTV, iOS (14, 5), MacCatalyst (14, 5)]
		[Abstract]
#else
		[NoMac, TV (16,0), NoiOS, NoMacCatalyst]
#endif
#else
		[Mac (11, 0), TV (16, 0), iOS (14, 5), MacCatalyst (14, 5)]
#endif
		[Export ("supports32BitMSAA")]
		bool Supports32BitMsaa { get; }

		[Mac (11, 0), iOS (16, 4), TV (16, 4), NoMacCatalyst]
		[NoMacCatalyst]
#if NET
		[Abstract]
#endif
		[Export ("supportsBCTextureCompression")]
		bool SupportsBCTextureCompression { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("supportsPullModelInterpolation")]
		bool SupportsPullModelInterpolation { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("supportsCounterSampling:")]
		bool SupportsCounterSampling (MTLCounterSamplingPoint samplingPoint);

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("supportsDynamicLibraries")]
		bool SupportsDynamicLibraries { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("newDynamicLibrary:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLDynamicLibrary CreateDynamicLibrary (IMTLLibrary library, [NullAllowed] out NSError error);

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("newDynamicLibraryWithURL:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLDynamicLibrary CreateDynamicLibrary (NSUrl url, [NullAllowed] out NSError error);

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("newBinaryArchiveWithDescriptor:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLBinaryArchive CreateBinaryArchive (MTLBinaryArchiveDescriptor descriptor, [NullAllowed] out NSError error);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (!6,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("supportsRaytracing")]
		bool SupportsRaytracing { get; }

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("accelerationStructureSizesWithDescriptor:")]
#pragma warning disable 0618 // warning CS0618: 'MTLAccelerationStructureSizes' is obsolete: 'This API is not available on this platform.'
		MTLAccelerationStructureSizes CreateAccelerationStructureSizes (MTLAccelerationStructureDescriptor descriptor);
#pragma warning restore

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("newAccelerationStructureWithSize:")]
		[return: NullAllowed]
		[return: Release]
		IMTLAccelerationStructure CreateAccelerationStructure (nuint size);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif
		
#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("newAccelerationStructureWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLAccelerationStructure CreateAccelerationStructure (MTLAccelerationStructureDescriptor descriptor);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("supportsFunctionPointers")]
		bool SupportsFunctionPointers { get; }

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), TV (16,0), iOS (14, 5), MacCatalyst (14, 5)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), NoTV, iOS (14, 5), MacCatalyst (14, 5)]
		[Abstract]
#else
		[NoMac, TV (16,0), NoiOS, NoMacCatalyst]
#endif

#else
		[Mac (11, 0), TV (16, 0), iOS (14, 5), MacCatalyst (14, 5)]
#endif
		[Export ("supportsQueryTextureLOD")]
		bool SupportsQueryTextureLod { get; }

#if NET
		[Abstract]
#endif
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("supportsRenderDynamicLibraries")]
		bool SupportsRenderDynamicLibraries { get; }

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16,0)]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Abstract]
#else
		[NoMac, NoiOS, NoMacCatalyst, TV (16,0)]
#endif

#else
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
#endif
		[Export ("supportsRaytracingFromRender")]
		bool SupportsRaytracingFromRender { get; }

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16,0)]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Abstract]
#else
		[NoMac, NoiOS, NoMacCatalyst, TV (16,0)]
#endif

#else
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
#endif
		[Export ("supportsPrimitiveMotionBlur")]
		bool SupportsPrimitiveMotionBlur { get; }

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16,0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV, NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, NoMacCatalyst, TV (16,0), NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0), NoWatch]
#endif
		[Export ("supportsFunctionPointersFromRender")]
		bool SupportsFunctionPointersFromRender { get; }

#if NET
		[Abstract]
#endif
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("newLibraryWithStitchedDescriptor:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLLibrary CreateLibrary (MTLStitchedLibraryDescriptor descriptor, [NullAllowed] out NSError error);

#if NET
		[Abstract]
#endif
		[Async]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("newLibraryWithStitchedDescriptor:completionHandler:")]
		void CreateLibrary (MTLStitchedLibraryDescriptor descriptor, Action<IMTLLibrary, NSError> completionHandler);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("architecture")]
		MTLArchitecture Architecture { get; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("newIOFileHandleWithURL:error:")]
		[return: NullAllowed]
		IMTLIOFileHandle CreateIOFileHandle (NSUrl url, [NullAllowed] out NSError error);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("newIOFileHandleWithURL:compressionMethod:error:")]
		[return: NullAllowed]
		IMTLIOFileHandle CreateIOFileHandle (NSUrl url, MTLIOCompressionMethod compressionMethod, [NullAllowed] out NSError error);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("heapAccelerationStructureSizeAndAlignWithDescriptor:")]
		MTLSizeAndAlign GetHeapAccelerationStructureSizeAndAlign (MTLAccelerationStructureDescriptor descriptor);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("heapAccelerationStructureSizeAndAlignWithSize:")]
		MTLSizeAndAlign GetHeapAccelerationStructureSizeAndAlign (nuint size);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("newArgumentEncoderWithBufferBinding:")]
		IMTLArgumentEncoder CreateArgumentEncoder (IMTLBufferBinding bufferBinding);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("newIOCommandQueueWithDescriptor:error:")]
		[return: NullAllowed]
		IMTLIOCommandQueue CreateIOCommandQueue (MTLIOCommandQueueDescriptor descriptor, [NullAllowed] out NSError error);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("newRenderPipelineStateWithMeshDescriptor:options:reflection:error:")]
		[return: NullAllowed]
		IMTLRenderPipelineState CreateRenderPipelineState (MTLMeshRenderPipelineDescriptor descriptor, MTLPipelineOption options, [NullAllowed] out MTLRenderPipelineReflection reflection, [NullAllowed] out NSError error);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("newRenderPipelineStateWithMeshDescriptor:options:completionHandler:")]
		void CreateRenderPipelineState (MTLMeshRenderPipelineDescriptor descriptor, MTLPipelineOption options, MTLNewRenderPipelineStateWithReflectionCompletionHandler completionHandler);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("sparseTileSizeInBytesForSparsePageSize:")]
		nuint GetSparseTileSizeInBytes (MTLSparsePageSize sparsePageSize);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("sparseTileSizeWithTextureType:pixelFormat:sampleCount:sparsePageSize:")]
		MTLSize GetSparseTileSize (MTLTextureType textureType, MTLPixelFormat pixelFormat, nuint sampleCount, MTLSparsePageSize sparsePageSize);

		[NoiOS, Mac (13, 3), NoTV, NoMacCatalyst]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("maximumConcurrentCompilationTaskCount")]
		nuint MaximumConcurrentCompilationTaskCount { get; }

		[NoiOS, Mac (13, 3), NoTV, NoMacCatalyst]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("shouldMaximizeConcurrentCompilation")]
		bool ShouldMaximizeConcurrentCompilation { get; set; }

	}

	interface IMTLDrawable { }
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface MTLDrawable {
		[Abstract, Export ("present")]
		void Present ();

		[Abstract, Export ("presentAtTime:")]
		void Present (double presentationTime);

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Export ("presentAfterMinimumDuration:")]
		void PresentAfter (double duration);

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Export ("addPresentedHandler:")]
		void AddPresentedHandler (Action<IMTLDrawable> block);

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Export ("presentedTime")]
		double /* CFTimeInterval */ PresentedTime { get; }

#if NET
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Export ("drawableID")]
#if NET
		nuint DrawableId { get; }
#else
		nuint DrawableID { get; }
#endif
	}

	interface IMTLTexture { }

	// Apple added several new *required* members in iOS 9,
	// but that breaks our binary compat, so we can't do that in our existing code.
	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLTexture : MTLResource {
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Abstract, Export ("rootResource")]
		IMTLResource RootResource { get; }

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("parentTexture")]
		IMTLTexture ParentTexture { get; }

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("parentRelativeLevel")]
		nuint ParentRelativeLevel { get; }

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("parentRelativeSlice")]
		nuint ParentRelativeSlice { get; }

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("buffer")]
		IMTLBuffer Buffer { get; }

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("bufferOffset")]
		nuint BufferOffset { get; }

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("bufferBytesPerRow")]
		nuint BufferBytesPerRow { get; }

		[Abstract, Export ("textureType")]
		MTLTextureType TextureType { get; }

		[Abstract, Export ("pixelFormat")]
		MTLPixelFormat PixelFormat { get; }

		[Abstract, Export ("width")]
		nuint Width { get; }

		[Abstract, Export ("height")]
		nuint Height { get; }

		[Abstract, Export ("depth")]
		nuint Depth { get; }

		[Abstract, Export ("mipmapLevelCount")]
		nuint MipmapLevelCount { get; }

		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Abstract, Export ("sampleCount")]
		nuint SampleCount { get; }

		[Abstract, Export ("arrayLength")]
		nuint ArrayLength { get; }

		[Abstract, Export ("framebufferOnly")]
		bool FramebufferOnly { [Bind ("isFramebufferOnly")] get; }

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("allowGPUOptimizedContents")]
		bool AllowGpuOptimizedContents { get; }

#if NET
#if XAMCORE_5_0
		[Mac (12,5), iOS (15, 0), NoMacCatalyst, TV (16,0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12,5), iOS (15, 0), NoMacCatalyst, NoTV, NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, NoMacCatalyst, TV (16,0), NoWatch]
#endif

#else
		[Mac (12, 5), iOS (15, 0), NoMacCatalyst, TV (16, 0), NoWatch]
#endif
		[Export ("compressionType")]
		MTLTextureCompressionType CompressionType { get; }

		[Abstract, Export ("newTextureViewWithPixelFormat:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateTextureView (MTLPixelFormat pixelFormat);

#if NET
		[Abstract]
#endif
		[Export ("usage")]
		MTLTextureUsage Usage { get; }

#if NET
		[Abstract]
#endif
		[Export ("newTextureViewWithPixelFormat:textureType:levels:slices:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateTextureView (MTLPixelFormat pixelFormat, MTLTextureType textureType, NSRange levelRange, NSRange sliceRange);

		[Abstract]
		[Export ("getBytes:bytesPerRow:bytesPerImage:fromRegion:mipmapLevel:slice:")]
		void GetBytes (IntPtr pixelBytes, nuint bytesPerRow, nuint bytesPerImage, MTLRegion region, nuint level, nuint slice);

		[Abstract]
		[Export ("getBytes:bytesPerRow:fromRegion:mipmapLevel:")]
		void GetBytes (IntPtr pixelBytes, nuint bytesPerRow, MTLRegion region, nuint level);

		[Abstract]
		[Export ("replaceRegion:mipmapLevel:slice:withBytes:bytesPerRow:bytesPerImage:")]
		void ReplaceRegion (MTLRegion region, nuint level, nuint slice, IntPtr pixelBytes, nuint bytesPerRow, nuint bytesPerImage);

		[Abstract]
		[Export ("replaceRegion:mipmapLevel:withBytes:bytesPerRow:")]
		void ReplaceRegion (MTLRegion region, nuint level, IntPtr pixelBytes, nuint bytesPerRow);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("iosurface")]
		IOSurface.IOSurface IOSurface { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("iosurfacePlane")]
		nuint IOSurfacePlane { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("shareable")]
		bool Shareable { [Bind ("isShareable")] get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[return: NullAllowed]
		[return: Release]
		[Export ("newSharedTextureHandle")]
		MTLSharedTextureHandle CreateSharedTextureHandle ();

#if NET && !__MACOS__ && !__MACCATALYST__
		[Abstract]
#endif

		[Mac (11, 0), TV (16, 0), iOS (13, 0), MacCatalyst (15, 0)]
		[Export ("firstMipmapInTail")]
		nuint FirstMipmapInTail { get; }

#if NET && !__MACOS__ && !__MACCATALYST__ && !TVOS
		[Abstract]
#endif
		[Mac (11, 0), TV (16, 0), iOS (13, 0), MacCatalyst (15, 0)]
		[Export ("tailSizeInBytes")]
		nuint TailSizeInBytes { get; }

#if NET && !__MACOS__ && !__MACCATALYST__ && !TVOS
		[Abstract]
#endif
		[Mac (11, 0), TV (16, 0), iOS (13, 0), MacCatalyst (15, 0)]
		[Export ("isSparse")]
		bool IsSparse { get; }

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("swizzle")]
		MTLTextureSwizzleChannels Swizzle { get; }

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("newTextureViewWithPixelFormat:textureType:levels:slices:swizzle:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture Create (MTLPixelFormat pixelFormat, MTLTextureType textureType, NSRange levelRange, NSRange sliceRange, MTLTextureSwizzleChannels swizzle);

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[NullAllowed, Export ("remoteStorageTexture")]
		IMTLTexture RemoteStorageTexture { get; }

#if NET
		[Abstract]
#endif
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("newRemoteTextureViewForDevice:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateRemoteTexture (IMTLDevice device);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("gpuResourceID")]
		MTLResourceId GpuResourceId { get; }
	}


	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface MTLTextureDescriptor : NSCopying {

		[Export ("textureType", ArgumentSemantic.Assign)]
		MTLTextureType TextureType { get; set; }

		[Export ("pixelFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat PixelFormat { get; set; }

		[Export ("width")]
		nuint Width { get; set; }

		[Export ("height")]
		nuint Height { get; set; }

		[Export ("depth")]
		nuint Depth { get; set; }

		[Export ("mipmapLevelCount")]
		nuint MipmapLevelCount { get; set; }

		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("sampleCount")]
		nuint SampleCount { get; set; }

		[Export ("arrayLength")]
		nuint ArrayLength { get; set; }

		[Export ("resourceOptions", ArgumentSemantic.Assign)]
		MTLResourceOptions ResourceOptions { get; set; }

		[Static, Export ("texture2DDescriptorWithPixelFormat:width:height:mipmapped:")]
		MTLTextureDescriptor CreateTexture2DDescriptor (MTLPixelFormat pixelFormat, nuint width, nuint height, bool mipmapped);

		[Static, Export ("textureCubeDescriptorWithPixelFormat:size:mipmapped:")]
		MTLTextureDescriptor CreateTextureCubeDescriptor (MTLPixelFormat pixelFormat, nuint size, bool mipmapped);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
		[Static, Export ("textureBufferDescriptorWithPixelFormat:width:resourceOptions:usage:")]
		MTLTextureDescriptor CreateTextureBufferDescriptor (MTLPixelFormat pixelFormat, nuint width, MTLResourceOptions resourceOptions, MTLTextureUsage usage);

		[MacCatalyst (13, 1)]
		[Export ("cpuCacheMode", ArgumentSemantic.Assign)]
		MTLCpuCacheMode CpuCacheMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("storageMode", ArgumentSemantic.Assign)]
		MTLStorageMode StorageMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("usage", ArgumentSemantic.Assign)]
		MTLTextureUsage Usage { get; set; }

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowGPUOptimizedContents")]
		bool AllowGpuOptimizedContents { get; set; }

		[Mac (12, 5), iOS (15, 0), MacCatalyst (15, 0), TV (17, 0), NoWatch]
		[Export ("compressionType")]
		MTLTextureCompressionType CompressionType { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hazardTrackingMode", ArgumentSemantic.Assign)]
		MTLHazardTrackingMode HazardTrackingMode { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("swizzle", ArgumentSemantic.Assign)]
		MTLTextureSwizzleChannels Swizzle { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface MTLSamplerDescriptor : NSCopying {

		[Export ("minFilter", ArgumentSemantic.Assign)]
		MTLSamplerMinMagFilter MinFilter { get; set; }

		[Export ("magFilter", ArgumentSemantic.Assign)]
		MTLSamplerMinMagFilter MagFilter { get; set; }

		[Export ("mipFilter", ArgumentSemantic.Assign)]
		MTLSamplerMipFilter MipFilter { get; set; }

		[Export ("maxAnisotropy")]
		nuint MaxAnisotropy { get; set; }

		[Export ("sAddressMode", ArgumentSemantic.Assign)]
		MTLSamplerAddressMode SAddressMode { get; set; }

		[Export ("tAddressMode", ArgumentSemantic.Assign)]
		MTLSamplerAddressMode TAddressMode { get; set; }

		[Export ("rAddressMode", ArgumentSemantic.Assign)]
		MTLSamplerAddressMode RAddressMode { get; set; }

		[Export ("normalizedCoordinates")]
		bool NormalizedCoordinates { get; set; }

		[Export ("lodMinClamp")]
		float LodMinClamp { get; set; } /* float, not CGFloat */

		[Export ("lodMaxClamp")]
		float LodMaxClamp { get; set; } /* float, not CGFloat */

		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[Export ("lodAverage")]
		bool LodAverage { get; set; }

		[iOS (14, 0), TV (17, 0), NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("borderColor", ArgumentSemantic.Assign)]
		MTLSamplerBorderColor BorderColor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("compareFunction")]
		MTLCompareFunction CompareFunction { get; set; }

		[Export ("label")]
		[NullAllowed]
		string Label { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("supportArgumentBuffers")]
		bool SupportArgumentBuffers { get; set; }
	}

	interface IMTLSamplerState { }
	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLSamplerState {

		[Abstract, Export ("label")]
		string Label { get; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("gpuResourceID")]
		MTLResourceId GpuResourceId { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface MTLRenderPipelineDescriptor : NSCopying {

		[Export ("label")]
		[NullAllowed]
		string Label { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("vertexFunction", ArgumentSemantic.Retain)]
		IMTLFunction VertexFunction { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fragmentFunction", ArgumentSemantic.Retain)]
		IMTLFunction FragmentFunction { get; set; }

		[Export ("vertexDescriptor", ArgumentSemantic.Copy)]
		[NullAllowed]
		MTLVertexDescriptor VertexDescriptor { get; set; }

		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("sampleCount")]
		nuint SampleCount { get; set; }

		[Export ("alphaToCoverageEnabled")]
		bool AlphaToCoverageEnabled { [Bind ("isAlphaToCoverageEnabled")] get; set; }

		[Export ("alphaToOneEnabled")]
		bool AlphaToOneEnabled { [Bind ("isAlphaToOneEnabled")] get; set; }

		[Export ("rasterizationEnabled")]
		bool RasterizationEnabled { [Bind ("isRasterizationEnabled")] get; set; }

		[Export ("reset")]
		void Reset ();

		[Export ("colorAttachments")]
		MTLRenderPipelineColorAttachmentDescriptorArray ColorAttachments { get; }

		[Export ("depthAttachmentPixelFormat")]
		MTLPixelFormat DepthAttachmentPixelFormat { get; set; }

		[Export ("stencilAttachmentPixelFormat")]
		MTLPixelFormat StencilAttachmentPixelFormat { get; set; }

		[iOS (12, 0)]
		[TV (14, 5)]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("inputPrimitiveTopology", ArgumentSemantic.Assign)]
		MTLPrimitiveTopologyClass InputPrimitiveTopology { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("tessellationPartitionMode", ArgumentSemantic.Assign)]
		MTLTessellationPartitionMode TessellationPartitionMode { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("maxTessellationFactor")]
		nuint MaxTessellationFactor { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("tessellationFactorScaleEnabled")]
		bool IsTessellationFactorScaleEnabled { [Bind ("isTessellationFactorScaleEnabled")] get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("tessellationFactorFormat", ArgumentSemantic.Assign)]
		MTLTessellationFactorFormat TessellationFactorFormat { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("tessellationControlPointIndexType", ArgumentSemantic.Assign)]
		MTLTessellationControlPointIndexType TessellationControlPointIndexType { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("tessellationFactorStepFunction", ArgumentSemantic.Assign)]
		MTLTessellationFactorStepFunction TessellationFactorStepFunction { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("tessellationOutputWindingOrder", ArgumentSemantic.Assign)]
		MTLWinding TessellationOutputWindingOrder { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("vertexBuffers")]
		MTLPipelineBufferDescriptorArray VertexBuffers { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("fragmentBuffers")]
		MTLPipelineBufferDescriptorArray FragmentBuffers { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("rasterSampleCount")]
		nuint RasterSampleCount { get; set; }

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; set; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[TV (17, 0), iOS (13, 0)]
		[Export ("maxVertexAmplificationCount")]
		nuint MaxVertexAmplificationCount { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("binaryArchives", ArgumentSemantic.Copy)]
		IMTLBinaryArchive [] BinaryArchives { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("vertexPreloadedLibraries", ArgumentSemantic.Copy)]
		IMTLDynamicLibrary [] VertexPreloadedLibraries { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[NullAllowed, Export ("vertexLinkedFunctions", ArgumentSemantic.Copy)]
		MTLLinkedFunctions VertexLinkedFunctions { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("supportAddingVertexBinaryFunctions")]
		bool SupportAddingVertexBinaryFunctions { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("supportAddingFragmentBinaryFunctions")]
		bool SupportAddingFragmentBinaryFunctions { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("maxVertexCallStackDepth")]
		nuint MaxVertexCallStackDepth { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("maxFragmentCallStackDepth")]
		nuint MaxFragmentCallStackDepth { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("fragmentPreloadedLibraries", ArgumentSemantic.Copy)]
		IMTLDynamicLibrary [] FragmentPreloadedLibraries { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[NullAllowed, Export ("fragmentLinkedFunctions", ArgumentSemantic.Copy)]
		MTLLinkedFunctions FragmentLinkedFunctions { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPipelineColorAttachmentDescriptorArray {

		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLRenderPipelineColorAttachmentDescriptor ObjectAtIndexedSubscript (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject ([NullAllowed] MTLRenderPipelineColorAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	interface IMTLRenderPipelineState { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLRenderPipelineState {

		[Abstract, Export ("label")]
		string Label { get; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; }

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("threadgroupSizeMatchesTileSize")]
		bool ThreadgroupSizeMatchesTileSize { get; }

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("imageblockSampleLength")]
		nuint ImageblockSampleLength { get; }

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("imageblockMemoryLengthForDimensions:")]
		nuint GetImageblockMemoryLength (MTLSize imageblockDimensions);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; }

#if NET

#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), NoWatch, MacCatalyst (15, 0)]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, NoWatch, MacCatalyst (15, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoWatch, NoMacCatalyst]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), NoWatch, MacCatalyst (15, 0)]
#endif
		[Export ("functionHandleWithFunction:stage:")]
		[return: NullAllowed]
		IMTLFunctionHandle FunctionHandleWithFunction (IMTLFunction function, MTLRenderStages stage);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (!6,0), NoWatch, MacCatalyst (15, 0)]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, NoWatch, MacCatalyst (15, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoWatch, NoMacCatalyst]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), NoWatch, MacCatalyst (15, 0)]
#endif
		[Export ("newVisibleFunctionTableWithDescriptor:stage:")]
		[return: NullAllowed]
		[return: Release]
		IMTLVisibleFunctionTable NewVisibleFunctionTableWithDescriptor (MTLVisibleFunctionTableDescriptor descriptor, MTLRenderStages stage);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), NoWatch, MacCatalyst (15, 0)]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, NoWatch, MacCatalyst (15, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoWatch, NoMacCatalyst]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), NoWatch, MacCatalyst (15, 0)]
#endif
		[Export ("newIntersectionFunctionTableWithDescriptor:stage:")]
		[return: NullAllowed]
		[return: Release]
		IMTLIntersectionFunctionTable NewIntersectionFunctionTableWithDescriptor (MTLIntersectionFunctionTableDescriptor descriptor, MTLRenderStages stage);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), NoWatch, MacCatalyst (15, 0)]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, NoWatch, MacCatalyst (15, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoWatch, NoMacCatalyst]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), NoWatch, MacCatalyst (15, 0)]
#endif
		[Export ("newRenderPipelineStateWithAdditionalBinaryFunctions:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLRenderPipelineState NewRenderPipelineStateWithAdditionalBinaryFunctions (MTLRenderPipelineFunctionsDescriptor additionalBinaryFunctions, [NullAllowed] out NSError error);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Abstract]
		[Export ("meshThreadExecutionWidth")]
		nuint MeshThreadExecutionWidth { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Abstract]
		[Export ("maxTotalThreadgroupsPerMeshGrid")]
		nuint MaxTotalThreadgroupsPerMeshGrid { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Abstract]
		[Export ("gpuResourceID")]
		MTLResourceId GpuResourceId { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("maxTotalThreadsPerMeshThreadgroup")]
		nuint MaxTotalThreadsPerMeshThreadgroup { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("maxTotalThreadsPerObjectThreadgroup")]
		nuint MaxTotalThreadsPerObjectThreadgroup { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("objectThreadExecutionWidth")]
		nuint ObjectThreadExecutionWidth { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLVertexBufferLayoutDescriptor : NSCopying {
		[Export ("stride", ArgumentSemantic.UnsafeUnretained)]
		nuint Stride { get; set; }

		[Export ("stepFunction", ArgumentSemantic.Assign)]
		MTLVertexStepFunction StepFunction { get; set; }

		[Export ("stepRate", ArgumentSemantic.UnsafeUnretained)]
		nuint StepRate { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLVertexBufferLayoutDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLVertexBufferLayoutDescriptor ObjectAtIndexedSubscript (nuint index);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject ([NullAllowed] MTLVertexBufferLayoutDescriptor bufferDesc, nuint index);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLAttribute {
		[Export ("name")]
		string Name { get; }

		[Export ("attributeIndex")]
		nuint AttributeIndex { get; }

		[Export ("attributeType")]
		MTLDataType AttributeType { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }

		[Export ("patchData")]
		bool IsPatchData { [Bind ("isPatchData")] get; }

		[Export ("patchControlPointData")]
		bool IsPatchControlPointData { [Bind ("isPatchControlPointData")] get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLAttributeDescriptor : NSCopying {
		[Export ("format", ArgumentSemantic.Assign)]
		MTLAttributeFormat Format { get; set; }

		[Export ("offset")]
		nuint Offset { get; set; }

		[Export ("bufferIndex")]
		nuint BufferIndex { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLAttributeDescriptorArray {
		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		MTLAttributeDescriptor ObjectAtIndexedSubscript (nuint index);

		[Internal]
		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLAttributeDescriptor attributeDesc, nuint index);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLVertexAttributeDescriptor : NSCopying {
		[Export ("format", ArgumentSemantic.Assign)]
		MTLVertexFormat Format { get; set; }

		[Export ("offset", ArgumentSemantic.Assign)]
		nuint Offset { get; set; }

		[Export ("bufferIndex", ArgumentSemantic.Assign)]
		nuint BufferIndex { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLVertexAttributeDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLVertexAttributeDescriptor ObjectAtIndexedSubscript (nuint index);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject ([NullAllowed] MTLVertexAttributeDescriptor attributeDesc, nuint index);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface MTLVertexDescriptor : NSCopying {
		[Static, Export ("vertexDescriptor")]
		MTLVertexDescriptor Create ();

		[Export ("reset")]
		void Reset ();

		[Export ("layouts")]
		MTLVertexBufferLayoutDescriptorArray Layouts { get; }

		[Export ("attributes")]
		MTLVertexAttributeDescriptorArray Attributes { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface MTLVertexAttribute {
		[Export ("attributeIndex")]
		nuint AttributeIndex { get; }

		[MacCatalyst (13, 1)]
		[Export ("attributeType")]
		MTLDataType AttributeType { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }

		[Export ("name")]
		string Name { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("patchData")]
		bool PatchData { [Bind ("isPatchData")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("patchControlPointData")]
		bool PatchControlPointData { [Bind ("isPatchControlPointData")] get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTLFunctionConstantValues : NSCopying {
		[MacCatalyst (13, 1)]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("setConstantValue:type:atIndex:")]
		void SetConstantValue (IntPtr value, MTLDataType type, nuint index);

		[Export ("setConstantValues:type:withRange:")]
		void SetConstantValues (IntPtr values, MTLDataType type, NSRange range);

		[Export ("setConstantValue:type:withName:")]
		void SetConstantValue (IntPtr value, MTLDataType type, string name);

		[Export ("reset")]
		void Reset ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLFunctionConstant {
		[Export ("name")]
		string Name { get; }

		[Export ("type")]
		MTLDataType Type { get; }

		[Export ("index")]
		nuint Index { get; }

		[Export ("required")]
		bool IsRequired { get; }
	}

	interface IMTLFunction { }
	[MacCatalyst (13, 1)]
	[Protocol] // // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLFunction {

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("functionType")]
		MTLFunctionType FunctionType { get; }

		[Abstract, Export ("vertexAttributes")]
		MTLVertexAttribute [] VertexAttributes { get; }

		[Abstract, Export ("name")]
		string Name { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("patchType")]
		MTLPatchType PatchType { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("patchControlPointCount")]
		nint PatchControlPointCount { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("stageInputAttributes")]
		MTLAttribute [] StageInputAttributes { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("functionConstantsDictionary")]
		NSDictionary<NSString, MTLFunctionConstant> FunctionConstants { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newArgumentEncoderWithBufferIndex:")]
		[return: Release]
		IMTLArgumentEncoder CreateArgumentEncoder (nuint bufferIndex);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newArgumentEncoderWithBufferIndex:reflection:")]
		[return: Release]
		IMTLArgumentEncoder CreateArgumentEncoder (nuint bufferIndex, [NullAllowed] out MTLArgument reflection);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("options")]
		MTLFunctionOptions Options { get; }
	}

	interface IMTLLibrary { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLLibrary {

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("functionNames")]
		string [] FunctionNames { get; }

		[Abstract, Export ("newFunctionWithName:")]
		[return: Release]
		IMTLFunction CreateFunction (string functionName);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newFunctionWithName:constantValues:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLFunction CreateFunction (string name, MTLFunctionConstantValues constantValues, out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newFunctionWithName:constantValues:completionHandler:")]
		[Async]
		void CreateFunction (string name, MTLFunctionConstantValues constantValues, Action<IMTLFunction, NSError> completionHandler);

		[Field ("MTLLibraryErrorDomain")]
		NSString ErrorDomain { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("newFunctionWithDescriptor:completionHandler:")]
		void CreateFunction (MTLFunctionDescriptor descriptor, Action<IMTLFunction, NSError> completionHandler);

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("newFunctionWithDescriptor:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLFunction CreateFunction (MTLFunctionDescriptor descriptor, [NullAllowed] out NSError error);

		// protocol, so no Async
#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("newIntersectionFunctionWithDescriptor:completionHandler:")]
		void CreateIntersectionFunction (MTLIntersectionFunctionDescriptor descriptor, Action<IMTLFunction, NSError> completionHandler);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif
		
#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("newIntersectionFunctionWithDescriptor:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLFunction CreateIntersectionFunction (MTLIntersectionFunctionDescriptor descriptor, [NullAllowed] out NSError error);

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("type")]
		MTLLibraryType Type { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("installName")]
		string InstallName { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface MTLCompileOptions : NSCopying {

		[NullAllowed] // by default this property is null
		[Export ("preprocessorMacros", ArgumentSemantic.Copy)]
#if NET
		NSDictionary<NSString, NSObject> PreprocessorMacros { get; set; }
#else
		NSDictionary PreprocessorMacros { get; set; }
#endif

		[Export ("fastMathEnabled")]
		bool FastMathEnabled { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("languageVersion", ArgumentSemantic.Assign)]
		MTLLanguageVersion LanguageVersion { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("libraryType", ArgumentSemantic.Assign)]
		MTLLibraryType LibraryType { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("installName")]
		string InstallName { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("libraries", ArgumentSemantic.Copy)]
		IMTLDynamicLibrary [] Libraries { get; set; }

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Mac (11, 0), iOS (13, 0), TV (14, 0)]
		[Export ("preserveInvariance")]
		bool PreserveInvariance { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Export ("optimizationLevel", ArgumentSemantic.Assign)]
		MTLLibraryOptimizationLevel OptimizationLevel { get; set; }

		[Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4), TV (16, 4)]
		[Export ("compileSymbolVisibility", ArgumentSemantic.Assign)]
		MTLCompileSymbolVisibility CompileSymbolVisibility { get; set; }

		[Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4), TV (16, 4)]
		[Export ("allowReferencingUndefinedSymbols")]
		bool AllowReferencingUndefinedSymbols { get; set; }

		[Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4), TV (16, 4)]
		[Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface MTLStencilDescriptor : NSCopying {
		[Export ("stencilCompareFunction")]
		MTLCompareFunction StencilCompareFunction { get; set; }

		[Export ("stencilFailureOperation")]
		MTLStencilOperation StencilFailureOperation { get; set; }

		[Export ("depthFailureOperation")]
		MTLStencilOperation DepthFailureOperation { get; set; }

		[Export ("depthStencilPassOperation")]
		MTLStencilOperation DepthStencilPassOperation { get; set; }

		[Export ("readMask")]
		uint ReadMask { get; set; } /* uint32_t */

		[Export ("writeMask")]
		uint WriteMask { get; set; } /* uint32_t */
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLStructMember {
		[Export ("name")]
		string Name { get; }

		[Export ("offset")]
		nuint Offset { get; }

		[Export ("dataType")]
		MTLDataType DataType { get; }

#if NET
		[Export ("structType")]
		[NullAllowed]
		MTLStructType StructType { get; }

		[Export ("arrayType")]
		[NullAllowed]
		MTLArrayType ArrayType { get; }
#else
		[Export ("structType")]
		[return: NullAllowed]
		MTLStructType StructType ();

		[Export ("arrayType")]
		[return: NullAllowed]
		MTLArrayType ArrayType ();
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("argumentIndex")]
		nuint ArgumentIndex { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("textureReferenceType")]
		MTLTextureReferenceType TextureReferenceType { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("pointerType")]
		MTLPointerType PointerType { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (MTLType))]
	interface MTLStructType {
		[Export ("members")]
		MTLStructMember [] Members { get; }

		[Export ("memberByName:")]
		[return: NullAllowed]
		MTLStructMember Lookup (string name);
	}

	interface IMTLDepthStencilState { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLDepthStencilState {
		[Abstract]
		[Export ("label")]
		string Label { get; }

		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface MTLDepthStencilDescriptor : NSCopying {

		[Export ("depthCompareFunction")]
		MTLCompareFunction DepthCompareFunction { get; set; }

		[Export ("depthWriteEnabled")]
		bool DepthWriteEnabled { [Bind ("isDepthWriteEnabled")] get; set; }

		[Export ("frontFaceStencil", ArgumentSemantic.Copy)]
		[NullAllowed]
		MTLStencilDescriptor FrontFaceStencil { get; set; }

		[Export ("backFaceStencil", ArgumentSemantic.Copy)]
		[NullAllowed]
		MTLStencilDescriptor BackFaceStencil { get; set; }

		[Export ("label")]
		[NullAllowed]
		string Label { get; set; }
	}

	interface IMTLParallelRenderCommandEncoder { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	interface MTLParallelRenderCommandEncoder : MTLCommandEncoder {
		[Abstract]
		[Export ("renderCommandEncoder")]
		[Autorelease]
		[return: NullAllowed]
		IMTLRenderCommandEncoder CreateRenderCommandEncoder ();

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setColorStoreAction:atIndex:")]
		void SetColorStoreAction (MTLStoreAction storeAction, nuint colorAttachmentIndex);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setDepthStoreAction:")]
		void SetDepthStoreAction (MTLStoreAction storeAction);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setStencilStoreAction:")]
		void SetStencilStoreAction (MTLStoreAction storeAction);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setColorStoreActionOptions:atIndex:")]
		void SetColorStoreActionOptions (MTLStoreActionOptions storeActionOptions, nuint colorAttachmentIndex);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setDepthStoreActionOptions:")]
		void SetDepthStoreActionOptions (MTLStoreActionOptions storeActionOptions);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setStencilStoreActionOptions:")]
		void SetStencilStoreActionOptions (MTLStoreActionOptions storeActionOptions);
	}

	interface IMTLRenderCommandEncoder { }

	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLRenderCommandEncoder : MTLCommandEncoder {

		[Abstract, Export ("setRenderPipelineState:")]
		void SetRenderPipelineState (IMTLRenderPipelineState pipelineState);

		[Abstract, Export ("setVertexBuffer:offset:atIndex:")]
		void SetVertexBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract, Export ("setVertexTexture:atIndex:")]
		void SetVertexTexture (IMTLTexture texture, nuint index);

		[Abstract, Export ("setVertexSamplerState:atIndex:")]
		void SetVertexSamplerState (IMTLSamplerState sampler, nuint index);

		[Abstract, Export ("setVertexSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetVertexSamplerState (IMTLSamplerState sampler, float /* float, not CGFloat */ lodMinClamp, float /* float, not CGFloat */ lodMaxClamp, nuint index);

		[Abstract, Export ("setViewport:")]
		void SetViewport (MTLViewport viewport);

		[Abstract, Export ("setFrontFacingWinding:")]
		void SetFrontFacingWinding (MTLWinding frontFacingWinding);

		[Abstract, Export ("setCullMode:")]
		void SetCullMode (MTLCullMode cullMode);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("setDepthClipMode:")]
		void SetDepthClipMode (MTLDepthClipMode depthClipMode);

		[Abstract, Export ("setDepthBias:slopeScale:clamp:")]
		void SetDepthBias (float /* float, not CGFloat */ depthBias, float /* float, not CGFloat */ slopeScale, float /* float, not CGFloat */ clamp);

		[Abstract, Export ("setScissorRect:")]
		void SetScissorRect (MTLScissorRect rect);

		[Abstract, Export ("setTriangleFillMode:")]
		void SetTriangleFillMode (MTLTriangleFillMode fillMode);

		[Abstract, Export ("setFragmentBuffer:offset:atIndex:")]
		void SetFragmentBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[MacCatalyst (13, 1)]
		[Abstract, Export ("setFragmentBufferOffset:atIndex:")]
		void SetFragmentBufferOffset (nuint offset, nuint index);

		[MacCatalyst (13, 1)]
		[Abstract, Export ("setFragmentBytes:length:atIndex:")]
		void SetFragmentBytes (IntPtr bytes, nuint length, nuint index);

		[Abstract, Export ("setFragmentTexture:atIndex:")]
		void SetFragmentTexture (IMTLTexture texture, nuint index);

		[Abstract, Export ("setFragmentSamplerState:atIndex:")]
		void SetFragmentSamplerState (IMTLSamplerState sampler, nuint index);

		[Abstract, Export ("setFragmentSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetFragmentSamplerState (IMTLSamplerState sampler, float /* float, not CGFloat */ lodMinClamp, float /* float, not CGFloat */ lodMaxClamp, nuint index);

		[Abstract, Export ("setBlendColorRed:green:blue:alpha:")]
		void SetBlendColor (float /* float, not CGFloat */ red, float /* float, not CGFloat */ green, float /* float, not CGFloat */ blue, float /* float, not CGFloat */ alpha);

		[Abstract, Export ("setDepthStencilState:")]
		void SetDepthStencilState (IMTLDepthStencilState depthStencilState);

		[Abstract, Export ("setStencilReferenceValue:")]
		void SetStencilReferenceValue (uint /* uint32_t */ referenceValue);

		[MacCatalyst (13, 1)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("setStencilFrontReferenceValue:backReferenceValue:")]
		void SetStencilFrontReferenceValue (uint frontReferenceValue, uint backReferenceValue);

		[Abstract, Export ("setVisibilityResultMode:offset:")]
		void SetVisibilityResultMode (MTLVisibilityResultMode mode, nuint offset);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setColorStoreAction:atIndex:")]
		void SetColorStoreAction (MTLStoreAction storeAction, nuint colorAttachmentIndex);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setDepthStoreAction:")]
		void SetDepthStoreAction (MTLStoreAction storeAction);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setStencilStoreAction:")]
		void SetStencilStoreAction (MTLStoreAction storeAction);

		[Abstract, Export ("drawPrimitives:vertexStart:vertexCount:instanceCount:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount, nuint instanceCount);

		[Abstract, Export ("drawPrimitives:vertexStart:vertexCount:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount);

		[Abstract, Export ("drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:instanceCount:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, nuint indexCount, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset, nuint instanceCount);

		[Abstract, Export ("drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, nuint indexCount, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset);

#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("drawPrimitives:vertexStart:vertexCount:instanceCount:baseInstance:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount, nuint instanceCount, nuint baseInstance);

#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:instanceCount:baseVertex:baseInstance:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, nuint indexCount, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset, nuint instanceCount, nint baseVertex, nuint baseInstance);

#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("drawPrimitives:indirectBuffer:indirectBufferOffset:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("drawIndexedPrimitives:indexType:indexBuffer:indexBufferOffset:indirectBuffer:indirectBufferOffset:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[Abstract, Export ("setFragmentBuffers:offsets:withRange:")]
		void SetFragmentBuffers (IMTLBuffer buffers, IntPtr IntPtrOffsets, NSRange range);

		[Abstract, Export ("setFragmentSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetFragmentSamplerStates (IMTLSamplerState [] samplers, IntPtr floatArrayPtrLodMinClamps, IntPtr floatArrayPtrLodMaxClamps, NSRange range);

		[Abstract, Export ("setFragmentSamplerStates:withRange:")]
		void SetFragmentSamplerStates (IMTLSamplerState [] samplers, NSRange range);

		[Abstract, Export ("setFragmentTextures:withRange:")]
		void SetFragmentTextures (IMTLTexture [] textures, NSRange range);

		[Abstract, Export ("setVertexBuffers:offsets:withRange:")]
		void SetVertexBuffers (IMTLBuffer [] buffers, IntPtr uintArrayPtrOffsets, NSRange range);

		[MacCatalyst (13, 1)]
		[Abstract, Export ("setVertexBufferOffset:atIndex:")]
		void SetVertexBufferOffset (nuint offset, nuint index);

		[MacCatalyst (13, 1)]
		[Abstract, Export ("setVertexBytes:length:atIndex:")]
		void SetVertexBytes (IntPtr bytes, nuint length, nuint index);

		[Abstract, Export ("setVertexSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetVertexSamplerStates (IMTLSamplerState [] samplers, IntPtr floatArrayPtrLodMinClamps, IntPtr floatArrayPtrLodMaxClamps, NSRange range);

		[Abstract, Export ("setVertexSamplerStates:withRange:")]
		void SetVertexSamplerStates (IMTLSamplerState [] samplers, NSRange range);

		[Abstract]
		[Export ("setVertexTextures:withRange:")]
		void SetVertexTextures (IMTLTexture [] textures, NSRange range);

		[NoiOS, NoTV, NoWatch]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'MemoryBarrier (MTLBarrierScope, MTLRenderStages, MTLRenderStages)' instead.")]
		[NoMacCatalyst]
#if NET
		[Abstract]
#endif
		[Export ("textureBarrier")]
		void TextureBarrier ();

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("updateFence:afterStages:")]
		void Update (IMTLFence fence, MTLRenderStages stages);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("waitForFence:beforeStages:")]
		void Wait (IMTLFence fence, MTLRenderStages stages);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTessellationFactorBuffer:offset:instanceStride:")]
		void SetTessellationFactorBuffer ([NullAllowed] IMTLBuffer buffer, nuint offset, nuint instanceStride);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTessellationFactorScale:")]
		void SetTessellationFactorScale (float scale);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("drawPatches:patchStart:patchCount:patchIndexBuffer:patchIndexBufferOffset:instanceCount:baseInstance:")]
		void DrawPatches (nuint numberOfPatchControlPoints, nuint patchStart, nuint patchCount, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, nuint instanceCount, nuint baseInstance);

		[iOS (12, 0)]
		[NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("drawPatches:patchIndexBuffer:patchIndexBufferOffset:indirectBuffer:indirectBufferOffset:")]
		void DrawPatches (nuint numberOfPatchControlPoints, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("drawIndexedPatches:patchStart:patchCount:patchIndexBuffer:patchIndexBufferOffset:controlPointIndexBuffer:controlPointIndexBufferOffset:instanceCount:baseInstance:")]
		void DrawIndexedPatches (nuint numberOfPatchControlPoints, nuint patchStart, nuint patchCount, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, IMTLBuffer controlPointIndexBuffer, nuint controlPointIndexBufferOffset, nuint instanceCount, nuint baseInstance);

		[iOS (12, 0)]
		[NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("drawIndexedPatches:patchIndexBuffer:patchIndexBufferOffset:controlPointIndexBuffer:controlPointIndexBufferOffset:indirectBuffer:indirectBufferOffset:")]
		void DrawIndexedPatches (nuint numberOfPatchControlPoints, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, IMTLBuffer controlPointIndexBuffer, nuint controlPointIndexBufferOffset, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[iOS (12, 0)]
		[NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setViewports:count:")]
		void SetViewports (IntPtr viewports, nuint count);

		[iOS (12, 0)]
		[NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setScissorRects:count:")]
		void SetScissorRects (IntPtr scissorRects, nuint count);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setColorStoreActionOptions:atIndex:")]
		void SetColorStoreActionOptions (MTLStoreActionOptions storeActionOptions, nuint colorAttachmentIndex);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setDepthStoreActionOptions:")]
		void SetDepthStoreActionOptions (MTLStoreActionOptions storeActionOptions);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setStencilStoreActionOptions:")]
		void SetStencilStoreActionOptions (MTLStoreActionOptions storeActionOptions);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("useResource:usage:")]
		void UseResource (IMTLResource resource, MTLResourceUsage usage);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("useResources:count:usage:")]
		void UseResources (IMTLResource [] resources, nuint count, MTLResourceUsage usage);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("useHeap:")]
		void UseHeap (IMTLHeap heap);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("useHeaps:count:")]
		void UseHeaps (IMTLHeap [] heaps, nuint count);

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("executeCommandsInBuffer:withRange:")]
		void ExecuteCommands (IMTLIndirectCommandBuffer indirectCommandBuffer, NSRange executionRange);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("executeCommandsInBuffer:indirectBuffer:indirectBufferOffset:")]
		void ExecuteCommands (IMTLIndirectCommandBuffer indirectCommandbuffer, IMTLBuffer indirectRangeBuffer, nuint indirectBufferOffset);

#if NET

#if MONOMAC || __MACCATALYST__
		[NoiOS, NoTV, MacCatalyst (15, 0)]
		[Abstract]
#else
		[iOS (16,0), TV (16,0), NoMacCatalyst, NoMac]
#endif

#else
		[iOS (16, 0), TV (16, 0), MacCatalyst (15, 0)]
#endif
		[Export ("memoryBarrierWithScope:afterStages:beforeStages:")]
		void MemoryBarrier (MTLBarrierScope scope, MTLRenderStages after, MTLRenderStages before);

#if NET

#if MONOMAC || __MACCATALYST__
		[NoiOS, NoTV, MacCatalyst (15, 0)]
		[Abstract]
#else
		[iOS (16,0), TV (16,0), NoMacCatalyst, NoMac]
#endif

#else
		[iOS (16, 0), TV (16, 0), MacCatalyst (15, 0)]
#endif
		[Export ("memoryBarrierWithResources:count:afterStages:beforeStages:")]
		void MemoryBarrier (IMTLResource [] resources, nuint count, MTLRenderStages after, MTLRenderStages before);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("tileWidth")]
		nuint TileWidth { get; }

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("tileHeight")]
		nuint TileHeight { get; }

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileBytes:length:atIndex:")]
		void SetTileBytes (IntPtr /* void* */ bytes, nuint length, nuint index);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileBuffer:offset:atIndex:")]
		void SetTileBuffer ([NullAllowed] IMTLBuffer buffer, nuint offset, nuint index);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileBufferOffset:atIndex:")]
		void SetTileBufferOffset (nuint offset, nuint index);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileBuffers:offsets:withRange:")]
		void SetTileBuffers (IMTLBuffer [] buffers, IntPtr offsets, NSRange range);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileTexture:atIndex:")]
		void SetTileTexture ([NullAllowed] IMTLTexture texture, nuint index);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileTextures:withRange:")]
		void SetTileTextures (IMTLTexture [] textures, NSRange range);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileSamplerState:atIndex:")]
		void SetTileSamplerState ([NullAllowed] IMTLSamplerState sampler, nuint index);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileSamplerStates:withRange:")]
		void SetTileSamplerStates (IMTLSamplerState [] samplers, NSRange range);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetTileSamplerState ([NullAllowed] IMTLSamplerState sampler, float lodMinClamp, float lodMaxClamp, nuint index);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setTileSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetTileSamplerStates (IMTLSamplerState [] samplers, IntPtr /* float[] */ lodMinClamps, IntPtr /* float[] */ lodMaxClamps, NSRange range);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("dispatchThreadsPerTile:")]
		void DispatchThreadsPerTile (MTLSize threadsPerTile);

		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setThreadgroupMemoryLength:offset:atIndex:")]
		void SetThreadgroupMemoryLength (nuint length, nuint offset, nuint index);

#if NET
#if XAMCORE_5_0
		[MacCatalyst (13, 4), TV (16,0), iOS (13, 0)]
		[Abstract]
#elif !TVOS
		[MacCatalyst (13, 4), NoTV, iOS (13, 0)]
		[Abstract]
#else
		[NoMacCatalyst, TV (16,0), NoiOS, NoMac]
#endif

#else
		[MacCatalyst (13, 4), TV (16, 0), iOS (13, 0)]
#endif
		[Export ("setVertexAmplificationCount:viewMappings:")]
		void SetVertexAmplificationCount (nuint count, MTLVertexAmplificationViewMapping viewMappings);

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("useResource:usage:stages:")]
		void UseResource (IMTLResource resource, MTLResourceUsage usage, MTLRenderStages stages);

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("useResources:count:usage:stages:")]
		void UseResources (IMTLResource [] resources, nuint count, MTLResourceUsage usage, MTLRenderStages stages);

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("useHeap:stages:")]
		void UseHeap (IMTLHeap heap, MTLRenderStages stages);

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("useHeaps:count:stages:")]
		void UseHeaps (IMTLHeap [] heaps, nuint count, MTLRenderStages stages);

#if NET
		[Abstract]
#endif
		[iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sampleCountersInBuffer:atSampleIndex:withBarrier:")]
#if NET
		void SampleCounters (IMTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#else
		void SampleCounters (MTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#endif

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setVertexVisibleFunctionTable:atBufferIndex:")]
		void SetVertexVisibleFunctionTable ([NullAllowed] IMTLVisibleFunctionTable functionTable, nuint bufferIndex);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setVertexVisibleFunctionTables:withBufferRange:")]
		void SetVertexVisibleFunctionTables (IMTLVisibleFunctionTable [] functionTables, NSRange range);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setVertexIntersectionFunctionTable:atBufferIndex:")]
		void SetVertexIntersectionFunctionTable ([NullAllowed] IMTLIntersectionFunctionTable intersectionFunctionTable, nuint bufferIndex);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setVertexIntersectionFunctionTables:withBufferRange:")]
		void SetVertexIntersectionFunctionTables (IMTLIntersectionFunctionTable [] intersectionFunctionTable, NSRange range);

#if NET

#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setVertexAccelerationStructure:atBufferIndex:")]
		void SetVertexAccelerationStructure ([NullAllowed] IMTLAccelerationStructure accelerationStructure, nuint bufferIndex);

#if NET

#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setFragmentAccelerationStructure:atBufferIndex:")]
		void SetFragmentAccelerationStructure ([NullAllowed] IMTLAccelerationStructure accelerationStructure, nuint bufferIndex);

#if NET

#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setFragmentIntersectionFunctionTable:atBufferIndex:")]
		void SetFragmentIntersectionFunctionTable ([NullAllowed] IMTLIntersectionFunctionTable intersectionFunctionTable, nuint bufferIndex);


#if NET

#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setFragmentIntersectionFunctionTables:withBufferRange:")]
		void SetFragmentIntersectionFunctionTables (IMTLIntersectionFunctionTable [] intersectionFunctionTable, NSRange range);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setFragmentVisibleFunctionTable:atBufferIndex:")]
		void SetFragmentVisibleFunctionTable ([NullAllowed] IMTLVisibleFunctionTable functionTable, nuint bufferIndex);

#if NET

#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setFragmentVisibleFunctionTables:withBufferRange:")]
		void SetFragmentVisibleFunctionTables (IMTLVisibleFunctionTable [] functionTables, NSRange range);

#if NET

#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setTileAccelerationStructure:atBufferIndex:")]
		void SetTileAccelerationStructure ([NullAllowed] IMTLAccelerationStructure accelerationStructure, nuint bufferIndex);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setTileIntersectionFunctionTable:atBufferIndex:")]
		void SetTileIntersectionFunctionTable ([NullAllowed] IMTLIntersectionFunctionTable intersectionFunctionTable, nuint bufferIndex);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setTileIntersectionFunctionTables:withBufferRange:")]
		void SetTileIntersectionFunctionTables (IMTLIntersectionFunctionTable [] intersectionFunctionTable, NSRange range);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setTileVisibleFunctionTable:atBufferIndex:")]
		void SetTileVisibleFunctionTable ([NullAllowed] IMTLVisibleFunctionTable functionTable, nuint bufferIndex);

#if NET
#if XAMCORE_5_0
		[Mac (12, 0), iOS (15, 0), TV (16,0), MacCatalyst (15, 0), NoWatch]
		[Abstract]
#elif !TVOS
		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst, NoWatch]
#endif

#else
		[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
#endif
		[Export ("setTileVisibleFunctionTables:withBufferRange:")]
		void SetTileVisibleFunctionTables (IMTLVisibleFunctionTable [] functionTables, NSRange range);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), NoWatch]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setVertexBuffer:offset:attributeStride:atIndex:")]
		void SetVertexBuffer ([NullAllowed] IMTLBuffer buffer, nuint offset, nuint stride, nuint index);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), NoWatch]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setVertexBuffers:offsets:attributeStrides:withRange:")]
		void SetVertexBuffers (IntPtr buffers, IntPtr offsets, IntPtr strides, NSRange range);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), NoWatch]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setVertexBufferOffset:attributeStride:atIndex:")]
		void SetVertexBufferOffset (nuint offset, nuint stride, nuint index);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), NoWatch]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setVertexBytes:length:attributeStride:atIndex:")]
		void SetVertexBytes (IntPtr bytes, nuint length, nuint stride, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("drawMeshThreadgroups:threadsPerObjectThreadgroup:threadsPerMeshThreadgroup:")]
		void DrawMeshThreadgroups (MTLSize threadgroupsPerGrid, MTLSize threadsPerObjectThreadgroup, MTLSize threadsPerMeshThreadgroup);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("drawMeshThreadgroupsWithIndirectBuffer:indirectBufferOffset:threadsPerObjectThreadgroup:threadsPerMeshThreadgroup:")]
		void DrawMeshThreadgroups (IMTLBuffer indirectBuffer, nuint indirectBufferOffset, MTLSize threadsPerObjectThreadgroup, MTLSize threadsPerMeshThreadgroup);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("drawMeshThreads:threadsPerObjectThreadgroup:threadsPerMeshThreadgroup:")]
		void DrawMeshThreads (MTLSize threadsPerGrid, MTLSize threadsPerObjectThreadgroup, MTLSize threadsPerMeshThreadgroup);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshBufferOffset:atIndex:")]
		void SetMeshBufferOffset (nuint offset, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshBuffers:offsets:withRange:")]
		void SetMeshBuffers (IntPtr buffers, IntPtr offsets, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshTexture:atIndex:")]
		void SetMeshTexture ([NullAllowed] IMTLTexture texture, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshTextures:withRange:")]
		void SetMeshTextures (IntPtr textures, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshSamplerState:atIndex:")]
		void SetMeshSamplerState ([NullAllowed] IMTLSamplerState sampler, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshSamplerStates:withRange:")]
		void SetMeshSamplerStates (IntPtr samplers, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetMeshSamplerState ([NullAllowed] IMTLSamplerState sampler, float lodMinClamp, float lodMaxClamp, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetMeshSamplerStates (IntPtr samplers, IntPtr lodMinClamps, IntPtr lodMaxClamps, NSRange range);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectBuffer:offset:atIndex:")]
		void SetObjectBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectBufferOffset:atIndex:")]
		void SetObjectBufferOffset (nuint offset, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectBuffers:offsets:withRange:")]
		void SetObjectBuffers (IntPtr buffers, IntPtr offsets, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectBytes:length:atIndex:")]
		void SetObjectBytes (IntPtr bytes, nuint length, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshBuffer:offset:atIndex:")]
		void SetMeshBuffer ([NullAllowed] IMTLBuffer buffer, nuint offset, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshBytes:length:atIndex:")]
		void SetMeshBytes (IntPtr bytes, nuint length, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectSamplerState:atIndex:")]
		void SetObjectSamplerState ([NullAllowed] IMTLSamplerState sampler, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetObjectSamplerState ([NullAllowed] IMTLSamplerState sampler, float lodMinClamp, float lodMaxClamp, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetObjectSamplerStates (IntPtr samplers, IntPtr lodMinClamps, IntPtr lodMaxClamps, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectSamplerStates:withRange:")]
		void SetObjectSamplerStates (IntPtr samplers, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectTexture:atIndex:")]
		void SetObjectTexture ([NullAllowed] IMTLTexture texture, nuint index);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectTextures:withRange:")]
		void SetObjectTextures (IntPtr textures, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectThreadgroupMemoryLength:atIndex:")]
		void SetObjectThreadgroupMemoryLength (nuint length, nuint index);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPipelineColorAttachmentDescriptor : NSCopying {

		[Export ("pixelFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat PixelFormat { get; set; }

		[Export ("blendingEnabled")]
		bool BlendingEnabled { [Bind ("isBlendingEnabled")] get; set; }

		[Export ("sourceRGBBlendFactor", ArgumentSemantic.Assign)]
		MTLBlendFactor SourceRgbBlendFactor { get; set; }

		[Export ("destinationRGBBlendFactor", ArgumentSemantic.Assign)]
		MTLBlendFactor DestinationRgbBlendFactor { get; set; }

		[Export ("rgbBlendOperation", ArgumentSemantic.Assign)]
		MTLBlendOperation RgbBlendOperation { get; set; }

		[Export ("sourceAlphaBlendFactor", ArgumentSemantic.Assign)]
		MTLBlendFactor SourceAlphaBlendFactor { get; set; }

		[Export ("destinationAlphaBlendFactor", ArgumentSemantic.Assign)]
		MTLBlendFactor DestinationAlphaBlendFactor { get; set; }

		[Export ("alphaBlendOperation", ArgumentSemantic.Assign)]
		MTLBlendOperation AlphaBlendOperation { get; set; }

		[Export ("writeMask", ArgumentSemantic.Assign)]
		MTLColorWriteMask WriteMask { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPipelineReflection {
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("vertexArguments")]
		[NullAllowed]
#if NET
		MTLArgument [] VertexArguments { get; }
#else
		NSObject [] VertexArguments { get; }
#endif

		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("fragmentArguments")]
		[NullAllowed]
#if NET
		MTLArgument [] FragmentArguments { get; }
#else
		NSObject [] FragmentArguments { get; }
#endif

		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Mac (11, 0), NoWatch]
		[TV (14, 5)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("tileArguments")]
		MTLArgument [] TileArguments { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("vertexBindings")]
		IMTLBinding [] VertexBindings { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("fragmentBindings")]
		IMTLBinding [] FragmentBindings { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("tileBindings")]
		IMTLBinding [] TileBindings { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("objectBindings")]
		IMTLBinding [] ObjectBindings { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("meshBindings")]
		IMTLBinding [] MeshBindings { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPassAttachmentDescriptor : NSCopying {

		[NullAllowed] // by default this property is null
		[Export ("texture", ArgumentSemantic.Retain)]
		IMTLTexture Texture { get; set; }

		[Export ("level")]
		nuint Level { get; set; }

		[Export ("slice")]
		nuint Slice { get; set; }

		[Export ("depthPlane")]
		nuint DepthPlane { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("resolveTexture", ArgumentSemantic.Retain)]
		IMTLTexture ResolveTexture { get; set; }

		[Export ("resolveLevel")]
		nuint ResolveLevel { get; set; }

		[Export ("resolveSlice")]
		nuint ResolveSlice { get; set; }

		[Export ("resolveDepthPlane")]
		nuint ResolveDepthPlane { get; set; }

		[Export ("loadAction")]
		MTLLoadAction LoadAction { get; set; }

		[Export ("storeAction")]
		MTLStoreAction StoreAction { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("storeActionOptions", ArgumentSemantic.Assign)]
		MTLStoreActionOptions StoreActionOptions { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	interface MTLRenderPassColorAttachmentDescriptor {
		[Export ("clearColor")]
		MTLClearColor ClearColor { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	interface MTLRenderPassDepthAttachmentDescriptor {

		[Export ("clearDepth")]
		double ClearDepth { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("depthResolveFilter")]
		MTLMultisampleDepthResolveFilter DepthResolveFilter { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	interface MTLRenderPassStencilAttachmentDescriptor {

		[Export ("clearStencil")]
		uint ClearStencil { get; set; } /* uint32_t */

		[iOS (12, 0)]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Export ("stencilResolveFilter", ArgumentSemantic.Assign)]
		MTLMultisampleStencilResolveFilter StencilResolveFilter { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPassColorAttachmentDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLRenderPassColorAttachmentDescriptor ObjectAtIndexedSubscript (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject ([NullAllowed] MTLRenderPassColorAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPassDescriptor : NSCopying {

		[Export ("colorAttachments")]
		MTLRenderPassColorAttachmentDescriptorArray ColorAttachments { get; }

		[Export ("depthAttachment", ArgumentSemantic.Copy)]
		[NullAllowed]
		MTLRenderPassDepthAttachmentDescriptor DepthAttachment { get; set; }

		[Export ("stencilAttachment", ArgumentSemantic.Copy)]
		[NullAllowed]
		MTLRenderPassStencilAttachmentDescriptor StencilAttachment { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("visibilityResultBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer VisibilityResultBuffer { get; set; }

		[Static, Export ("renderPassDescriptor")]
		[Autorelease]
		MTLRenderPassDescriptor CreateRenderPassDescriptor ();

		[iOS (12, 0)]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Export ("renderTargetArrayLength")]
		nuint RenderTargetArrayLength { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("setSamplePositions:count:")]
		unsafe void SetSamplePositions ([NullAllowed] IntPtr positions, nuint count);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("getSamplePositions:count:")]
		nuint GetSamplePositions ([NullAllowed] IntPtr positions, nuint count);

		[NoWatch, Mac (11, 0)]
		[TV (14, 5)]
		[MacCatalyst (14, 0)]
		[Export ("imageblockSampleLength")]
		nuint ImageblockSampleLength { get; set; }

		[NoWatch, Mac (11, 0)]
		[TV (14, 5)]
		[MacCatalyst (14, 0)]
		[Export ("threadgroupMemoryLength")]
		nuint ThreadgroupMemoryLength { get; set; }

		[NoWatch, Mac (11, 0)]
		[TV (14, 5)]
		[MacCatalyst (14, 0)]
		[Export ("tileWidth")]
		nuint TileWidth { get; set; }

		[NoWatch, Mac (11, 0)]
		[TV (14, 5)]
		[MacCatalyst (14, 0)]
		[Export ("tileHeight")]
		nuint TileHeight { get; set; }

		[NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Export ("defaultRasterSampleCount")]
		nuint DefaultRasterSampleCount { get; set; }

		[NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Export ("renderTargetWidth")]
		nuint RenderTargetWidth { get; set; }

		[NoWatch]
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Export ("renderTargetHeight")]
		nuint RenderTargetHeight { get; set; }

		/* Selectors reported missing by instrospection: https://github.com/xamarin/maccore/issues/1978
				[NoMac, NoTV, iOS (13, 0)]
				[NoMacCatalyst]
				[Export ("maxVertexAmplificationCount")]
				nuint MaxVertexAmplificationCount { get; set; }
		*/

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[TV (17, 0), iOS (13, 0)]
		[NullAllowed, Export ("rasterizationRateMap", ArgumentSemantic.Strong)]
		IMTLRasterizationRateMap RasterizationRateMap { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sampleBufferAttachments")]
		MTLRenderPassSampleBufferAttachmentDescriptorArray SampleBufferAttachments { get; }
	}


	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// note: type works only on devices, symbol is missing on the simulator
	interface MTLHeapDescriptor : NSCopying {
		[Export ("size")]
		nuint Size { get; set; }

		[Export ("storageMode", ArgumentSemantic.Assign)]
		MTLStorageMode StorageMode { get; set; }

		[Export ("cpuCacheMode", ArgumentSemantic.Assign)]
		MTLCpuCacheMode CpuCacheMode { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("hazardTrackingMode", ArgumentSemantic.Assign)]
		MTLHazardTrackingMode HazardTrackingMode { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("resourceOptions", ArgumentSemantic.Assign)]
		MTLResourceOptions ResourceOptions { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("type", ArgumentSemantic.Assign)]
		MTLHeapType Type { get; set; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("sparsePageSize", ArgumentSemantic.Assign)]
		MTLSparsePageSize SparsePageSize { get; set; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	interface MTLHeap {
		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[Export ("storageMode")]
		MTLStorageMode StorageMode { get; }

		[Abstract]
		[Export ("cpuCacheMode")]
		MTLCpuCacheMode CpuCacheMode { get; }

		[Abstract]
		[Export ("size")]
		nuint Size { get; }

		[Abstract]
		[Export ("usedSize")]
		nuint UsedSize { get; }

		[Abstract]
		[Export ("maxAvailableSizeWithAlignment:")]
		nuint GetMaxAvailableSize (nuint alignment);

		[Abstract]
		[Export ("newBufferWithLength:options:")]
		[return: NullAllowed]
		[return: Release]
		IMTLBuffer CreateBuffer (nuint length, MTLResourceOptions options);

		[Abstract]
		[Export ("newTextureWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateTexture (MTLTextureDescriptor desc);

		[Abstract]
		[Export ("setPurgeableState:")]
		MTLPurgeableState SetPurgeableState (MTLPurgeableState state);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("currentAllocatedSize")]
		nuint CurrentAllocatedSize { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("hazardTrackingMode")]
		MTLHazardTrackingMode HazardTrackingMode { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("resourceOptions")]
		MTLResourceOptions ResourceOptions { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("type")]
		MTLHeapType Type { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newBufferWithLength:options:offset:")]
		[return: NullAllowed]
		[return: Release]
		IMTLBuffer CreateBuffer (nuint length, MTLResourceOptions options, nuint offset);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("newTextureWithDescriptor:offset:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor, nuint offset);

#if XAMCORE_5_0
		[Abstract]
#endif
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("newAccelerationStructureWithSize:")]
		[return: NullAllowed, Release]
		IMTLAccelerationStructure CreateAccelerationStructure (nuint size);

#if XAMCORE_5_0
		[Abstract]
#endif
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("newAccelerationStructureWithDescriptor:")]
		[return: NullAllowed, Release]
		IMTLAccelerationStructure CreateAccelerationStructure (MTLAccelerationStructureDescriptor descriptor);

#if XAMCORE_5_0
		[Abstract]
#endif
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("newAccelerationStructureWithSize:offset:")]
		[return: NullAllowed, Release]
		IMTLAccelerationStructure CreateAccelerationStructure (nuint size, nuint offset);

#if XAMCORE_5_0
		[Abstract]
#endif
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("newAccelerationStructureWithDescriptor:offset:")]
		[return: NullAllowed, Release]
		IMTLAccelerationStructure CreateAccelerationStructure (MTLAccelerationStructureDescriptor descriptor, nuint offset);
	}

	interface IMTLResource { }
	interface IMTLHeap { }
	[MacCatalyst (13, 1)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLResource {

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("cpuCacheMode")]
		MTLCpuCacheMode CpuCacheMode { get; }

#if NET
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[MacCatalyst (13, 1)]
		[Export ("storageMode")]
		MTLStorageMode StorageMode { get; }

		[Abstract, Export ("setPurgeableState:")]
		MTLPurgeableState SetPurgeableState (MTLPurgeableState state);

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("heap")]
		IMTLHeap Heap { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("makeAliasable")]
		void MakeAliasable ();

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("isAliasable")]
		bool IsAliasable { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("allocatedSize")]
		nuint AllocatedSize { get; }

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resourceOptions")]
		MTLResourceOptions ResourceOptions { get; }

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("heapOffset")]
		nuint HeapOffset { get; }

#if NET
		[Abstract]
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hazardTrackingMode")]
		MTLHazardTrackingMode HazardTrackingMode { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLComputePipelineDescriptor : NSCopying {

		[Export ("label")]
		[NullAllowed]
		string Label { get; set; }

		[Export ("computeFunction", ArgumentSemantic.Strong)]
		[NullAllowed]
		IMTLFunction ComputeFunction { get; set; }

		[Export ("threadGroupSizeIsMultipleOfThreadExecutionWidth")]
		bool ThreadGroupSizeIsMultipleOfThreadExecutionWidth { get; set; }

		[Export ("reset")]
		void Reset ();

		[iOS (12, 0), TV (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("stageInputDescriptor", ArgumentSemantic.Copy)]
		MTLStageInputOutputDescriptor StageInputDescriptor { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("buffers")]
		MTLPipelineBufferDescriptorArray Buffers { get; }

		[Mac (11, 0), iOS (13, 0), TV (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'PreloadedLibraries' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'PreloadedLibraries' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'PreloadedLibraries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'PreloadedLibraries' instead.")]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("insertLibraries", ArgumentSemantic.Copy)]
		IMTLDynamicLibrary [] InsertLibraries { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("binaryArchives", ArgumentSemantic.Copy)]
		IMTLBinaryArchive [] BinaryArchives { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("linkedFunctions", ArgumentSemantic.Copy)]
		MTLLinkedFunctions LinkedFunctions { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (17, 0)]
		[MacCatalyst (14, 0)]
		[Export ("supportAddingBinaryFunctions")]
		bool SupportAddingBinaryFunctions { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (17, 0)]
		[MacCatalyst (14, 0)]
		[Export ("maxCallStackDepth")]
		nuint MaxCallStackDepth { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), NoWatch, MacCatalyst (15, 0)]
		[Export ("preloadedLibraries", ArgumentSemantic.Copy)]
		IMTLDynamicLibrary [] PreloadedLibraries { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLStageInputOutputDescriptor : NSCopying {
		[Static]
		[Export ("stageInputOutputDescriptor")]
		MTLStageInputOutputDescriptor Create ();

		[Export ("layouts")]
		MTLBufferLayoutDescriptorArray Layouts { get; }

		[Export ("attributes")]
		MTLAttributeDescriptorArray Attributes { get; }

		[Export ("indexType", ArgumentSemantic.Assign)]
		MTLIndexType IndexType { get; set; }

		[Export ("indexBufferIndex")]
		nuint IndexBufferIndex { get; set; }

		[Export ("reset")]
		void Reset ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLType {
		[Export ("dataType")]
		MTLDataType DataType { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MTLType))]
	interface MTLPointerType {
		[Export ("elementType")]
		MTLDataType ElementType { get; }

		[Export ("access")]
		MTLArgumentAccess Access { get; }

		[Export ("alignment")]
		nuint Alignment { get; }

		[Export ("dataSize")]
		nuint DataSize { get; }

		[Export ("elementIsArgumentBuffer")]
		bool ElementIsArgumentBuffer { get; }

		[NullAllowed, Export ("elementStructType")]
		MTLStructType ElementStructType { get; }

		[NullAllowed, Export ("elementArrayType")]
		MTLArrayType ElementArrayType { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MTLType))]
	interface MTLTextureReferenceType {
		[Export ("textureDataType")]
		MTLDataType TextureDataType { get; }

		[Export ("textureType")]
		MTLTextureType TextureType { get; }

		[Export ("access")]
		MTLArgumentAccess Access { get; }

		[Export ("isDepthTexture")]
		bool IsDepthTexture { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface IMTLCaptureScope { }

	[NoWatch]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface MTLCaptureScope {
		[Abstract]
		[Export ("beginScope")]
		void BeginScope ();

		[Abstract]
		[Export ("endScope")]
		void EndScope ();

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[NullAllowed, Export ("commandQueue")]
		IMTLCommandQueue CommandQueue { get; }
	}


	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTLCaptureManager {
		[Static]
		[Export ("sharedCaptureManager")]
		MTLCaptureManager Shared { get; }

		[Export ("newCaptureScopeWithDevice:")]
		[return: Release]
		IMTLCaptureScope CreateNewCaptureScope (IMTLDevice device);

		[Export ("newCaptureScopeWithCommandQueue:")]
		[return: Release]
		IMTLCaptureScope CreateNewCaptureScope (IMTLCommandQueue commandQueue);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Export ("startCaptureWithDevice:")]
		void StartCapture (IMTLDevice device);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Export ("startCaptureWithCommandQueue:")]
		void StartCapture (IMTLCommandQueue commandQueue);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Export ("startCaptureWithScope:")]
		void StartCapture (IMTLCaptureScope captureScope);

		[Export ("stopCapture")]
		void StopCapture ();

		[NullAllowed, Export ("defaultCaptureScope", ArgumentSemantic.Strong)]
		IMTLCaptureScope DefaultCaptureScope { get; set; }

		[Export ("isCapturing")]
		bool IsCapturing { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("supportsDestination:")]
		bool SupportsDestination (MTLCaptureDestination destination);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("startCaptureWithDescriptor:error:")]
		bool StartCapture (MTLCaptureDescriptor descriptor, [NullAllowed] out NSError error);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLPipelineBufferDescriptor : NSCopying {
		[Export ("mutability", ArgumentSemantic.Assign)]
		MTLMutability Mutability { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLPipelineBufferDescriptorArray {
		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		MTLPipelineBufferDescriptor GetObject (nuint bufferIndex);

		[Internal]
		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLPipelineBufferDescriptor buffer, nuint bufferIndex);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLArgumentDescriptor : NSCopying {
		[Static]
		[Export ("argumentDescriptor")]
		MTLArgumentDescriptor Create ();

		[Export ("dataType", ArgumentSemantic.Assign)]
		MTLDataType DataType { get; set; }

		[Export ("index")]
		nuint Index { get; set; }

		[Export ("arrayLength")]
		nuint ArrayLength { get; set; }

		[Export ("access", ArgumentSemantic.Assign)]
		MTLArgumentAccess Access { get; set; }

		[Export ("textureType", ArgumentSemantic.Assign)]
		MTLTextureType TextureType { get; set; }

		[Export ("constantBlockAlignment")]
		nuint ConstantBlockAlignment { get; set; }
	}

	interface IMTLArgumentEncoder { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface MTLArgumentEncoder {
		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Abstract]
		[Export ("encodedLength")]
		nuint EncodedLength { get; }

		[Abstract]
		[Export ("alignment")]
		nuint Alignment { get; }

		[Abstract]
		[Export ("setArgumentBuffer:offset:")]
		void SetArgumentBuffer ([NullAllowed] IMTLBuffer argumentBuffer, nuint offset);

		[Abstract]
		[Export ("setArgumentBuffer:startOffset:arrayElement:")]
		void SetArgumentBuffer ([NullAllowed] IMTLBuffer argumentBuffer, nuint startOffset, nuint arrayElement);

		[Abstract]
		[Export ("setBuffer:offset:atIndex:")]
		void SetBuffer ([NullAllowed] IMTLBuffer buffer, nuint offset, nuint index);

#if NET
		[Abstract]
		[Export ("setBuffers:offsets:withRange:")]
		void SetBuffers (IntPtr buffers, IntPtr offsets, NSRange range);
#else
		[Abstract]
		[Export ("setBuffers:offsets:withRange:")]
		void SetBuffers (IMTLBuffer [] buffers, IntPtr offsets, NSRange range);
#endif

		[Abstract]
		[Export ("setTexture:atIndex:")]
		void SetTexture ([NullAllowed] IMTLTexture texture, nuint index);

		[Abstract]
		[Export ("setTextures:withRange:")]
		void SetTextures (IMTLTexture [] textures, NSRange range);

		[Abstract]
		[Export ("setSamplerState:atIndex:")]
		void SetSamplerState ([NullAllowed] IMTLSamplerState sampler, nuint index);

		[Abstract]
		[Export ("setSamplerStates:withRange:")]
		void SetSamplerStates (IMTLSamplerState [] samplers, NSRange range);

		[Abstract]
		[Export ("constantDataAtIndex:")]
		IntPtr GetConstantData (nuint index);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setRenderPipelineState:atIndex:")]
		void SetRenderPipelineState ([NullAllowed] IMTLRenderPipelineState pipeline, nuint index);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setRenderPipelineStates:withRange:")]
		void SetRenderPipelineStates (IMTLRenderPipelineState [] pipelines, NSRange range);

		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setIndirectCommandBuffer:atIndex:")]
		void SetIndirectCommandBuffer ([NullAllowed] IMTLIndirectCommandBuffer indirectCommandBuffer, nuint index);

		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setIndirectCommandBuffers:withRange:")]
		void SetIndirectCommandBuffers (IMTLIndirectCommandBuffer [] buffers, NSRange range);

#if MONOMAC || NET
		[Abstract]
#endif
		[Export ("newArgumentEncoderForBufferAtIndex:")]
		[return: NullAllowed]
		[return: Release]
		IMTLArgumentEncoder CreateArgumentEncoder (nuint index);

		[TV (13, 0), Mac (11, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setComputePipelineState:atIndex:")]
		void SetComputePipelineState ([NullAllowed] IMTLComputePipelineState pipeline, nuint index);

		[TV (13, 0), Mac (11, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("setComputePipelineStates:withRange:")]
		void SetComputePipelineStates (IMTLComputePipelineState [] pipelines, NSRange range);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setAccelerationStructure:atIndex:")]
		void SetAccelerationStructure ([NullAllowed] IMTLAccelerationStructure accelerationStructure, nuint index);

#if NET

#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setVisibleFunctionTable:atIndex:")]
		void SetVisibleFunctionTable ([NullAllowed] IMTLVisibleFunctionTable visibleFunctionTable, nuint index);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setVisibleFunctionTables:withRange:")]
		void SetVisibleFunctionTables (IMTLVisibleFunctionTable [] visibleFunctionTables, NSRange range);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setIntersectionFunctionTable:atIndex:")]
		void SetIntersectionFunctionTable ([NullAllowed] IMTLIntersectionFunctionTable intersectionFunctionTable, nuint index);

#if NET
#if XAMCORE_5_0
		[Mac (11, 0), iOS (14, 0), TV (16,0), MacCatalyst (14, 0)]
		[Abstract]
#elif !TVOS
		[Mac (11, 0), iOS (14, 0), NoTV, MacCatalyst (14, 0)]
		[Abstract]
#else
		[NoMac, NoiOS, TV (16,0), NoMacCatalyst]
#endif

#else
		[Mac (11, 0), iOS (14, 0), TV (16, 0), MacCatalyst (14, 0)]
#endif
		[Export ("setIntersectionFunctionTables:withRange:")]
		void SetIntersectionFunctionTables (IMTLIntersectionFunctionTable [] intersectionFunctionTables, NSRange range);

	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Mac (11, 0), NoWatch]
	[TV (14, 5)]
	[BaseType (typeof (NSObject))]
	interface MTLTileRenderPipelineColorAttachmentDescriptor : NSCopying {
		[Export ("pixelFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat PixelFormat { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Mac (11, 0), NoWatch]
	[TV (14, 5)]
	[BaseType (typeof (NSObject))]
	interface MTLTileRenderPipelineColorAttachmentDescriptorArray {
		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		MTLTileRenderPipelineColorAttachmentDescriptor GetObject (nuint attachmentIndex);

		[Internal]
		[Export ("setObject:atIndexedSubscript:")]
		void SetObject (MTLTileRenderPipelineColorAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	interface IMTLBinaryArchive { }

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLBinaryArchive {

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[Export ("addComputePipelineFunctionsWithDescriptor:error:")]
		bool AddComputePipelineFunctions (MTLComputePipelineDescriptor descriptor, [NullAllowed] out NSError error);

		[Abstract]
		[Export ("addRenderPipelineFunctionsWithDescriptor:error:")]
		bool AddRenderPipelineFunctions (MTLRenderPipelineDescriptor descriptor, [NullAllowed] out NSError error);

#if !TVOS || NET
		[Abstract]
#endif
		[TV (14, 5)]
		[MacCatalyst (14, 0)]
		[Export ("addTileRenderPipelineFunctionsWithDescriptor:error:")]
		bool AddTileRenderPipelineFunctions (MTLTileRenderPipelineDescriptor descriptor, [NullAllowed] out NSError error);

		[Abstract]
		[Export ("serializeToURL:error:")]
		bool Serialize (NSUrl url, [NullAllowed] out NSError error);

#if NET
		[Abstract]
#endif
		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("addFunctionWithDescriptor:library:error:")]
		bool AddFunctionWithDescriptor (MTLFunctionDescriptor descriptor, IMTLLibrary library, [NullAllowed] out NSError error);
	}


	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Mac (11, 0), NoWatch]
	[TV (14, 5)]
	[BaseType (typeof (NSObject))]
	interface MTLTileRenderPipelineDescriptor : NSCopying {
		[NullAllowed]
		[Export ("label")]
		string Label { get; set; }

		[Export ("tileFunction", ArgumentSemantic.Strong)]
		IMTLFunction TileFunction { get; set; }

		[Export ("rasterSampleCount")]
		nuint RasterSampleCount { get; set; }

		[Export ("colorAttachments")]
		MTLTileRenderPipelineColorAttachmentDescriptorArray ColorAttachments { get; }

		[Export ("threadgroupSizeMatchesTileSize")]
		bool ThreadgroupSizeMatchesTileSize { get; set; }

		[Export ("tileBuffers")]
		MTLPipelineBufferDescriptorArray TileBuffers { get; }

		[iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; set; }

		[Export ("reset")]
		void Reset ();

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("binaryArchives", ArgumentSemantic.Copy)]
		IMTLBinaryArchive [] BinaryArchives { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (17, 0), NoWatch]
		[Export ("supportAddingBinaryFunctions")]
		bool SupportAddingBinaryFunctions { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("preloadedLibraries", ArgumentSemantic.Copy)]
		IMTLDynamicLibrary [] PreloadedLibraries { get; set; }

		[Mac (11, 0), iOS (14, 0), MacCatalyst (15, 0), TV (17, 0), NoWatch]
		[Export ("maxCallStackDepth")]
		nuint MaxCallStackDepth { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[NullAllowed, Export ("linkedFunctions", ArgumentSemantic.Copy)]
		MTLLinkedFunctions LinkedFunctions { get; set; }
	}

	interface IMTLEvent { }

	[iOS (12, 0), TV (12, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface MTLEvent {
		[Abstract]
		[NullAllowed, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	[iOS (12, 0), TV (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface MTLSharedEventListener {
		[Export ("initWithDispatchQueue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DispatchQueue dispatchQueue);

		[Export ("dispatchQueue")]
		DispatchQueue DispatchQueue { get; }
	}

	delegate void MTLSharedEventNotificationBlock (IMTLSharedEvent @event, ulong value);

	interface IMTLSharedEvent { }

	[iOS (12, 0), TV (12, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface MTLSharedEvent : MTLEvent {
		[Abstract]
		[Export ("notifyListener:atValue:block:")]
		void NotifyListener (MTLSharedEventListener listener, ulong atValue, MTLSharedEventNotificationBlock block);

		[Abstract]
		[Export ("newSharedEventHandle")]
		[return: Release]
		MTLSharedEventHandle CreateSharedEventHandle ();

		[Abstract]
		[Export ("signaledValue")]
		ulong SignaledValue { get; set; }
	}

	[iOS (12, 0), TV (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLSharedEventHandle : NSSecureCoding {
		[NullAllowed, Export ("label")]
		string Label { get; }
	}

	interface IMTLIndirectRenderCommand { }

	[iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface MTLIndirectRenderCommand {

#if MONOMAC && !NET
		[Abstract]
#endif
#if NET
		[Abstract]
#endif
		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setRenderPipelineState:")]
		void SetRenderPipelineState (IMTLRenderPipelineState pipelineState);

		[Abstract]
		[Export ("setVertexBuffer:offset:atIndex:")]
		void SetVertexBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract]
		[Export ("setFragmentBuffer:offset:atIndex:")]
		void SetFragmentBuffer (IMTLBuffer buffer, nuint offset, nuint index);

#if !TVOS || NET
		[Abstract]
#endif
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Export ("drawPatches:patchStart:patchCount:patchIndexBuffer:patchIndexBufferOffset:instanceCount:baseInstance:tessellationFactorBuffer:tessellationFactorBufferOffset:tessellationFactorBufferInstanceStride:")]
		void DrawPatches (nuint numberOfPatchControlPoints, nuint patchStart, nuint patchCount, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, nuint instanceCount, nuint baseInstance, IMTLBuffer buffer, nuint offset, nuint instanceStride);

#if !TVOS || NET
		[Abstract]
#endif
		[TV (14, 5)]
		[MacCatalyst (13, 1)]
		[Export ("drawIndexedPatches:patchStart:patchCount:patchIndexBuffer:patchIndexBufferOffset:controlPointIndexBuffer:controlPointIndexBufferOffset:instanceCount:baseInstance:tessellationFactorBuffer:tessellationFactorBufferOffset:tessellationFactorBufferInstanceStride:")]
		void DrawIndexedPatches (nuint numberOfPatchControlPoints, nuint patchStart, nuint patchCount, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, IMTLBuffer controlPointIndexBuffer, nuint controlPointIndexBufferOffset, nuint instanceCount, nuint baseInstance, IMTLBuffer buffer, nuint offset, nuint instanceStride);

		[Abstract]
		[Export ("drawPrimitives:vertexStart:vertexCount:instanceCount:baseInstance:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount, nuint instanceCount, nuint baseInstance);

		[Abstract]
		[Export ("drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:instanceCount:baseVertex:baseInstance:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, nuint indexCount, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset, nuint instanceCount, nint baseVertex, nuint baseInstance);

		[Abstract]
		[Export ("reset")]
		void Reset ();

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setVertexBuffer:offset:attributeStride:atIndex:")]
		void SetVertexBuffer (IMTLBuffer buffer, nuint offset, nuint stride, nuint index);

		[NoMac, iOS (17, 0), NoTV, MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectThreadgroupMemoryLength:atIndex:")]
		void SetObjectThreadgroupMemoryLength (nuint length, nuint index);

		[NoMac, iOS (17, 0), NoTV, MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setObjectBuffer:offset:atIndex:")]
		void SetObjectBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[NoMac, iOS (17, 0), NoTV, MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setMeshBuffer:offset:atIndex:")]
		void SetMeshBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[NoMac, iOS (17, 0), NoTV, MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("drawMeshThreadgroups:threadsPerObjectThreadgroup:threadsPerMeshThreadgroup:")]
		void DrawMeshThreadgroups (MTLSize threadgroupsPerGrid, MTLSize threadsPerObjectThreadgroup, MTLSize threadsPerMeshThreadgroup);

		[NoMac, iOS (17, 0), NoTV, MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("drawMeshThreads:threadsPerObjectThreadgroup:threadsPerMeshThreadgroup:")]
		void DrawMeshThreads (MTLSize threadsPerGrid, MTLSize threadsPerObjectThreadgroup, MTLSize threadsPerMeshThreadgroup);

		[NoMac, iOS (17, 0), NoTV, MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setBarrier")]
		void SetBarrier ();

		[NoMac, iOS (17, 0), NoTV, MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("clearBarrier")]
		void ClearBarrier ();
	}

	[iOS (12, 0), TV (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLIndirectCommandBufferDescriptor : NSCopying {
		[Export ("commandTypes", ArgumentSemantic.Assign)]
		MTLIndirectCommandType CommandTypes { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("inheritPipelineState")]
		bool InheritPipelineState { get; set; }

		[Export ("inheritBuffers")]
		bool InheritBuffers { get; set; }

		[Export ("maxVertexBufferBindCount")]
		nuint MaxVertexBufferBindCount { get; set; }

		[Export ("maxFragmentBufferBindCount")]
		nuint MaxFragmentBufferBindCount { get; set; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("maxKernelBufferBindCount")]
		nuint MaxKernelBufferBindCount { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("maxKernelThreadgroupMemoryBindCount")]
		nuint MaxKernelThreadgroupMemoryBindCount { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("maxObjectBufferBindCount")]
		nuint MaxObjectBufferBindCount { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("maxMeshBufferBindCount")]
		nuint MaxMeshBufferBindCount { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("maxObjectThreadgroupMemoryBindCount")]
		nuint MaxObjectThreadgroupMemoryBindCount { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("supportDynamicAttributeStride")]
		bool SupportDynamicAttributeStride { get; set; }

		[Mac (13, 0), iOS (16, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("supportRayTracing")]
		bool SupportRayTracing { get; set; }

	}

	interface IMTLIndirectCommandBuffer { }

	[iOS (12, 0), TV (12, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface MTLIndirectCommandBuffer : MTLResource {
		[Abstract]
		[Export ("size")]
		nuint Size { get; }

		[Abstract]
		[Export ("resetWithRange:")]
		void Reset (NSRange range);

		[Abstract]
		[Export ("indirectRenderCommandAtIndex:")]
		IMTLIndirectRenderCommand GetCommand (nuint commandIndex);

#if NET
		[Abstract]
#endif
		[TV (13, 0), Mac (11, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("indirectComputeCommandAtIndex:")]
		IMTLIndirectComputeCommand GetIndirectComputeCommand (nuint commandIndex);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("gpuResourceID")]
		MTLResourceId GpuResourceID { get; }
	}

	[iOS (13, 0), TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTLSharedTextureHandle : NSSecureCoding {
		[Export ("device")]
		IMTLDevice Device { get; }

		[NullAllowed, Export ("label")]
		string Label { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[TV (16, 0), iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLRasterizationRateSampleArray {
		[Export ("objectAtIndexedSubscript:")]
		NSNumber GetObject (nuint index);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject (NSNumber value, nuint index);
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[TV (16, 0), iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLRasterizationRateMapDescriptor : NSCopying {
		[Static]
		[Export ("rasterizationRateMapDescriptorWithScreenSize:")]
		MTLRasterizationRateMapDescriptor Create (MTLSize screenSize);

		[Static]
		[Export ("rasterizationRateMapDescriptorWithScreenSize:layer:")]
		MTLRasterizationRateMapDescriptor Create (MTLSize screenSize, MTLRasterizationRateLayerDescriptor layer);

		[Static]
		[Export ("rasterizationRateMapDescriptorWithScreenSize:layerCount:layers:")]
		MTLRasterizationRateMapDescriptor Create (MTLSize screenSize, nuint layerCount, out MTLRasterizationRateLayerDescriptor layers);

		[Export ("layerAtIndex:")]
		[return: NullAllowed]
		MTLRasterizationRateLayerDescriptor GetLayer (nuint layerIndex);

		[Export ("setLayer:atIndex:")]
		void SetLayer ([NullAllowed] MTLRasterizationRateLayerDescriptor layer, nuint layerIndex);

		[iOS (15, 0), MacCatalyst (15, 0), Mac (12, 0), NoWatch]
		[Export ("layers")]
		MTLRasterizationRateLayerArray Layers { get; }

		[iOS (15, 0), MacCatalyst (15, 0), Mac (12, 0), NoWatch]
		[Export ("screenSize", ArgumentSemantic.Assign)]
		MTLSize ScreenSize { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0), Mac (12, 0), NoWatch]
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0), Mac (12, 0), NoWatch]
		[Export ("layerCount")]
		nuint LayerCount { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[TV (16, 0), iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTLRasterizationRateLayerDescriptor : NSCopying {

		[Export ("initWithSampleCount:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MTLSize sampleCount);

		[Internal]
		[Export ("initWithSampleCount:horizontal:vertical:")]
		NativeHandle Constructor (MTLSize sampleCount, IntPtr horizontal, IntPtr vertical);

		[MacCatalyst (15, 0)]
		[Internal]
		[Export ("horizontalSampleStorage")]
		IntPtr _HorizontalSampleStorage { get; }

		[MacCatalyst (15, 0)]
		[Internal]
		[Export ("verticalSampleStorage")]
		IntPtr _VerticalSampleStorage { get; }

		[MacCatalyst (15, 0)]
		[Export ("horizontal")]
		MTLRasterizationRateSampleArray Horizontal { get; }

		[MacCatalyst (15, 0)]
		[Export ("vertical")]
		MTLRasterizationRateSampleArray Vertical { get; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("maxSampleCount")]
		MTLSize MaxSampleCount { get; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("sampleCount", ArgumentSemantic.Assign)]
		MTLSize SampleCount { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[TV (16, 0), iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLRasterizationRateLayerArray {
		[Export ("objectAtIndexedSubscript:")]
		[return: NullAllowed]
		MTLRasterizationRateLayerDescriptor GetObject (nuint layerIndex);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLRasterizationRateLayerDescriptor layer, nuint layerIndex);
	}

	interface IMTLRasterizationRateMap { }

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[TV (16, 0), iOS (13, 0)]
	[Protocol]
	interface MTLRasterizationRateMap {
		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; }

		[Abstract]
		[Export ("screenSize")]
		MTLSize ScreenSize { get; }

		[Abstract]
		[Export ("physicalGranularity")]
		MTLSize PhysicalGranularity { get; }

		[Abstract]
		[Export ("layerCount")]
		nuint LayerCount { get; }

		[Abstract]
		[Export ("parameterBufferSizeAndAlign")]
		MTLSizeAndAlign ParameterBufferSizeAndAlign { get; }

		[Abstract]
		[Export ("copyParameterDataToBuffer:offset:")]
		void CopyParameterData (IMTLBuffer buffer, nuint offset);

		[Abstract]
		[Export ("physicalSizeForLayer:")]
		MTLSize GetPhysicalSize (nuint layerIndex);

		[Abstract]
		[Export ("mapScreenToPhysicalCoordinates:forLayer:")]
		MTLCoordinate2D MapScreenToPhysicalCoordinates (MTLCoordinate2D screenCoordinates, nuint layerIndex);

		[Abstract]
		[Export ("mapPhysicalToScreenCoordinates:forLayer:")]
		MTLCoordinate2D MapPhysicalToScreenCoordinates (MTLCoordinate2D physicalCoordinates, nuint layerIndex);
	}

	interface IMTLResourceStateCommandEncoder { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Mac (11, 0), iOS (13, 0), TV (16, 0)]
	[Protocol]
	interface MTLResourceStateCommandEncoder : MTLCommandEncoder {
#if !MONOMAC && !__MACCATALYST__
		[Abstract]
#endif
		[Export ("updateTextureMappings:mode:regions:mipLevels:slices:numRegions:")]
		void Update (IMTLTexture texture, MTLSparseTextureMappingMode mode, IntPtr regions, IntPtr mipLevels, IntPtr slices, nuint numRegions);

#if !MONOMAC && !__MACCATALYST__
		[Abstract]
#endif
		[Export ("updateTextureMapping:mode:region:mipLevel:slice:")]
		void Update (IMTLTexture texture, MTLSparseTextureMappingMode mode, MTLRegion region, nuint mipLevel, nuint slice);

#if !MONOMAC && !__MACCATALYST__
		[Abstract]
#endif
		[Export ("updateTextureMapping:mode:indirectBuffer:indirectBufferOffset:")]
		void Update (IMTLTexture texture, MTLSparseTextureMappingMode mode, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

#if !MONOMAC && !__MACCATALYST__
		[Abstract]
#endif
		[Export ("updateFence:")]
		void Update (IMTLFence fence);

#if !MONOMAC && !__MACCATALYST__
		[Abstract]
#endif
		[Export ("waitForFence:")]
		void Wait (IMTLFence fence);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("moveTextureMappingsFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:")]
		void MoveTextureMappings (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, MTLOrigin sourceOrigin, MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel, MTLOrigin destinationOrigin);
	}

	[iOS (13, 0), TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MTLCaptureDescriptor : NSCopying {
		[NullAllowed, Export ("captureObject", ArgumentSemantic.Strong)]
		NSObject CaptureObject { get; set; }

		[Export ("destination", ArgumentSemantic.Assign)]
		MTLCaptureDestination Destination { get; set; }

		[NullAllowed, Export ("outputURL", ArgumentSemantic.Copy)]
		NSUrl OutputUrl { get; set; }
	}

	interface IMTLIndirectComputeCommand { }

	[TV (13, 0), iOS (13, 0), Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface MTLIndirectComputeCommand {
		[Abstract]
		[Export ("setComputePipelineState:")]
		void SetComputePipelineState (IMTLComputePipelineState pipelineState);

		[Abstract]
		[Export ("setKernelBuffer:offset:atIndex:")]
		void SetKernelBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract]
		[Export ("concurrentDispatchThreadgroups:threadsPerThreadgroup:")]
		void ConcurrentDispatchThreadgroups (MTLSize threadgroupsPerGrid, MTLSize threadsPerThreadgroup);

		[Abstract]
		[Export ("concurrentDispatchThreads:threadsPerThreadgroup:")]
		void ConcurrentDispatchThreads (MTLSize threadsPerGrid, MTLSize threadsPerThreadgroup);

		[Abstract]
		[Export ("setBarrier")]
		void SetBarrier ();

		[Abstract]
		[Export ("clearBarrier")]
		void ClearBarrier ();

		[Abstract]
		[Export ("reset")]
		void Reset ();

		[Abstract]
		[Export ("setThreadgroupMemoryLength:atIndex:")]
		void SetThreadgroupMemoryLength (nuint length, nuint index);

		[Abstract]
		[Export ("setStageInRegion:")]
		void SetStageInRegion (MTLRegion region);

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("setImageblockWidth:height:")]
		void SetImageblock (nuint width, nuint height);

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setKernelBuffer:offset:attributeStride:atIndex:")]
		void SetKernelBuffer (IMTLBuffer buffer, nuint offset, nuint stride, nuint index);
	}

	interface IMTLCounter { }

	[iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
#if !NET
	[BaseType (typeof (NSObject))]
#endif
	interface MTLCounter {
		[Abstract]
		[Export ("name")]
		string Name { get; }
	}

	interface IMTLCounterSet { }

	[iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
#if !NET
	[BaseType (typeof (NSObject))]
#endif
	interface MTLCounterSet {
		[Abstract]
		[Export ("name")]
		string Name { get; }

		[Abstract]
		[Export ("counters", ArgumentSemantic.Copy)]
		IMTLCounter [] Counters { get; }
	}

	interface IMTLCounterSampleBuffer { }

	[iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
#if !NET
	[BaseType (typeof (NSObject))]
#endif
	interface MTLCounterSampleBuffer {
		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[Export ("label")]
		string Label { get; }

		[Abstract]
		[Export ("sampleCount")]
		nuint SampleCount { get; }

		[Abstract]
		[Export ("resolveCounterRange:")]
		[return: NullAllowed]
		NSData ResolveCounterRange (NSRange range);
	}

	[iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLCounterSampleBufferDescriptor : NSCopying {
		[NullAllowed]
		[Export ("counterSet", ArgumentSemantic.Retain)]
		IMTLCounterSet CounterSet { get; set; }

		[Export ("label")]
		string Label { get; set; }

		[Export ("storageMode", ArgumentSemantic.Assign)]
		MTLStorageMode StorageMode { get; set; }

		[Export ("sampleCount")]
		nuint SampleCount { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), NoTV]
	[MacCatalyst (14, 0)]
	interface IMTLAccelerationStructure { }

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLAccelerationStructure : MTLResource {
		[Abstract]
		[Export ("size")]
		nuint Size { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Abstract]
		[Export ("gpuResourceID")]
		MTLResourceId GpuResourceId { get; }
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (MTLAccelerationStructureGeometryDescriptor))]
	interface MTLAccelerationStructureBoundingBoxGeometryDescriptor {
		[NullAllowed, Export ("boundingBoxBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer BoundingBoxBuffer { get; set; }

		[Export ("boundingBoxBufferOffset")]
		nuint BoundingBoxBufferOffset { get; set; }

		[Export ("boundingBoxStride")]
		nuint BoundingBoxStride { get; set; }

		[Export ("boundingBoxCount")]
		nuint BoundingBoxCount { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLAccelerationStructureBoundingBoxGeometryDescriptor Create ();
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLAccelerationStructureDescriptor : NSCopying {
		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Export ("usage", ArgumentSemantic.Assign)]
		MTLAccelerationStructureUsage Usage { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLAccelerationStructureGeometryDescriptor : NSCopying {
		[Export ("intersectionFunctionTableOffset")]
		nuint IntersectionFunctionTableOffset { get; set; }

		[Export ("opaque")]
		bool Opaque { get; set; }

		[Export ("allowDuplicateIntersectionFunctionInvocation")]
		bool AllowDuplicateIntersectionFunctionInvocation { get; set; }

		[Mac (12, 0), iOS (15, 0), NoWatch, MacCatalyst (15, 0)]
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Mac (13, 0), iOS (16, 0), NoWatch, MacCatalyst (16, 0)]
		[NullAllowed, Export ("primitiveDataBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer PrimitiveDataBuffer { get; set; }

		[Mac (13, 0), iOS (16, 0), NoWatch, MacCatalyst (16, 0)]
		[Export ("primitiveDataBufferOffset")]
		nuint PrimitiveDataBufferOffset { get; set; }

		[Mac (13, 0), iOS (16, 0), NoWatch, MacCatalyst (16, 0)]
		[Export ("primitiveDataStride")]
		nuint PrimitiveDataStride { get; set; }

		[Mac (13, 0), iOS (16, 0), NoWatch, MacCatalyst (16, 0)]
		[Export ("primitiveDataElementSize")]
		nuint PrimitiveDataElementSize { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MTLAccelerationStructureGeometryDescriptor))]
	interface MTLAccelerationStructureTriangleGeometryDescriptor {
		[NullAllowed, Export ("vertexBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer VertexBuffer { get; set; }

		[Export ("vertexBufferOffset")]
		nuint VertexBufferOffset { get; set; }

		[Export ("vertexStride")]
		nuint VertexStride { get; set; }

		[NullAllowed, Export ("indexBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer IndexBuffer { get; set; }

		[Export ("indexBufferOffset")]
		nuint IndexBufferOffset { get; set; }

		[Export ("indexType", ArgumentSemantic.Assign)]
		MTLIndexType IndexType { get; set; }

		[Export ("triangleCount")]
		nuint TriangleCount { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLAccelerationStructureTriangleGeometryDescriptor Create ();

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Export ("vertexFormat", ArgumentSemantic.Assign)]
		MTLAttributeFormat VertexFormat { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[NullAllowed, Export ("transformationMatrixBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer TransformationMatrixBuffer { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Export ("transformationMatrixBufferOffset")]
		nuint TransformationMatrixBufferOffset { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLBinaryArchiveDescriptor : NSCopying {
		[NullAllowed, Export ("url", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLBlitPassDescriptor : NSCopying {
		[Static]
		[Export ("blitPassDescriptor")]
		MTLBlitPassDescriptor Create ();

		[Export ("sampleBufferAttachments")]
		MTLBlitPassSampleBufferAttachmentDescriptorArray SampleBufferAttachments { get; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLBlitPassSampleBufferAttachmentDescriptor : NSCopying {
		[NullAllowed, Export ("sampleBuffer", ArgumentSemantic.Retain)]
		IMTLCounterSampleBuffer SampleBuffer { get; set; }

		[Export ("startOfEncoderSampleIndex")]
		nuint StartOfEncoderSampleIndex { get; set; }

		[Export ("endOfEncoderSampleIndex")]
		nuint EndOfEncoderSampleIndex { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLBlitPassSampleBufferAttachmentDescriptorArray {
		[Export ("objectAtIndexedSubscript:")]
		MTLBlitPassSampleBufferAttachmentDescriptor GetObject (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLBlitPassSampleBufferAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLCommandBufferDescriptor : NSCopying {

		[Field ("MTLCommandBufferEncoderInfoErrorKey")]
		NSString BufferEncoderInfoErrorKey { get; }

		[Export ("retainedReferences")]
		bool RetainedReferences { get; set; }

		[Export ("errorOptions", ArgumentSemantic.Assign)]
		MTLCommandBufferErrorOption ErrorOptions { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLComputePassDescriptor : NSCopying {
		[Static]
		[Export ("computePassDescriptor")]
		MTLComputePassDescriptor Create ();

		[Export ("dispatchType", ArgumentSemantic.Assign)]
		MTLDispatchType DispatchType { get; set; }

		[Export ("sampleBufferAttachments")]
		MTLComputePassSampleBufferAttachmentDescriptorArray SampleBufferAttachments { get; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLComputePassSampleBufferAttachmentDescriptor : NSCopying {

		[NullAllowed, Export ("sampleBuffer", ArgumentSemantic.Retain)]
		IMTLCounterSampleBuffer SampleBuffer { get; set; }

		[Export ("startOfEncoderSampleIndex")]
		nuint StartOfEncoderSampleIndex { get; set; }

		[Export ("endOfEncoderSampleIndex")]
		nuint EndOfEncoderSampleIndex { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLComputePassSampleBufferAttachmentDescriptorArray {
		[Export ("objectAtIndexedSubscript:")]
		MTLComputePassSampleBufferAttachmentDescriptor GetObject (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLComputePassSampleBufferAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MTLFunctionDescriptor : NSCopying {
		[Static]
		[Export ("functionDescriptor")]
		MTLFunctionDescriptor Create ();

		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[NullAllowed, Export ("specializedName")]
		string SpecializedName { get; set; }

		[NullAllowed, Export ("constantValues", ArgumentSemantic.Copy)]
		MTLFunctionConstantValues ConstantValues { get; set; }

		[Export ("options", ArgumentSemantic.Assign)]
		MTLFunctionOptions Options { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), NoWatch, MacCatalyst (15, 0)]
		[NullAllowed, Export ("binaryArchives", ArgumentSemantic.Copy)]
		IMTLBinaryArchive [] BinaryArchives { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (MTLAccelerationStructureDescriptor))]
	interface MTLInstanceAccelerationStructureDescriptor {
		[NullAllowed, Export ("instanceDescriptorBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer InstanceDescriptorBuffer { get; set; }

		[Export ("instanceDescriptorBufferOffset")]
		nuint InstanceDescriptorBufferOffset { get; set; }

		[Export ("instanceDescriptorStride")]
		nuint InstanceDescriptorStride { get; set; }

		[Export ("instanceCount")]
		nuint InstanceCount { get; set; }

		[NullAllowed, Export ("instancedAccelerationStructures", ArgumentSemantic.Retain)]
		IMTLAccelerationStructure [] InstancedAccelerationStructures { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLInstanceAccelerationStructureDescriptor Create ();

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (17, 0), NoWatch]
		[Export ("instanceDescriptorType", ArgumentSemantic.Assign)]
		MTLAccelerationStructureInstanceDescriptorType InstanceDescriptorType { get; set; }

		[Mac (12, 0), iOS (15, 0), NoWatch, MacCatalyst (15, 0)]
		[NullAllowed, Export ("motionTransformBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer MotionTransformBuffer { get; set; }

		[Mac (12, 0), iOS (15, 0), NoWatch, MacCatalyst (15, 0)]
		[Export ("motionTransformBufferOffset")]
		nuint MotionTransformBufferOffset { get; set; }

		[Mac (12, 0), iOS (15, 0), NoWatch, MacCatalyst (15, 0)]
		[Export ("motionTransformCount")]
		nuint MotionTransformCount { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (MTLFunctionDescriptor))]
	interface MTLIntersectionFunctionDescriptor : NSCopying { }

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MTLIntersectionFunctionTableDescriptor : NSCopying {
		[Static]
		[Export ("intersectionFunctionTableDescriptor")]
		MTLIntersectionFunctionTableDescriptor Create ();

		[Export ("functionCount")]
		nuint FunctionCount { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MTLLinkedFunctions : NSCopying {
		[Static]
		[Export ("linkedFunctions")]
		MTLLinkedFunctions Create ();

		[NullAllowed, Export ("functions", ArgumentSemantic.Copy)]
		IMTLFunction [] Functions { get; set; }

		[TV (17, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("binaryFunctions", ArgumentSemantic.Copy)]
		IMTLFunction [] BinaryFunctions { get; set; }

		[NullAllowed, Export ("groups", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSArray<IMTLFunction>> Groups { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[NullAllowed, Export ("privateFunctions", ArgumentSemantic.Copy)]
		IMTLFunction [] PrivateFunctions { get; set; }

		[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0), NoWatch]
		[Export ("instanceDescriptorType", ArgumentSemantic.Assign)]
		MTLAccelerationStructureInstanceDescriptorType InstanceDescriptorType { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[NullAllowed, Export ("motionTransformBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer MotionTransformBuffer { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("motionTransformBufferOffset")]
		nuint MotionTransformBufferOffset { get; set; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("motionTransformCount")]
		nuint MotionTransformCount { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (MTLAccelerationStructureDescriptor))]
	interface MTLPrimitiveAccelerationStructureDescriptor {
		[NullAllowed, Export ("geometryDescriptors", ArgumentSemantic.Retain)]
		MTLAccelerationStructureGeometryDescriptor [] GeometryDescriptors { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLPrimitiveAccelerationStructureDescriptor Create ();

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("motionStartBorderMode", ArgumentSemantic.Assign)]
		MTLMotionBorderMode MotionStartBorderMode { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("motionEndBorderMode", ArgumentSemantic.Assign)]
		MTLMotionBorderMode MotionEndBorderMode { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("motionStartTime")]
		float MotionStartTime { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("motionEndTime")]
		float MotionEndTime { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("motionKeyframeCount")]
		nuint MotionKeyframeCount { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPassSampleBufferAttachmentDescriptor : NSCopying {
		[NullAllowed, Export ("sampleBuffer", ArgumentSemantic.Retain)]
		IMTLCounterSampleBuffer SampleBuffer { get; set; }

		[Export ("startOfVertexSampleIndex")]
		nuint StartOfVertexSampleIndex { get; set; }

		[Export ("endOfVertexSampleIndex")]
		nuint EndOfVertexSampleIndex { get; set; }

		[Export ("startOfFragmentSampleIndex")]
		nuint StartOfFragmentSampleIndex { get; set; }

		[Export ("endOfFragmentSampleIndex")]
		nuint EndOfFragmentSampleIndex { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPassSampleBufferAttachmentDescriptorArray {
		[Export ("objectAtIndexedSubscript:")]
		MTLRenderPassSampleBufferAttachmentDescriptor GetObject (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLRenderPassSampleBufferAttachmentDescriptor attachment, nuint attachmentIndex);

	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MTLResourceStatePassDescriptor : NSCopying {
		[Static]
		[Export ("resourceStatePassDescriptor")]
		MTLResourceStatePassDescriptor Create ();

		[Export ("sampleBufferAttachments")]
		MTLResourceStatePassSampleBufferAttachmentDescriptorArray SampleBufferAttachments { get; }
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLResourceStatePassSampleBufferAttachmentDescriptor : NSCopying {
		[NullAllowed, Export ("sampleBuffer", ArgumentSemantic.Retain)]
		IMTLCounterSampleBuffer SampleBuffer { get; set; }

		[Export ("startOfEncoderSampleIndex")]
		nuint StartOfEncoderSampleIndex { get; set; }

		[Export ("endOfEncoderSampleIndex")]
		nuint EndOfEncoderSampleIndex { get; set; }
	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLResourceStatePassSampleBufferAttachmentDescriptorArray {
		[Export ("objectAtIndexedSubscript:")]
		MTLResourceStatePassSampleBufferAttachmentDescriptor GetObject (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLResourceStatePassSampleBufferAttachmentDescriptor attachment, nuint attachmentIndex);

	}

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MTLVisibleFunctionTableDescriptor : NSCopying {
		[Static]
		[Export ("visibleFunctionTableDescriptor")]
		MTLVisibleFunctionTableDescriptor Create ();

		[Export ("functionCount")]
		nuint FunctionCount { get; set; }
	}

	interface IMTLFunctionHandle { }

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLFunctionHandle {
		[Abstract]
		[Export ("functionType")]
		MTLFunctionType FunctionType { get; }

		[Abstract]
		[Export ("name")]
		string Name { get; }

		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }
	}

	interface IMTLAccelerationStructureCommandEncoder { }

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLAccelerationStructureCommandEncoder : MTLCommandEncoder {
		[Abstract]
		[Export ("buildAccelerationStructure:descriptor:scratchBuffer:scratchBufferOffset:")]
		void BuildAccelerationStructure (IMTLAccelerationStructure accelerationStructure, MTLAccelerationStructureDescriptor descriptor, IMTLBuffer scratchBuffer, nuint scratchBufferOffset);

		[Abstract]
		[Export ("refitAccelerationStructure:descriptor:destination:scratchBuffer:scratchBufferOffset:")]
		void RefitAccelerationStructure (IMTLAccelerationStructure sourceAccelerationStructure, MTLAccelerationStructureDescriptor descriptor, [NullAllowed] IMTLAccelerationStructure destinationAccelerationStructure, IMTLBuffer scratchBuffer, nuint scratchBufferOffset);

		[Abstract]
		[Export ("copyAccelerationStructure:toAccelerationStructure:")]
		void CopyAccelerationStructure (IMTLAccelerationStructure sourceAccelerationStructure, IMTLAccelerationStructure destinationAccelerationStructure);

		[Abstract]
		[Export ("writeCompactedAccelerationStructureSize:toBuffer:offset:")]
		void WriteCompactedAccelerationStructureSize (IMTLAccelerationStructure accelerationStructure, IMTLBuffer buffer, nuint offset);

		[Abstract]
		[Export ("copyAndCompactAccelerationStructure:toAccelerationStructure:")]
		void CopyAndCompactAccelerationStructure (IMTLAccelerationStructure sourceAccelerationStructure, IMTLAccelerationStructure destinationAccelerationStructure);

		[Abstract]
		[Export ("updateFence:")]
		void UpdateFence (IMTLFence fence);

		[Abstract]
		[Export ("waitForFence:")]
		void WaitForFence (IMTLFence fence);

		[Abstract]
		[Export ("useResource:usage:")]
		void UseResource (IMTLResource resource, MTLResourceUsage usage);

		[Abstract]
		[Export ("useResources:count:usage:")]
		void UseResources (IMTLResource [] resources, nuint count, MTLResourceUsage usage);

		[Abstract]
		[Export ("useHeap:")]
		void UseHeap (IMTLHeap heap);

		[Abstract]
		[Export ("useHeaps:count:")]
		void UseHeaps (IMTLHeap [] heaps, nuint count);

		[Abstract]
		[Export ("sampleCountersInBuffer:atSampleIndex:withBarrier:")]
#if NET
		void SampleCountersInBuffer (IMTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#else
		void SampleCountersInBuffer (MTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#endif

#if NET
		[Abstract]
#endif
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("writeCompactedAccelerationStructureSize:toBuffer:offset:sizeDataType:")]
		void WriteCompactedAccelerationStructureSize (IMTLAccelerationStructure accelerationStructure, IMTLBuffer buffer, nuint offset, MTLDataType sizeDataType);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("refitAccelerationStructure:descriptor:destination:scratchBuffer:scratchBufferOffset:options:")]
		void RefitAccelerationStructure (IMTLAccelerationStructure sourceAccelerationStructure, MTLAccelerationStructureDescriptor descriptor, [NullAllowed] IMTLAccelerationStructure destinationAccelerationStructure, [NullAllowed] IMTLBuffer scratchBuffer, nuint scratchBufferOffset, MTLAccelerationStructureRefitOptions options);

	}

	interface IMTLVisibleFunctionTable { }

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLVisibleFunctionTable : MTLResource {
		[Abstract]
		[Export ("setFunction:atIndex:")]
		void SetFunction ([NullAllowed] IMTLFunctionHandle function, nuint index);

		[Abstract]
		[Export ("setFunctions:withRange:")]
		void SetFunctions (IMTLFunctionHandle [] functions, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("gpuResourceID")]
		MTLResourceId GpuResourceId { get; }
	}

	interface IMTLIntersectionFunctionTable { }

	[Mac (11, 0), iOS (14, 0), TV (16, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLIntersectionFunctionTable : MTLResource {
		[Abstract]
		[Export ("setBuffer:offset:atIndex:")]
		void SetBuffer ([NullAllowed] IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract]
		[Export ("setBuffers:offsets:withRange:")]
		void SetBuffers (IntPtr /* IMTLBuffer[] */ buffers, /* nuint[]*/ IntPtr offsets, NSRange range);

		[Abstract]
		[Export ("setFunction:atIndex:")]
		void SetFunction ([NullAllowed] IMTLFunctionHandle function, nuint index);

		[Abstract]
		[Export ("setFunctions:withRange:")]
		void SetFunctions (IMTLFunctionHandle [] functions, NSRange range);

		[Abstract]
		[Export ("setOpaqueTriangleIntersectionFunctionWithSignature:atIndex:")]
		void SetOpaqueTriangleIntersectionFunction (MTLIntersectionFunctionSignature signature, nuint index);

		[Abstract]
		[Export ("setOpaqueTriangleIntersectionFunctionWithSignature:withRange:")]
		void SetOpaqueTriangleIntersectionFunction (MTLIntersectionFunctionSignature signature, NSRange range);

		[Abstract]
		[Export ("setVisibleFunctionTable:atBufferIndex:")]
		void SetVisibleFunctionTable ([NullAllowed] IMTLVisibleFunctionTable functionTable, nuint bufferIndex);

		[Abstract]
		[Export ("setVisibleFunctionTables:withBufferRange:")]
		void SetVisibleFunctionTables (IMTLVisibleFunctionTable [] functionTables, NSRange bufferRange);

#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setOpaqueCurveIntersectionFunctionWithSignature:atIndex:")]
		void SetOpaqueCurveIntersectionFunction (MTLIntersectionFunctionSignature signature, nuint index);

#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("setOpaqueCurveIntersectionFunctionWithSignature:withRange:")]
		void SetOpaqueCurveIntersectionFunction (MTLIntersectionFunctionSignature signature, NSRange range);

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("gpuResourceID")]
		MTLResourceId GpuResourceId { get; }
	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLCommandBufferEncoderInfo {

		[Abstract]
		[Export ("label")]
		string Label { get; }

		[Abstract]
		[Export ("debugSignposts")]
		string [] DebugSignposts { get; }

		[Abstract]
		[Export ("errorState")]
		MTLCommandEncoderErrorState ErrorState { get; }
	}

	interface IMTLDynamicLibrary { }

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLDynamicLibrary {

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[Export ("installName")]
		string InstallName { get; }

		[Abstract]
		[Export ("serializeToURL:error:")]
		bool Serialize (NSUrl url, [NullAllowed] out NSError error);
	}

	interface IMTLLogContainer { }

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLLogContainer {

	}

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLFunctionLog {
		[Abstract]
		[Export ("type")]
		MTLFunctionLogType Type { get; }

		[Abstract]
		[NullAllowed, Export ("encoderLabel")]
		string EncoderLabel { get; }

		[Abstract]
		[NullAllowed, Export ("function")]
		IMTLFunction Function { get; }

		[Abstract]
		[NullAllowed, Export ("debugLocation")]
		IMTLFunctionLogDebugLocation DebugLocation { get; }
	}

	interface IMTLFunctionLogDebugLocation { }

	[Mac (11, 0), iOS (14, 0), TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface MTLFunctionLogDebugLocation {
		[Abstract]
		[NullAllowed, Export ("functionName")]
		string FunctionName { get; }

		[Abstract]
		[NullAllowed, Export ("URL")]
		NSUrl Url { get; }

		[Abstract]
		[Export ("line")]
		nuint Line { get; }

		[Abstract]
		[Export ("column")]
		nuint Column { get; }
	}

	[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
	[BaseType (typeof (NSObject))]
	interface MTLStitchedLibraryDescriptor : NSCopying {
		[Export ("functionGraphs", ArgumentSemantic.Copy)]
		MTLFunctionStitchingGraph [] FunctionGraphs { get; set; }

		[Export ("functions", ArgumentSemantic.Copy)]
		IMTLFunction [] Functions { get; set; }
	}

	[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPipelineFunctionsDescriptor : NSCopying {
		[NullAllowed, Export ("vertexAdditionalBinaryFunctions", ArgumentSemantic.Copy)]
		IMTLFunction [] VertexAdditionalBinaryFunctions { get; set; }

		[NullAllowed, Export ("fragmentAdditionalBinaryFunctions", ArgumentSemantic.Copy)]
		IMTLFunction [] FragmentAdditionalBinaryFunctions { get; set; }

		[NullAllowed, Export ("tileAdditionalBinaryFunctions", ArgumentSemantic.Copy)]
		IMTLFunction [] TileAdditionalBinaryFunctions { get; set; }
	}

	[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
	[BaseType (typeof (NSObject))]
	interface MTLMotionKeyframeData {
		[NullAllowed, Export ("buffer", ArgumentSemantic.Retain)]
		IMTLBuffer Buffer { get; set; }

		[Export ("offset")]
		nuint Offset { get; set; }

		[Static]
		[Export ("data")]
		MTLMotionKeyframeData Create ();
	}

	[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	interface MTLFunctionStitchingNode : NSCopying { }

	interface IMTLFunctionStitchingNode { }

	[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	interface MTLFunctionStitchingAttribute : NSCopying { }

	interface IMTLFunctionStitchingAttribute { }

	[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
	interface MTLFunctionStitchingAttributeAlwaysInline : MTLFunctionStitchingAttribute { }

	[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
	[BaseType (typeof (NSObject))]
	interface MTLFunctionStitchingInputNode : MTLFunctionStitchingNode {
		[Export ("argumentIndex")]
		nuint ArgumentIndex { get; set; }

		[Export ("initWithArgumentIndex:")]
		NativeHandle Constructor (nuint argument);
	}

	[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTLFunctionStitchingGraph : NSCopying {
		[Export ("functionName")]
		string FunctionName { get; set; }

		[Export ("nodes", ArgumentSemantic.Copy)]
		MTLFunctionStitchingFunctionNode [] Nodes { get; set; }

		[NullAllowed, Export ("outputNode", ArgumentSemantic.Copy)]
		MTLFunctionStitchingFunctionNode OutputNode { get; set; }

		[Export ("attributes", ArgumentSemantic.Copy)]
		IMTLFunctionStitchingAttribute [] Attributes { get; set; }

		[Export ("initWithFunctionName:nodes:outputNode:attributes:")]
		NativeHandle Constructor (string functionName, MTLFunctionStitchingFunctionNode [] nodes, [NullAllowed] MTLFunctionStitchingFunctionNode outputNode, IMTLFunctionStitchingAttribute [] attributes);
	}

	[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTLFunctionStitchingFunctionNode : MTLFunctionStitchingNode {
		[Export ("name")]
		string Name { get; set; }

		[Export ("arguments", ArgumentSemantic.Copy)]
		IMTLFunctionStitchingNode [] Arguments { get; set; }

		[Export ("controlDependencies", ArgumentSemantic.Copy)]
		MTLFunctionStitchingFunctionNode [] ControlDependencies { get; set; }

		[Export ("initWithName:arguments:controlDependencies:")]
		NativeHandle Constructor (string name, IMTLFunctionStitchingNode [] arguments, MTLFunctionStitchingFunctionNode [] controlDependencies);
	}

	[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
	[BaseType (typeof (MTLAccelerationStructureGeometryDescriptor))]
	interface MTLAccelerationStructureMotionTriangleGeometryDescriptor {
		[Export ("vertexBuffers", ArgumentSemantic.Copy)]
		MTLMotionKeyframeData [] VertexBuffers { get; set; }

		[Export ("vertexStride")]
		nuint VertexStride { get; set; }

		[NullAllowed, Export ("indexBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer IndexBuffer { get; set; }

		[Export ("indexBufferOffset")]
		nuint IndexBufferOffset { get; set; }

		[Export ("indexType", ArgumentSemantic.Assign)]
		MTLIndexType IndexType { get; set; }

		[Export ("triangleCount")]
		nuint TriangleCount { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLAccelerationStructureMotionTriangleGeometryDescriptor Create ();

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("vertexFormat", ArgumentSemantic.Assign)]
		MTLAttributeFormat VertexFormat { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("transformationMatrixBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer TransformationMatrixBuffer { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("transformationMatrixBufferOffset")]
		nuint TransformationMatrixBufferOffset { get; set; }
	}

	[Mac (12, 0), iOS (15, 0), TV (16, 0), MacCatalyst (15, 0), NoWatch]
	[BaseType (typeof (MTLAccelerationStructureGeometryDescriptor))]
	interface MTLAccelerationStructureMotionBoundingBoxGeometryDescriptor {
		[Export ("boundingBoxBuffers", ArgumentSemantic.Copy)]
		MTLMotionKeyframeData [] BoundingBoxBuffers { get; set; }

		[Export ("boundingBoxStride")]
		nuint BoundingBoxStride { get; set; }

		[Export ("boundingBoxCount")]
		nuint BoundingBoxCount { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLAccelerationStructureMotionBoundingBoxGeometryDescriptor Create ();
	}

	interface IMTLBinding { }

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLBinding {
		[Abstract]
		[Export ("name")]
		string Name { get; }

		[Abstract]
		[Export ("type")]
		MTLBindingType Type { get; }

		[Abstract]
		[Export ("access")]
		MTLBindingAccess Access { get; }

		[Abstract]
		[Export ("index")]
		nuint Index { get; }

		[Abstract]
		[Export ("used")]
		bool Used { [Bind ("isUsed")] get; }

		[Abstract]
		[Export ("argument")]
		bool Argument { [Bind ("isArgument")] get; }
	}

	interface IMTLBufferBinding { }

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLBufferBinding : MTLBinding {
		[Abstract]
		[Export ("bufferAlignment")]
		nuint BufferAlignment { get; }

		[Abstract]
		[Export ("bufferDataSize")]
		nuint BufferDataSize { get; }

		[Abstract]
		[Export ("bufferDataType")]
		MTLDataType BufferDataType { get; }

		[Abstract]
		[NullAllowed, Export ("bufferStructType")]
		MTLStructType BufferStructType { get; }

		[Abstract]
		[NullAllowed, Export ("bufferPointerType")]
		MTLPointerType BufferPointerType { get; }
	}

	interface IMTLIOFileHandle { }

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLIOFileHandle {
		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	interface IMTLIOCommandBuffer { }

	delegate void MTLIOCommandBufferHandler (IMTLIOCommandBuffer buffer);

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLIOCommandBuffer {
		[Abstract]
		[Export ("addCompletedHandler:")]
		void AddCompletedHandler (MTLIOCommandBufferHandler handler);

		[Abstract]
		[Export ("loadBytes:size:sourceHandle:sourceHandleOffset:")]
		void LoadBytes (IntPtr bytes, nuint size, IMTLIOFileHandle sourceHandle, nuint sourceHandleOffset);

		[Abstract]
		[Export ("loadBuffer:offset:size:sourceHandle:sourceHandleOffset:")]
		void LoadBuffer (IMTLBuffer buffer, nuint offset, nuint size, IMTLIOFileHandle sourceHandle, nuint sourceHandleOffset);

		[Abstract]
		[Export ("loadTexture:slice:level:size:sourceBytesPerRow:sourceBytesPerImage:destinationOrigin:sourceHandle:sourceHandleOffset:")]
		void LoadTexture (IMTLTexture texture, nuint slice, nuint level, MTLSize size, nuint sourceBytesPerRow, nuint sourceBytesPerImage, MTLOrigin destinationOrigin, IMTLIOFileHandle sourceHandle, nuint sourceHandleOffset);

		[Abstract]
		[Export ("copyStatusToBuffer:offset:")]
		void CopyStatus (IMTLBuffer buffer, nuint offset);

		[Abstract]
		[Export ("commit")]
		void Commit ();

		[Abstract]
		[Export ("waitUntilCompleted")]
		void WaitUntilCompleted ();

		[Abstract]
		[Export ("tryCancel")]
		void TryCancel ();

		[Abstract]
		[Export ("addBarrier")]
		void AddBarrier ();

		[Abstract]
		[Export ("pushDebugGroup:")]
		void PushDebugGroup (string group);

		[Abstract]
		[Export ("popDebugGroup")]
		void PopDebugGroup ();

		[Abstract]
		[Export ("enqueue")]
		void Enqueue ();

		[Abstract]
		[Export ("waitForEvent:value:")]
		void WaitForEvent (IMTLSharedEvent @event, ulong value);

		[Abstract]
		[Export ("signalEvent:value:")]
		void SignalEvent (IMTLSharedEvent @event, ulong value);

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Abstract]
		[Export ("status")]
		MTLIOStatus Status { get; }

		[Abstract]
		[NullAllowed, Export ("error")]
		NSError Error { get; }
	}

	interface IMTLIOCommandQueue { }

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLIOCommandQueue {
		[Abstract]
		[Export ("enqueueBarrier")]
		void EnqueueBarrier ();

		[Abstract]
		[Export ("commandBuffer")]
		IMTLIOCommandBuffer CommandBuffer { get; }

		[Abstract]
		[Export ("commandBufferWithUnretainedReferences")]
		IMTLIOCommandBuffer CommandBufferWithUnretainedReferences { get; }

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	interface IMTLIOScratchBuffer { }

	[Mac (13, 0), iOS (16, 0)]
	[Protocol]
	interface MTLIOScratchBuffer {
		[Abstract]
		[Export ("buffer")]
		IMTLBuffer Buffer { get; }
	}

	interface IMTLIOScratchBufferAllocator { }

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLIOScratchBufferAllocator {
		[Abstract]
		[Export ("newScratchBufferWithMinimumSize:")]
		[return: NullAllowed, Release]
		IMTLIOScratchBuffer Create (nuint minimumSize);
	}

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLObjectPayloadBinding : MTLBinding {
		[Abstract]
		[Export ("objectPayloadAlignment")]
		nuint ObjectPayloadAlignment { get; }

		[Abstract]
		[Export ("objectPayloadDataSize")]
		nuint ObjectPayloadDataSize { get; }
	}

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLTextureBinding : MTLBinding {
		[Abstract]
		[Export ("textureType")]
		MTLTextureType TextureType { get; }

		[Abstract]
		[Export ("textureDataType")]
		MTLDataType TextureDataType { get; }

		[Abstract]
		[Export ("depthTexture")]
		bool DepthTexture { [Bind ("isDepthTexture")] get; }

		[Abstract]
		[Export ("arrayLength")]
		nuint ArrayLength { get; }
	}

	[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (MTLAccelerationStructureGeometryDescriptor))]
	interface MTLAccelerationStructureCurveGeometryDescriptor {
		[NullAllowed, Export ("controlPointBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer ControlPointBuffer { get; set; }

		[Export ("controlPointBufferOffset")]
		nuint ControlPointBufferOffset { get; set; }

		[Export ("controlPointCount")]
		nuint ControlPointCount { get; set; }

		[Export ("controlPointStride")]
		nuint ControlPointStride { get; set; }

		[Export ("controlPointFormat", ArgumentSemantic.Assign)]
		MTLAttributeFormat ControlPointFormat { get; set; }

		[NullAllowed, Export ("radiusBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer RadiusBuffer { get; set; }

		[Export ("radiusBufferOffset")]
		nuint RadiusBufferOffset { get; set; }

		[Export ("radiusFormat", ArgumentSemantic.Assign)]
		MTLAttributeFormat RadiusFormat { get; set; }

		[Export ("radiusStride")]
		nuint RadiusStride { get; set; }

		[NullAllowed, Export ("indexBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer IndexBuffer { get; set; }

		[Export ("indexBufferOffset")]
		nuint IndexBufferOffset { get; set; }

		[Export ("indexType", ArgumentSemantic.Assign)]
		MTLIndexType IndexType { get; set; }

		[Export ("segmentCount")]
		nuint SegmentCount { get; set; }

		[Export ("segmentControlPointCount")]
		nuint SegmentControlPointCount { get; set; }

		[Export ("curveType", ArgumentSemantic.Assign)]
		MTLCurveType CurveType { get; set; }

		[Export ("curveBasis", ArgumentSemantic.Assign)]
		MTLCurveBasis CurveBasis { get; set; }

		[Export ("curveEndCaps", ArgumentSemantic.Assign)]
		MTLCurveEndCaps CurveEndCaps { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLAccelerationStructureCurveGeometryDescriptor GetDescriptor ();
	}

	[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (MTLAccelerationStructureGeometryDescriptor))]
	interface MTLAccelerationStructureMotionCurveGeometryDescriptor {
		[Export ("controlPointBuffers", ArgumentSemantic.Copy)]
		MTLMotionKeyframeData [] ControlPointBuffers { get; set; }

		[Export ("controlPointCount")]
		nuint ControlPointCount { get; set; }

		[Export ("controlPointStride")]
		nuint ControlPointStride { get; set; }

		[Export ("controlPointFormat", ArgumentSemantic.Assign)]
		MTLAttributeFormat ControlPointFormat { get; set; }

		[Export ("radiusBuffers", ArgumentSemantic.Copy)]
		MTLMotionKeyframeData [] RadiusBuffers { get; set; }

		[Export ("radiusFormat", ArgumentSemantic.Assign)]
		MTLAttributeFormat RadiusFormat { get; set; }

		[Export ("radiusStride")]
		nuint RadiusStride { get; set; }

		[NullAllowed, Export ("indexBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer IndexBuffer { get; set; }

		[Export ("indexBufferOffset")]
		nuint IndexBufferOffset { get; set; }

		[Export ("indexType", ArgumentSemantic.Assign)]
		MTLIndexType IndexType { get; set; }

		[Export ("segmentCount")]
		nuint SegmentCount { get; set; }

		[Export ("segmentControlPointCount")]
		nuint SegmentControlPointCount { get; set; }

		[Export ("curveType", ArgumentSemantic.Assign)]
		MTLCurveType CurveType { get; set; }

		[Export ("curveBasis", ArgumentSemantic.Assign)]
		MTLCurveBasis CurveBasis { get; set; }

		[Export ("curveEndCaps", ArgumentSemantic.Assign)]
		MTLCurveEndCaps CurveEndCaps { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLAccelerationStructureMotionCurveGeometryDescriptor GetDescriptor ();
	}

	[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLArchitecture : NSCopying {
		[Export ("name")]
		string Name { get; }
	}

	[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (MTLAccelerationStructureDescriptor))]
	interface MTLIndirectInstanceAccelerationStructureDescriptor {
		[NullAllowed, Export ("instanceDescriptorBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer InstanceDescriptorBuffer { get; set; }

		[Export ("instanceDescriptorBufferOffset")]
		nuint InstanceDescriptorBufferOffset { get; set; }

		[Export ("instanceDescriptorStride")]
		nuint InstanceDescriptorStride { get; set; }

		[Export ("maxInstanceCount")]
		nuint MaxInstanceCount { get; set; }

		[NullAllowed, Export ("instanceCountBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer InstanceCountBuffer { get; set; }

		[Export ("instanceCountBufferOffset")]
		nuint InstanceCountBufferOffset { get; set; }

		[Export ("instanceDescriptorType", ArgumentSemantic.Assign)]
		MTLAccelerationStructureInstanceDescriptorType InstanceDescriptorType { get; set; }

		[NullAllowed, Export ("motionTransformBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer MotionTransformBuffer { get; set; }

		[Export ("motionTransformBufferOffset")]
		nuint MotionTransformBufferOffset { get; set; }

		[Export ("maxMotionTransformCount")]
		nuint MaxMotionTransformCount { get; set; }

		[NullAllowed, Export ("motionTransformCountBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer MotionTransformCountBuffer { get; set; }

		[Export ("motionTransformCountBufferOffset")]
		nuint MotionTransformCountBufferOffset { get; set; }

		[Static]
		[Export ("descriptor")]
		MTLIndirectInstanceAccelerationStructureDescriptor GetDescriptor ();
	}

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLMeshRenderPipelineDescriptor : NSCopying {
		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[NullAllowed, Export ("objectFunction", ArgumentSemantic.Strong)]
		IMTLFunction ObjectFunction { get; set; }

		[NullAllowed, Export ("meshFunction", ArgumentSemantic.Strong)]
		IMTLFunction MeshFunction { get; set; }

		[NullAllowed, Export ("fragmentFunction", ArgumentSemantic.Strong)]
		IMTLFunction FragmentFunction { get; set; }

		[Export ("maxTotalThreadsPerObjectThreadgroup")]
		nuint MaxTotalThreadsPerObjectThreadgroup { get; set; }

		[Export ("maxTotalThreadsPerMeshThreadgroup")]
		nuint MaxTotalThreadsPerMeshThreadgroup { get; set; }

		[Export ("objectThreadgroupSizeIsMultipleOfThreadExecutionWidth")]
		bool ObjectThreadgroupSizeIsMultipleOfThreadExecutionWidth { get; set; }

		[Export ("meshThreadgroupSizeIsMultipleOfThreadExecutionWidth")]
		bool MeshThreadgroupSizeIsMultipleOfThreadExecutionWidth { get; set; }

		[Export ("payloadMemoryLength")]
		nuint PayloadMemoryLength { get; set; }

		[Export ("maxTotalThreadgroupsPerMeshGrid")]
		nuint MaxTotalThreadgroupsPerMeshGrid { get; set; }

		[Export ("objectBuffers")]
		MTLPipelineBufferDescriptorArray ObjectBuffers { get; }

		[Export ("meshBuffers")]
		MTLPipelineBufferDescriptorArray MeshBuffers { get; }

		[Export ("fragmentBuffers")]
		MTLPipelineBufferDescriptorArray FragmentBuffers { get; }

		[Export ("rasterSampleCount")]
		nuint RasterSampleCount { get; set; }

		[Export ("alphaToCoverageEnabled")]
		bool AlphaToCoverageEnabled { [Bind ("isAlphaToCoverageEnabled")] get; set; }

		[Export ("alphaToOneEnabled")]
		bool AlphaToOneEnabled { [Bind ("isAlphaToOneEnabled")] get; set; }

		[Export ("rasterizationEnabled")]
		bool RasterizationEnabled { [Bind ("isRasterizationEnabled")] get; set; }

		[Export ("maxVertexAmplificationCount")]
		nuint MaxVertexAmplificationCount { get; set; }

		[Export ("colorAttachments")]
		MTLRenderPipelineColorAttachmentDescriptorArray ColorAttachments { get; }

		[Export ("depthAttachmentPixelFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat DepthAttachmentPixelFormat { get; set; }

		[Export ("stencilAttachmentPixelFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat StencilAttachmentPixelFormat { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("objectLinkedFunctions", ArgumentSemantic.Copy)]
		MTLLinkedFunctions ObjectLinkedFunctions { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("meshLinkedFunctions", ArgumentSemantic.Copy)]
		MTLLinkedFunctions MeshLinkedFunctions { get; set; }

		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("fragmentLinkedFunctions", ArgumentSemantic.Copy)]
		MTLLinkedFunctions FragmentLinkedFunctions { get; set; }

		[Export ("reset")]
		void Reset ();
	}

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLAccelerationStructurePassSampleBufferAttachmentDescriptor : NSCopying {
		[NullAllowed, Export ("sampleBuffer", ArgumentSemantic.Retain)]
		IMTLCounterSampleBuffer SampleBuffer { get; set; }

		[Export ("startOfEncoderSampleIndex")]
		nuint StartOfEncoderSampleIndex { get; set; }

		[Export ("endOfEncoderSampleIndex")]
		nuint EndOfEncoderSampleIndex { get; set; }
	}

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLAccelerationStructurePassSampleBufferAttachmentDescriptorArray {
		[Export ("objectAtIndexedSubscript:")]
		MTLAccelerationStructurePassSampleBufferAttachmentDescriptor GetObject (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLAccelerationStructurePassSampleBufferAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTLIOCommandQueueDescriptor : NSCopying {
		[Export ("maxCommandBufferCount")]
		nuint MaxCommandBufferCount { get; set; }

		[Export ("priority", ArgumentSemantic.Assign)]
		MTLIOPriority Priority { get; set; }

		[Export ("type", ArgumentSemantic.Assign)]
		MTLIOCommandQueueType Type { get; set; }

		[Export ("maxCommandsInFlight")]
		nuint MaxCommandsInFlight { get; set; }

		[NullAllowed, Export ("scratchBufferAllocator", ArgumentSemantic.Retain)]
		IMTLIOScratchBufferAllocator ScratchBufferAllocator { get; set; }
	}

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface MTLAccelerationStructurePassDescriptor : NSCopying {
		[Static]
		[Export ("accelerationStructurePassDescriptor")]
		MTLAccelerationStructurePassDescriptor AccelerationStructurePassDescriptor { get; }

		[Export ("sampleBufferAttachments")]
		MTLAccelerationStructurePassSampleBufferAttachmentDescriptorArray SampleBufferAttachments { get; }
	}

	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface MTLThreadgroupBinding : MTLBinding {
		[Abstract]
		[Export ("threadgroupMemoryAlignment")]
		nuint ThreadgroupMemoryAlignment { get; }

		[Abstract]
		[Export ("threadgroupMemoryDataSize")]
		nuint ThreadgroupMemoryDataSize { get; }
	}


}
