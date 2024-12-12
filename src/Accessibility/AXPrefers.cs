#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace Accessibility {

	// accessibility.cs already provide the following attributes on the type
	// [Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public static partial class AXPrefers {

		[DllImport (Constants.AccessibilityLibrary)]
		static extern byte AXPrefersHorizontalTextLayout ();

		public static bool HorizontalTextEnabled ()
		{
			return AXPrefersHorizontalTextLayout () != 0;
		}

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		[DllImport (Constants.AccessibilityLibrary)]
		static extern byte AXPrefersNonBlinkingTextInsertionIndicator ();

		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		public static bool NonBlinkingTextInsertionIndicator ()
		{
			return AXPrefersNonBlinkingTextInsertionIndicator () != 0;
		}
#endif // NET
	}
}
