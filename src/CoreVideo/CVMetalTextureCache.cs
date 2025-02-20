//
// CVMetalTextureCache.cs: Implementation of the CVMetalTextureCache class
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2015 Xamarin Inc
//
//

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;
using Metal;

#nullable enable

namespace CoreVideo {

	public partial class CVMetalTextureCache : NativeObject {
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static CVReturn /* CVReturn = int32_t */ CVMetalTextureCacheCreate (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CFDictionaryRef __nullable */ IntPtr cacheAttributes,
			/* id<MTLDevice> __nonnull */ IntPtr metalDevice,
			/* CFDictionaryRef __nullable */ IntPtr textureAttributes,
			/* CVMetalTextureCacheRef __nullable * __nonnull */ IntPtr* cacheOut);

		[Preserve (Conditional = true)]
		internal CVMetalTextureCache (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		static IntPtr Create (IMTLDevice metalDevice, CVMetalTextureAttributes? textureAttributes)
		{
			if (metalDevice is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (metalDevice));

			IntPtr handle;
			CVReturn err;
			unsafe {
				err = CVMetalTextureCacheCreate (IntPtr.Zero,
								IntPtr.Zero, /* change one day to support cache attributes */
								metalDevice.Handle,
								textureAttributes.GetHandle (),
								&handle);
			}
			if (err == CVReturn.Success)
				return handle;

			throw new Exception ($"Could not create the texture cache, Reason: {err}.");
		}

		public CVMetalTextureCache (IMTLDevice metalDevice)
			: base (Create (metalDevice, null), true)
		{
		}

		public static CVMetalTextureCache? FromDevice (IMTLDevice metalDevice)
		{
			if (metalDevice is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (metalDevice));
			IntPtr handle;
			CVReturn err;
			unsafe {
				err = CVMetalTextureCacheCreate (IntPtr.Zero,
							   IntPtr.Zero, /* change one day to support cache attributes */
							   metalDevice.Handle,
							   IntPtr.Zero, /* change one day to support texture attribuets */
							   &handle);
			}
			if (err == 0)
				return new CVMetalTextureCache (handle, true);
			return null;
		}

		public CVMetalTextureCache (IMTLDevice metalDevice, CVMetalTextureAttributes textureAttributes)
			: base (Create (metalDevice, textureAttributes), true)
		{
		}

		public static CVMetalTextureCache? FromDevice (IMTLDevice metalDevice, CVMetalTextureAttributes? textureAttributes, out CVReturn creationErr)
		{
			if (metalDevice is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (metalDevice));
			IntPtr handle;
			unsafe {
				creationErr = CVMetalTextureCacheCreate (IntPtr.Zero,
								IntPtr.Zero, /* change one day to support cache attributes */
								metalDevice.Handle,
								textureAttributes.GetHandle (),
								&handle);
			}
			if (creationErr == CVReturn.Success)
				return new CVMetalTextureCache (handle, true);
			return null;
		}

		public static CVMetalTextureCache? FromDevice (IMTLDevice metalDevice, CVMetalTextureAttributes textureAttributes)
		{
			CVReturn creationErr;
			return FromDevice (metalDevice, textureAttributes, out creationErr);
		}

		public CVMetalTexture? TextureFromImage (CVImageBuffer imageBuffer, MTLPixelFormat format, nint width, nint height, nint planeIndex, out CVReturn errorCode)
		{
			if (imageBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (imageBuffer));

			IntPtr texture;
			unsafe {
				errorCode = CVMetalTextureCacheCreateTextureFromImage (
					allocator: IntPtr.Zero,
					textureCache: Handle, /* textureCache dict, one day we might add it */
					sourceImage: imageBuffer.Handle,
					textureAttr: IntPtr.Zero,
					format: (nuint) (ulong) format,
					width: width,
					height: height,
					planeIndex: planeIndex,
					textureOut: &texture);
			}
			if (errorCode != 0)
				return null;
			return new CVMetalTexture (texture, true);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVMetalTextureCacheFlush (
			/* CVMetalTextureCacheRef __nonnull */ IntPtr textureCache, CVOptionFlags flags);

		public void Flush (CVOptionFlags flags)
		{
			CVMetalTextureCacheFlush (Handle, flags);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static CVReturn CVMetalTextureCacheCreateTextureFromImage (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CVMetalTextureCacheRef __nonnull */ IntPtr textureCache,
			/* CVImageBufferRef __nonnull */ IntPtr sourceImage,
			/* CFDictionaryRef __nullable */ IntPtr textureAttr,
			/* MTLPixelFormat */ nuint format,  // MTLPixelFormat is nuint [Native] which will always be 64bits on managed code
			/* size_t */ nint width,
			/* size_t */ nint height,
			/* size_t */ nint planeIndex,
			/* CVMetalTextureRef __nullable * __nonnull */ IntPtr* textureOut);
	}
}
