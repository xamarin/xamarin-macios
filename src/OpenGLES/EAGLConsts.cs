//
// Author:
//   Jonathan Pryor:
//
// (C) 2009 Novell, Inc.
//
using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace OpenGLES {

	[Deprecated (PlatformName.iOS, 12,0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12,0, message: "Use 'Metal' instead.")]
	public static class EAGLDrawableProperty {
		public static readonly NSString ColorFormat;
		public static readonly NSString RetainedBacking;

		static EAGLDrawableProperty ()
		{
			var handle = Dlfcn.dlopen (Constants.OpenGLESLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				ColorFormat     = Dlfcn.GetStringConstant (handle, 
						"kEAGLDrawablePropertyColorFormat");
				RetainedBacking = Dlfcn.GetStringConstant (handle, 
						"kEAGLDrawablePropertyRetainedBacking");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}
	}

	[Deprecated (PlatformName.iOS, 12,0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12,0, message: "Use 'Metal' instead.")]
	public static class EAGLColorFormat {
		public static readonly NSString RGB565;
		public static readonly NSString RGBA8;

		static EAGLColorFormat ()
		{
			var handle = Dlfcn.dlopen (Constants.OpenGLESLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				RGB565  = Dlfcn.GetStringConstant (handle, "kEAGLColorFormatRGB565");
				RGBA8   = Dlfcn.GetStringConstant (handle, "kEAGLColorFormatRGBA8");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}
	}
}

