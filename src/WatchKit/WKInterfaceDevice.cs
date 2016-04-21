// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.UIKit;

namespace XamCore.WatchKit {

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

		// The watch doesn't have UIContentSizeCategory, neither do the corresponding
		// string constants show up in any header (but it looks like they're present
		// in the framework). Removing from the API to make this build for now.
		public UIContentSizeCategory PreferredContentSizeCategory {
			get {
				return _UIContentSizeCategory.ToEnum (_PreferredContentSizeCategory);
			}
		}
#endif
		
		public bool CheckSystemVersion (int major, int minor)
		{
			return Runtime.CheckSystemVersion (major, minor, SystemVersion);
		}
	}
}