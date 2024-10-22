using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace OpenGLES {
	public partial class EAGLContext {
		public enum PresentationMode {
			AtTime = 0,
			AfterMinimumDuration = 1,
		}

		[DllImport (Constants.OpenGLESLibrary)]
		unsafe extern static void EAGLGetVersion (nuint* major, nuint* minor);

		public unsafe static void EAGLGetVersion (out nuint major, out nuint minor)
		{
			major = default;
			minor = default;
			EAGLGetVersion ((nuint*) Unsafe.AsPointer<nuint> (ref major), (nuint*) Unsafe.AsPointer<nuint> (ref minor));
		}

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
