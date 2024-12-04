#if !WATCH
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using CoreFoundation;
using CoreGraphics;
using Foundation;
using Metal;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CoreVideo {

	/// <summary>A cache used to manage <see cref="CVMetalBuffer" /> instances.</summary>
#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
#else
	[NoWatch, TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	public class CVMetalBufferCache : NativeObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CVMetalBufferCache (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFTypeID */ nint CVMetalBufferGetTypeID ();

		public static nint GetTypeId ()
		{
			return CVMetalBufferGetTypeID ();
		}

		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static /* CVReturn */ CVReturn CVMetalBufferCacheCreate (
			IntPtr /* CFAllocatorRef CV_NULLABLE */ allocator,
			IntPtr /* CFDictionaryRef CV_NULLABLE */ cacheAttributes,
			IntPtr /* id<MTLDevice> CV_NONNULL */ metalDevice,
			IntPtr* /* CV_RETURNS_RETAINED_PARAMETER CVMetalBufferCacheRef CV_NULLABLE * CV_NONNULL */ cacheOut
		);

		static IntPtr Create (IMTLDevice device, NSDictionary? attributes)
		{
			if (!TryCreateHandle (device, attributes, out IntPtr handle, out var res))
				throw new Exception ($"Could not create CVMetalBufferCache, CVMetalBufferCacheCreate returned: {res}");
			return handle;
		}

		/// <summary>Try to create a new <see cref="CVMetalBufferCache" /> instance.</summary>
		/// <param name="device">The Metal device to create the <see cref="CVMetalBufferCache" /> instance for.</param>
		/// <param name="attributes">An optional dictionary of attributes to apply to the cache.</param>
		/// <param name="metalBufferCache">The new <see cref="CVMetalBufferCache" /> instance, if successful, null otherwise.</param>
		/// <param name="status">An error code if failed, or <see cref="CVReturn.Success" /> if successful.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public static bool TryCreate (IMTLDevice device, NSDictionary? attributes, [NotNullWhen (true)] out CVMetalBufferCache? metalBufferCache, out CVReturn status)
		{
			var rv = TryCreateHandle (device, attributes, out IntPtr handle, out status);
			if (rv) {
				metalBufferCache = new CVMetalBufferCache (handle, true);
			} else {
				metalBufferCache = null;
			}
			return rv;
		}

		/// <summary>Try to create a new <see cref="CVMetalBufferCache" /> instance.</summary>
		/// <param name="device">The Metal device to create the <see cref="CVMetalBufferCache" /> instance for.</param>
		/// <param name="attributes">An optional dictionary of attributes to apply to the cache.</param>
		/// <param name="handle">The pointer to the new <see cref="CVMetalBufferCache" /> instance, if successful.</param>
		/// <param name="status">An error code if failed, or <see cref="CVReturn.Success" /> if successful.</param>
		/// <returns>True if successful, otherwise false.</returns>
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static bool TryCreateHandle (IMTLDevice device, NSDictionary? attributes, out IntPtr handle, out CVReturn status)
		{
			handle = IntPtr.Zero;
			unsafe {
				status = CVMetalBufferCacheCreate (IntPtr.Zero, attributes.GetHandle (), device.GetNonNullHandle (nameof (device)), (IntPtr *) Unsafe.AsPointer<IntPtr> (ref handle));
			}
			return status == CVReturn.Success;
		}

		/// <summary>Create a new <see cref="CVMetalBufferCache" /> instance.</summary>
		/// <param name="device">The Metal device to create the <see cref="CVMetalBufferCache" /> instance for.</param>
		/// <param name="attributes">An optional dictionary of attributes to apply to the cache.</param>
		public CVMetalBufferCache (IMTLDevice device, NSDictionary? attributes)
			: base (Create (device, attributes), owns: true)
		{
		}

		/// <summary>Create a new <see cref="CVMetalBufferCache" /> instance.</summary>
		/// <param name="device">The Metal device to create the <see cref="CVMetalBufferCache" /> instance for.</param>
		/// <param name="attributes">Optional attributes to apply to the cache.</param>
		public CVMetalBufferCache (IMTLDevice device, CVMetalBufferCacheAttributes? attributes)
			: this (device, attributes?.Dictionary)
		{
		}

		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static /* CVReturn */ CVReturn CVMetalBufferCacheCreateBufferFromImage (
			IntPtr /* CFAllocatorRef CV_NULLABLE */ allocator,
			IntPtr /* CVMetalBufferCacheRef CV_NONNULL */ bufferCache,
			IntPtr /* CVImageBufferRef CV_NONNULL */ imageBuffer,
			IntPtr* /* CV_RETURNS_RETAINED_PARAMETER CVMetalBufferRef CV_NULLABLE * CV_NONNULL */ bufferOut
		);

		/// <summary>Create a <see cref="CVMetalBuffer" /> for an existing <see cref="CVImageBuffer" />.</summary>
		/// <param name="imageBuffer">The image buffer to create the <see cref="CVMetalBuffer" /> from.</param>
		public CVMetalBuffer? CreateBufferFromImage (CVImageBuffer imageBuffer)
		{
			IntPtr handle;
			CVReturn res;
			unsafe {
				res = CVMetalBufferCacheCreateBufferFromImage (IntPtr.Zero, GetCheckedHandle (), imageBuffer.GetNonNullHandle (nameof (imageBuffer)), &handle);
			}
			if (res != CVReturn.Success)
				throw new Exception ($"Could not create CVMetalBuffer, CVMetalBufferCacheCreateBufferFromImage returned: {res}");
			return Runtime.GetINativeObject<CVMetalBuffer> (handle, true);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static /* CVReturn */ CVReturn CVMetalBufferCacheFlush (
			IntPtr /* CVMetalBufferCacheRef CV_NONNULL */ bufferCache,
			CVOptionFlags options
		);

		/// <summary>Perform internal housekeeping/recycling operations.</summary>
		/// <remarks>This method must be called periodically.</remarks>
		public void Flush ()
		{
			Flush (CVOptionFlags.None);
		}

		/// <summary>Perform internal housekeeping/recycling operations.</summary>
		/// <param name="options">Any flags for the flush operation. Currently unused, always pass <see cref="CVOptionFlags.None" />.</param>
		/// <remarks>This method must be called periodically.</remarks>
		public void Flush (CVOptionFlags options)
		{
			CVMetalBufferCacheFlush (GetCheckedHandle (), options);
		}
#endif // !COREBUILD
	}
}
#endif // !WATCH
