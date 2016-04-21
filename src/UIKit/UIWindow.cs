//
// UIWindow.cs: Extensions to UIWindow
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2014, Xamarin Inc. All rights reserved.
//

#if !XAMCORE_2_0

using System;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;
using XamCore.Foundation;

namespace XamCore.UIKit {
	public partial class UIWindow {
		// we already have a UIWindowLevel static class for the constants
		[Obsolete ("Use UIWindowLevel.Normal")]
		public const float LevelNormal = 0f;

		[Obsolete ("Use UIWindowLevel.Alert")]
		public const float LevelAlert = 100f;

		[Obsolete ("Use UIWindowLevel.StatusBar")]
		public const float LevelStatusBar = 1000f;
	}
}

#endif
