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

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace OpenGLES {

#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("tvos12.0", "Use 'Metal' instead.")]
	[ObsoletedOSPlatform ("ios12.0", "Use 'Metal' instead.")]
#else
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
#endif
	public static class EAGLDrawableProperty {
		public static readonly NSString ColorFormat;
		public static readonly NSString RetainedBacking;

		static EAGLDrawableProperty ()
		{
			var handle = Libraries.OpenGLES.Handle;
			ColorFormat = Dlfcn.GetStringConstant (handle,
					"kEAGLDrawablePropertyColorFormat");
			RetainedBacking = Dlfcn.GetStringConstant (handle,
					"kEAGLDrawablePropertyRetainedBacking");
		}
	}

#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("tvos12.0", "Use 'Metal' instead.")]
	[ObsoletedOSPlatform ("ios12.0", "Use 'Metal' instead.")]
#else
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
#endif
	public static class EAGLColorFormat {
		public static readonly NSString RGB565;
		public static readonly NSString RGBA8;

		static EAGLColorFormat ()
		{
			var handle = Libraries.OpenGLES.Handle;
			RGB565 = Dlfcn.GetStringConstant (handle, "kEAGLColorFormatRGB565");
			RGBA8 = Dlfcn.GetStringConstant (handle, "kEAGLColorFormatRGBA8");
		}
	}
}
