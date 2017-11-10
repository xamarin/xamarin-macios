// Copyright 2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.IO;

#if XAMCORE_2_0
using Foundation;
using UIKit;
using CoreGraphics;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PasteboardTest {
		
		[Test]
		public void ImagesTest ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var dp = new CGDataProvider (file)) {
				using (var cgimg = CGImage.FromPNG (dp, null, false, CGColorRenderingIntent.Default)) {
					using (var img = new UIImage (cgimg)) {
						UIPasteboard.General.Images = new UIImage[] { img };
						Assert.AreEqual (1, UIPasteboard.General.Images.Length, "a - length");

						UIPasteboard.General.Images = new UIImage[] { img, img };
						Assert.AreEqual (2, UIPasteboard.General.Images.Length, "b - length");
						Assert.IsNotNull (UIPasteboard.General.Images [0], "b - nonnull[0]");
						Assert.IsNotNull (UIPasteboard.General.Images [1], "b - nonnull[0]");
					}
				}
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
