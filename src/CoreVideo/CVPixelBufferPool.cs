// 
// CVPixelBufferPool.cs: Implements the managed CVPixelBufferPool
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2010 Novell, Inc
// Copyright 2012-2014 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CoreVideo {

	// CVPixelBufferPool.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class CVPixelBufferPool : NativeObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CVPixelBufferPool (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVPixelBufferPoolRelease (/* CVPixelBufferPoolRef __nullable */ IntPtr handle);

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CVPixelBufferPoolRef __nullable */ IntPtr CVPixelBufferPoolRetain (
			/* CVPixelBufferPoolRef __nullable */ IntPtr handle);

		protected internal override void Retain ()
		{
			CVPixelBufferPoolRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CVPixelBufferPoolRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static nint CVPixelBufferPoolGetTypeID ();
		public nint TypeID {
			get {
				return CVPixelBufferPoolGetTypeID ();
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CVPixelBufferPoolGetPixelBufferAttributes (
			/* CVPixelBufferPoolRef __nonnull */ IntPtr pool);

		// TODO: Return type is CVPixelBufferAttributes but need different name when this one is not WeakXXXX
		public NSDictionary? PixelBufferAttributes {
			get {
				return Runtime.GetNSObject<NSDictionary> (CVPixelBufferPoolGetPixelBufferAttributes (Handle));
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CVPixelBufferPoolGetAttributes (
			/* CVPixelBufferPoolRef __nonnull */ IntPtr pool);

		public NSDictionary? Attributes {
			get {
				return Runtime.GetNSObject<NSDictionary> (CVPixelBufferPoolGetAttributes (Handle));
			}
		}

		public CVPixelBufferPoolSettings? Settings {
			get {
				var attr = Attributes;
				return attr is null ? null : new CVPixelBufferPoolSettings (attr);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferPoolCreatePixelBuffer (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CVPixelBufferPoolRef __nonnull */ IntPtr pixelBufferPool,
			/* CVPixelBufferRef  __nullable * __nonnull */ out IntPtr pixelBufferOut);

		public CVPixelBuffer CreatePixelBuffer ()
		{
			var ret = CVPixelBufferPoolCreatePixelBuffer (IntPtr.Zero, Handle, out var pixelBufferOut);

			if (ret != CVReturn.Success)
				throw new Exception ("CVPixelBufferPoolCreatePixelBuffer returned " + ret.ToString ());

			return new CVPixelBuffer (pixelBufferOut, true);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferPoolCreatePixelBufferWithAuxAttributes (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CVPixelBufferPoolRef __nonnull */ IntPtr pixelBufferPool,
			/* CFDictionaryRef __nullable */ IntPtr auxAttributes,
			/* CVPixelBufferRef  __nullable * __nonnull */ out IntPtr pixelBufferOut);

		public CVPixelBuffer? CreatePixelBuffer (CVPixelBufferPoolAllocationSettings? allocationSettings, out CVReturn error)
		{
			error = CVPixelBufferPoolCreatePixelBufferWithAuxAttributes (IntPtr.Zero, Handle, allocationSettings.GetHandle (), out var pb);
			if (error != CVReturn.Success)
				return null;

			return new CVPixelBuffer (pb, true);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferPoolCreate (/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CFDictionaryRef __nullable */ IntPtr poolAttributes,
			/* CFDictionaryRef __nullable */ IntPtr pixelBufferAttributes,
			/* CVPixelBufferPoolRef  __nullable * __nonnull */ out IntPtr poolOut);

		static IntPtr Create (NSDictionary? poolAttributes, NSDictionary? pixelBufferAttributes)
		{
			var ret = CVPixelBufferPoolCreate (IntPtr.Zero, poolAttributes.GetHandle (), pixelBufferAttributes.GetHandle (), out var handle);

			if (ret != CVReturn.Success)
				throw new Exception ("CVPixelBufferPoolCreate returned " + ret.ToString ());

			return handle;
		}

		[Advice ("Use overload with CVPixelBufferPoolSettings")]
		public CVPixelBufferPool (NSDictionary? poolAttributes, NSDictionary? pixelBufferAttributes)
			: base (Create (poolAttributes, pixelBufferAttributes), true)
		{
		}

		public CVPixelBufferPool (CVPixelBufferPoolSettings? settings, CVPixelBufferAttributes? pixelBufferAttributes)
			: this (settings?.GetDictionary (), pixelBufferAttributes?.GetDictionary ())
		{
		}


#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		static extern void CVPixelBufferPoolFlush (/* CVPixelBufferPoolRef __nonnull */ IntPtr pool,
			CVPixelBufferPoolFlushFlags options);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public void Flush (CVPixelBufferPoolFlushFlags options)
		{
			CVPixelBufferPoolFlush (Handle, options);
		}

#endif // !COREBUILD
	}
}
