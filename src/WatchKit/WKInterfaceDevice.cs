// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace WatchKit {

	public partial class WKInterfaceDevice {

#if !WATCH
		public IReadOnlyDictionary<string,long> CachedImages {
			get {
				// the NSDictionary contains NSString keys with NSNumber values, which are not friendly to use
				var wd = WeakCachedImages;
				var md = new Dictionary<string,long> ((int) wd.Count);
				foreach (var kvp in wd) {
					md.Add (kvp.Key.ToString (), (kvp.Value as NSNumber).Int64Value);
				}
				return md;
			}
		}

		[Obsolete ("Use PreferredContentSizeCategoryString instead.")]
		public UIContentSizeCategory PreferredContentSizeCategory {
			get {
				return UIContentSizeCategoryExtensions.GetValue (_PreferredContentSizeCategory);
			}
		}
#endif

		// This method (preferredContentSizeCategory) is defined as "NSString *"
		// in the headers, and the return values are not documented
		// (documentation says the return values are the same as for
		// UIApplication.ContentSizeCategory, but testing shows that to be
		// incorrect).
		//
		// Unfortunately we've already bound this using the
		// UIContentSizeCategory for Xamarin.iOS.dll, and since we can't
		// change that, use a different name instead.
		public string PreferredContentSizeCategoryString {
			get {
				return _PreferredContentSizeCategory;
			}
		}

		public bool CheckSystemVersion (int major, int minor)
		{
			return Runtime.CheckSystemVersion (major, minor, SystemVersion);
		}
	}
}
