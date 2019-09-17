#if __IOS__
using System;
using System.ComponentModel;

using ObjCRuntime;

namespace WatchKit {
	[Introduced (PlatformName.iOS, 8,2, PlatformArchitecture.All)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("The WatchKit framework has been removed from iOS")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public static class WKAccessibility  {
		public static void SetAccessibilityHint (this WKInterfaceObject This, string accessibilityHint)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static void SetAccessibilityIdentifier (this WKInterfaceObject This, string accessibilityIdentifier)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static void SetAccessibilityImageRegions (this WKInterfaceObject This, WKAccessibilityImageRegion[] accessibilityImageRegions)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static void SetAccessibilityLabel (this WKInterfaceObject This, string accessibilityLabel)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static void SetAccessibilityTraits (this WKInterfaceObject This, global::UIKit.UIAccessibilityTrait accessibilityTraits)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static void SetAccessibilityValue (this WKInterfaceObject This, string accessibilityValue)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static void SetIsAccessibilityElement (this WKInterfaceObject This, bool isAccessibilityElement)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}
	} /* class WKAccessibility */
}
#endif // __IOS__
