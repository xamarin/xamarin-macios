#if __IOS__ || __MACCATALYST__ || __MACOS__

using System.Diagnostics.CodeAnalysis;

#if HAS_APPKIT
using AppKit;
#endif

#if HAS_UIKIT
using UIKit;
#endif

namespace StoreKit {

	/// <summary>A class to interact with the App Store.</summary>
#if !NET10_0_OR_GREATER
	[Experimental ("APL0004")]
#endif
	public static class AppStore {
		/// <summary>Ask StoreKit to request an App Store review or rating from the user.</summary>
		/// <param name="in">The scene or view controller to display the interface in.</param>
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
