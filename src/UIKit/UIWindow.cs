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
#if !XAMCORE_2_0
		// we already have a UIWindowLevel static class for the constants
		[Obsolete ("Use UIWindowLevel.Normal")]
		public const float LevelNormal = 0f;

		[Obsolete ("Use UIWindowLevel.Alert")]
		public const float LevelAlert = 100f;

		[Obsolete ("Use UIWindowLevel.StatusBar")]
		public const float LevelStatusBar = 1000f;
#endif
#if !XAMCORE_4_0
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
