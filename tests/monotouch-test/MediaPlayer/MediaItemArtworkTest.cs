// Copyright 2014 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.IO;
using System.Drawing;
using CoreGraphics;
using Foundation;
using MediaPlayer;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MediaItemArtworkTest {

		[Test]
		public void ImageWithSize ()
		{
			// trying to reproduce issue from bug #23619 - but turned out to be an iOS8 bug
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var img = UIImage.FromFile (file))
			using (var mia = new MPMediaItemArtwork (img)) {
#if NET
				const string expected = "{32, 32}";
#else
				const string expected = "{Width=32, Height=32}";
#endif
				Assert.That (img.Size.ToString (), Is.EqualTo (expected), "original");
				var upscale = mia.ImageWithSize (new CGSize (100, 100));
				Assert.That (upscale.Size.ToString (), Is.EqualTo (expected), "upscale");
				var downscale = mia.ImageWithSize (new CGSize (16, 16));
				Assert.That (downscale.Size.ToString (), Is.EqualTo (expected), "downscale");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
