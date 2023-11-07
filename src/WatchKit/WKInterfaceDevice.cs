// Copyright 2015 Xamarin Inc. All rights reserved.

#nullable enable

#if WATCH

using System;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace WatchKit {

	public partial class WKInterfaceDevice {

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

		public bool CheckSystemVersion (int major, int minor, int build)
		{
			return Runtime.CheckSystemVersion (major, minor, build, SystemVersion);
		}
	}
}

#endif // WATCH
