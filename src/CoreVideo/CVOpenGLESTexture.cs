//
// CVOpenGLESTexture.cs: Implementation of the CVOpenGLESTexture class
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2015 Xamarin Inc
//
//

#if HAS_OPENGLES

using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;

using ObjCRuntime;
using CoreFoundation;
using Foundation;
using OpenGLES;

#nullable enable

namespace CoreVideo {

	// CVOpenGLESTexture.h
#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("tvos12.0", "Use 'CVMetalTexture' instead.")]
	[ObsoletedOSPlatform ("ios12.0", "Use 'CVMetalTexture' instead.")]
#else
	[Deprecated (PlatformName.iOS, 12,0, message: "Use 'CVMetalTexture' instead.")]
	[Deprecated (PlatformName.TvOS, 12,0, message: "Use 'CVMetalTexture' instead.")]
#endif
	public class CVOpenGLESTexture : INativeObject, IDisposable {

		internal IntPtr handle;

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static IntPtr CFRelease (IntPtr obj);

		public IntPtr Handle {
			get { return handle; }
		}

		~CVOpenGLESTexture ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		internal CVOpenGLESTexture (IntPtr handle)
		{
			this.handle = handle;
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* GLenum */ TextureTarget CVOpenGLESTextureGetTarget (
			/* CVOpenGLESTextureRef __nonnull */ IntPtr image);

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* GLuint */ int CVOpenGLESTextureGetName (
			/* CVOpenGLESTextureRef __nonnull */ IntPtr image);
		// note: kept int for compatibility

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* Boolean */ bool CVOpenGLESTextureIsFlipped (
			/* CVOpenGLESTextureRef __nonnull */ IntPtr image);

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVOpenGLESTextureGetCleanTexCoords (
			/* CVOpenGLESTextureRef __nonnull */ IntPtr image, 
			/* GLfloat[2] */ IntPtr lowerLeft, /* GLfloat[2] */ IntPtr lowerRight, /* GLfloat[2] */ IntPtr upperRight, 
			/* GLfloat[2] */ IntPtr upperLeft);
		// note: a GLfloat is 4 bytes even on 64bits iOS
		
		public TextureTarget Target {
			get {
				return CVOpenGLESTextureGetTarget (handle);
			}
		}

		public int Name {
			get {
				return CVOpenGLESTextureGetName (handle);
			}
		}

		public bool IsFlipped {
			get {
				return CVOpenGLESTextureIsFlipped (handle);
			}
		}

		public void GetCleanTexCoords (out float [] lowerLeft, out float [] lowerRight, out float [] upperRight, out float [] upperLeft)
		{
			lowerLeft = new float [2];
			lowerRight = new float [2];
			upperRight = new float [2];
			upperLeft = new float [2];

			unsafe {
				fixed (float *ll = &lowerLeft[0], lr = &lowerRight [0], ur = &upperRight [0], ul = &upperLeft[0]){
					CVOpenGLESTextureGetCleanTexCoords (handle, (IntPtr) ll, (IntPtr) lr, (IntPtr) ur, (IntPtr) ul);
				}
			}
		}
	}
	
}

#endif
