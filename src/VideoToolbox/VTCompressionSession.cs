// 
// VTCompressionSession.cs: VideoTools Compression Session class 
//
// Authors: 
//    Miguel de Icaza (miguel@xamarin.com)
//    Alex Soto (alex.soto@xamarin.com
// 
// Copyright 2014 Xamarin Inc.
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
	public class VTCompressionSession : VTSession {
		GCHandle callbackHandle;

		/* invoked by marshallers */
		protected internal VTCompressionSession (IntPtr handle) : base (handle)
		{
		}

		[Preserve (Conditional=true)]
		internal VTCompressionSession (IntPtr handle, bool owns) : base (handle, owns)
		{
		}

		~VTCompressionSession ()
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
				VTCompressionSessionInvalidate (Handle);

			if (callbackHandle.IsAllocated)
				callbackHandle.Free();

			base.Dispose (disposing);
		}

		// sourceFrame: It seems it's only used as a parameter to be passed into EncodeFrame so no need to strong type it
		public delegate void VTCompressionOutputCallback (/* void* */ IntPtr sourceFrame, /* OSStatus */ VTStatus status, VTEncodeInfoFlags flags, CMSampleBuffer buffer);
		delegate void CompressionOutputCallback (/* void* CM_NULLABLE */ IntPtr outputCallbackClosure, /* void* CM_NULLABLE */ IntPtr sourceFrame, /* OSStatus */ VTStatus status, VTEncodeInfoFlags infoFlags, /* CMSampleBufferRef CM_NULLABLE */ IntPtr cmSampleBufferPtr);

		#region Legacy code start
		//
		// Here for legacy code, which would only work under duress (user had to manually ref the CMSampleBuffer on the callback)
		//
		static CompressionOutputCallback _static_CompressionOutputCallback;
		static CompressionOutputCallback static_CompressionOutputCallback {
			get {
				if (_static_CompressionOutputCallback == null)
					_static_CompressionOutputCallback = new CompressionOutputCallback (CompressionCallback);
				return _static_CompressionOutputCallback;
			}
		}

		static void CompressionCallback (IntPtr outputCallbackClosure, IntPtr sourceFrame, VTStatus status, VTEncodeInfoFlags infoFlags, IntPtr cmSampleBufferPtr, bool owns)
		{
			var gch = GCHandle.FromIntPtr (outputCallbackClosure);
			var func = (VTCompressionOutputCallback) gch.Target;
			if (cmSampleBufferPtr == IntPtr.Zero) {
				func (sourceFrame, status, infoFlags, null);
			} else {
				using (var sampleBuffer = new CMSampleBuffer (cmSampleBufferPtr, owns: owns))
					func (sourceFrame, status, infoFlags, sampleBuffer);
			}
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (CompressionOutputCallback))]
#endif
		static void CompressionCallback (IntPtr outputCallbackClosure, IntPtr sourceFrame, VTStatus status, VTEncodeInfoFlags infoFlags, IntPtr cmSampleBufferPtr)
		{
			CompressionCallback (outputCallbackClosure, sourceFrame, status, infoFlags, cmSampleBufferPtr, true);
		}

		[Obsolete ("This overload requires that the provided compressionOutputCallback manually CFRetain the passed CMSampleBuffer, use Create(int,int,CMVideoCodecType,VTCompressionOutputCallback,VTVideoEncoderSpecification,CVPixelBufferAttributes) variant instead which does not have that requirement.")]
		public static VTCompressionSession Create (int width, int height, CMVideoCodecType codecType,
			VTCompressionOutputCallback compressionOutputCallback,
			VTVideoEncoderSpecification encoderSpecification = null, // hardware acceleration is default behavior on iOS. no opt-in required.
			NSDictionary sourceImageBufferAttributes = null)
		{
			return Create (width, height, codecType, compressionOutputCallback, encoderSpecification, sourceImageBufferAttributes, static_newCompressionOutputCallback);
		}
		// End region of legacy code
		#endregion

		static CompressionOutputCallback _static_newCompressionOutputCallback;
		static CompressionOutputCallback static_newCompressionOutputCallback {
			get {
				if (_static_newCompressionOutputCallback == null)
					_static_newCompressionOutputCallback = new CompressionOutputCallback (NewCompressionCallback);
				return _static_newCompressionOutputCallback;
			}
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (CompressionOutputCallback))]
#endif
		static void NewCompressionCallback (IntPtr outputCallbackClosure, IntPtr sourceFrame, VTStatus status, VTEncodeInfoFlags infoFlags, IntPtr cmSampleBufferPtr)
		{
			CompressionCallback (outputCallbackClosure, sourceFrame, status, infoFlags, cmSampleBufferPtr, false);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionCreate (
			/* CFAllocatorRef */ IntPtr allocator, /* can be null */
			/* int32_t */ int width,
			/* int32_t */ int height,
			/* CMVideoCodecType */ CMVideoCodecType codecType,
			/* CFDictionaryRef */ IntPtr dictionaryEncoderSpecification, /* can be null */
			/* CFDictionaryRef */ IntPtr dictionarySourceImageBufferAttributes, /* can be null */
			/* CFDictionaryRef */ IntPtr compressedDataAllocator, /* can be null */
			/* VTCompressionOutputCallback */ CompressionOutputCallback outputCallback,
			/* void* */ IntPtr outputCallbackClosure,
			/* VTCompressionSessionRef* */ out IntPtr compressionSessionOut);

#if false // Disabling for now until we have some tests on this
		[Mac (10,11), iOS (9,0)]
		public static VTCompressionSession Create (int width, int height, CMVideoCodecType codecType,
			VTVideoEncoderSpecification encoderSpecification = null,
			NSDictionary sourceImageBufferAttributes = null)
		{
			return Create (width, height, codecType, null,
				encoderSpecification, sourceImageBufferAttributes);
		}
#endif
		public static VTCompressionSession Create (int width, int height, CMVideoCodecType codecType,
			VTCompressionOutputCallback compressionOutputCallback,
			VTVideoEncoderSpecification encoderSpecification, // hardware acceleration is default behavior on iOS. no opt-in required.
			CVPixelBufferAttributes sourceImageBufferAttributes)
		{
			return Create (width, height, codecType, compressionOutputCallback, encoderSpecification, sourceImageBufferAttributes == null ? null : sourceImageBufferAttributes.Dictionary, static_CompressionOutputCallback);
		}

		static VTCompressionSession Create (int width, int height, CMVideoCodecType codecType,
			VTCompressionOutputCallback compressionOutputCallback,
			VTVideoEncoderSpecification encoderSpecification, // hardware acceleration is default behavior on iOS. no opt-in required.
		        NSDictionary sourceImageBufferAttributes, CompressionOutputCallback staticCback) // Undocumented options, probably always null
		{
			var callbackHandle = default (GCHandle);
			if (compressionOutputCallback != null)
				callbackHandle = GCHandle.Alloc (compressionOutputCallback);

			IntPtr ret;

			var result = VTCompressionSessionCreate (IntPtr.Zero, width, height, codecType,
				encoderSpecification != null ? encoderSpecification.Dictionary.Handle : IntPtr.Zero,
				sourceImageBufferAttributes != null ? sourceImageBufferAttributes.Handle : IntPtr.Zero,
				IntPtr.Zero,
				callbackHandle.IsAllocated ? (staticCback) : null,
				GCHandle.ToIntPtr (callbackHandle),
	            out ret);

			if (result == VTStatus.Ok && ret != IntPtr.Zero)
				return new VTCompressionSession (ret, true) {
					callbackHandle = callbackHandle
				};

			if (callbackHandle.IsAllocated)
				callbackHandle.Free ();

			return null;
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static void VTCompressionSessionInvalidate (IntPtr handle);

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static IntPtr /* cvpixelbufferpoolref */ VTCompressionSessionGetPixelBufferPool (IntPtr handle);

		public CVPixelBufferPool GetPixelBufferPool ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");

			var ret = VTCompressionSessionGetPixelBufferPool (Handle);

			if (ret != IntPtr.Zero) 
				return new CVPixelBufferPool (ret, false);

			return null;
		}

		[Mac (10,9), iOS (8,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionPrepareToEncodeFrames (IntPtr handle);

		[Mac (10,9), iOS (8,0)]
		public VTStatus PrepareToEncodeFrames ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");
			
			return VTCompressionSessionPrepareToEncodeFrames (Handle);
		}
		
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionEncodeFrame (
			/* VTCompressionSessionRef */ IntPtr session, 
			/* CVImageBufferRef */ IntPtr imageBuffer, 
			/* CMTime */ CMTime presentation, 
			/* CMTime */ CMTime duration, // can ve CMTime.Invalid
			/* CFDictionaryRef */ IntPtr dict, // can be null, undocumented options
			/* void* */ IntPtr sourceFrame, 
			/* VTEncodeInfoFlags */ out VTEncodeInfoFlags flags);

		public VTStatus EncodeFrame (CVImageBuffer imageBuffer, CMTime presentationTimestampe, CMTime duration, 
			NSDictionary frameProperties, IntPtr sourceFrame, out VTEncodeInfoFlags infoFlags)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");
			if (imageBuffer == null)
				throw new ArgumentNullException ("imageBuffer");
			
			return VTCompressionSessionEncodeFrame (Handle, imageBuffer.Handle, presentationTimestampe, duration,
				frameProperties == null ? IntPtr.Zero : frameProperties.Handle,
				sourceFrame, out infoFlags);
		}		

#if false // Disabling for now until we have some tests on this
		[Mac (10,11), iOS (9,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static unsafe VTStatus VTCompressionSessionEncodeFrameWithOutputHandler (
			/* VTCompressionSessionRef */ IntPtr session,
			/* CVImageBufferRef */ IntPtr imageBuffer,
			/* CMTime */ CMTime presentation,
			/* CMTime */ CMTime duration, // can ve CMTime.Invalid
			/* CFDictionaryRef */ IntPtr dict, // can be null, undocumented options
			/* VTEncodeInfoFlags */ out VTEncodeInfoFlags flags,
			/* VTCompressionOutputHandler */ BlockLiteral *outputHandler);

		public delegate void VTCompressionOutputHandler (VTStatus status, VTEncodeInfoFlags infoFlags, CMSampleBuffer sampleBuffer);

		unsafe delegate void VTCompressionOutputHandlerProxy (BlockLiteral *block,
			VTStatus status, VTEncodeInfoFlags infoFlags, IntPtr sampleBuffer);

		static unsafe readonly VTCompressionOutputHandlerProxy compressionOutputHandlerTrampoline = VTCompressionOutputHandlerTrampoline;

		[MonoPInvokeCallback (typeof (VTCompressionOutputHandlerProxy))]
		static unsafe void VTCompressionOutputHandlerTrampoline (BlockLiteral *block,
			VTStatus status, VTEncodeInfoFlags infoFlags, IntPtr sampleBuffer)
		{
			var del = (VTCompressionOutputHandler)(block->Target);
			if (del != null)
				del (status, infoFlags, new CMSampleBuffer (sampleBuffer));
		}

		[Mac (10,11), iOS (9,0)]
		public VTStatus EncodeFrame (CVImageBuffer imageBuffer, CMTime presentationTimestamp, CMTime duration,
			NSDictionary frameProperties, IntPtr sourceFrame, out VTEncodeInfoFlags infoFlags,
			VTCompressionOutputHandler outputHandler)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");
			if (imageBuffer == null)
				throw new ArgumentNullException ("imageBuffer");
			if (outputHandler == null)
				throw new ArgumentNullException ("outputHandler");

			unsafe {
				var block = new BlockLiteral ();
				var blockPtr = &block;
				block.SetupBlockUnsafe (compressionOutputHandlerTrampoline, outputHandler);

				try {
					return VTCompressionSessionEncodeFrameWithOutputHandler (Handle,
						imageBuffer.Handle, presentationTimestamp, duration,
						frameProperties == null ? IntPtr.Zero : frameProperties.Handle,
						out infoFlags, blockPtr);
				} finally {
					blockPtr->CleanupBlock ();
				}
			}
		}
#endif
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionCompleteFrames (IntPtr session, CMTime completeUntilPresentationTimeStamp);

		public VTStatus CompleteFrames (CMTime completeUntilPresentationTimeStamp)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");
			return VTCompressionSessionCompleteFrames (Handle, completeUntilPresentationTimeStamp);
		}

		[Mac (10,10), iOS (8,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionBeginPass (IntPtr session, VTCompressionSessionOptionFlags flags, IntPtr reserved);

		[Mac (10,10), iOS (8,0)]
		public VTStatus BeginPass (VTCompressionSessionOptionFlags flags)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");
			return VTCompressionSessionBeginPass (Handle, flags, IntPtr.Zero);
		}

		[Mac (10,10), iOS (8,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionEndPass (IntPtr session, out byte furtherPassesRequestedOut, IntPtr reserved);

		[Mac (10,10), iOS (8,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionEndPass (IntPtr session, IntPtr ptrByte, IntPtr reserved);

		[Mac (10,10), iOS (8,0)]
		public VTStatus EndPass (out bool furtherPassesRequested)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");

			byte b;
			var result = VTCompressionSessionEndPass (Handle, out b, IntPtr.Zero);
			furtherPassesRequested = b != 0;
			return result;
		}

		// Like EndPass, but this will be the final pass, so the encoder will skip the evaluation.
		public VTStatus EndPassAsFinal ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");

			return VTCompressionSessionEndPass (Handle, IntPtr.Zero, IntPtr.Zero);
		}

		[Mac (10,10), iOS (8,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionGetTimeRangesForNextPass (
			/* VTCompressionSessionRef */ IntPtr session, 
			/* CMItemCount* */ out int itemCount, 
			/* const CMTimeRange** */ out IntPtr target);

		[Mac (10,10), iOS (8,0)]
		public VTStatus GetTimeRangesForNextPass (out CMTimeRange [] timeRanges)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");
			IntPtr target;
			int count;
			var v = VTCompressionSessionGetTimeRangesForNextPass (Handle, out count, out target);
			if (v != VTStatus.Ok) {
				timeRanges = null;
				return v;
			}
			timeRanges = new CMTimeRange [count];
			unsafe {
				CMTimeRange *ptr = (CMTimeRange *)target;
				for (int i = 0; i < count; i++)
					timeRanges [i] = ptr [i];
			}
			return VTStatus.Ok;
		}

		public VTStatus SetCompressionProperties (VTCompressionProperties options)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("CompressionSession");
			if (options == null)
				throw new ArgumentNullException ("options");

			return VTSessionSetProperties (Handle, options.Dictionary.Handle);
		}
	}	
}
