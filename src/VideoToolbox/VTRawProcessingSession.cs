#if MONOMAC && NET
#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using AVFoundation;
using CoreFoundation;
using CoreMedia;
using CoreVideo;
using Foundation;
using ObjCRuntime;

namespace VideoToolbox {
	/// <summary>This delegate is called when available parameters or parameter values change without an explicit call to <see cref="VTRawProcessingSession.SetProcessingParameters(VTRawProcessingParameters)" />.</summary>
	/// <param name="newParameters">The new parameters.</param>
	/// <remarks>This callback is intended to be used by clients to update their UIs.</remarks>
	public delegate void VTRawProcessingParameterChangeHandler (NSObject []? newParameters);

	/// <summary>This delegate is called when frame processing is finished.</summary>
	/// <param name="status">If processing was successful, this will be <see cref="VTStatus.Ok" />, otherwise an error code.</param>
	/// <param name="processedPixelBuffer">The <see cref="CoreVideo.CVPixelBuffer" /> with the processed video frame if the processing was successful, otherwise null.</param>
	public delegate void VTRawProcessingOutputHandler (VTStatus status, CVPixelBuffer? processedPixelBuffer);

#if NET
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
#endif
	public class VTRawProcessingSession : NativeObject {
		[Preserve (Conditional = true)]
		protected VTRawProcessingSession (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern VTStatus VTRAWProcessingSessionCreate (
			IntPtr /* CM_NULLABLE CFAllocatorRef */ allocator,
			IntPtr /* CMVideoFormatDescriptionRef */ formatDescription,
			IntPtr /* CM_NULLABLE CFDictionaryRef */ outputPixelBufferAttributes,
			IntPtr /* CM_NULLABLE CFDictionaryRef */ processingSessionOptions,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CM_NULLABLE VTRAWProcessingSessionRef * CM_NONNULL */ processingSessionOut
		);

		/// <summary>Create a new <see cref="VTRawProcessingSession" /> instance.</summary>
		/// <param name="formatDescription">This format description corresponding to the original media samples.</param>
		/// <param name="outputPixelBufferAttributes">An optional CoreVideo pixel buffer dictionary.</param>
		/// <param name="processingSessionOptions">An optional dictionary of creation options.</param>
		/// <param name="error">An error code if the operation was unsuccessful, otherwise <see cref="VTStatus.Ok" />.</param>
		/// <returns>A new <see cref="VTRawProcessingSession" /> instance, or null in case of failure. See the <paramref name="error" /> parameter for the error code.</returns>
		public static VTRawProcessingSession? Create (CMVideoFormatDescription formatDescription, NSDictionary? outputPixelBufferAttributes, NSDictionary? processingSessionOptions, out VTStatus error)
		{
			IntPtr handle;
			unsafe {
				error = VTRAWProcessingSessionCreate (IntPtr.Zero, formatDescription.GetNonNullHandle (nameof (formatDescription)), outputPixelBufferAttributes.GetHandle (), processingSessionOptions.GetHandle (), &handle);
			}
			if (handle != IntPtr.Zero && error == VTStatus.Ok)
				return new VTRawProcessingSession (handle, owns: true);
			return null;
		}

		/// <summary>Create a new <see cref="VTRawProcessingSession" /> instance.</summary>
		/// <param name="formatDescription">This format description corresponding to the original media samples.</param>
		/// <param name="outputPixelBufferAttributes">An optional CoreVideo pixel buffer dictionary.</param>
		/// <param name="processingSessionOptions">An optional dictionary of creation options.</param>
		/// <param name="error">An error code if the operation was unsuccessful, otherwise <see cref="VTStatus.Ok" />.</param>
		/// <returns>A new <see cref="VTRawProcessingSession" /> instance, or null in case of failure. See the <paramref name="error" /> parameter for the error code.</returns>
		public static VTRawProcessingSession? Create (CMVideoFormatDescription formatDescription, CVPixelBufferAttributes? outputPixelBufferAttributes, VTRawProcessingParameters? processingSessionOptions, out VTStatus error)
		{
			return Create (formatDescription, outputPixelBufferAttributes?.Dictionary, processingSessionOptions?.Dictionary, out error);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		static extern void VTRAWProcessingSessionInvalidate (IntPtr /* VTRAWProcessingSessionRef */ session);

		/// <summary>Invalidate this processing session.</summary>
		/// <remarks>
		///    <para>The session will automatically be invalidated when its retain count reaches zero, but because sessions can be retained internally in numerous places, it can be hard to predict when it will happen.</para>
		///    <para>Calling Invalidate manually will ensure a deterministic and orderly teardown.</para>
		///    <para><c>Invalidate</c> is also called automatically from <see cref="Dispose" />.</para>
		/// </remarks>
		public void Invalidate ()
		{
			VTRAWProcessingSessionInvalidate (GetCheckedHandle ());
		}

		/// <summary>Dispose of this instance.</summary>
		/// <remarks>This will also call <see cref="Invalidate" />.</remarks>
		protected override void Dispose (bool disposing)
		{
			if (disposing)
				Invalidate ();
			base.Dispose (disposing);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		static extern nint VTRAWProcessingSessionGetTypeID ();

		/// <summary>Get this type's CFTypeID.</summary>
		public static nint GetTypeId ()
		{
			return VTRAWProcessingSessionGetTypeID ();
		}

#if NET
		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern VTStatus VTRAWProcessingSessionSetParameterChangedHander (
			IntPtr /* VTRAWProcessingSessionRef */ session,
			BlockLiteral* /* VTRAWProcessingParameterChangeHandler */ parameterChangeHandler
			);

		/// <summary>Provide a callback that will be called when the VTRawProcessingPlugin changes the set of processing parameters.</summary>
		/// <param name="handler">The callback that will be called. Set to null to remove the current handler.</param>
		public unsafe VTStatus SetParameterChangedHandler (VTRawProcessingParameterChangeHandler? handler)
		{
			if (handler is null) {
				return VTRAWProcessingSessionSetParameterChangedHander (GetCheckedHandle (), null);
			} else {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &VTRawProcessingParameterChangeHandlerCallback;
				using var block = new BlockLiteral (trampoline, handler, typeof (VTRawProcessingSession), nameof (VTRawProcessingParameterChangeHandlerCallback));
				return VTRAWProcessingSessionSetParameterChangedHander (GetCheckedHandle (), &block);
			}
		}

		[UnmanagedCallersOnly]
		static void VTRawProcessingParameterChangeHandlerCallback (IntPtr block, IntPtr newParameters)
		{
			var del = BlockLiteral.GetTarget<VTRawProcessingParameterChangeHandler> (block);
			if (del is not null) {
				var newParams = NSArray.ArrayFromHandle<NSObject> (newParameters);
				del (newParams);
			}
		}
#endif


#if NET
		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern VTStatus VTRAWProcessingSessionProcessFrame (
			IntPtr /* VTRAWProcessingSessionRef */ session,
			IntPtr /* CVPixelBufferRef */ inputPixelBuffer,
			IntPtr /* CM_NULLABLE CFDictionaryRef */ frameOptions,
			BlockLiteral* /* VTRAWProcessingOutputHandler */ outputHandler
			);

		/// <summary>Use this function to submit RAW frames for processing using sequence and frame level parameters.</summary>
		/// <param name="inputPixelBuffer">The input video frame to process.</param>
		/// <param name="frameOptions">An optional dictionary of options.</param>
		/// <param name="handler">The callback that will be called when processing is complete.</param>
		public unsafe VTStatus ProcessFrame (CVPixelBuffer inputPixelBuffer, NSDictionary? frameOptions, VTRawProcessingOutputHandler handler)
		{
			delegate* unmanaged<IntPtr, VTStatus, IntPtr, void> trampoline = &VTRawProcessingOutputHandlerCallback;
			using var block = new BlockLiteral (trampoline, handler, typeof (VTRawProcessingSession), nameof (VTRawProcessingOutputHandlerCallback));
			return VTRAWProcessingSessionProcessFrame (GetCheckedHandle (), inputPixelBuffer.GetNonNullHandle (nameof (inputPixelBuffer)), frameOptions.GetHandle (), &block);
		}
#endif

		[UnmanagedCallersOnly]
		static void VTRawProcessingOutputHandlerCallback (IntPtr block, VTStatus status, IntPtr processedPixelBuffer)
		{
			var del = BlockLiteral.GetTarget<VTRawProcessingOutputHandler> (block);
			if (del is not null) {
				var pb = Runtime.GetINativeObject<CVPixelBuffer> (processedPixelBuffer, owns: false);
				del (status, pb);
			}
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		static extern VTStatus VTRAWProcessingSessionCompleteFrames (IntPtr /* VTRAWProcessingSessionRef */ session);

		/// <summary>Force the RAW processor to complete processing frames.</summary>
		public void CompleteFrames ()
		{
			VTRAWProcessingSessionCompleteFrames (GetCheckedHandle ());
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern VTStatus VTRAWProcessingSessionCopyProcessingParameters (
			IntPtr /* VTRAWProcessingSessionRef */ session,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CM_NULLABLE CFArrayRef * CM_NONNULL */ outParameterArray
		);

		/// <summary>Get an array of dictionaries describing the parameters provided by the RAW processor for frame processing.</summary>
		/// <param name="status">An error code if the operation was unsuccessful, otherwise <see cref="VTStatus.Ok" />.</param>
		/// <returns>An array of dictionaries describing the parameters, or null in case of failure.</returns>
		/// <remarks>Use <see cref="ProcessingParameters" /> to get an array of strongly typed dictionaries.</remarks>
		public NSDictionary []? CopyProcessingParameters (out VTStatus status)
		{
			IntPtr handle;
			unsafe {
				status = VTRAWProcessingSessionCopyProcessingParameters (GetCheckedHandle (), &handle);
			}
			if (status == VTStatus.Ok && handle != IntPtr.Zero) {
				var rv = NSArray.ArrayFromHandle<NSDictionary> (handle)!;
				NSObject.DangerousRelease (handle); // owns: true
				return rv;
			}
			return null;
		}

		/// <summary>Get an array of strongly typed dictionaries describing the parameters provided by the RAW processor for frame processing.</summary>
		/// <returns>An array of strongly typed dictionaries describing the parameters, or null in case of failure.</returns>
		/// <remarks>Use <see cref="CopyProcessingParameters" /> to get an array of weakly typed dictionaries, or any error codes in case of failure.</remarks>
		public VTRawProcessingParameters []? ProcessingParameters {
			get {
				var dictionaries = CopyProcessingParameters (out var _);
				if (dictionaries is null)
					return null;
				var rv = new VTRawProcessingParameters [dictionaries.Length];
				for (var i = 0; i < rv.Length; i++)
					rv [i] = new VTRawProcessingParameters (dictionaries [i]);

				return rv;
			}
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		static extern VTStatus VTRAWProcessingSessionSetProcessingParameters (
			IntPtr /* VTRAWProcessingSessionRef */ session,
			IntPtr /* CFDictionaryRef */ processingParameters
		);

		/// <summary>Set RAW Processing parameters.</summary>
		/// <param name="processingParameters">The parameters to set.</param>
		/// <returns>An error code if the operation was unsuccessful, otherwise <see cref="VTStatus.Ok" />.</returns>
		public VTStatus SetProcessingParameters (NSDictionary processingParameters)
		{
			return VTRAWProcessingSessionSetProcessingParameters (GetCheckedHandle (), processingParameters.GetNonNullHandle (nameof (processingParameters)));
		}

		/// <summary>Set RAW Processing parameters.</summary>
		/// <param name="processingParameters">The parameters to set.</param>
		/// <returns>An error code if the operation was unsuccessful, otherwise <see cref="VTStatus.Ok" />.</returns>
		public VTStatus SetProcessingParameters (VTRawProcessingParameters processingParameters)
		{
			return SetProcessingParameters (processingParameters.Dictionary);
		}
	}
}
#endif // MONOMAC
