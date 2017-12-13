#if !TVOS && !WATCH // __TVOS_PROHIBITED, doesn't show up in WatchOS headers
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.CoreAnimation;
using XamCore.CoreLocation;
using XamCore.MapKit;
using XamCore.UIKit;
using XamCore.CoreGraphics;

namespace XamCore.UIKit {
	public partial class UISearchDisplayController {
		public UITableViewSource SearchResultsSource {
			get {
				var d = SearchResultsWeakDelegate as UITableViewSource;
				if (d != null)
					return d;
				return null;
			}

			set {
				SearchResultsWeakDelegate = value;
				SearchResultsWeakDataSource = value;
			}
		}
	}
}

#endif // !TVOS && !WATCH
