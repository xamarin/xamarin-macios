#if !WATCH && NET

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using CoreVideo;
using Foundation;
using ObjCRuntime;

namespace VideoToolbox {

	/// <summary>This class can be used to perform HDR Per Frame Metadata Generation.</summary>
#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
#else
	[NoWatch, TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	public class VTHdrPerFrameMetadataGenerationSession : NativeObject {
		[Preserve (Conditional = true)]
		protected VTHdrPerFrameMetadataGenerationSession (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		static extern nint VTHDRPerFrameMetadataGenerationSessionGetTypeID ();

		/// <summary>Get this type's CFTypeID.</summary>
		public static nint GetTypeId ()
		{
			return VTHDRPerFrameMetadataGenerationSessionGetTypeID ();
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern int /* OSStatus */ VTHDRPerFrameMetadataGenerationSessionCreate (
			IntPtr /* CM_NULLABLE CFAllocatorRef */ allocator,
			float framesPerSecond,
			IntPtr /* CM_NULLABLE CFDictionaryRef */ options,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CM_NULLABLE VTHDRPerFrameMetadataGenerationSessionRef * CM_NONNULL */ hdrPerFrameMetadataGenerationSessionOut
		);

		/// <summary>Create a new <see cref="VTHdrPerFrameMetadataGenerationSession" /> instance.</summary>
		/// <param name="framesPerSecond">This value must be greater than 0.0.</param>
		/// <param name="options">An optional dictionary of options to use.</param>
		/// <param name="error">An error code if the operation was unsuccessful, otherwise <see cref="VTStatus.Ok" />.</param>
		/// <returns>A new <see cref="VTHdrPerFrameMetadataGenerationSession" /> instance, or null in case of failure. See the <paramref name="error" /> parameter for the error code.</returns>
		public static VTHdrPerFrameMetadataGenerationSession? Create (float framesPerSecond, NSDictionary? options, out VTStatus error)
		{
			IntPtr handle;
			unsafe {
				error = (VTStatus) VTHDRPerFrameMetadataGenerationSessionCreate (IntPtr.Zero, framesPerSecond, options.GetHandle (), &handle);
			}
			if (error == VTStatus.Ok && handle != IntPtr.Zero)
				return new VTHdrPerFrameMetadataGenerationSession (handle, owns: true);
			return null;
		}

		/// <summary>Create a new <see cref="VTHdrPerFrameMetadataGenerationSession" /> instance.</summary>
		/// <param name="framesPerSecond">This value must be greater than 0.0.</param>
		/// <param name="options">An optional dictionary of options to use.</param>
		/// <param name="error">An error code if the operation was unsuccessful, otherwise <see cref="VTStatus.Ok" />.</param>
		/// <returns>A new <see cref="VTHdrPerFrameMetadataGenerationSession" /> instance, or null in case of failure. See the <paramref name="error" /> parameter for the error code.</returns>
		public static VTHdrPerFrameMetadataGenerationSession? Create (float framesPerSecond, VTHdrPerFrameMetadataGenerationOptions? options, out VTStatus error)
		{
			return Create (framesPerSecond, options?.Dictionary, out error);
		}

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern VTStatus /* OSStatus */ VTHDRPerFrameMetadataGenerationSessionAttachMetadata (
			IntPtr /* VTHDRPerFrameMetadataGenerationSessionRef */ hdrPerFrameMetadataGenerationSession,
			IntPtr /* CVPixelBufferRef */ pixelBuffer,
			byte /* Boolean */ sceneChange
		);

		/// <summary>Analyze and attach per-frame HDR metadata to the <see cref="CVPixelBuffer" /> and its backing <see cref="IOSurface" />.</summary>
		/// <param name="pixelBuffer">The pixel buffer to attach the metadata to.</param>
		/// <param name="sceneChange">Set this value to true if this frame's brightness changed significantly compared to the previous frame (for example if going from an outdoor scene to an indoor scene).</param>
		/// <returns>An error code if the operation was unsuccessful, otherwise <see cref="VTStatus.Ok" />.</returns>
		public VTStatus AttachMetadata (CVPixelBuffer pixelBuffer, bool sceneChange)
		{
			return VTHDRPerFrameMetadataGenerationSessionAttachMetadata (GetCheckedHandle (), pixelBuffer.GetNonNullHandle (nameof (pixelBuffer)), sceneChange.AsByte ());
		}
	}
}

#endif // !WATCH
