// 
// VTCompressionSession.cs: VideoTools Compression Session class 
//
// Authors: 
//    Miguel de Icaza (miguel@xamarin.com)
//    Alex Soto (alex.soto@xamarin.com
// 
// Copyright 2014 Xamarin Inc.
//

using System.ComponentModel;

using CoreFoundation;
using CoreMedia;
using CoreVideo;

#nullable enable

namespace VideoToolbox {
	/// <summary>Turns uncompressed frames into compressed video frames</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class VTCompressionSession : VTSession {
		GCHandle callbackHandle;

		[Preserve (Conditional = true)]
		internal VTCompressionSession (NativeHandle handle, bool owns) : base (handle, owns)
		{
		}

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero)
				VTCompressionSessionInvalidate (Handle);

			if (callbackHandle.IsAllocated)
				callbackHandle.Free ();

			base.Dispose (disposing);
		}

		/// <summary>A delegate that will be called for each compressed frame.</summary>
		/// <param name="sourceFrame">The token passed starting the encoding operation.</param>
		/// <param name="status">Status code indicating if the operation was successful or not.</param>
		/// <param name="flags">Contains information about the encoding operation.</param>
		/// <param name="buffer">Contains a pointer to the encoded buffer if successful and the frame was not dropped. A <see langword="null" /> value indicates either an error, or that the frame was dropped.</param>
		/// <remarks>The delegate will be called in the order the frames are decoded, which is not necessarily the same as the display order.</remarks>
		public delegate void VTCompressionOutputCallback (/* void* */ IntPtr sourceFrame, /* OSStatus */ VTStatus status, VTEncodeInfoFlags flags, CMSampleBuffer? buffer);

		/// <summary>Create a new compression session</summary>
		/// <param name="width">Frame width in pixels.</param>
		/// <param name="height">Frame height in pixels.</param>
		/// <param name="codecType">Encoder to use to compress the frames.</param>
		/// <param name="compressionOutputCallback">A callback that will be invoked to process a compressed frame. See the delegate type for more information on the received parameters.</param>
		/// <param name="encoderSpecification">Parameters to choose the encoder, or <see langword="null" /> to let VideoToolbox choose it.</param>
		/// <param name="sourceImageBufferAttributes">Any additional attributes for the compressed data.</param>
		/// <returns>A new <see cref="VTCompressionSession" /> if successful, <see langword="null" /> otherwise.</returns>
		/// <remarks>The <paramref name="compressionOutputCallback" /> callback will be invoked for each frame in decode order, not necessarily the display order.</remarks>
		public static VTCompressionSession? Create (int width, int height, CMVideoCodecType codecType,
			VTCompressionOutputCallback compressionOutputCallback,
			VTVideoEncoderSpecification? encoderSpecification = null, // hardware acceleration is default behavior on iOS. no opt-in required.
			NSDictionary? sourceImageBufferAttributes = null)
		{
			unsafe {
				return Create (width, height, codecType, compressionOutputCallback, encoderSpecification, sourceImageBufferAttributes, &CompressionCallback);
			}
		}

		[UnmanagedCallersOnly]
		static void CompressionCallback (IntPtr outputCallbackClosure, IntPtr sourceFrame, VTStatus status, VTEncodeInfoFlags infoFlags, IntPtr cmSampleBufferPtr)
		{
			var gch = GCHandle.FromIntPtr (outputCallbackClosure);
			var func = (VTCompressionOutputCallback) gch.Target!;
			using var sampleBuffer = cmSampleBufferPtr == IntPtr.Zero ? null : new CMSampleBuffer (cmSampleBufferPtr, owns: false);
			func (sourceFrame, status, infoFlags, sampleBuffer);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe extern static VTStatus VTCompressionSessionCreate (
			/* CFAllocatorRef */ IntPtr allocator, /* can be null */
			/* int32_t */ int width,
			/* int32_t */ int height,
			/* CMVideoCodecType */ CMVideoCodecType codecType,
			/* CFDictionaryRef */ IntPtr dictionaryEncoderSpecification, /* can be null */
			/* CFDictionaryRef */ IntPtr dictionarySourceImageBufferAttributes, /* can be null */
			/* CFDictionaryRef */ IntPtr compressedDataAllocator, /* can be null */
			/* VTCompressionOutputCallback */ delegate* unmanaged</* void* CM_NULLABLE */ IntPtr, /* void* CM_NULLABLE */ IntPtr, /* OSStatus */ VTStatus, VTEncodeInfoFlags, /* CMSampleBufferRef CM_NULLABLE */ IntPtr, void> outputCallback,
			/* void* */ IntPtr outputCallbackClosure,
			/* VTCompressionSessionRef* */ IntPtr* compressionSessionOut);

		/// <summary>Create a new compression session</summary>
		/// <param name="width">Frame width in pixels.</param>
		/// <param name="height">Frame height in pixels.</param>
		/// <param name="codecType">Encoder to use to compress the frames.</param>
		/// <param name="compressionOutputCallback">A callback that will be invoked to process a compressed frame. See the delegate type for more information on the received parameters.</param>
		/// <param name="encoderSpecification">Parameters to choose the encoder, or <see langword="null" /> to let VideoToolbox choose it.</param>
		/// <param name="sourceImageBufferAttributes">Any additional attributes for the compressed data.</param>
		/// <returns>A new <see cref="VTCompressionSession" /> if successful, <see langword="null" /> otherwise.</returns>
		/// <remarks>The <paramref name="compressionOutputCallback" /> callback will be invoked for each frame in decode order, not necessarily the display order.</remarks>
		public static VTCompressionSession? Create (int width, int height, CMVideoCodecType codecType,
			VTCompressionOutputCallback compressionOutputCallback,
			VTVideoEncoderSpecification? encoderSpecification, // hardware acceleration is default behavior on iOS. no opt-in required.
			CVPixelBufferAttributes? sourceImageBufferAttributes)
		{
			return Create (width, height, codecType, compressionOutputCallback, encoderSpecification, sourceImageBufferAttributes?.Dictionary);
		}

		unsafe static VTCompressionSession? Create (int width, int height, CMVideoCodecType codecType,
			VTCompressionOutputCallback compressionOutputCallback,
			VTVideoEncoderSpecification? encoderSpecification, // hardware acceleration is default behavior on iOS. no opt-in required.
				NSDictionary? sourceImageBufferAttributes, // Undocumented options, probably always null
				delegate* unmanaged</* void* CM_NULLABLE */ IntPtr, /* void* CM_NULLABLE */ IntPtr, /* OSStatus */ VTStatus, VTEncodeInfoFlags, /* CMSampleBufferRef CM_NULLABLE */ IntPtr, void> staticCback)
		{
			var callbackHandle = default (GCHandle);
			if (compressionOutputCallback is not null)
				callbackHandle = GCHandle.Alloc (compressionOutputCallback);

			IntPtr ret;
			var result = VTCompressionSessionCreate (IntPtr.Zero, width, height, codecType,
				encoderSpecification.GetHandle (),
				sourceImageBufferAttributes.GetHandle (),
				IntPtr.Zero,
				callbackHandle.IsAllocated ? (staticCback) : null,
				GCHandle.ToIntPtr (callbackHandle),
				&ret);
			GC.KeepAlive (encoderSpecification);
			GC.KeepAlive (sourceImageBufferAttributes);

			if (result == VTStatus.Ok && ret != IntPtr.Zero)
				return new VTCompressionSession (ret, true) {
					callbackHandle = callbackHandle,
				};

			if (callbackHandle.IsAllocated)
				callbackHandle.Free ();

			return null;
		}

		// Helpers used by AVFoundation's AVPlannedVideoSegmentWritingRequest.CreateResumableCompressionSession,
		// which creates the native VTCompressionSession through an AVFoundation selector but reuses this class's
		// managed callback trampoline and GCHandle lifetime management.
		internal unsafe static (IntPtr OutputCallback, IntPtr OutputCallbackRefCon) PrepareOutputCallback (VTCompressionOutputCallback? compressionOutputCallback, out GCHandle callbackHandle)
		{
			if (compressionOutputCallback is null) {
				callbackHandle = default;
				return (IntPtr.Zero, IntPtr.Zero);
			}
			callbackHandle = GCHandle.Alloc (compressionOutputCallback);
			delegate* unmanaged</* void* */ IntPtr, /* void* */ IntPtr, /* OSStatus */ VTStatus, VTEncodeInfoFlags, /* CMSampleBufferRef */ IntPtr, void> trampoline = &CompressionCallback;
			return ((IntPtr) trampoline, GCHandle.ToIntPtr (callbackHandle));
		}

		internal static VTCompressionSession CreateFromOwnedHandle (IntPtr handle, GCHandle callbackHandle)
			=> new VTCompressionSession (handle, true) {
				callbackHandle = callbackHandle,
			};

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static void VTCompressionSessionInvalidate (IntPtr handle);

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static IntPtr /* cvpixelbufferpoolref */ VTCompressionSessionGetPixelBufferPool (IntPtr handle);

		/// <summary>Get the pixel buffer pool for this compression session.</summary>
		/// <returns>The pixel buffer pool for this compression session.</returns>
		public CVPixelBufferPool? GetPixelBufferPool ()
		{
			var ret = VTCompressionSessionGetPixelBufferPool (GetCheckedHandle ());

			if (ret != IntPtr.Zero)
				return new CVPixelBufferPool (ret, false);

			return null;
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionPrepareToEncodeFrames (IntPtr handle);

		/// <summary>Prepare to encode frames.</summary>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		/// <remarks>Calling this method before starting an encoding operation is optional.</remarks>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public VTStatus PrepareToEncodeFrames ()
		{
			return VTCompressionSessionPrepareToEncodeFrames (GetCheckedHandle ());
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe extern static VTStatus VTCompressionSessionEncodeFrame (
			/* VTCompressionSessionRef */ IntPtr session,
			/* CVImageBufferRef */ IntPtr imageBuffer,
			/* CMTime */ CMTime presentation,
			/* CMTime */ CMTime duration, // can ve CMTime.Invalid
			/* CFDictionaryRef */ IntPtr dict, // can be null, undocumented options
			/* void* */ IntPtr sourceFrame,
			/* VTEncodeInfoFlags */ VTEncodeInfoFlags* flags);

#if !XAMCORE_5_0
		// The 'sourceFrame' parameter is just a user-provided value, it's not tied to any particular type/object.
		// In this overload it's typed as 'CVImageBuffer', which doesn't make much sense - thus remove this overload when we can.
		[Obsolete ("Call 'EncodeFrame(CVImageBuffer,CMTime,CMTime,NSDictionary,IntPtr,out VTEncodeInfoFlags)' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public VTStatus EncodeFrame (CVImageBuffer imageBuffer, CMTime presentationTimestamp, CMTime duration,
			NSDictionary frameProperties, CVImageBuffer sourceFrame, out VTEncodeInfoFlags infoFlags)
		{
			if (sourceFrame is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourceFrame));

			VTStatus status = EncodeFrame (imageBuffer, presentationTimestamp, duration, frameProperties, sourceFrame.GetCheckedHandle (), out infoFlags);
			GC.KeepAlive (sourceFrame);
			return status;
		}
#endif

		/// <summary>Encode a video frame.</summary>
		/// <param name="imageBuffer">The image buffer with the image data to compress.</param>
		/// <param name="presentationTimestamp">The presentation timestamp for this frame.</param>
		/// <param name="duration">The duration of this frame.</param>
		/// <param name="frameProperties">Any frame properties for this frame.</param>
		/// <param name="sourceFrame">This value will be passed to the <see cref="VTCompressionOutputCallback" /> callback that was specified when the compression session was created.</param>
		/// <param name="infoFlags">Upon return, any information flags from the encoder for this frame.</param>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		public VTStatus EncodeFrame (CVImageBuffer imageBuffer, CMTime presentationTimestamp, CMTime duration,
			NSDictionary? frameProperties, IntPtr sourceFrame, out VTEncodeInfoFlags infoFlags)
		{
			if (imageBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (imageBuffer));

			infoFlags = default;
			unsafe {
				fixed (VTEncodeInfoFlags* infoFlagsPtr = &infoFlags) {
					VTStatus status = VTCompressionSessionEncodeFrame (GetCheckedHandle (), imageBuffer.Handle, presentationTimestamp, duration,
						frameProperties.GetHandle (),
						sourceFrame, infoFlagsPtr);
					GC.KeepAlive (imageBuffer);
					GC.KeepAlive (frameProperties);
					return status;
				}
			}
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionCompleteFrames (IntPtr session, CMTime completeUntilPresentationTimeStamp);

		/// <summary>Finish decoding all frames.</summary>
		/// <param name="completeUntilPresentationTimeStamp">If this value is numeric, all frames with presentation timestamps lower or equal to this value will be emitted. If this value is not numeric, all frames will be emitted.</param>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		public VTStatus CompleteFrames (CMTime completeUntilPresentationTimeStamp)
		{
			return VTCompressionSessionCompleteFrames (GetCheckedHandle (), completeUntilPresentationTimeStamp);
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTCompressionSessionBeginPass (IntPtr session, VTCompressionSessionOptionFlags flags, IntPtr reserved);

		/// <summary>Start a compression pass.</summary>
		/// <param name="flags">Any flags for this compression pass.</param>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public VTStatus BeginPass (VTCompressionSessionOptionFlags flags)
		{
			return VTCompressionSessionBeginPass (GetCheckedHandle (), flags, IntPtr.Zero);
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe extern static VTStatus VTCompressionSessionEndPass (IntPtr session, byte* furtherPassesRequestedOut, IntPtr reserved);

		/// <summary>End a compression pass.</summary>
		/// <param name="furtherPassesRequested">Will be set to <see langword="true" /> if the decoder requests another pass.</param>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public VTStatus EndPass (out bool furtherPassesRequested)
		{
			byte b;
			VTStatus result;
			unsafe {
				result = VTCompressionSessionEndPass (GetCheckedHandle (), &b, IntPtr.Zero);
			}
			furtherPassesRequested = b != 0;
			return result;
		}

		/// <summary>End a compression pass, marking this pass as the final pass.</summary>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		public VTStatus EndPassAsFinal ()
		{
			unsafe {
				return VTCompressionSessionEndPass (GetCheckedHandle (), null, IntPtr.Zero);
			}
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe extern static VTStatus VTCompressionSessionGetTimeRangesForNextPass (
			/* VTCompressionSessionRef */ IntPtr session,
			/* CMItemCount* */ int* itemCount,
			/* const CMTimeRange** */ IntPtr* target);

		/// <summary>Get the time ranges for the next pass.</summary>
		/// <param name="timeRanges">Upon return, the time ranges for the next pass.</param>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public VTStatus GetTimeRangesForNextPass (out CMTimeRange []? timeRanges)
		{
			VTStatus v;
			int count;
			IntPtr target;
			unsafe {
				v = VTCompressionSessionGetTimeRangesForNextPass (GetCheckedHandle (), &count, &target);
			}
			if (v != VTStatus.Ok) {
				timeRanges = null;
				return v;
			}
			timeRanges = new CMTimeRange [count];
			unsafe {
				CMTimeRange* ptr = (CMTimeRange*) target;
				for (int i = 0; i < count; i++)
					timeRanges [i] = ptr [i];
			}
			return VTStatus.Ok;
		}

		/// <summary>Set any compression properties.</summary>
		/// <param name="options">The compression properties to set.</param>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		public VTStatus SetCompressionProperties (VTCompressionProperties options)
		{
			if (options is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (options));

			var dictionary = options.Dictionary;
			var rv = VTSessionSetProperties (GetCheckedHandle (), dictionary.Handle);
			GC.KeepAlive (dictionary);
			return rv;
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.VideoToolboxLibrary)]
		extern static /* Boolean */ byte VTIsStereoMVHEVCEncodeSupported ();

		/// <summary>Returns whether the current system supports stereo MV-HEVC encode.</summary>
		/// <returns>True if the current system supports stereo MV-HEVC encode, false otherwise.</returns>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		public static bool IsStereoMvHevcEncodeSupported ()
		{
			return VTIsStereoMVHEVCEncodeSupported () != 0;
		}

#if !__TVOS__
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern VTStatus VTCompressionSessionEncodeMultiImageFrame (
			IntPtr /* CM_NONNULL VTCompressionSessionRef */ session,
			IntPtr /* CM_NONNULL CMTaggedBufferGroupRef */ taggedBufferGroup,
			CMTime presentationTimeStamp,
			CMTime duration, // may be kCMTimeInvalid
			IntPtr /* CM_NULLABLE CFDictionaryRef */ frameProperties,
			IntPtr /* void * CM_NULLABLE */ sourceFrameRefcon,
			VTEncodeInfoFlags* /* VTEncodeInfoFlags * CM_NULLABLE */ infoFlagsOut);

		/// <summary>Encode a multi-image video frame.</summary>
		/// <param name="taggedBufferGroup">The tagged buffer group with multiple images to compress.</param>
		/// <param name="presentationTimestamp">The presentation timestamp for this frame.</param>
		/// <param name="duration">The duration of this frame.</param>
		/// <param name="frameProperties">Any frame properties for this frame.</param>
		/// <param name="sourceFrame">This value will be passed to the <see cref="VTCompressionOutputCallback" /> callback that was specified when the compression session was created.</param>
		/// <param name="infoFlags">Upon return, any information flags from the encoder for this frame.</param>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public unsafe VTStatus EncodeMultiImageFrame (CMTaggedBufferGroup taggedBufferGroup, CMTime presentationTimestamp, CMTime duration, NSDictionary? frameProperties, IntPtr sourceFrame, out VTEncodeInfoFlags infoFlags)
		{
			infoFlags = default;

			fixed (VTEncodeInfoFlags* infoFlagsPtr = &infoFlags) {
				var rv = VTCompressionSessionEncodeMultiImageFrame (
							GetCheckedHandle (),
							taggedBufferGroup.GetNonNullHandle (nameof (taggedBufferGroup)),
							presentationTimestamp,
							duration,
							frameProperties.GetHandle (),
							sourceFrame,
							infoFlagsPtr);

				GC.KeepAlive (taggedBufferGroup);
				GC.KeepAlive (frameProperties);

				return rv;
			}
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern VTStatus VTCompressionSessionEncodeMultiImageFrameWithOutputHandler (
			IntPtr /* CM_NONNULL VTCompressionSessionRef */ session,
			IntPtr /* CM_NONNULL CMTaggedBufferGroupRef */ taggedBufferGroup,
			CMTime presentationTimeStamp,
			CMTime duration, // may be kCMTimeInvalid
			IntPtr /* CM_NULLABLE CFDictionaryRef */ frameProperties, // may be NULL
			VTEncodeInfoFlags* /* VTEncodeInfoFlags * CM_NULLABLE */ infoFlagsOut,
			BlockLiteral* /* CM_NONNULL VTCompressionOutputHandler	*/ outputHandler);

		/// <summary>Encode a multi-image video frame.</summary>
		/// <param name="taggedBufferGroup">The tagged buffer group with multiple images to compress.</param>
		/// <param name="presentationTimestamp">The presentation timestamp for this frame.</param>
		/// <param name="duration">The duration of this frame.</param>
		/// <param name="frameProperties">Any frame properties for this frame.</param>
		/// <param name="infoFlags">Upon return, any information flags from the encoder for this frame.</param>
		/// <param name="outputHandler">A callback that will be invoked to process a compressed frame. See the delegate type for more information on the received parameters.</param>
		/// <returns><see cref="VTStatus.Ok" /> if successful, or an error code otherwise.</returns>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe VTStatus EncodeMultiImageFrame (CMTaggedBufferGroup taggedBufferGroup, CMTime presentationTimestamp, CMTime duration, NSDictionary? frameProperties, out VTEncodeInfoFlags infoFlags, VTCompressionOutputHandler outputHandler)
		{
			delegate* unmanaged<BlockLiteral*, VTStatus, VTEncodeInfoFlags, IntPtr, void> trampoline = &VTCompressionOutputHandlerCallback;
			using var trampolineBlock = new BlockLiteral (trampoline, outputHandler, typeof (VTCompressionSession), nameof (VTCompressionOutputHandlerCallback));

			infoFlags = default;

			fixed (VTEncodeInfoFlags* infoFlagsPtr = &infoFlags) {
				var rv = VTCompressionSessionEncodeMultiImageFrameWithOutputHandler (
							GetCheckedHandle (),
							taggedBufferGroup.GetNonNullHandle (nameof (taggedBufferGroup)),
							presentationTimestamp,
							duration,
							frameProperties.GetHandle (),
							infoFlagsPtr,
							&trampolineBlock);

				GC.KeepAlive (taggedBufferGroup);
				GC.KeepAlive (frameProperties);

				return rv;
			}
		}

		[UnmanagedCallersOnly]
		unsafe static void VTCompressionOutputHandlerCallback (BlockLiteral* block, VTStatus status, VTEncodeInfoFlags infoFlags, IntPtr sampleBuffer)
		{
			var del = BlockLiteral.GetTarget<VTCompressionOutputHandler> ((IntPtr) block);
			if (del is not null) {
				var sampleBufferObj = sampleBuffer == IntPtr.Zero ? null : new CMSampleBuffer (sampleBuffer, owns: false);
				del (status, infoFlags, sampleBufferObj);
			}
		}

		/// <summary>A delegate that will be called for each compressed frame.</summary>
		/// <param name="status">Status code indicating if the operation was successful or not.</param>
		/// <param name="infoFlags">Contains information about the encoding operation.</param>
		/// <param name="sampleBuffer">Contains a pointer to the encoded buffer if successful and the frame was not dropped. A <see langword="null" /> value indicates either an error, or that the frame was dropped.</param>
		/// <remarks>The delegate will be called in the order the frames are decoded, which is not necessarily the same as the display order.</remarks>
		public delegate void VTCompressionOutputHandler (VTStatus status, VTEncodeInfoFlags infoFlags, CMSampleBuffer? sampleBuffer);
#endif // !__TVOS__
	}
}
