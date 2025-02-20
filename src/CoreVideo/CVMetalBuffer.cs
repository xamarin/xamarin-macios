using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using CoreGraphics;
using Foundation;
using Metal;
using ObjCRuntime;

#nullable enable

namespace CoreVideo {

	/// <summary>A CVPixelBuffer wrapped in a Metal based buffer.</summary>
	/// <remarks>This type is used to provide buffers to Metal.</remarks>
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
	public class CVMetalBuffer : CVBuffer {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CVMetalBuffer (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFTypeID */ nint CVMetalBufferCacheGetTypeID ();

		public static nint GetTypeId ()
		{
			return CVMetalBufferCacheGetTypeID ();
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* id<MTLBuffer> CV_NULLABLE */ IntPtr CVMetalBufferGetBuffer (IntPtr /* CVMetalBufferRef CV_NONNULL */ buffer);

		/// <summary>Retrieve the Metal MTLBuffer for the CVMetalBuffer.</summary>
		public IMTLBuffer? GetMetalBuffer ()
		{
			return Runtime.GetINativeObject<IMTLBuffer> (CVMetalBufferGetBuffer (GetCheckedHandle ()), owns: false);
		}

#endif // !COREBUILD
	}
}
