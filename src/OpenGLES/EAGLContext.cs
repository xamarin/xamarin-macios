using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace OpenGLES
{
	public partial class EAGLContext
	{
		public enum PresentationMode {
			AtTime = 0,
			AfterMinimumDuration = 1,
		}

		[DllImport (Constants.OpenGLESLibrary)]
		public extern static void EAGLGetVersion (out nuint major, out nuint minor);

#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public EAGLContext ()
		{
		}
#endif

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos12.0")]
		[UnsupportedOSPlatform ("ios12.0")]
#if TVOS
		[Obsolete ("Starting with tvos12.0 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios12.0 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[iOS (10,0)]
		[TV (10,0)]
#endif
		public virtual bool PresentRenderBuffer (nuint target, double presentationTime)
		{
			return _PresentRenderbufferAtTime (target, presentationTime);
		}

#if NET
		[SupportedOSPlatform ("ios10.3")]
		[SupportedOSPlatform ("tvos10.2")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos12.0")]
		[UnsupportedOSPlatform ("ios12.0")]
#if TVOS
		[Obsolete ("Starting with tvos12.0 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios12.0 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[iOS (10,3)]
		[TV (10,2)]
#endif
		public virtual bool PresentRenderBuffer (nuint target, double presentationTime, PresentationMode mode)
		{
			switch (mode) {
			case PresentationMode.AtTime:
				return _PresentRenderbufferAtTime (target, presentationTime);
			case PresentationMode.AfterMinimumDuration:
				return _PresentRenderbufferAfterMinimumDuration (target, presentationTime);
			default:
				throw new ArgumentOutOfRangeException ($"Unknown presentation mode: {mode}", nameof (mode));
			}
		}
	}
}
