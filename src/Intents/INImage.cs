//
// INImage extensions and syntax sugar
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && IOS
using System;
using XamCore.CoreGraphics;
using XamCore.IntentsUI;
using XamCore.UIKit;

namespace XamCore.Intents {
	public partial class INImage {

		public static INImage FromImage (CGImage image)
		{
			return (null as INImage).ImageWithCGImage (image);
		}

		public static INImage FromImage (UIImage image)
		{
			return (null as INImage).ImageWithUIImage (image);
		}

		public static CGSize GetImageSize (INIntentResponse response)
		{
			return (null as INImage).ImageSizeForIntentResponse (response);
		}
	}
}
#endif // XAMCORE_2_0 && IOS
