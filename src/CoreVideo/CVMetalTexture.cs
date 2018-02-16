//
// CVMetalTexture.cs: Implementation of the CVMetalTexture class
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2015 Xamarin Inc
//
//

#if IOS || TVOS

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;
using Metal;

namespace CoreVideo {

	[iOS (8,0)]
	public class CVMetalTexture : INativeObject, IDisposable {

		internal IntPtr handle;

		public IntPtr Handle {
			get { return handle; }
		}

		~CVMetalTexture ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected
		virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		internal CVMetalTexture (IntPtr handle)
		{
			this.handle = handle;
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* id<MTLTexture> __nullable */ IntPtr CVMetalTextureGetTexture (
			/* CVMetalTextureRef __nonnull */ IntPtr image);

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* Boolean */ bool CVMetalTextureIsFlipped (/* CVMetalTextureRef __nonnull */ IntPtr image);

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVMetalTextureGetCleanTexCoords (/* CVMetalTextureRef __nonnull */ IntPtr image, 
			/* float[2] */ IntPtr lowerLeft, /* float[2] */ IntPtr lowerRight, /* float[2] */ IntPtr upperRight, 
			/* float[2] */ IntPtr upperLeft);

		public IMTLTexture Texture {
			get {
				return Runtime.GetINativeObject<IMTLTexture> (CVMetalTextureGetTexture (handle), owns: false);
			}
		}
			
		public bool IsFlipped {
			get {
				return CVMetalTextureIsFlipped (handle);
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
					CVMetalTextureGetCleanTexCoords (handle, (IntPtr) ll, (IntPtr) lr, (IntPtr) ur, (IntPtr) ul);
				}
			}
		}
	}
}

#endif // IOS || TVOS

