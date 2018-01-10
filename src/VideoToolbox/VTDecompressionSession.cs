// 
// VTDecompressionSession.cs: VideoTools Decompression Session class 
//
// Authors:
//    Alex Soto (alex.soto@xamarin.com
// 
// Copyright 2015 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;
using CoreVideo;

namespace VideoToolbox {
	[Mac (10,8), iOS (8,0), TV (10,2)]
	public class VTDecompressionSession : VTSession {

		GCHandle callbackHandle;

		/* invoked by marshallers */
		protected internal VTDecompressionSession (IntPtr handle) : base (handle)
		{
		}

		[Preserve (Conditional=true)]
		internal VTDecompressionSession (IntPtr handle, bool owns) : base (handle, owns)
		{
		}

		~VTDecompressionSession ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero)
				VTDecompressionSessionInvalidate (Handle);

			if (callbackHandle.IsAllocated)
				callbackHandle.Free();

			base.Dispose (disposing);
		}

		[StructLayout(LayoutKind.Sequential)]
		struct VTDecompressionOutputCallbackRecord
		{
			public DecompressionOutputCallback Proc;
			public IntPtr DecompressionOutputRefCon; 
		}

		// sourceFrame: It seems it's only used as a parameter to be passed into DecodeFrame so no need to strong type it
		public delegate void VTDecompressionOutputCallback (/* void* */ IntPtr sourceFrame, /* OSStatus */ VTStatus status, VTDecodeInfoFlags flags, CVImageBuffer buffer, CMTime presentationTimeStamp, CMTime presentationDuration);
		delegate void DecompressionOutputCallback (/* void* */ IntPtr outputCallbackClosure, /* void* */ IntPtr sourceFrame, /* OSStatus */ VTStatus status, 
			VTDecodeInfoFlags infoFlags, /* CVImageBuffer */ IntPtr cmSampleBufferPtr, CMTime presentationTimeStamp, CMTime presentationDuration);

		//
		// Here for legacy code, which would only work under duress (user had to manually ref the CMSampleBuffer on the callback)
		//
		static DecompressionOutputCallback _static_decompressionCallback;
		static DecompressionOutputCallback static_DecompressionOutputCallback {
			get {
				if (_static_decompressionCallback == null)
					_static_decompressionCallback = new DecompressionOutputCallback (DecompressionCallback);
				return _static_decompressionCallback;
			}
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (DecompressionOutputCallback))]
#endif
		static void DecompressionCallback (IntPtr outputCallbackClosure, IntPtr sourceFrame, VTStatus status, 
			VTDecodeInfoFlags infoFlags, IntPtr imageBufferPtr, CMTime presentationTimeStamp, CMTime presentationDuration)
		{
			var gch = GCHandle.FromIntPtr (outputCallbackClosure);
			var func = (VTDecompressionOutputCallback) gch.Target;

			// Apple headers states that the callback should get a CVImageBuffer but it turned out that not all of them are a
			// CVImageBuffer, some can be instances of CVImageBuffer and others can be instances of CVPixelBuffer. So we go one 
			// step further in the inheritance hierarchy and supply the callback a CVPixelBuffer and the callback supplies 
			// to the developer a CVImageBuffer, so the developer can choose when to use one or the other and we mimic
			// what Apple provides on its headers.
			using (var sampleBuffer = new CVPixelBuffer (imageBufferPtr)) {
				func (sourceFrame, status, infoFlags, sampleBuffer, presentationTimeStamp, presentationDuration);
			}
		}

		static DecompressionOutputCallback _static_newDecompressionCallback;
		static DecompressionOutputCallback static_newDecompressionOutputCallback {
			get {
				if (_static_newDecompressionCallback == null)
					_static_newDecompressionCallback = new DecompressionOutputCallback (NewDecompressionCallback);
				return _static_newDecompressionCallback;
			}
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (DecompressionOutputCallback))]
#endif
		static void NewDecompressionCallback (IntPtr outputCallbackClosure, IntPtr sourceFrame, VTStatus status, 
			VTDecodeInfoFlags infoFlags, IntPtr imageBufferPtr, CMTime presentationTimeStamp, CMTime presentationDuration)
		{
			var gch = GCHandle.FromIntPtr (outputCallbackClosure);
			var func = (VTDecompressionOutputCallback) gch.Target;

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
		[Mac (10,11), iOS (9,0)]
		public static VTDecompressionSession Create (CMVideoFormatDescription formatDescription,
			VTVideoDecoderSpecification decoderSpecification = null, // hardware acceleration is default behavior on iOS. no opt-in required.
			NSDictionary destinationImageBufferAttributes = null) // Undocumented options, probably always null
		{
			if (formatDescription == null)
				throw new ArgumentNullException ("formatDescription");

			var callbackStruct = default (VTDecompressionOutputCallbackRecord);

			IntPtr ret;

			var result = VTDecompressionSessionCreate (IntPtr.Zero, formatDescription.Handle,
				decoderSpecification != null ? decoderSpecification.Dictionary.Handle : IntPtr.Zero,
				destinationImageBufferAttributes != null ? destinationImageBufferAttributes.Handle : IntPtr.Zero,
				ref callbackStruct,
				out ret);

			return result == VTStatus.Ok && ret != IntPtr.Zero
				? new VTDecompressionSession (ret, true)
				: null;
		}
#endif
		[Obsolete ("This overload requires that the provided compressionOutputCallback manually CFRetain the passed CMSampleBuffer, use Create(VTDecompressionOutputCallback,CMVideoFormatDescription,VTVideoDecoderSpecification,CVPixelBufferAttributes) variant instead which does not have that requirement.")]

		public static VTDecompressionSession Create (VTDecompressionOutputCallback outputCallback,
							     CMVideoFormatDescription formatDescription,
							     VTVideoDecoderSpecification decoderSpecification = null, // hardware acceleration is default behavior on iOS. no opt-in required.
							     NSDictionary destinationImageBufferAttributes = null)
		{
			return Create (outputCallback, formatDescription, decoderSpecification, destinationImageBufferAttributes, static_DecompressionOutputCallback);
		}
	
		public static VTDecompressionSession Create (VTDecompressionOutputCallback outputCallback,
							     CMVideoFormatDescription formatDescription,
							     VTVideoDecoderSpecification decoderSpecification, // hardware acceleration is default behavior on iOS. no opt-in required.
							     CVPixelBufferAttributes destinationImageBufferAttributes)
		{
			return Create (outputCallback, formatDescription, decoderSpecification, destinationImageBufferAttributes == null ? null : destinationImageBufferAttributes.Dictionary, static_newDecompressionOutputCallback);
		}
	
		static VTDecompressionSession Create (VTDecompressionOutputCallback outputCallback,
						      CMVideoFormatDescription formatDescription,
						      VTVideoDecoderSpecification decoderSpecification, // hardware acceleration is default behavior on iOS. no opt-in required.
						      NSDictionary destinationImageBufferAttributes,
						      DecompressionOutputCallback cback)
		{	
			if (formatDescription == null)
				throw new ArgumentNullException ("formatDescription");

			var callbackHandle = GCHandle.Alloc (outputCallback);
			var callbackStruct = new VTDecompressionOutputCallbackRecord () {
				Proc = cback,
				DecompressionOutputRefCon = GCHandle.ToIntPtr (callbackHandle)
			};
			IntPtr ret;

			var result = VTDecompressionSessionCreate (IntPtr.Zero, formatDescription.Handle,
				decoderSpecification != null ? decoderSpecification.Dictionary.Handle : IntPtr.Zero,
				destinationImageBufferAttributes != null ? destinationImageBufferAttributes.Handle : IntPtr.Zero,
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
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("DecompressionSession");
			if (sampleBuffer == null)
				throw new ArgumentNullException ("sampleBuffer");

			return VTDecompressionSessionDecodeFrame (Handle, sampleBuffer.Handle, decodeFlags, sourceFrame, out infoFlags);
		}
#if false // Disabling for now until we have some tests on this
		[Mac (10,11), iOS (9,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static unsafe VTStatus VTDecompressionSessionDecodeFrameWithOutputHandler (
			/* VTDecompressionSessionRef */ IntPtr session,
			/* CMSampleBufferRef */ IntPtr sampleBuffer,
			/* VTDecodeFrameFlags */ VTDecodeFrameFlags decodeFlags,
			/* VTDecodeInfoFlags */ out VTDecodeInfoFlags infoFlagsOut,
			/* VTDecompressionOutputHandler */ BlockLiteral *outputHandler);

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
			if (del != null)
				del (status, infoFlags, new CVImageBuffer (imageBuffer), presentationTimeStamp, presentationDuration);
		}

		[Mac (10,11), iOS (9,0)]
		public VTStatus DecodeFrame (CMSampleBuffer sampleBuffer, VTDecodeFrameFlags decodeFlags,
			out VTDecodeInfoFlags infoFlags, VTDecompressionOutputHandler outputHandler)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("DecompressionSession");
			if (sampleBuffer == null)
				throw new ArgumentNullException ("sampleBuffer");
			if (outputHandler == null)
				throw new ArgumentNullException ("outputHandler");

			unsafe {
				var block = new BlockLiteral ();
				var blockPtr = &block;
				block.SetupBlockUnsafe (decompressionOutputHandlerTrampoline, outputHandler);

				try {
					return VTDecompressionSessionDecodeFrameWithOutputHandler (Handle,
						sampleBuffer.Handle, decodeFlags, out infoFlags, blockPtr);
				} finally {
					blockPtr->CleanupBlock ();
				}
			}
		}
#endif
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionFinishDelayedFrames (IntPtr sesion);

		public VTStatus FinishDelayedFrames ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("DecompressionSession");

			return VTDecompressionSessionFinishDelayedFrames (Handle);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionCanAcceptFormatDescription (IntPtr sesion, IntPtr newFormatDescriptor);

		public VTStatus CanAcceptFormatDescriptor (CMFormatDescription newDescriptor)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("DecompressionSession");

			return VTDecompressionSessionCanAcceptFormatDescription (Handle, newDescriptor.Handle);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionWaitForAsynchronousFrames (IntPtr sesion);

		public VTStatus WaitForAsynchronousFrames ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("DecompressionSession");

			return VTDecompressionSessionWaitForAsynchronousFrames (Handle);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTDecompressionSessionCopyBlackPixelBuffer (IntPtr sesion, out IntPtr pixelBufferOut);

		public VTStatus CopyBlackPixelBuffer (out CVPixelBuffer pixelBuffer)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("DecompressionSession");

			IntPtr ret;
			var result = VTDecompressionSessionCopyBlackPixelBuffer (Handle, out ret);
			pixelBuffer = Runtime.GetINativeObject<CVPixelBuffer> (ret, true);
			CFObject.CFRelease (ret);
			return result;
		}

		public VTStatus SetDecompressionProperties (VTDecompressionProperties options)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("DecompressionSession");
			if (options == null)
				throw new ArgumentNullException ("options");

			return VTSessionSetProperties (Handle, options.Dictionary.Handle);
		}

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static bool VTIsHardwareDecodeSupported (CMVideoCodecType codecType);

		[Mac (10,13), iOS (11,0), TV (11,0)]
		public static bool IsHardwareDecodeSupported (CMVideoCodecType codecType)
		{
			return VTIsHardwareDecodeSupported (codecType);
		}
	}
}

