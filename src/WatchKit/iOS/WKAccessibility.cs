#if __IOS__
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace WatchKit {

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
#endif
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public static class WKAccessibility  {
		public static void SetAccessibilityHint (this WKInterfaceObject This, string accessibilityHint)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public static void SetAccessibilityIdentifier (this WKInterfaceObject This, string accessibilityIdentifier)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public static void SetAccessibilityImageRegions (this WKInterfaceObject This, WKAccessibilityImageRegion[] accessibilityImageRegions)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public static void SetAccessibilityLabel (this WKInterfaceObject This, string accessibilityLabel)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public static void SetAccessibilityTraits (this WKInterfaceObject This, global::UIKit.UIAccessibilityTrait accessibilityTraits)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public static void SetAccessibilityValue (this WKInterfaceObject This, string accessibilityValue)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public static void SetIsAccessibilityElement (this WKInterfaceObject This, bool isAccessibilityElement)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}
	} /* class WKAccessibility */
}
#endif // __IOS__
