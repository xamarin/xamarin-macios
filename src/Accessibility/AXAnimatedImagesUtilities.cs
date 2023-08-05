#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace Accessibility {

#if NET
	[SupportedOSPlatform ("tvos17.0")]
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
	[SupportedOSPlatform ("macos14.0")]
#else
	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
#endif
	public static partial class AXAnimatedImagesUtilities {

		[DllImport (Constants.AccessibilityLibrary)]
		static extern bool AXAnimatedImagesEnabled ();

		public static bool Enabled => AXAnimatedImagesEnabled ();
	}
}
