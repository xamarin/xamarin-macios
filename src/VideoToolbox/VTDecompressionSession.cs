// 
// VTDecompressionSession.cs: VideoTools Decompression Session class 
//
// Authors:
//    Alex Soto (alex.soto@xamarin.com
// 
// Copyright 2015 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;
using CoreVideo;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace VideoToolbox {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class VTDecompressionSession : VTSession {

		GCHandle callbackHandle;

#if !NET
		protected internal VTDecompressionSession (NativeHandle handle) : base (handle)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal VTDecompressionSession (NativeHandle handle, bool owns) : base (handle, owns)
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero)
				VTDecompressionSessionInvalidate (Handle);

			if (callbackHandle.IsAllocated)
				callbackHandle.Free ();

			base.Dispose (disposing);
		}

		[StructLayout (LayoutKind.Sequential)]
		struct VTDecompressionOutputCallbackRecord {
#if NET
			public unsafe delegate* unmanaged</* void* */ IntPtr, /* void* */ IntPtr, /* OSStatus */ VTStatus, VTDecodeInfoFlags, /* CVImageBuffer */ IntPtr, CMTime, CMTime, void> Proc;
#else
			public DecompressionOutputCallback Proc;
#endif
			public IntPtr DecompressionOutputRefCon;
		}

		// sourceFrame: It seems it's only used as a parameter to be passed into DecodeFrame so no need to strong type it
		public delegate void VTDecompressionOutputCallback (/* void* */ IntPtr sourceFrame, /* OSStatus */ VTStatus status, VTDecodeInfoFlags flags, CVImageBuffer buffer, CMTime presentationTimeStamp, CMTime presentationDuration);
#if !NET
		delegate void DecompressionOutputCallback (/* void* */ IntPtr outputCallbackClosure, /* void* */ IntPtr sourceFrame, /* OSStatus */ VTStatus status,
			VTDecodeInfoFlags infoFlags, /* CVImageBuffer */ IntPtr cmSampleBufferPtr, CMTime presentationTimeStamp, CMTime presentationDuration);
#endif

#if !NET
		//
		// Here for legacy code, which would only work under duress (user had to manually ref the CMSampleBuffer on the callback)
		//
		static DecompressionOutputCallback? _static_decompressionCallback;
		static DecompressionOutputCallback static_DecompressionOutputCallback {
			get {
				if (_static_decompressionCallback is null)
					_static_decompressionCallback = new DecompressionOutputCallback (DecompressionCallback);
				return _static_decompressionCallback!;
			}
		}
#endif

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (DecompressionOutputCallback))]
#endif
#endif
		static void DecompressionCallback (IntPtr outputCallbackClosure, IntPtr sourceFrame, VTStatus status,
			VTDecodeInfoFlags infoFlags, IntPtr imageBufferPtr, CMTime presentationTimeStamp, CMTime presentationDuration)
		{
			var gch = GCHandle.FromIntPtr (outputCallbackClosure);
			var func = gch.Target as VTDecompressionOutputCallback;

			if (func is null)
				return;

			// Apple headers states that the callback should get a CVImageBuffer but it turned out that not all of them are a
			// CVImageBuffer, some can be instances of CVImageBuffer and others can be instances of CVPixelBuffer. So we go one 
			// step further in the inheritance hierarchy and supply the callback a CVPixelBuffer and the callback supplies 
			// to the developer a CVImageBuffer, so the developer can choose when to use one or the other and we mimic
			// what Apple provides on its headers.
			using (var sampleBuffer = new CVPixelBuffer (imageBufferPtr, false)) {
				func (sourceFrame, status, infoFlags, sampleBuffer, presentationTimeStamp, presentationDuration);
			}
		}

#if !NET
		static DecompressionOutputCallback? _static_newDecompressionCallback;
		static DecompressionOutputCallback static_newDecompressionOutputCallback {
			get {
				if (_static_newDecompressionCallback is null)
					_static_newDecompressionCallback = new DecompressionOutputCallback (NewDecompressionCallback);
				return _static_newDecompressionCallback!;
			}
		}
#endif

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (DecompressionOutputCallback))]
#endif
#endif
		static void NewDecompressionCallback (IntPtr outputCallbackClosure, IntPtr sourceFrame, VTStatus status,
			VTDecodeInfoFlags infoFlags, IntPtr imageBufferPtr, CMTime presentationTimeStamp, CMTime presentationDuration)
		{
			var gch = GCHandle.FromIntPtr (outputCallbackClosure);
			var func = gch.Target as VTDecompressionOutputCallback;

			if (func is null)
				return;

			// Apple headers states that the callback should get a CVImageBuffer but it turned out that not all of them are a
			// CVImageBuffer, some can be instances of CVImageBuffer and others can be instances of CVPixelBuffer. So we go one 
			// step further in the inheritance hierarchy and supply the callback a CVPixelBuffer and the callback supplies 
			// to the developer a CVImageBuffer, so the developer can choose when to use one or the other and we mimic
			// what Apple provides on its headers.
			using (var sampleBuffer = new CVPixelBuffer (imageBufferPtr, owns: false)) {
				func (sourceFrame, status, infoFlags, sampleBuffer, presentationTimeStamp, presentationDuration);
			}
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionCreate (
			/* CFAllocatorRef */ IntPtr allocator, // can be null
			/* CMVideoFormatDescriptionRef */ IntPtr videoFormatDescription,
			/* CFDictionaryRef */ IntPtr videoDecoderSpecification, // can be null
			/* CFDictionaryRef */ IntPtr destinationImageBufferAttributes, // can be null
			/* const VTDecompressionOutputCallbackRecord* */ ref VTDecompressionOutputCallbackRecord outputCallback,
			/* VTDecompressionSessionRef* */ out IntPtr decompressionSessionOut);

#if false // Disabling for now until we have some tests on this
		public static VTDecompressionSession Create (CMVideoFormatDescription formatDescription,
			VTVideoDecoderSpecification decoderSpecification = null, // hardware acceleration is default behavior on iOS. no opt-in required.
			NSDictionary destinationImageBufferAttributes = null) // Undocumented options, probably always null
		{
			if (formatDescription is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (formatDescription));

			var callbackStruct = default (VTDecompressionOutputCallbackRecord);

			IntPtr ret;

			var result = VTDecompressionSessionCreate (IntPtr.Zero, formatDescription.Handle,
				decoderSpecification is not null ? decoderSpecification.Dictionary.Handle : IntPtr.Zero,
				destinationImageBufferAttributes.GetHandle (),
				ref callbackStruct,
				out ret);

			return result == VTStatus.Ok && ret != IntPtr.Zero
				? new VTDecompressionSession (ret, true)
				: null;
		}
#endif

#if !NET
		[Obsolete ("This overload requires that the provided compressionOutputCallback manually CFRetain the passed CMSampleBuffer, use Create(VTDecompressionOutputCallback,CMVideoFormatDescription,VTVideoDecoderSpecification,CVPixelBufferAttributes) variant instead which does not have that requirement.")]
		public static VTDecompressionSession? Create (VTDecompressionOutputCallback outputCallback,
								 CMVideoFormatDescription formatDescription,
								 VTVideoDecoderSpecification? decoderSpecification = null, // hardware acceleration is default behavior on iOS. no opt-in required.
								 NSDictionary? destinationImageBufferAttributes = null)
		{
			return Create (outputCallback, formatDescription, decoderSpecification, destinationImageBufferAttributes, static_DecompressionOutputCallback);
		}
#endif // !NET

		public static VTDecompressionSession? Create (VTDecompressionOutputCallback outputCallback,
								 CMVideoFormatDescription formatDescription,
#if NET
							     VTVideoDecoderSpecification? decoderSpecification = null, // hardware acceleration is default behavior on iOS. no opt-in required.
							     CVPixelBufferAttributes? destinationImageBufferAttributes = null)
#else
								 VTVideoDecoderSpecification? decoderSpecification, // hardware acceleration is default behavior on iOS. no opt-in required.
								 CVPixelBufferAttributes? destinationImageBufferAttributes)
#endif
		{
#if NET
			unsafe {
				return Create (outputCallback, formatDescription, decoderSpecification, destinationImageBufferAttributes?.Dictionary, &NewDecompressionCallback);
			}
#else
			return Create (outputCallback, formatDescription, decoderSpecification, destinationImageBufferAttributes?.Dictionary, static_newDecompressionOutputCallback);
#endif
		}

		unsafe static VTDecompressionSession? Create (VTDecompressionOutputCallback outputCallback,
							  CMVideoFormatDescription formatDescription,
							  VTVideoDecoderSpecification? decoderSpecification, // hardware acceleration is default behavior on iOS. no opt-in required.
							  NSDictionary? destinationImageBufferAttributes,
#if NET
						      delegate* unmanaged</* void* */ IntPtr, /* void* */ IntPtr, /* OSStatus */ VTStatus, VTDecodeInfoFlags, /* CVImageBuffer */ IntPtr, CMTime, CMTime, void> cback)
#else
							  DecompressionOutputCallback cback)
#endif
		{
			if (outputCallback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputCallback));

			if (formatDescription is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (formatDescription));

			var callbackHandle = GCHandle.Alloc (outputCallback);
			var callbackStruct = new VTDecompressionOutputCallbackRecord () {
				Proc = cback,
				DecompressionOutputRefCon = GCHandle.ToIntPtr (callbackHandle)
			};
			IntPtr ret;

			var result = VTDecompressionSessionCreate (IntPtr.Zero, formatDescription.Handle,
				decoderSpecification.GetHandle (),
				destinationImageBufferAttributes.GetHandle (),
				ref callbackStruct,
				out ret);

			if (result == VTStatus.Ok && ret != IntPtr.Zero)
				return new VTDecompressionSession (ret, true) {
					callbackHandle = callbackHandle
				};

			callbackHandle.Free ();
			return null;
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static void VTDecompressionSessionInvalidate (IntPtr sesion);

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionDecodeFrame (
			/* VTDecompressionSessionRef */ IntPtr session,
			/* CMSampleBufferRef */ IntPtr sampleBuffer,
			/* VTDecodeFrameFlags */ VTDecodeFrameFlags decodeFlags,
			/* void* */ IntPtr sourceFrame,
			/* VTDecodeInfoFlags */ out VTDecodeInfoFlags infoFlagsOut);

		public VTStatus DecodeFrame (CMSampleBuffer sampleBuffer, VTDecodeFrameFlags decodeFlags, IntPtr sourceFrame, out VTDecodeInfoFlags infoFlags)
		{
			if (sampleBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sampleBuffer));

			return VTDecompressionSessionDecodeFrame (GetCheckedHandle (), sampleBuffer.Handle, decodeFlags, sourceFrame, out infoFlags);
		}
#if false // Disabling for now until we have some tests on this
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionDecodeFrameWithOutputHandler (
			/* VTDecompressionSessionRef */ IntPtr session,
			/* CMSampleBufferRef */ IntPtr sampleBuffer,
			/* VTDecodeFrameFlags */ VTDecodeFrameFlags decodeFlags,
			/* VTDecodeInfoFlags */ out VTDecodeInfoFlags infoFlagsOut,
			/* VTDecompressionOutputHandler */ ref BlockLiteral outputHandler);

		public delegate void VTDecompressionOutputHandler (VTStatus status, VTDecodeInfoFlags infoFlags,
			CVImageBuffer imageBuffer, CMTime presentationTimeStamp, CMTime presentationDuration);

		unsafe delegate void VTDecompressionOutputHandlerProxy (BlockLiteral *block, VTStatus status, VTDecodeInfoFlags infoFlags,
			IntPtr imageBuffer, CMTime presentationTimeStamp, CMTime presentationDuration);

		static unsafe readonly VTDecompressionOutputHandlerProxy decompressionOutputHandlerTrampoline = VTDecompressionOutputHandlerTrampoline;

		[MonoPInvokeCallback (typeof (VTDecompressionOutputHandlerProxy))]
		static unsafe void VTDecompressionOutputHandlerTrampoline (BlockLiteral *block,
			VTStatus status, VTDecodeInfoFlags infoFlags, IntPtr imageBuffer,
			CMTime presentationTimeStamp, CMTime presentationDuration)
		{
			var del = (VTDecompressionOutputHandler)(block->Target);
			if (del is not null)
				del (status, infoFlags, new CVImageBuffer (imageBuffer, false), presentationTimeStamp, presentationDuration);
		}

		public VTStatus DecodeFrame (CMSampleBuffer sampleBuffer, VTDecodeFrameFlags decodeFlags,
			out VTDecodeInfoFlags infoFlags, VTDecompressionOutputHandler outputHandler)
		{
			if (sampleBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sampleBuffer));
			if (outputHandler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputHandler));

			var block = new BlockLiteral ();
			block.SetupBlockUnsafe (decompressionOutputHandlerTrampoline, outputHandler);
			try {
				return VTDecompressionSessionDecodeFrameWithOutputHandler (GetCheckedHandle (),
					sampleBuffer.Handle, decodeFlags, out infoFlags, ref block);
			} finally {
				block.CleanupBlock ();
			}
		}
#endif
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionFinishDelayedFrames (IntPtr sesion);

		public VTStatus FinishDelayedFrames ()
		{
			return VTDecompressionSessionFinishDelayedFrames (GetCheckedHandle ());
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionCanAcceptFormatDescription (IntPtr sesion, IntPtr newFormatDescriptor);

		public VTStatus CanAcceptFormatDescriptor (CMFormatDescription newDescriptor)
		{
			if (newDescriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (newDescriptor));

			return VTDecompressionSessionCanAcceptFormatDescription (GetCheckedHandle (), newDescriptor.Handle);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionWaitForAsynchronousFrames (IntPtr sesion);

		public VTStatus WaitForAsynchronousFrames ()
		{
			return VTDecompressionSessionWaitForAsynchronousFrames (GetCheckedHandle ());
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionCopyBlackPixelBuffer (IntPtr sesion, out IntPtr pixelBufferOut);

		public VTStatus CopyBlackPixelBuffer (out CVPixelBuffer? pixelBuffer)
		{
			var result = VTDecompressionSessionCopyBlackPixelBuffer (GetCheckedHandle (), out var ret);
			pixelBuffer = Runtime.GetINativeObject<CVPixelBuffer> (ret, true);
			return result;
		}

		public VTStatus SetDecompressionProperties (VTDecompressionProperties options)
		{
			if (options is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (options));

			return VTSessionSetProperties (GetCheckedHandle (), options.Dictionary.Handle);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.VideoToolboxLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool VTIsHardwareDecodeSupported (CMVideoCodecType codecType);

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static bool IsHardwareDecodeSupported (CMVideoCodecType codecType)
		{
			return VTIsHardwareDecodeSupported (codecType);
		}
	}
}
