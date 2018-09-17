//
// CVOpenGLESTexture.cs: Implementation of the CVOpenGLESTexture class
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2015 Xamarin Inc
//
//

#if !WATCH

using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;

#if XAMCORE_2_0
using ObjCRuntime;
using CoreFoundation;
using Foundation;
using OpenGLES;

namespace CoreVideo {
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;

namespace MonoTouch.CoreVideo {
#endif

	// CVOpenGLESTextureCache.h
	[Deprecated (PlatformName.iOS, 12,0, message: "Use 'CVMetalTextureCache' instead.")]
	[Deprecated (PlatformName.TvOS, 12,0, message: "Use 'CVMetalTextureCache' instead.")]
	public class CVOpenGLESTextureCache : INativeObject, IDisposable {
		internal IntPtr handle;

		public IntPtr Handle {
			get { return handle; }
		}

		~CVOpenGLESTextureCache ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

#if XAMCORE_2_0
		protected
#else
		public
#endif
		virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CVOpenGLESTexture.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static int CVOpenGLESTextureCacheCreate (
			/* CFAllocatorRef __nullable */ IntPtr allocator, 
			/* CFDictionaryRef __nullable */ IntPtr cacheAttributes,
			/* CVEAGLContext __nonnull */ IntPtr eaglContext, 
			/* CFDictionaryRef __nullable */ IntPtr textureAttextureAttributestr, 
			/* CVOpenGLESTextureCacheRef __nullable * __nonnull */ out IntPtr cacheOut);

		CVOpenGLESTextureCache (IntPtr handle)
		{
			this.handle = handle;
		}
		
		public CVOpenGLESTextureCache (EAGLContext context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");
			
			if (CVOpenGLESTextureCacheCreate (IntPtr.Zero,
							  IntPtr.Zero, /* change one day to support cache attributes */
							  context.Handle,
							  IntPtr.Zero, /* change one day to support texture attributes */
							  out handle) == 0)
				return;
			
			throw new Exception ("Could not create the texture cache");
		}

		public static CVOpenGLESTextureCache FromEAGLContext (EAGLContext context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");
			IntPtr handle;
			if (CVOpenGLESTextureCacheCreate (IntPtr.Zero,
							  IntPtr.Zero, /* change one day to support cache attributes */
							  context.Handle,
							  IntPtr.Zero, /* change one day to support texture attribuets */
							  out handle) == 0)
				return new CVOpenGLESTextureCache (handle);
			return null;
		}

		public CVOpenGLESTexture TextureFromImage (CVImageBuffer imageBuffer, bool isTexture2d, OpenTK.Graphics.ES20.All internalFormat, int width, int height, OpenTK.Graphics.ES20.All pixelFormat, OpenTK.Graphics.ES20.DataType pixelType, int planeIndex, out CVReturn errorCode)
		{
			if (imageBuffer == null)
				throw new ArgumentNullException ("imageBuffer");
			
			int target = isTexture2d ? 0x0DE1 /* GL_TEXTURE_2D */ : 0x8D41 /* GL_RENDERBUFFER */;
			IntPtr texture;
			errorCode = CVOpenGLESTextureCacheCreateTextureFromImage (
				IntPtr.Zero,
				handle, /* textureCache dict, one day we might add it */
				imageBuffer.Handle,
				IntPtr.Zero,
				target,
				internalFormat, width, height, pixelFormat,
				pixelType, (IntPtr) planeIndex, out texture);
			if (errorCode != 0)
				return null;
			return new CVOpenGLESTexture (texture);
		}
		
#if OPENTK_1_1
		public CVOpenGLESTexture TextureFromImage (CVImageBuffer imageBuffer, bool isTexture2d, OpenTK.Graphics.ES30.All internalFormat, int width, int height, OpenTK.Graphics.ES30.All pixelFormat, OpenTK.Graphics.ES30.DataType pixelType, int planeIndex, out CVReturn errorCode)
		{
			return TextureFromImage (imageBuffer, isTexture2d, (OpenTK.Graphics.ES20.All) internalFormat, width, height, (OpenTK.Graphics.ES20.All) pixelFormat, (OpenTK.Graphics.ES20.DataType) pixelType, planeIndex, out errorCode);
		}
#endif

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVOpenGLESTextureCacheFlush (
			/* CVOpenGLESTextureCacheRef __nonnull */ IntPtr textureCache, CVOptionFlags flags);

		public void Flush (CVOptionFlags flags)
		{
			CVOpenGLESTextureCacheFlush (handle, flags);
		}
			
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVOpenGLESTextureCacheCreateTextureFromImage (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CVOpenGLESTextureCacheRef */ IntPtr textureCache,
			/* CVImageBufferRef __nonnull */ IntPtr sourceImage,
			/* CFDictionaryRef __nullable */ IntPtr textureAttr,
			/* GLenum */ int target,
			/* GLint */ OpenTK.Graphics.ES20.All internalFormat,
			/* GLsizei */ int width,
			/* GLsizei */ int height,
			/* GLenum */ OpenTK.Graphics.ES20.All format,
			/* GLenum */ OpenTK.Graphics.ES20.DataType type,
			/* size_t */ IntPtr planeIndex,
			/* CVOpenGLESTextureRef __nullable * __nonnull */ out IntPtr textureOut);
	}
}

#endif
