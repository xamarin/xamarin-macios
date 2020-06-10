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

namespace Metal {

	delegate void MTLDeallocator (IntPtr pointer, nuint length);

	delegate void MTLNewComputePipelineStateWithReflectionCompletionHandler (IMTLComputePipelineState computePipelineState, MTLComputePipelineReflection reflection, NSError error);

	delegate void MTLDrawablePresentedHandler (IMTLDrawable drawable);

	delegate void MTLNewRenderPipelineStateWithReflectionCompletionHandler (IMTLRenderPipelineState renderPipelineState, MTLRenderPipelineReflection reflection, NSError error);

	interface IMTLCommandEncoder {}
	
	[iOS (8,0)][Mac (10,11)]
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

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("isDepthTexture")]
		bool IsDepthTexture { get; }
		
		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,13)]
		[Export ("arrayLength")]
		nuint ArrayLength { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
		[NullAllowed, Export ("bufferPointerType")]
		MTLPointerType BufferPointerType { get; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (MTLType))]
	interface MTLArrayType {
		[Export ("arrayLength")]
		nuint Length { get; }

		[Export ("elementType")]
		MTLDataType ElementType { get; }

		[Export ("stride")]
		nuint Stride { get; }

		[Export ("elementStructType")]
		MTLStructType ElementStructType ();

		[Export ("elementArrayType")]
		MTLArrayType ElementArrayType ();

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("argumentIndexStride")]
		nuint ArgumentIndexStride { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
		[NullAllowed, Export ("elementTextureReferenceType")]
		MTLTextureReferenceType ElementTextureReferenceType { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
		[NullAllowed, Export ("elementPointerType")]
		MTLPointerType ElementPointerType { get; }
	}

	[iOS (8,0)][Mac (10,11)]
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

	interface IMTLBuffer {}
	
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLBuffer : MTLResource {
		[Abstract, Export ("length")]
		nuint Length { get; }

		[Abstract, Export ("contents")]
		IntPtr Contents { get; }
#if MONOMAC
		[Abstract, Export ("didModifyRange:")]
		void DidModify (NSRange range);
#endif
		[Mac (10,13)]
		[return: NullAllowed]
#if XAMCORE_4_0 || !MONOMAC
		[Abstract]
#endif
		[Export ("newTextureWithDescriptor:offset:bytesPerRow:")]
		[return: Release]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor, nuint offset, nuint bytesPerRow);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("addDebugMarker:range:")]
		void AddDebugMarker (string marker, NSRange range);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("removeAllDebugMarkers")]
		void RemoveAllDebugMarkers ();

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[NullAllowed, Export ("remoteStorageBuffer")]
		IMTLBuffer RemoteStorageBuffer { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10,15)]
		[Export ("newRemoteBufferViewForDevice:")]
		[return: NullAllowed]
		IMTLBuffer CreateRemoteBuffer (IMTLDevice device);
	}
	
	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface MTLBufferLayoutDescriptor : NSCopying
	{
		[Export ("stride")]
		nuint Stride { get; set; }

		[Export ("stepFunction", ArgumentSemantic.Assign)]
		MTLStepFunction StepFunction { get; set; }

		[Export ("stepRate")]
		nuint StepRate { get; set; }
	}

	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface MTLBufferLayoutDescriptorArray
	{
		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		MTLBufferLayoutDescriptor ObjectAtIndexedSubscript (nuint index);

		[Internal]
		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLBufferLayoutDescriptor bufferDesc, nuint index);
	}
	

	interface IMTLCommandBuffer {}

	[iOS (8,0)][Mac (10,11)]
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

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("computeCommandEncoderWithDispatchType:")]
		[return: NullAllowed]
		IMTLComputeCommandEncoder ComputeCommandEncoderDispatch (MTLDispatchType dispatchType);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("encodeWaitForEvent:value:")]
		void EncodeWait (IMTLEvent @event, ulong value);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
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

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (10,3)][TV (10,2)][Mac (10,15,4)]
		[Export ("presentDrawable:afterMinimumDuration:")]
		void PresentDrawableAfter (IMTLDrawable drawable, double duration);

		[Abstract]
		[Export ("renderCommandEncoderWithDescriptor:")]
		IMTLRenderCommandEncoder CreateRenderCommandEncoder (MTLRenderPassDescriptor renderPassDescriptor);

#if !XAMCORE_4_0 || !MONOMAC // These were incorrectly released as available for mac, but are actually iOS/tvOS only.  Have to wait for XAMCORE_4_0 to remove the mac availability since it's a breaking change.
#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[iOS (10,3)][TV (10,2)][Mac (10,12,4)]
		[Export ("kernelStartTime")]
		double /* CFTimeInterval */ KernelStartTime { get; }

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[iOS (10,3)][TV (10,2)][Mac (10,12,4)]
		[Export ("kernelEndTime")]
		double /* CFTimeInterval */ KernelEndTime { get; }

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[iOS (10,3)][TV (10,2)][Mac (10,12,4)]
		[Export ("GPUStartTime")]
		double /* CFTimeInterval */ GpuStartTime { get; }

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[iOS (10,3)][TV (10,2)][Mac (10,12,4)]
		[Export ("GPUEndTime")]
		double /* CFTimeInterval */ GpuEndTime { get; }
#endif // !XAMCORE_4_0 || !MONOMAC

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Export ("pushDebugGroup:")]
		void PushDebugGroup (string @string);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Export ("popDebugGroup")]
		void PopDebugGroup ();

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[NoMac, iOS (13, 0), NoTV]
		[NullAllowed, Export ("resourceStateCommandEncoder")]
		IMTLResourceStateCommandEncoder ResourceStateCommandEncoder { get; }
	}

	interface IMTLCommandQueue {}

	[iOS (8,0)][Mac (10,11)]
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

		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'MTLCaptureScope' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'MTLCaptureScope' instead.")]
		[Abstract, Export ("insertDebugCaptureBoundary")]
		void InsertDebugCaptureBoundary ();
	}

	interface IMTLComputeCommandEncoder {}
	
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLComputeCommandEncoder : MTLCommandEncoder {
		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
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

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)]
		[Export ("dispatchThreadgroupsWithIndirectBuffer:indirectBufferOffset:threadsPerThreadgroup:")]
		void DispatchThreadgroups (IMTLBuffer indirectBuffer, nuint indirectBufferOffset, MTLSize threadsPerThreadgroup);

		[Abstract]
		[Export ("setBuffers:offsets:withRange:")]
		void SetBuffers (IMTLBuffer [] buffers, IntPtr offsets, NSRange range);

		[Abstract]
		[Export ("setSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetSamplerStates (IMTLSamplerState [] samplers, IntPtr floatArrayPtrLodMinClamps, IntPtr floatArrayPtrLodMaxClamps, NSRange range);

		[Abstract]
		[Export ("setSamplerStates:withRange:")]
		void SetSamplerStates (IMTLSamplerState [] samplers, NSRange range);
		
		[Abstract]
		[Export ("setTextures:withRange:")]
		void SetTextures (IMTLTexture [] textures, NSRange range);

		[iOS (8,3)]
		[Abstract]
		[Export ("setBufferOffset:atIndex:")]
		void SetBufferOffset (nuint offset, nuint index);

		[iOS (8,3)]
		[Abstract]
		[Export ("setBytes:length:atIndex:")]
		void SetBytes (IntPtr bytes, nuint length, nuint index);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setStageInRegion:")]
		void SetStage (MTLRegion region);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setStageInRegionWithIndirectBuffer:indirectBufferOffset:")]
		void SetStageInRegion (IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("updateFence:")]
		void Update (IMTLFence fence);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("waitForFence:")]
		void Wait (IMTLFence fence);

		[Mac (10,13)]
		[iOS (11,0), NoTV]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("dispatchThreads:threadsPerThreadgroup:")]
		void DispatchThreads (MTLSize threadsPerGrid, MTLSize threadsPerThreadgroup);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("useResource:usage:")]
		void UseResource (IMTLResource resource, MTLResourceUsage usage);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("useResources:count:usage:")]
		void UseResources (IMTLResource[] resources, nuint count, MTLResourceUsage usage);
		
		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("useHeap:")]
		void UseHeap (IMTLHeap heap);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("useHeaps:count:")]
		void UseHeaps (IMTLHeap[] heaps, nuint count);

		[iOS (11,0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setImageblockWidth:height:")]
		void SetImageblock (nuint width, nuint height);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("memoryBarrierWithScope:")]
		void MemoryBarrier (MTLBarrierScope scope);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("memoryBarrierWithResources:count:")]
		void MemoryBarrier (IMTLResource[] resources, nuint count);

		[TV (13,0), NoMac, iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("executeCommandsInBuffer:withRange:")]
		void ExecuteCommands (IMTLIndirectCommandBuffer indirectCommandBuffer, NSRange executionRange);

		[TV (13,0), NoMac, iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("executeCommandsInBuffer:indirectBuffer:indirectBufferOffset:")]
		void ExecuteCommands (IMTLIndirectCommandBuffer indirectCommandbuffer, IMTLBuffer indirectRangeBuffer, nuint indirectBufferOffset);

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10,15)]
		[Export ("sampleCountersInBuffer:atSampleIndex:withBarrier:")]
#if XAMCORE_4_0
		void SampleCounters (IMTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#else
		[Obsolete ("Use the overload that takes an IMTLCounterSampleBuffer instead.")]
		void SampleCounters (MTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
#endif
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLComputePipelineReflection {
		[Export ("arguments")]
#if XAMCORE_4_0
		MTLArgument [] Arguments { get; }
#else
		NSObject [] Arguments { get; }
#endif
	}

	interface IMTLComputePipelineState {}
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLComputePipelineState {
		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; }

		[Abstract, Export ("threadExecutionWidth")]
		nuint ThreadExecutionWidth { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[NullAllowed, Export ("label")]
		string Label { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("staticThreadgroupMemoryLength")]
		nuint StaticThreadgroupMemoryLength { get; }

		[iOS (11,0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("imageblockMemoryLengthForDimensions:")]
		nuint GetImageblockMemoryLength (MTLSize imageblockDimensions);

		[TV (13, 0), NoMac, iOS (13, 0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; }
	}

	interface IMTLBlitCommandEncoder {}
	
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLBlitCommandEncoder : MTLCommandEncoder {

#if MONOMAC
		[Abstract, Export ("synchronizeResource:")]
		void Synchronize (IMTLResource resource);

		[Abstract, Export ("synchronizeTexture:slice:level:")]
		void Synchronize (IMTLTexture texture, nuint slice, nuint level);
#endif

		[Abstract, Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel,  MTLOrigin sourceOrigin,  MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel,  MTLOrigin destinationOrigin);

		[Abstract, Export ("copyFromBuffer:sourceOffset:sourceBytesPerRow:sourceBytesPerImage:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, nuint sourceBytesPerRow, nuint sourceBytesPerImage,  MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel,  MTLOrigin destinationOrigin);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("copyFromBuffer:sourceOffset:sourceBytesPerRow:sourceBytesPerImage:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:options:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, nuint sourceBytesPerRow, nuint sourceBytesPerImage, MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel, MTLOrigin destinationOrigin, MTLBlitOption options);

		[Abstract, Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toBuffer:destinationOffset:destinationBytesPerRow:destinationBytesPerImage:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, MTLOrigin sourceOrigin,  MTLSize sourceSize, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint destinatinBytesPerRow, nuint destinationBytesPerImage);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toBuffer:destinationOffset:destinationBytesPerRow:destinationBytesPerImage:options:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, MTLOrigin sourceOrigin,  MTLSize sourceSize, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint destinatinBytesPerRow, nuint destinationBytesPerImage, MTLBlitOption options);

		[Abstract, Export ("generateMipmapsForTexture:")]
		void GenerateMipmapsForTexture (IMTLTexture texture);

		[Abstract, Export ("fillBuffer:range:value:")]
		void FillBuffer (IMTLBuffer buffer, NSRange range, byte value);

		[Abstract, Export ("copyFromBuffer:sourceOffset:toBuffer:destinationOffset:size:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint size);
		
		[iOS (10,0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("updateFence:")]
		void Update (IMTLFence fence);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("waitForFence:")]
		void Wait (IMTLFence fence);

		[Mac (10,14), iOS (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("optimizeContentsForGPUAccess:")]
		void OptimizeContentsForGpuAccess (IMTLTexture texture);

		[Mac (10,14), iOS (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("optimizeContentsForGPUAccess:slice:level:")]
		void OptimizeContentsForGpuAccess (IMTLTexture texture, nuint slice, nuint level);

		[Mac (10,14), iOS (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("optimizeContentsForCPUAccess:")]
		void OptimizeContentsForCpuAccess (IMTLTexture texture);

		[Mac (10,14), iOS (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("optimizeContentsForCPUAccess:slice:level:")]
		void OptimizeContentsForCpuAccess (IMTLTexture texture, nuint slice, nuint level);

		[Mac (10,14), iOS (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("resetCommandsInBuffer:withRange:")]
		void ResetCommands (IMTLIndirectCommandBuffer buffer, NSRange range);

		[Mac (10,14), iOS (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("copyIndirectCommandBuffer:sourceRange:destination:destinationIndex:")]
		void Copy (IMTLIndirectCommandBuffer source, NSRange sourceRange, IMTLIndirectCommandBuffer destination, nuint destinationIndex);

		[Mac (10,14), iOS (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("optimizeIndirectCommandBuffer:withRange:")]
		void Optimize (IMTLIndirectCommandBuffer indirectCommandBuffer, NSRange range);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13,0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("getTextureAccessCounters:region:mipLevel:slice:resetCounters:countersBuffer:countersBufferOffset:")]
		void GetTextureAccessCounters (IMTLTexture texture, MTLRegion region, nuint mipLevel, nuint slice, bool resetCounters, IMTLBuffer countersBuffer, nuint countersBufferOffset);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13,0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("resetTextureAccessCounters:region:mipLevel:slice:")]
		void ResetTextureAccessCounters (IMTLTexture texture, MTLRegion region, nuint mipLevel, nuint slice);

		[TV (13,0), Mac (10,15), iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("copyFromTexture:sourceSlice:sourceLevel:toTexture:destinationSlice:destinationLevel:sliceCount:levelCount:")]
		void Copy (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel, nuint sliceCount, nuint levelCount);

		[TV (13,0), Mac (10,15), iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("copyFromTexture:toTexture:")]
		void Copy (IMTLTexture sourceTexture, IMTLTexture destinationTexture);

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10,15)]
		[Export ("sampleCountersInBuffer:atSampleIndex:withBarrier:")]
		void SampleCounters (MTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10,15)]
		[Export ("resolveCounters:inRange:destinationBuffer:destinationOffset:")]
		void ResolveCounters (MTLCounterSampleBuffer sampleBuffer, NSRange range, IMTLBuffer destinationBuffer, nuint destinationOffset);
	}
	
	interface IMTLFence {}

	[iOS (10,0), TV (10,0), NoWatch, Mac (10,13)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	interface MTLFence
	{
		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	interface IMTLDevice {}
	
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLDevice {

		[Abstract, Export ("name")]
		string Name { get; }

#if XAMCORE_4_0
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[iOS (9,0)]
		[Export ("maxThreadsPerThreadgroup")]
		MTLSize MaxThreadsPerThreadgroup { get; }

#if XAMCORE_4_0
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[NoiOS]
		[NoTV]
		[Export ("lowPower")]
		bool LowPower { [Bind ("isLowPower")] get; }

#if XAMCORE_4_0
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[NoiOS]
		[NoTV]
		[Export ("headless")]
		bool Headless { [Bind ("isHeadless")] get; }
		
		[NoiOS, NoTV, NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("recommendedMaxWorkingSetSize")]
		ulong RecommendedMaxWorkingSetSize { get; }

#if XAMCORE_4_0
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[NoiOS]
		[NoTV]
		[Export ("depth24Stencil8PixelFormatSupported")]
		bool Depth24Stencil8PixelFormatSupported { [Bind ("isDepth24Stencil8PixelFormatSupported")] get; }
		
		[iOS (10,0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("heapTextureSizeAndAlignWithDescriptor:")]
		MTLSizeAndAlign GetHeapTextureSizeAndAlign (MTLTextureDescriptor desc);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10, 13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("heapBufferSizeAndAlignWithLength:options:")]
		MTLSizeAndAlign GetHeapBufferSizeAndAlignWithLength (nuint length, MTLResourceOptions options);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10, 13)]
#if XAMCORE_4_0
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

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (11,0), TV (11,0), NoWatch, Mac (10,11)]
		[return: NullAllowed]
		[return: Release]
		[Export ("newTextureWithDescriptor:iosurface:plane:")]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor, IOSurface.IOSurface iosurface, nuint plane);

		[iOS (13,0), TV (13,0), Mac (10,14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newSharedTextureWithDescriptor:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateSharedTexture (MTLTextureDescriptor descriptor);

		[iOS (13,0), TV (13,0), Mac (10,14)]
#if XAMCORE_4_0
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

#if !XAMCORE_4_0
		[Abstract, Export ("newLibraryWithData:error:")]
		[return: Release]
		[Obsolete ("Use the overload that take a 'DispatchData' instead.")]
		IMTLLibrary CreateLibrary (NSObject data, out NSError error);
#endif

#if XAMCORE_4_0
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
		
		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("newDefaultLibraryWithBundle:error:")]
		[return: Release]
		[return: NullAllowed]
#if XAMCORE_4_0
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

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("newComputePipelineStateWithDescriptor:options:reflection:error:")]
		[return: Release]
		IMTLComputePipelineState CreateComputePipelineState (MTLComputePipelineDescriptor descriptor, MTLPipelineOption options, out MTLComputePipelineReflection reflection, out NSError error);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("newComputePipelineStateWithDescriptor:options:completionHandler:")]
		void CreateComputePipelineState (MTLComputePipelineDescriptor descriptor, MTLPipelineOption options, MTLNewComputePipelineStateWithReflectionCompletionHandler completionHandler);
		
		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newFence")]
		[return: Release]
		IMTLFence CreateFence ();

		[Abstract, Export ("supportsFeatureSet:")]
		bool SupportsFeatureSet (MTLFeatureSet featureSet);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("supportsTextureSampleCount:")]
		bool SupportsTextureSampleCount (nuint sampleCount);

		[Mac (10, 13), NoiOS, NoWatch, NoTV]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("removable")]
		bool Removable { [Bind ("isRemovable")] get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("readWriteTextureSupport")]
		MTLReadWriteTextureTier ReadWriteTextureSupport { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("argumentBuffersSupport")]
		MTLArgumentBuffersTier ArgumentBuffersSupport { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("rasterOrderGroupsSupported")]
		bool RasterOrderGroupsSupported { [Bind ("areRasterOrderGroupsSupported")] get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newLibraryWithURL:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLLibrary CreateLibrary (NSUrl url, [NullAllowed] out NSError error);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("minimumLinearTextureAlignmentForPixelFormat:")]
		nuint GetMinimumLinearTextureAlignment (MTLPixelFormat format);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("minimumTextureBufferAlignmentForPixelFormat:")]
		nuint GetMinimumTextureBufferAlignment (MTLPixelFormat format);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("maxThreadgroupMemoryLength")]
		nuint MaxThreadgroupMemoryLength { get; }

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("maxArgumentBufferSamplerCount")]
		nuint MaxArgumentBufferSamplerCount { get; }

[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("programmableSamplePositionsSupported")]
		bool ProgrammableSamplePositionsSupported { [Bind ("areProgrammableSamplePositionsSupported")] get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("getDefaultSamplePositions:count:")]
		void GetDefaultSamplePositions (IntPtr positions, nuint count);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newArgumentEncoderWithArguments:")]
		[return: NullAllowed]
		[return: Release]
		IMTLArgumentEncoder CreateArgumentEncoder (MTLArgumentDescriptor[] arguments);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newIndirectCommandBufferWithDescriptor:maxCommandCount:options:")]
		[return: NullAllowed]
		[return: Release]
		IMTLIndirectCommandBuffer CreateIndirectCommandBuffer (MTLIndirectCommandBufferDescriptor descriptor, nuint maxCount, MTLResourceOptions options);

		[Mac (10, 14), iOS (12, 0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[return: NullAllowed]
		[return: Release]
		[Export ("newEvent")]
		IMTLEvent CreateEvent ();

		[Mac (10, 14), iOS (12, 0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[return: NullAllowed]
		[return: Release]
		[Export ("newSharedEvent")]
		IMTLSharedEvent CreateSharedEvent ();

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newSharedEventWithHandle:")]
		[return: NullAllowed]
		[return: Release]
		IMTLSharedEvent CreateSharedEvent (MTLSharedEventHandle sharedEventHandle);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("maxBufferLength")]
		nuint MaxBufferLength { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("registryID")]
		ulong RegistryId { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("currentAllocatedSize")]
		nuint CurrentAllocatedSize { get; }

#if false // https://bugzilla.xamarin.com/show_bug.cgi?id=59342
		[Mac (10,13), NoiOS, NoTV, NoWatch]
		[Notification]
		[Field ("MTLDeviceWasAddedNotification")]
		NSString DeviceWasAdded { get; }

		[Mac (10,13), NoiOS, NoTV, NoWatch]
		[Notification]
		[Field ("MTLDeviceRemovalRequestedNotification")]
		NSString DeviceRemovalRequested { get; }

		[Mac (10,13), NoiOS, NoTV, NoWatch]
		[Notification]
		[Field ("MTLDeviceWasRemovedNotification")]
		NSString DeviceWasRemoved { get; }
#endif

		[iOS (11,0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newRenderPipelineStateWithTileDescriptor:options:reflection:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLRenderPipelineState CreateRenderPipelineState (MTLTileRenderPipelineDescriptor descriptor, MTLPipelineOption options, [NullAllowed] out MTLRenderPipelineReflection reflection, [NullAllowed] out NSError error);

		[iOS (11,0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newRenderPipelineStateWithTileDescriptor:options:completionHandler:")]
		void CreateRenderPipelineState (MTLTileRenderPipelineDescriptor descriptor, MTLPipelineOption options, MTLNewRenderPipelineStateWithReflectionCompletionHandler completionHandler);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Mac (10,15,4), NoTV, iOS (13,0)]
		[Export ("supportsVertexAmplificationCount:")]
		bool SupportsVertexAmplification (nuint count);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Mac (10,15,4), NoTV, iOS (13,0)]
		[Export ("supportsRasterizationRateMapWithLayerCount:")]
		bool SupportsRasterizationRateMap (nuint layerCount);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13,0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("sparseTileSizeWithTextureType:pixelFormat:sampleCount:")]
		MTLSize GetSparseTileSize (MTLTextureType textureType, MTLPixelFormat pixelFormat, nuint sampleCount);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13, 0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("sparseTileSizeInBytes")]
		nuint SparseTileSizeInBytes { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Mac (10,15,4), NoTV, iOS (13,0)]
		[Export ("newRasterizationRateMapWithDescriptor:")]
		[return: NullAllowed]
		IMTLRasterizationRateMap CreateRasterizationRateMap (MTLRasterizationRateMapDescriptor descriptor);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13,0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("convertSparseTileRegions:toPixelRegions:withTileSize:numRegions:")]
		void ConvertSparseTileRegions (IntPtr tileRegions, IntPtr pixelRegions, MTLSize tileSize, nuint numRegions);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13,0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("convertSparsePixelRegions:toTileRegions:withTileSize:alignmentMode:numRegions:")]
		void ConvertSparsePixelRegions (IntPtr pixelRegions, IntPtr tileRegions, MTLSize tileSize, MTLSparseTextureRegionAlignmentMode mode, nuint numRegions);

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("hasUnifiedMemory")]
		bool HasUnifiedMemory { get; }

		[TV (13,0), Mac (10,15), iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("supportsFamily:")]
		bool SupportsFamily (MTLGpuFamily gpuFamily);

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[Export ("barycentricCoordsSupported")]
		bool BarycentricCoordsSupported { [Bind ("areBarycentricCoordsSupported")] get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[Export ("supportsShaderBarycentricCoordinates")]
		bool SupportsShaderBarycentricCoordinates { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[Export ("peerIndex")]
		uint PeerIndex { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[Export ("peerCount")]
		uint PeerCount { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[NullAllowed, Export ("counterSets")]
#if XAMCORE_4_0
		IMTLCounterSet[] CounterSets { get; }
#else
		[Obsolete ("Use 'GetIMTLCounterSets' instead.")]
		MTLCounterSet[] CounterSets { get; }
#endif

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10,15)]
		[Export ("newCounterSampleBufferWithDescriptor:error:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		IMTLCounterSampleBuffer CreateCounterSampleBuffer (MTLCounterSampleBufferDescriptor descriptor, [NullAllowed] out NSError error);
#else
		[Obsolete ("Use 'CreateIMTLCounterSampleBuffer' instead.")]
		MTLCounterSampleBuffer CreateCounterSampleBuffer (MTLCounterSampleBufferDescriptor descriptor, [NullAllowed] out NSError error);
#endif

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10,15)]
		[Export ("sampleTimestamps:gpuTimestamp:")]
		void GetSampleTimestamps (nuint cpuTimestamp, nuint gpuTimestamp);

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[Export ("peerGroupID")]
		ulong PeerGroupId { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[Export ("maxTransferRate")]
		ulong MaxTransferRate { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[Export ("location")]
		MTLDeviceLocation Location { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[Export ("locationNumber")]
		nuint LocationNumber { get; }
	}

	interface IMTLDrawable {}
	[iOS (8,0)][Mac (10,11)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface MTLDrawable {
		[Abstract, Export ("present")]
		void Present ();
		
		[Abstract, Export ("presentAtTime:")]
		void Present (double presentationTime);

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (10,3)][TV (10,2)][Mac (10,15,4)]
		[Export ("presentAfterMinimumDuration:")]
		void PresentAfter (double duration);

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (10,3)][TV (10,2)][Mac (10,15,4)]
		[Export ("addPresentedHandler:")]
		void AddPresentedHandler (Action<IMTLDrawable> block);

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (10,3)][TV (10,2)][Mac (10,15,4)]
		[Export ("presentedTime")]
		double /* CFTimeInterval */ PresentedTime { get; }

#if XAMCORE_4_0
		[Abstract] // @required but we can't add abstract members in C# and keep binary compatibility
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (10,3)][TV (10,2)][Mac (10,15,4)]
		[Export ("drawableID")]
		nuint DrawableID { get; }
	}

	interface IMTLTexture {}

	// Apple added several new *required* members in iOS 9,
	// but that breaks our binary compat, so we can't do that in our existing code.
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLTexture : MTLResource {
		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_10_0)]
		[Abstract, Export ("rootResource")]
		IMTLResource RootResource { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed] // by default this property is null
		[Export ("parentTexture")]
		IMTLTexture ParentTexture { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11)]
		[Export ("parentRelativeLevel")]
		nuint ParentRelativeLevel { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11)]
		[Export ("parentRelativeSlice")]
		nuint ParentRelativeSlice { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed] // by default this property is null
		[Export ("buffer")]
		IMTLBuffer Buffer { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11)]
		[Export ("bufferOffset")]
		nuint BufferOffset { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11)]
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

		[Abstract, Export ("sampleCount")]
		nuint SampleCount { get; }

		[Abstract, Export ("arrayLength")]
		nuint ArrayLength { get; }

		[Abstract, Export ("framebufferOnly")]
		bool FramebufferOnly { [Bind ("isFramebufferOnly")] get; }

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("allowGPUOptimizedContents")]
		bool AllowGpuOptimizedContents { get; }

		[Abstract, Export ("newTextureViewWithPixelFormat:")]
		[return: NullAllowed]
		[return: Release]
		IMTLTexture CreateTextureView (MTLPixelFormat pixelFormat);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("usage")]
		MTLTextureUsage Usage { get; }

#if XAMCORE_4_0
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
		void GetBytes (IntPtr pixelBytes, nuint bytesPerRow,  MTLRegion region, nuint level);

		[Abstract]
		[Export ("replaceRegion:mipmapLevel:slice:withBytes:bytesPerRow:bytesPerImage:")]
		void ReplaceRegion (MTLRegion region, nuint level, nuint slice, IntPtr pixelBytes, nuint bytesPerRow, nuint bytesPerImage);

		[Abstract]
		[Export ("replaceRegion:mipmapLevel:withBytes:bytesPerRow:")]
		void ReplaceRegion (MTLRegion region, nuint level, IntPtr pixelBytes, nuint bytesPerRow);

		[Mac (10, 11), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[NullAllowed, Export ("iosurface")]
		IOSurface.IOSurface IOSurface { get; }

		[Mac (10, 11), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("iosurfacePlane")]
		nuint IOSurfacePlane { get; }

		[iOS (13,0), TV (13,0), Mac (10, 14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("shareable")]
		bool Shareable { [Bind ("isShareable")] get; }

		[iOS (13,0), TV (13,0), Mac (10, 14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[return: NullAllowed]
		[return: Release]
		[Export ("newSharedTextureHandle")]
		MTLSharedTextureHandle CreateSharedTextureHandle ();

#if XAMCORE_4_0
		[Abstract]
#endif
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13, 0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("firstMipmapInTail")]
		nuint FirstMipmapInTail { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13, 0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("tailSizeInBytes")]
		nuint TailSizeInBytes { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13, 0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("isSparse")]
		bool IsSparse { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Mac (10, 15), TV (13,0), iOS (13, 0)]
		[Export ("swizzle")]
		MTLTextureSwizzleChannels Swizzle { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Mac (10,15), TV (13,0), iOS (13,0)]
		[Export ("newTextureViewWithPixelFormat:textureType:levels:slices:swizzle:")]
		[return: NullAllowed]
		IMTLTexture Create (MTLPixelFormat pixelFormat, MTLTextureType textureType, NSRange levelRange, NSRange sliceRange, MTLTextureSwizzleChannels swizzle);

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10, 15)]
		[NullAllowed, Export ("remoteStorageTexture")]
		IMTLTexture RemoteStorageTexture { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10,15)]
		[Export ("newRemoteTextureViewForDevice:")]
		[return: NullAllowed]
		IMTLTexture CreateRemoteTexture (IMTLDevice device);
	}
	

	[iOS (8,0)][Mac (10,11)]
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

		[Mac (10,14), iOS (12,0), TV (12,0)]
		[Static, Export ("textureBufferDescriptorWithPixelFormat:width:resourceOptions:usage:")]
		MTLTextureDescriptor CreateTextureBufferDescriptor (MTLPixelFormat pixelFormat, nuint width, MTLResourceOptions resourceOptions, MTLTextureUsage usage);

		[iOS (9,0)]
		[Export ("cpuCacheMode", ArgumentSemantic.Assign)]
		MTLCpuCacheMode CpuCacheMode { get; set; }

		[iOS (9,0)]
		[Export ("storageMode", ArgumentSemantic.Assign)]
		MTLStorageMode StorageMode { get; set; }

		[iOS (9,0)]
		[Export ("usage", ArgumentSemantic.Assign)]
		MTLTextureUsage Usage { get; set; }		

		[Mac (10,14), iOS (12,0), TV (12,0)]
		[Export ("allowGPUOptimizedContents")]
		bool AllowGpuOptimizedContents { get; set; }

		[Mac (10, 15), iOS (13, 0), TV (13,0)]
		[Export ("hazardTrackingMode", ArgumentSemantic.Assign)]
		MTLHazardTrackingMode HazardTrackingMode { get; set; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("swizzle", ArgumentSemantic.Assign)]
		MTLTextureSwizzleChannels Swizzle { get; set; }
	}

	[iOS (8,0)][Mac (10,11)]
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

#if !MONOMAC
		[iOS (9,0)]
		[Export ("lodAverage")]
		bool LodAverage { get; set; }
#endif

		[NoiOS, NoTV, NoWatch, Mac (10,12)]
		[Export ("borderColor", ArgumentSemantic.Assign)]
		MTLSamplerBorderColor BorderColor { get; set; }

		[iOS (9,0)]
		[Export ("compareFunction")]
		MTLCompareFunction CompareFunction { get; set; }

		// [NullAllowed] we can't allow setting null - even if the default value is null
		// /SourceCache/AcceleratorKit/AcceleratorKit-14.9/Framework/MTLSampler.m:240: failed assertion `label must not be nil.'
		[Export ("label")]
		string Label { get; set; }

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("supportArgumentBuffers")]
		bool SupportArgumentBuffers { get; set; }
	}

	interface IMTLSamplerState {}
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLSamplerState  {

		[Abstract, Export ("label")]
		string Label { get; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	partial interface MTLRenderPipelineDescriptor : NSCopying {

		// [NullAllowed] we can't allow setting null - even if the default value is null
		// /SourceCache/AcceleratorKit/AcceleratorKit-14.9/Framework/MTLRenderPipeline.mm:627: failed assertion `label must not be nil.'
		[Export ("label")]
		string Label { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("vertexFunction", ArgumentSemantic.Retain)]
		IMTLFunction VertexFunction { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fragmentFunction", ArgumentSemantic.Retain)]
		IMTLFunction FragmentFunction { get; set; }

		[Export ("vertexDescriptor", ArgumentSemantic.Copy)]
		MTLVertexDescriptor VertexDescriptor { get; set; }

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
		
		[iOS (12,0)]
		[NoTV, NoWatch, Mac (10,11)]
		[Export ("inputPrimitiveTopology", ArgumentSemantic.Assign)]
		MTLPrimitiveTopologyClass InputPrimitiveTopology { get; set; }
		
		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("tessellationPartitionMode", ArgumentSemantic.Assign)]
		MTLTessellationPartitionMode TessellationPartitionMode { get; set; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("maxTessellationFactor")]
		nuint MaxTessellationFactor { get; set; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("tessellationFactorScaleEnabled")]
		bool IsTessellationFactorScaleEnabled { [Bind ("isTessellationFactorScaleEnabled")] get; set; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("tessellationFactorFormat", ArgumentSemantic.Assign)]
		MTLTessellationFactorFormat TessellationFactorFormat { get; set; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("tessellationControlPointIndexType", ArgumentSemantic.Assign)]
		MTLTessellationControlPointIndexType TessellationControlPointIndexType { get; set; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("tessellationFactorStepFunction", ArgumentSemantic.Assign)]
		MTLTessellationFactorStepFunction TessellationFactorStepFunction { get; set; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("tessellationOutputWindingOrder", ArgumentSemantic.Assign)]
		MTLWinding TessellationOutputWindingOrder { get; set; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("vertexBuffers")]
		MTLPipelineBufferDescriptorArray VertexBuffers { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("fragmentBuffers")]
		MTLPipelineBufferDescriptorArray FragmentBuffers { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("rasterSampleCount")]
		nuint RasterSampleCount { get; set; }

		[Mac (10,14), iOS (12,0), TV (12,0)]
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; set; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Mac (10,15,4), NoTV, iOS (13, 0)]
		[Export ("maxVertexAmplificationCount")]
		nuint MaxVertexAmplificationCount { get; set; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPipelineColorAttachmentDescriptorArray {

		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLRenderPipelineColorAttachmentDescriptor ObjectAtIndexedSubscript (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject (MTLRenderPipelineColorAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	interface IMTLRenderPipelineState {}

	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLRenderPipelineState {

		[Abstract, Export ("label")]
		string Label { get; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; }

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("threadgroupSizeMatchesTileSize")]
		bool ThreadgroupSizeMatchesTileSize { get; }

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("imageblockSampleLength")]
		nuint ImageblockSampleLength { get; }

		[iOS (11,0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("imageblockMemoryLengthForDimensions:")]
		nuint GetImageblockMemoryLength (MTLSize imageblockDimensions);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; }

	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLVertexBufferLayoutDescriptor : NSCopying {
		[Export ("stride", ArgumentSemantic.UnsafeUnretained)]
		nuint Stride { get; set; }

		[Export ("stepFunction", ArgumentSemantic.Assign)]
		MTLVertexStepFunction StepFunction { get; set; }

		[Export ("stepRate", ArgumentSemantic.UnsafeUnretained)]
		nuint StepRate { get; set; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLVertexBufferLayoutDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLVertexBufferLayoutDescriptor ObjectAtIndexedSubscript (nuint index);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject (MTLVertexBufferLayoutDescriptor bufferDesc, nuint index);
	}

	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface MTLAttribute
	{
		[NullAllowed, Export ("name")]
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
	
	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface MTLAttributeDescriptor : NSCopying
	{
		[Export ("format", ArgumentSemantic.Assign)]
		MTLAttributeFormat Format { get; set; }

		[Export ("offset")]
		nuint Offset { get; set; }

		[Export ("bufferIndex")]
		nuint BufferIndex { get; set; }
	}

	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface MTLAttributeDescriptorArray
	{
		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		MTLAttributeDescriptor ObjectAtIndexedSubscript (nuint index);

		[Internal]
		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLAttributeDescriptor attributeDesc, nuint index);
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLVertexAttributeDescriptor : NSCopying {
		[Export ("format", ArgumentSemantic.Assign)]
		MTLVertexFormat Format { get; set; }

		[Export ("offset", ArgumentSemantic.Assign)]
		nuint Offset { get; set; }

		[Export ("bufferIndex", ArgumentSemantic.Assign)]
		nuint BufferIndex { get; set; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLVertexAttributeDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLVertexAttributeDescriptor ObjectAtIndexedSubscript (nuint index);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject ([NullAllowed] MTLVertexAttributeDescriptor attributeDesc, nuint index);
	}

	[iOS (8,0)][Mac (10,11)]
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

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	partial interface MTLVertexAttribute {
		[Export ("attributeIndex")]
		nuint AttributeIndex { get; }

		[iOS (8,3)]
		[Export ("attributeType")]
		MTLDataType AttributeType { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }

		[Export ("name")]
		string Name { get; }
		
		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("patchData")]
		bool PatchData { [Bind ("isPatchData")] get; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[Export ("patchControlPointData")]
		bool PatchControlPointData { [Bind ("isPatchControlPointData")] get; }
	}

	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MTLFunctionConstantValues : NSCopying
	{
		[iOS (11,0), TV (11,0), Mac (10,13)]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("setConstantValue:type:atIndex:")]
		void SetConstantValue (IntPtr value, MTLDataType type, nuint index);

		[Export ("setConstantValues:type:withRange:")]
		void SetConstantValues (IntPtr values, MTLDataType type, NSRange range);

		[Export ("setConstantValue:type:withName:")]
		void SetConstantValue (IntPtr value, MTLDataType type, string name);

		[Export ("reset")]
		void Reset ();
	}
	
	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface MTLFunctionConstant
	{
		[Export ("name")]
		string Name { get; }

		[Export ("type")]
		MTLDataType Type { get; }

		[Export ("index")]
		nuint Index { get; }

		[Export ("required")]
		bool IsRequired { get; }
	}

	interface IMTLFunction {}
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLFunction  {

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
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

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("patchType")]
		MTLPatchType PatchType { get; }

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("patchControlPointCount")]
		nint PatchControlPointCount { get; }

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[NullAllowed, Export ("stageInputAttributes")]
		MTLAttribute[] StageInputAttributes { get; }

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("functionConstantsDictionary")]
		NSDictionary<NSString, MTLFunctionConstant> FunctionConstants { get; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newArgumentEncoderWithBufferIndex:")]
		[return: Release]
		IMTLArgumentEncoder CreateArgumentEncoder (nuint bufferIndex);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newArgumentEncoderWithBufferIndex:reflection:")]
		[return: Release]
		IMTLArgumentEncoder CreateArgumentEncoder (nuint bufferIndex, [NullAllowed] out MTLArgument reflection);
	}

	interface IMTLLibrary {}

	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLLibrary  {

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("functionNames")]
		string [] FunctionNames { get; }

		[Abstract, Export ("newFunctionWithName:")]
		[return: Release]
		IMTLFunction CreateFunction (string functionName);
		
		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newFunctionWithName:constantValues:error:")]
		[return: NullAllowed]
		[return: Release]
		IMTLFunction CreateFunction (string name, MTLFunctionConstantValues constantValues, out NSError error);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newFunctionWithName:constantValues:completionHandler:")]
		[Async]
		void CreateFunction (string name, MTLFunctionConstantValues constantValues, Action<IMTLFunction, NSError> completionHandler);

		[Field ("MTLLibraryErrorDomain")]
		NSString ErrorDomain { get; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	partial interface MTLCompileOptions : NSCopying {

		[NullAllowed] // by default this property is null
		[Export ("preprocessorMacros", ArgumentSemantic.Copy)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> PreprocessorMacros { get; set; }
#else
		NSDictionary PreprocessorMacros { get; set; }
#endif

		[Export ("fastMathEnabled")]
		bool FastMathEnabled { get; set; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("languageVersion", ArgumentSemantic.Assign)]
		MTLLanguageVersion LanguageVersion { get; set; }
	}
	
	[iOS (8,0)][Mac (10,11)]
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

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLStructMember {
		[Export ("name")]
		string Name { get; }

		[Export ("offset")]
		nuint Offset { get; }

		[Export ("dataType")]
		MTLDataType DataType { get; }

#if XAMCORE_4_0
		[Export ("structType")]
		MTLStructType StructType { get; }

		[Export ("arrayType")]
		MTLArrayType ArrayType { get; }
#else
		[Export ("structType")]
		MTLStructType StructType ();

		[Export ("arrayType")]
		MTLArrayType ArrayType ();
#endif

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("argumentIndex")]
		nuint ArgumentIndex { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
		[NullAllowed, Export ("textureReferenceType")]
		MTLTextureReferenceType TextureReferenceType { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
		[NullAllowed, Export ("pointerType")]
		MTLPointerType PointerType { get; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (MTLType))]
	interface MTLStructType {
		[Export ("members")]
		MTLStructMember [] Members { get; }

		[Export ("memberByName:")]
		MTLStructMember Lookup (string name);
	}

	interface IMTLDepthStencilState {}

	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLDepthStencilState  {
		[Abstract]
		[Export ("label")]
		string Label { get; }

		[Abstract]
		[Export ("device")]
		IMTLDevice Device { get; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	partial interface MTLDepthStencilDescriptor : NSCopying {

		[Export ("depthCompareFunction")]
		MTLCompareFunction DepthCompareFunction { get; set; }

		[Export ("depthWriteEnabled")]
		bool DepthWriteEnabled { [Bind ("isDepthWriteEnabled")] get; set; }

		[Export ("frontFaceStencil", ArgumentSemantic.Copy)]
		MTLStencilDescriptor FrontFaceStencil { get; set; }

		[Export ("backFaceStencil", ArgumentSemantic.Copy)]
		MTLStencilDescriptor BackFaceStencil { get; set; }

		// [NullAllowed] we can't allow setting null - even if the default value is null
		// /SourceCache/AcceleratorKit/AcceleratorKit-14.9/Framework/MTLDepthStencil.m:393: failed assertion `label must not be nil.'
		[Export ("label")]
		string Label { get; set; }
	}

	interface IMTLParallelRenderCommandEncoder {}

	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	interface MTLParallelRenderCommandEncoder : MTLCommandEncoder {
		[Abstract]
		[Export ("renderCommandEncoder")]
		[Autorelease]
		[return: NullAllowed]
		IMTLRenderCommandEncoder CreateRenderCommandEncoder ();
		
		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setColorStoreAction:atIndex:")]
		void SetColorStoreAction (MTLStoreAction storeAction, nuint colorAttachmentIndex);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setDepthStoreAction:")]
		void SetDepthStoreAction (MTLStoreAction storeAction);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setStencilStoreAction:")]
		void SetStencilStoreAction (MTLStoreAction storeAction);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setColorStoreActionOptions:atIndex:")]
		void SetColorStoreActionOptions (MTLStoreActionOptions storeActionOptions, nuint colorAttachmentIndex);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setDepthStoreActionOptions:")]
		void SetDepthStoreActionOptions (MTLStoreActionOptions storeActionOptions);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setStencilStoreActionOptions:")]
		void SetStencilStoreActionOptions (MTLStoreActionOptions storeActionOptions);
	}

	interface IMTLRenderCommandEncoder {}

	[iOS (8,0)][Mac (10,11)]
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

		[iOS (9,0)] // Current headers claim this member was introduced in iOS 11, but it also shows up in Xcode 7's header (as introduced in iOS 9)
		[Mac (10,11), TV (11,0), NoWatch]
#if XAMCORE_4_0
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

		[iOS (8,3)]
		[Abstract, Export ("setFragmentBufferOffset:atIndex:")]
		void SetFragmentBufferOffset (nuint offset, nuint index);

		[iOS (8,3)]
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

		[iOS (9,0)][Mac (10,11)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("setStencilFrontReferenceValue:backReferenceValue:")]
		void SetStencilFrontReferenceValue (uint frontReferenceValue, uint backReferenceValue);

		[Abstract, Export ("setVisibilityResultMode:offset:")]
		void SetVisibilityResultMode (MTLVisibilityResultMode mode, nuint offset);
		
		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setColorStoreAction:atIndex:")]
		void SetColorStoreAction (MTLStoreAction storeAction, nuint colorAttachmentIndex);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setDepthStoreAction:")]
		void SetDepthStoreAction (MTLStoreAction storeAction);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
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

#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[iOS(9,0)]
		[Export ("drawPrimitives:vertexStart:vertexCount:instanceCount:baseInstance:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount, nuint instanceCount, nuint baseInstance);

#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[iOS(9,0)]
		[Export ("drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:instanceCount:baseVertex:baseInstance:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, nuint indexCount, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset, nuint instanceCount, nint baseVertex, nuint baseInstance);

#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[iOS(9,0)]
		[Export ("drawPrimitives:indirectBuffer:indirectBufferOffset:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[iOS(9,0)]
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

		[iOS (8,3)]
		[Abstract, Export ("setVertexBufferOffset:atIndex:")]
		void SetVertexBufferOffset (nuint offset, nuint index);

		[iOS (8,3)]
		[Abstract, Export ("setVertexBytes:length:atIndex:")]
		void SetVertexBytes (IntPtr bytes, nuint length, nuint index);

		[Abstract, Export ("setVertexSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetVertexSamplerStates (IMTLSamplerState [] samplers, IntPtr floatArrayPtrLodMinClamps, IntPtr floatArrayPtrLodMaxClamps, NSRange range);

		[Abstract, Export ("setVertexSamplerStates:withRange:")]
		void SetVertexSamplerStates (IMTLSamplerState [] samplers, NSRange range);

		[Abstract]
		[Export ("setVertexTextures:withRange:")]
		void SetVertexTextures (IMTLTexture [] textures, NSRange range);

		[NoiOS, NoTV, NoWatch, Mac (10,11)]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'MemoryBarrier (MTLBarrierScope, MTLRenderStages, MTLRenderStages)' instead.")]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("textureBarrier")]
		void TextureBarrier ();

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("updateFence:afterStages:")]
		void Update (IMTLFence fence, MTLRenderStages stages);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("waitForFence:beforeStages:")]
		void Wait (IMTLFence fence, MTLRenderStages stages);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTessellationFactorBuffer:offset:instanceStride:")]
		void SetTessellationFactorBuffer ([NullAllowed] IMTLBuffer buffer, nuint offset, nuint instanceStride);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTessellationFactorScale:")]
		void SetTessellationFactorScale (float scale);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("drawPatches:patchStart:patchCount:patchIndexBuffer:patchIndexBufferOffset:instanceCount:baseInstance:")]
		void DrawPatches (nuint numberOfPatchControlPoints, nuint patchStart, nuint patchCount, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, nuint instanceCount, nuint baseInstance);

		[iOS (12,0)]
		[NoTV, NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("drawPatches:patchIndexBuffer:patchIndexBufferOffset:indirectBuffer:indirectBufferOffset:")]
		void DrawPatches (nuint numberOfPatchControlPoints, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("drawIndexedPatches:patchStart:patchCount:patchIndexBuffer:patchIndexBufferOffset:controlPointIndexBuffer:controlPointIndexBufferOffset:instanceCount:baseInstance:")]
		void DrawIndexedPatches (nuint numberOfPatchControlPoints, nuint patchStart, nuint patchCount, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, IMTLBuffer controlPointIndexBuffer, nuint controlPointIndexBufferOffset, nuint instanceCount, nuint baseInstance);

		[iOS (12,0)]
		[NoTV, NoWatch, Mac (10,12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("drawIndexedPatches:patchIndexBuffer:patchIndexBufferOffset:controlPointIndexBuffer:controlPointIndexBufferOffset:indirectBuffer:indirectBufferOffset:")]
		void DrawIndexedPatches (nuint numberOfPatchControlPoints, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, IMTLBuffer controlPointIndexBuffer, nuint controlPointIndexBufferOffset, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[iOS (12,0)]
		[Mac (10,13), NoTV, NoWatch]
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "API removed, please do not use.")]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setViewports:count:")]
		void SetViewports (IntPtr viewports, nuint count);

		[iOS (12,0)]
		[Mac (10,13), NoTV, NoWatch]
		[Obsoleted (PlatformName.MacOSX, 10, 14, message: "API removed, please do not use.")]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setScissorRects:count:")]
		void SetScissorRects (IntPtr scissorRects, nuint count);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setColorStoreActionOptions:atIndex:")]
		void SetColorStoreActionOptions (MTLStoreActionOptions storeActionOptions, nuint colorAttachmentIndex);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setDepthStoreActionOptions:")]
		void SetDepthStoreActionOptions (MTLStoreActionOptions storeActionOptions);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setStencilStoreActionOptions:")]
		void SetStencilStoreActionOptions (MTLStoreActionOptions storeActionOptions);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("useResource:usage:")]
		void UseResource (IMTLResource resource, MTLResourceUsage usage);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("useResources:count:usage:")]
		void UseResources (IMTLResource[] resources, nuint count, MTLResourceUsage usage);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("useHeap:")]
		void UseHeap (IMTLHeap heap);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("useHeaps:count:")]
		void UseHeaps (IMTLHeap[] heaps, nuint count);

		[Mac (10,14), iOS (12,0), TV (12,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("executeCommandsInBuffer:withRange:")]
		void ExecuteCommands (IMTLIndirectCommandBuffer indirectCommandBuffer, NSRange executionRange);

		[iOS (13,0), TV (13,0), Mac (10,14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("executeCommandsInBuffer:indirectBuffer:indirectBufferOffset:")]
		void ExecuteCommands (IMTLIndirectCommandBuffer indirectCommandbuffer, IMTLBuffer indirectRangeBuffer, nuint indirectBufferOffset);

		[NoiOS, NoTV, Mac (10,14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("memoryBarrierWithScope:afterStages:beforeStages:")]
		void MemoryBarrier (MTLBarrierScope scope, MTLRenderStages after, MTLRenderStages before);

		[NoiOS, NoTV, Mac (10,14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("memoryBarrierWithResources:count:afterStages:beforeStages:")]
		void MemoryBarrier (IMTLResource[] resources, nuint count, MTLRenderStages after, MTLRenderStages before);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("tileWidth")]
		nuint TileWidth { get; }

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("tileHeight")]
		nuint TileHeight { get; }

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileBytes:length:atIndex:")]
		void SetTileBytes (IntPtr /* void* */ bytes, nuint length, nuint index);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileBuffer:offset:atIndex:")]
		void SetTileBuffer ([NullAllowed] IMTLBuffer buffer, nuint offset, nuint index);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileBufferOffset:atIndex:")]
		void SetTileBufferOffset (nuint offset, nuint index);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileBuffers:offsets:withRange:")]
		void SetTileBuffers (IMTLBuffer[] buffers, IntPtr offsets, NSRange range);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileTexture:atIndex:")]
		void SetTileTexture ([NullAllowed] IMTLTexture texture, nuint index);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileTextures:withRange:")]
		void SetTileTextures (IMTLTexture[] textures, NSRange range);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileSamplerState:atIndex:")]
		void SetTileSamplerState ([NullAllowed] IMTLSamplerState sampler, nuint index);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileSamplerStates:withRange:")]
		void SetTileSamplerStates (IMTLSamplerState[] samplers, NSRange range);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetTileSamplerState ([NullAllowed] IMTLSamplerState sampler, float lodMinClamp, float lodMaxClamp, nuint index);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setTileSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetTileSamplerStates (IMTLSamplerState[] samplers, IntPtr /* float[] */ lodMinClamps, IntPtr /* float[] */ lodMaxClamps, NSRange range);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("dispatchThreadsPerTile:")]
		void DispatchThreadsPerTile (MTLSize threadsPerTile);

		[iOS (11, 0), NoTV, NoMac, NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setThreadgroupMemoryLength:offset:atIndex:")]
		void SetThreadgroupMemoryLength (nuint length, nuint offset, nuint index);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Mac (10,15,4), NoTV, iOS (13,0)]
		[Export ("setVertexAmplificationCount:viewMappings:")]
		void SetVertexAmplificationCount (nuint count, MTLVertexAmplificationViewMapping viewMappings);

#if XAMCORE_4_0
		[Abstract]
#endif
		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("useResource:usage:stages:")]
		void UseResource (IMTLResource resource, MTLResourceUsage usage, MTLRenderStages stages);

#if XAMCORE_4_0
		[Abstract]
#endif
		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("useResources:count:usage:stages:")]
		void UseResources (IMTLResource[] resources, nuint count, MTLResourceUsage usage, MTLRenderStages stages);

#if XAMCORE_4_0
		[Abstract]
#endif
		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("useHeap:stages:")]
		void UseHeap (IMTLHeap heap, MTLRenderStages stages);

#if XAMCORE_4_0
		[Abstract]
#endif
		[TV(13,0), Mac (10,15), iOS (13,0)]
		[Export ("useHeaps:count:stages:")]
		void UseHeaps (IMTLHeap[] heaps, nuint count, MTLRenderStages stages);

#if XAMCORE_4_0
		[Abstract]
#endif
		[NoiOS, NoTV, Mac (10,15)]
		[Export ("sampleCountersInBuffer:atSampleIndex:withBarrier:")]
		void SampleCounters (MTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier);
	}

	[iOS (8,0)][Mac (10,11)]
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

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPipelineReflection {
		[Export ("vertexArguments")]
#if XAMCORE_4_0
		MTLArgument [] VertexArguments { get; }
#else
		NSObject [] VertexArguments { get; }
#endif

		[Export ("fragmentArguments")]
#if XAMCORE_4_0
		MTLArgument [] FragmentArguments { get; }
#else
		NSObject [] FragmentArguments { get; }
#endif

		[iOS (11, 0), NoTV, NoMac, NoWatch]
		[NullAllowed, Export ("tileArguments")]
		MTLArgument[] TileArguments { get; }
	}

	[iOS (8,0)][Mac (10,11)]
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

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("storeActionOptions", ArgumentSemantic.Assign)]
		MTLStoreActionOptions StoreActionOptions { get; set; }
	}
	
	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	interface MTLRenderPassColorAttachmentDescriptor {
		[Export ("clearColor")]
		MTLClearColor ClearColor { get; set; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	interface MTLRenderPassDepthAttachmentDescriptor {

		[Export ("clearDepth")]
		double ClearDepth { get; set; }

		[iOS (9,0)]
		[Mac (10, 14)]
		[Export ("depthResolveFilter")]
		MTLMultisampleDepthResolveFilter DepthResolveFilter { get; set; } 
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	interface MTLRenderPassStencilAttachmentDescriptor {

		[Export ("clearStencil")]
		uint ClearStencil { get; set; } /* uint32_t */

		[iOS (12,0)]
		[NoTV]
		[Mac (10, 14)]
		[Export ("stencilResolveFilter", ArgumentSemantic.Assign)]
		MTLMultisampleStencilResolveFilter StencilResolveFilter { get; set; }
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPassColorAttachmentDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLRenderPassColorAttachmentDescriptor ObjectAtIndexedSubscript (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject (MTLRenderPassColorAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPassDescriptor : NSCopying {

		[Export ("colorAttachments")]
		MTLRenderPassColorAttachmentDescriptorArray ColorAttachments { get; }

		[Export ("depthAttachment", ArgumentSemantic.Copy)]
		MTLRenderPassDepthAttachmentDescriptor DepthAttachment { get; set; }

		[Export ("stencilAttachment", ArgumentSemantic.Copy)]
		MTLRenderPassStencilAttachmentDescriptor StencilAttachment { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("visibilityResultBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer VisibilityResultBuffer { get; set; }

		[Static, Export ("renderPassDescriptor")]
		[Autorelease]
		MTLRenderPassDescriptor CreateRenderPassDescriptor ();
		
		[iOS (12,0)]
		[Mac (10,11)]
		[NoTV]
		[Export ("renderTargetArrayLength")]
		nuint RenderTargetArrayLength { get; set; }

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("setSamplePositions:count:")]
		unsafe void SetSamplePositions ([NullAllowed] IntPtr positions, nuint count);

		[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("getSamplePositions:count:")]
		nuint GetSamplePositions ([NullAllowed] IntPtr positions, nuint count);

		[iOS (11, 0), NoTV, NoWatch, NoMac]
		[Export ("imageblockSampleLength")]
		nuint ImageblockSampleLength { get; set; }

		[iOS (11, 0), NoTV, NoWatch, NoMac]
		[Export ("threadgroupMemoryLength")]
		nuint ThreadgroupMemoryLength { get; set; }

		[iOS (11, 0), NoTV, NoWatch, NoMac]
		[Export ("tileWidth")]
		nuint TileWidth { get; set; }

		[iOS (11, 0), NoTV, NoWatch, NoMac]
		[Export ("tileHeight")]
		nuint TileHeight { get; set; }

		[iOS (11, 0), NoTV, NoWatch, Mac (10,15)]
		[Export ("defaultRasterSampleCount")]
		nuint DefaultRasterSampleCount { get; set; }

		[iOS (11, 0), NoTV, NoWatch, Mac(10,15)]
		[Export ("renderTargetWidth")]
		nuint RenderTargetWidth { get; set; }

		[iOS (11, 0), NoTV, NoWatch, Mac (10,15)]
		[Export ("renderTargetHeight")]
		nuint RenderTargetHeight { get; set; }

/* Selectors reported missing by instrospection: https://github.com/xamarin/maccore/issues/1978
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac, NoTV, iOS (13, 0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("maxVertexAmplificationCount")]
		nuint MaxVertexAmplificationCount { get; set; }
*/

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[Mac (10,15,4), NoTV, iOS (13, 0)]
		[NullAllowed, Export ("rasterizationRateMap", ArgumentSemantic.Strong)]
		IMTLRasterizationRateMap RasterizationRateMap { get; set; }
	}


	[iOS (10, 0), TV (10,0), NoWatch, Mac (10,13)]
	[BaseType (typeof(NSObject))]
	// note: type works only on devices, symbol is missing on the simulator
	interface MTLHeapDescriptor : NSCopying
	{
		[Export ("size")]
		nuint Size { get; set; }

		[Export ("storageMode", ArgumentSemantic.Assign)]
		MTLStorageMode StorageMode { get; set; }

		[Export ("cpuCacheMode", ArgumentSemantic.Assign)]
		MTLCpuCacheMode CpuCacheMode { get; set; }

/* Selectors missing reported by introspection: https://github.com/xamarin/maccore/issues/1978
		[Mac (10, 15), iOS (13, 0)]
		[Export ("hazardTrackingMode", ArgumentSemantic.Assign)]
		MTLHazardTrackingMode HazardTrackingMode { get; set; }

		[Mac (10, 15), iOS (13, 0)]
		[Export ("resourceOptions", ArgumentSemantic.Assign)]
		MTLResourceOptions ResourceOptions { get; set; }

		[Mac (10, 15), iOS (13, 0)]
		[Export ("type", ArgumentSemantic.Assign)]
		MTLHeapType Type { get; set; }
 */
	}
	
	[iOS (10, 0), TV (10,0), NoWatch, Mac (10,13)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	interface MTLHeap
	{
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

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("currentAllocatedSize")]
		nuint CurrentAllocatedSize { get; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("hazardTrackingMode")]
		MTLHazardTrackingMode HazardTrackingMode { get; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("resourceOptions")]
		MTLResourceOptions ResourceOptions { get; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("type")]
		MTLHeapType Type { get; }

		[TV (13,0), Mac (10,15), iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newBufferWithLength:options:offset:")]
		[return: NullAllowed]
		IMTLBuffer CreateBuffer (nuint length, MTLResourceOptions options, nuint offset);

		[TV (13,0), Mac (10,15), iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newTextureWithDescriptor:offset:")]
		[return: NullAllowed]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor, nuint offset);
	}
	
	interface IMTLResource {}
	interface IMTLHeap {}
	[iOS (8,0)][Mac (10,11)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	partial interface MTLResource  {

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("cpuCacheMode")]
		MTLCpuCacheMode CpuCacheMode { get; }

#if XAMCORE_4_0
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[iOS (9,0)]
		[Export ("storageMode")]
		MTLStorageMode StorageMode { get; }

		[Abstract, Export ("setPurgeableState:")]
		MTLPurgeableState SetPurgeableState (MTLPurgeableState state);
		
		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[NullAllowed, Export ("heap")]
		IMTLHeap Heap { get; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("makeAliasable")]
		void MakeAliasable ();

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("isAliasable")]
		bool IsAliasable { get; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("allocatedSize")]
		nuint AllocatedSize { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("resourceOptions")]
		MTLResourceOptions ResourceOptions { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("heapOffset")]
		nuint HeapOffset { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Export ("hazardTrackingMode")]
		MTLHazardTrackingMode HazardTrackingMode { get; }
	}

	[iOS (9,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface MTLComputePipelineDescriptor : NSCopying {
		// it's marked as `nullable` but it asserts with
		// /BuildRoot/Library/Caches/com.apple.xbs/Sources/Metal/Metal-54.18/Framework/MTLComputePipeline.mm:216: failed assertion `label must not be nil.'
		[Export ("label")]
		string Label { get; set; }

		// it's marked as `nullable` but it asserts with
		// /BuildRoot/Library/Caches/com.apple.xbs/Sources/Metal/Metal-54.18/Framework/MTLComputePipeline.mm:230: failed assertion `computeFunction must not be nil.'
		[Export ("computeFunction", ArgumentSemantic.Strong)]
		IMTLFunction ComputeFunction { get; set; }

		[Export ("threadGroupSizeIsMultipleOfThreadExecutionWidth")]
		bool ThreadGroupSizeIsMultipleOfThreadExecutionWidth { get; set; }

		[Export ("reset")]
		void Reset ();

		[Mac (10,14), iOS (12,0), TV (12,0)]
		[Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; set; }
		
		[iOS (10, 0), TV (10,0), NoWatch, Mac (10,12)]
		[NullAllowed, Export ("stageInputDescriptor", ArgumentSemantic.Copy)]
		MTLStageInputOutputDescriptor StageInputDescriptor { get; set; }

		[Mac (10, 13), iOS (11,0), TV (11,0), NoWatch]
		[Export ("buffers")]
		MTLPipelineBufferDescriptorArray Buffers { get; }

		[NoMac, iOS (13, 0), TV (13,0)]
		[Export ("supportIndirectCommandBuffers")]
		bool SupportIndirectCommandBuffers { get; set; }
	}
	
	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface MTLStageInputOutputDescriptor : NSCopying
	{
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

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[BaseType (typeof(NSObject))]
	interface MTLType
	{
		[Export ("dataType")]
		MTLDataType DataType { get; }
	}

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[BaseType (typeof(MTLType))]
	interface MTLPointerType
	{
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

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[BaseType (typeof(MTLType))]
	interface MTLTextureReferenceType
	{
		[Export ("textureDataType")]
		MTLDataType TextureDataType { get; }

		[Export ("textureType")]
		MTLTextureType TextureType { get; }

		[Export ("access")]
		MTLArgumentAccess Access { get; }

		[Export ("isDepthTexture")]
		bool IsDepthTexture { get; }
	}

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	interface IMTLCaptureScope { }

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof(NSObject))]
	interface MTLCaptureScope
	{
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


	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MTLCaptureManager
	{
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
		[Export ("startCaptureWithDevice:")]
		void StartCapture (IMTLDevice device);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Export ("startCaptureWithCommandQueue:")]
		void StartCapture (IMTLCommandQueue commandQueue);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'StartCapture (MTLCaptureDescriptor, NSError)' instead.")]
		[Export ("startCaptureWithScope:")]
		void StartCapture (IMTLCaptureScope captureScope);

		[Export ("stopCapture")]
		void StopCapture ();

		[NullAllowed, Export ("defaultCaptureScope", ArgumentSemantic.Strong)]
		IMTLCaptureScope DefaultCaptureScope { get; set; }

		[Export ("isCapturing")]
		bool IsCapturing { get; }

		[Mac (10,15), iOS (13,0), TV (13,0)]
		[Export ("supportsDestination:")]
		bool SupportsDestination (MTLCaptureDestination destination);

		[Mac (10,15), iOS (13,0), TV (13,0)]
		[Export ("startCaptureWithDescriptor:error:")]
		bool StartCapture (MTLCaptureDescriptor descriptor, [NullAllowed] out NSError error);
	}

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[BaseType (typeof(NSObject))]
	interface MTLPipelineBufferDescriptor : NSCopying
	{
		[Export ("mutability", ArgumentSemantic.Assign)]
		MTLMutability Mutability { get; set; }
	}

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[BaseType (typeof(NSObject))]
	interface MTLPipelineBufferDescriptorArray
	{
		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		MTLPipelineBufferDescriptor GetObject (nuint bufferIndex);

		[Internal]
		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLPipelineBufferDescriptor buffer, nuint bufferIndex);
	}

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[BaseType (typeof(NSObject))]
	interface MTLArgumentDescriptor : NSCopying
	{
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

	[Mac (10,13), iOS (11,0), TV (11,0), NoWatch]
	[Protocol]
	interface MTLArgumentEncoder
	{
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

		[Abstract]
		[Export ("setBuffers:offsets:withRange:")]
		void SetBuffers (IMTLBuffer[] buffers, IntPtr offsets, NSRange range);

		[Abstract]
		[Export ("setTexture:atIndex:")]
		void SetTexture ([NullAllowed] IMTLTexture texture, nuint index);

		[Abstract]
		[Export ("setTextures:withRange:")]
		void SetTextures (IMTLTexture[] textures, NSRange range);

		[Abstract]
		[Export ("setSamplerState:atIndex:")]
		void SetSamplerState ([NullAllowed] IMTLSamplerState sampler, nuint index);

		[Abstract]
		[Export ("setSamplerStates:withRange:")]
		void SetSamplerStates (IMTLSamplerState[] samplers, NSRange range);

		[Abstract]
		[Export ("constantDataAtIndex:")]
		IntPtr GetConstantData (nuint index);

		[iOS (13,0), TV (13,0)]
		[Mac (10,14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setRenderPipelineState:atIndex:")]
		void SetRenderPipelineState ([NullAllowed] IMTLRenderPipelineState pipeline, nuint index);

		[iOS (13,0), TV (13,0)]
		[Mac (10,14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setRenderPipelineStates:withRange:")]
		void SetRenderPipelineStates (IMTLRenderPipelineState[] pipelines, NSRange range);

		[Mac (10,14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setIndirectCommandBuffer:atIndex:")]
		void SetIndirectCommandBuffer ([NullAllowed] IMTLIndirectCommandBuffer indirectCommandBuffer, nuint index);

		[Mac (10,14)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setIndirectCommandBuffers:withRange:")]
		void SetIndirectCommandBuffers (IMTLIndirectCommandBuffer[] buffers, NSRange range);

#if MONOMAC || XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newArgumentEncoderForBufferAtIndex:")]
		[return: NullAllowed]
		[return: Release]
		IMTLArgumentEncoder CreateArgumentEncoder (nuint index);

		[TV (13,0), NoMac, iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setComputePipelineState:atIndex:")]
		void SetComputePipelineState ([NullAllowed] IMTLComputePipelineState pipeline, nuint index);

		[TV (13,0), NoMac, iOS (13,0)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("setComputePipelineStates:withRange:")]
		void SetComputePipelineStates (IMTLComputePipelineState[] pipelines, NSRange range);

	}

	[iOS (11, 0), NoTV, NoMac, NoWatch]
	[BaseType (typeof (NSObject))]
	interface MTLTileRenderPipelineColorAttachmentDescriptor : NSCopying {
		[Export ("pixelFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat PixelFormat { get; set; }
	}

	[iOS (11, 0), NoTV, NoMac, NoWatch]
	[BaseType (typeof (NSObject))]
	interface MTLTileRenderPipelineColorAttachmentDescriptorArray {
		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		MTLTileRenderPipelineColorAttachmentDescriptor GetObject (nuint attachmentIndex);

		[Internal]
		[Export ("setObject:atIndexedSubscript:")]
		void SetObject (MTLTileRenderPipelineColorAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	[iOS (11, 0), NoTV, NoMac, NoWatch]
	[BaseType (typeof (NSObject))]
	interface MTLTileRenderPipelineDescriptor : NSCopying {
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

		[iOS (12,0)]
		[Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; set; }

		[Export ("reset")]
		void Reset ();
	}

	interface IMTLEvent {}

	[Mac (10,14), iOS (12,0), TV (12,0)]
	[Protocol]
	interface MTLEvent {
		[Abstract]
		[NullAllowed, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	[Mac (10,14), iOS (12,0), TV (12,0)]
	[BaseType (typeof(NSObject))]
	[DesignatedDefaultCtor]
	interface MTLSharedEventListener {
		[Export ("initWithDispatchQueue:")]
		[DesignatedInitializer]
		IntPtr Constructor (DispatchQueue dispatchQueue);

		[Export ("dispatchQueue")]
		DispatchQueue DispatchQueue { get; }
	}

	delegate void MTLSharedEventNotificationBlock (IMTLSharedEvent @event, ulong value);

	interface IMTLSharedEvent {}

	[Mac (10,14), iOS (12,0), TV (12,0)]
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

	[Mac (10,14), iOS (12,0), TV (12,0)]
	[BaseType (typeof(NSObject))]
	interface MTLSharedEventHandle : NSSecureCoding {
		[NullAllowed, Export ("label")]
		string Label { get; }
	}

	interface IMTLIndirectRenderCommand {}

	[Mac (10,14), iOS (12,0)]
	[Protocol]
	interface MTLIndirectRenderCommand {

#if MONOMAC && !XAMCORE_4_0 
		[Abstract]
#endif
#if XAMCORE_4_0 
		[Abstract]
#endif
		[iOS (13,0), TV (13,0), Mac (10,14)]
		[Export ("setRenderPipelineState:")]
		void SetRenderPipelineState (IMTLRenderPipelineState pipelineState);

		[Abstract]
		[Export ("setVertexBuffer:offset:atIndex:")]
		void SetVertexBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract]
		[Export ("setFragmentBuffer:offset:atIndex:")]
		void SetFragmentBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract]
		[NoTV]
		[Export ("drawPatches:patchStart:patchCount:patchIndexBuffer:patchIndexBufferOffset:instanceCount:baseInstance:tessellationFactorBuffer:tessellationFactorBufferOffset:tessellationFactorBufferInstanceStride:")]
		void DrawPatches (nuint numberOfPatchControlPoints, nuint patchStart, nuint patchCount, [NullAllowed] IMTLBuffer patchIndexBuffer, nuint patchIndexBufferOffset, nuint instanceCount, nuint baseInstance, IMTLBuffer buffer, nuint offset, nuint instanceStride);

		[Abstract]
		[NoTV]
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
	}

	[Mac (10,14), iOS (12,0), TV (12,0)]
	[BaseType (typeof(NSObject))]
	interface MTLIndirectCommandBufferDescriptor {
		[Export ("commandTypes", ArgumentSemantic.Assign)]
		MTLIndirectCommandType CommandTypes { get; set; }

		[iOS (13,0), TV (13,0)]
		[Export ("inheritPipelineState")]
		bool InheritPipelineState { get; set; }

		[Export ("inheritBuffers")]
		bool InheritBuffers { get; set; }

		[Export ("maxVertexBufferBindCount")]
		nuint MaxVertexBufferBindCount { get; set; }

		[Export ("maxFragmentBufferBindCount")]
		nuint MaxFragmentBufferBindCount { get; set; }
/* Fails introspection issue: https://github.com/xamarin/maccore/issues/1979
		[NoMac, iOS (13, 0)]
		[Export ("maxKernelBufferBindCount")]
		nuint MaxKernelBufferBindCount { get; set; }
*/
	}

	interface IMTLIndirectCommandBuffer {}

	[Mac (10,14), iOS (12,0), TV (12,0)]
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

#if XAMCORE_4_0
		[Abstract]
#endif
		[TV (13,0), NoMac, iOS (13,0)]
		[Export ("indirectComputeCommandAtIndex:")]
		IMTLIndirectComputeCommand GetIndirectComputeCommand (nuint commandIndex);
	}

	[iOS (13,0), TV (13,0), Mac (10,14)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MTLSharedTextureHandle : NSSecureCoding {
		[Export ("device")]
		IMTLDevice Device { get; }

		[NullAllowed, Export ("label")]
		string Label { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Mac (10,15,4), NoTV, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MTLRasterizationRateSampleArray {
		[Export ("objectAtIndexedSubscript:")]
		NSNumber GetObject (nuint index);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject (NSNumber value, nuint index);
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Mac (10,15,4), NoTV, iOS (13,0)]
	[BaseType (typeof(NSObject))]
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

/* Selectors reported as missing by introspection:  https://github.com/xamarin/maccore/issues/1980
		[Export ("layers")]
		MTLRasterizationRateLayerArray Layers { get; }

		[Export ("screenSize", ArgumentSemantic.Assign)]
		MTLSize ScreenSize { get; set; }

		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Export ("layerCount")]
		nuint LayerCount { get; }
*/
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Mac (10,15,4), NoTV, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MTLRasterizationRateLayerDescriptor : NSCopying {

		[Export ("initWithSampleCount:")]
		[DesignatedInitializer]
		IntPtr Constructor (MTLSize sampleCount);

		[Internal]
		[Export ("initWithSampleCount:horizontal:vertical:")]
		IntPtr Constructor (MTLSize sampleCount, IntPtr horizontal, IntPtr vertical);

/* Selectors reported as not working by introspection: https://github.com/xamarin/maccore/issues/1976
		[Export ("sampleCount")]
		MTLSize SampleCount { get; }

		[Internal]
		[Export ("horizontalSampleStorage")]
		IntPtr _HorizontalSampleStorage { get; }

		[Internal]
		[Export ("verticalSampleStorage")]
		IntPtr _VerticalSampleStorage { get; }

		[Export ("horizontal")]
		MTLRasterizationRateSampleArray Horizontal { get; }

		[Export ("vertical")]
		MTLRasterizationRateSampleArray Vertical { get; }
 */
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Mac (10,15,4), NoTV, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MTLRasterizationRateLayerArray {
		[Export ("objectAtIndexedSubscript:")]
		[return: NullAllowed]
		MTLRasterizationRateLayerDescriptor GetObject (nuint layerIndex);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject ([NullAllowed] MTLRasterizationRateLayerDescriptor layer, nuint layerIndex);
	}

	interface IMTLRasterizationRateMap {}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Mac (10,15,4), NoTV, iOS (13,0)]
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

	interface IMTLResourceStateCommandEncoder {}

	[NoMac, iOS (13,0), NoTV]
	[Protocol]
	interface MTLResourceStateCommandEncoder : MTLCommandEncoder {
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac]
		[Advice ("This API is not available when using UIKit on macOS."), Abstract]
		[Export ("updateTextureMappings:mode:regions:mipLevels:slices:numRegions:")]
		void Update (IMTLTexture texture, MTLSparseTextureMappingMode mode, IntPtr regions, IntPtr mipLevels, IntPtr slices, nuint numRegions);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac]
		[Advice ("This API is not available when using UIKit on macOS."), Abstract]
		[Export ("updateTextureMapping:mode:region:mipLevel:slice:")]
		void Update (IMTLTexture texture, MTLSparseTextureMappingMode mode, MTLRegion region, nuint mipLevel, nuint slice);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac]
		[Advice ("This API is not available when using UIKit on macOS."), Abstract]
		[Export ("updateTextureMapping:mode:indirectBuffer:indirectBufferOffset:")]
		void Update (IMTLTexture texture, MTLSparseTextureMappingMode mode, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac]
		[Advice ("This API is not available when using UIKit on macOS."), Abstract]
		[Export ("updateFence:")]
		void Update (IMTLFence fence);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac]
		[Advice ("This API is not available when using UIKit on macOS."), Abstract]
		[Export ("waitForFence:")]
		void Wait (IMTLFence fence);
	}

	[Mac (10,15), iOS (13,0), TV (13,0)]
	[BaseType (typeof(NSObject))]
	interface MTLCaptureDescriptor : NSCopying {
		[NullAllowed, Export ("captureObject", ArgumentSemantic.Strong)]
		NSObject CaptureObject { get; set; }

		[Export ("destination", ArgumentSemantic.Assign)]
		MTLCaptureDestination Destination { get; set; }

		[NullAllowed, Export ("outputURL", ArgumentSemantic.Copy)]
		NSUrl OutputUrl { get; set; }
	}

	interface IMTLIndirectComputeCommand {}

	[TV (13,0), iOS (13,0), NoMac]
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
	}

	interface IMTLCounter {}

	[NoiOS, NoTV, Mac (10,15)]
	[Protocol]
#if !XAMCORE_4_0
	[BaseType (typeof(NSObject))]
#endif
	interface MTLCounter {
		[Abstract]
		[Export ("name")]
		string Name { get; }
	}

	interface IMTLCounterSet {}

	[NoiOS, NoTV, Mac (10,15)]
	[Protocol]
#if !XAMCORE_4_0
	[BaseType (typeof(NSObject))]
#endif
	interface MTLCounterSet {
		[Abstract]
		[Export ("name")]
		string Name { get; }

		[Abstract]
		[Export ("counters", ArgumentSemantic.Copy)]
		IMTLCounter[] Counters { get; }
	}

	interface IMTLCounterSampleBuffer {}

	[NoiOS, NoTV, Mac (10,15)]
	[Protocol]
#if !XAMCORE_4_0
	[BaseType (typeof(NSObject))]
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

	[NoiOS, NoTV, Mac (10,15)]
	[BaseType (typeof(NSObject))]
	interface MTLCounterSampleBufferDescriptor : NSCopying
	{
		[NoiOS, Mac (10, 15)]
		[Export ("counterSet", ArgumentSemantic.Retain)]
		IMTLCounterSet CounterSet { get; set; }

		[NoiOS, Mac (10, 15)]
		[Export ("label")]
		string Label { get; set; }

		[NoiOS, Mac (10, 15)]
		[Export ("storageMode", ArgumentSemantic.Assign)]
		MTLStorageMode StorageMode { get; set; }

		[NoiOS, Mac (10, 15)]
		[Export ("sampleCount")]
		nuint SampleCount { get; set; }
	}
}
