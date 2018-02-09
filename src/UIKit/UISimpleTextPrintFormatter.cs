//
// UISimpleTextPrintFormatter helpers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013, 2015 Xamarin Inc. All rights reserved.
//

#if IOS && !XAMCORE_3_0

namespace UIKit {

	public partial class UISimpleTextPrintFormatter : UIPrintFormatter {

		// since 7.0 GM calling `init` returns an instance where we can't call properties
		// without an Objective-C exception. We work around this since it will accept a null string
		public UISimpleTextPrintFormatter () : this ((string) null)
		{
		}
	}
}

#endif
