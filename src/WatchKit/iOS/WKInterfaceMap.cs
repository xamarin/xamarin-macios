#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKInterfaceMap", SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceMap : WKInterfaceObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.WatchKitRemoved); } }

		protected WKInterfaceMap (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected internal WKInterfaceMap (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void AddAnnotation (global::CoreLocation.CLLocationCoordinate2D location, global::UIKit.UIImage image, CGPoint offset)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void AddAnnotation (global::CoreLocation.CLLocationCoordinate2D location, string name, CGPoint offset)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void AddAnnotation (global::CoreLocation.CLLocationCoordinate2D location, WKInterfaceMapPinColor pinColor)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void RemoveAllAnnotations ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetRegion (global::MapKit.MKCoordinateRegion coordinateRegion)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetVisible (global::MapKit.MKMapRect mapRect)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

	} /* class WKInterfaceMap */
}
#endif // __IOS__ && !NET
