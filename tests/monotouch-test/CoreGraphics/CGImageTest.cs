//
// Unit tests for CGImage
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013, 2015 Xamarin Inc. All rights reserved.
//

using System.IO;

using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreGraphics;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CGImageTest {

		[Test]
		public void FromPNG ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var dp = new CGDataProvider (file))
			using (var img = CGImage.FromPNG (dp, null, false, CGColorRenderingIntent.Default))
#if MONOMAC
			using (var ui = new NSImage (img, new CGSize (10, 10))) {
#else
			using (var ui = new UIImage (img, 1.0f, UIImageOrientation.Up)) {
#endif
				Assert.IsNotNull (ui.CGImage, "CGImage");
				if (TestRuntime.CheckXcodeVersion (7, 0))
					Assert.That (img.UTType.ToString (), Is.EqualTo ("public.png"), "UTType");
			}
		}
	}
}
