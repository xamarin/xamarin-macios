using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace OpenGLES {
	public partial class EAGLContext {
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
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("tvos12.0", "Use 'Metal' instead.")]
		[ObsoletedOSPlatform ("ios12.0", "Use 'Metal' instead.")]
#endif
		public virtual bool PresentRenderBuffer (nuint target, double presentationTime)
		{
			return _PresentRenderbufferAtTime (target, presentationTime);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("tvos12.0", "Use 'Metal' instead.")]
		[ObsoletedOSPlatform ("ios12.0", "Use 'Metal' instead.")]
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
