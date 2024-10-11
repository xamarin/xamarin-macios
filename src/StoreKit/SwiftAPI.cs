#if __IOS__ || __MACCATALYST__ || __MACOS__

using System.Diagnostics.CodeAnalysis;

#if HAS_APPKIT
using AppKit;
#endif

#if HAS_UIKIT
using UIKit;
#endif

namespace StoreKit {

#if !NET10_0_OR_GREATER
	[Experimental ("APL0004")]
#endif
	public static class AppStore {
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[UnsupportedOSPlatform ("tvos")]
#if __MACOS__
		public static void RequestReview (NSViewController @in)
#else
		public static void RequestReview (UIWindowScene @in)
#endif
		{
			XamarinSwiftFunctions.RequestReview (@in);
		}
	}
}

#endif //  __IOS__ || __MACCATALYST__ || __MACOS__
