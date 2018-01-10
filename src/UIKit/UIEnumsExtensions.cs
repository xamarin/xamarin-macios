//
// UIEnums.cs:
//
// Copyright 2009-2011 Novell, Inc.
// Copyright 2011-2012, Xamarin Inc.
//
// Author:
//  Miguel de Icaza
//

#if IOS

using System;
using Foundation;
using ObjCRuntime;

namespace UIKit {

	public static class UIDeviceOrientationExtensions {
		public static bool IsPortrait (this UIDeviceOrientation orientation)
		{
			return orientation == UIDeviceOrientation.PortraitUpsideDown ||
				orientation == UIDeviceOrientation.Portrait;
		}

		public static bool IsLandscape (this UIDeviceOrientation orientation)
		{
			return orientation == UIDeviceOrientation.LandscapeRight || orientation == UIDeviceOrientation.LandscapeLeft;
		}

		public static bool IsFlat (this UIDeviceOrientation orientation)
		{
			return orientation == UIDeviceOrientation.FaceUp || orientation == UIDeviceOrientation.FaceDown;
		}
	}

	public static class UIInterfaceOrientationExtensions {
		public static bool IsPortrait (this UIInterfaceOrientation orientation)
		{
			return orientation == UIInterfaceOrientation.PortraitUpsideDown ||
				orientation == UIInterfaceOrientation.Portrait;
		}
		public static bool IsLandscape (this UIInterfaceOrientation orientation)
		{
			return orientation == UIInterfaceOrientation.LandscapeRight ||
				orientation == UIInterfaceOrientation.LandscapeLeft;
		}
	}
}

#endif // IOS
