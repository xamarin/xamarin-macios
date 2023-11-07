//
// UIWindow.cs: Extensions to UIWindow
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2014, Xamarin Inc. All rights reserved.
//

#if IOS

using System;
using ObjCRuntime;
using CoreGraphics;
using Foundation;

namespace UIKit {
	public partial class UIWindow {
#if !NET
		// duplicates from UIKeyboard without a [Notification]

		public static NSString KeyboardDidChangeFrameNotification {
			get { return UIKeyboard.DidChangeFrameNotification; }
		}

		public static NSString KeyboardWillChangeFrameNotification {
			get { return UIKeyboard.WillChangeFrameNotification; }
		}
#endif
	}
}

#endif
