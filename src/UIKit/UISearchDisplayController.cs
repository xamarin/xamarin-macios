#if !TVOS && !WATCH && !__MACCATALYST__ // __TVOS_PROHIBITED, doesn't show up in WatchOS headers
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using CoreLocation;
using MapKit;
using UIKit;
using CoreGraphics;

namespace UIKit {
	public partial class UISearchDisplayController {
		public UITableViewSource SearchResultsSource {
			get {
				var d = SearchResultsWeakDelegate as UITableViewSource;
				if (d is not null)
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

#endif // !TVOS && !WATCH && !__MACCATALYST__
