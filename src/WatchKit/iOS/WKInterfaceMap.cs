#if __IOS__
using System;
using System.ComponentModel;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKInterfaceMap", SkipRegistration = true)]
	[Introduced (PlatformName.iOS, 8,2, PlatformArchitecture.All)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("The WatchKit framework has been removed from iOS")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceMap : WKInterfaceObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS"); } }

		protected WKInterfaceMap (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		protected internal WKInterfaceMap (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void AddAnnotation (global::CoreLocation.CLLocationCoordinate2D location, global::UIKit.UIImage image, CGPoint offset)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void AddAnnotation (global::CoreLocation.CLLocationCoordinate2D location, string name, CGPoint offset)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void AddAnnotation (global::CoreLocation.CLLocationCoordinate2D location, WKInterfaceMapPinColor pinColor)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void RemoveAllAnnotations ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetRegion (global::MapKit.MKCoordinateRegion coordinateRegion)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetVisible (global::MapKit.MKMapRect mapRect)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

	} /* class WKInterfaceMap */
}
#endif // __IOS__
