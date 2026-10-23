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
	public static class AppStore {
		/// <summary>Ask StoreKit to request an App Store review or rating from the user.</summary>
		/// <param name="in">The scene or view controller to display the interface in.</param>
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#if __MACOS__
		public static void RequestReview (NSViewController @in)
#else
		public static void RequestReview (UIWindowScene @in)
#endif
		{
			XamarinStoreKitSwiftFunctions.RequestReview (@in);
		}
	}
}

#endif //  __IOS__ || __MACCATALYST__ || __MACOS__
