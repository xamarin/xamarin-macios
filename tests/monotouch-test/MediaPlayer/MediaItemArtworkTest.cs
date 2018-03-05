// Copyright 2014 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.IO;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
#endif
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
				Assert.That (img.Size.ToString (), Is.EqualTo ("{Width=32, Height=32}"), "original");
				var upscale = mia.ImageWithSize (new SizeF (100, 100));
				Assert.That (upscale.Size.ToString (), Is.EqualTo ("{Width=32, Height=32}"), "upscale");
				var downscale = mia.ImageWithSize (new SizeF (16, 16));
				Assert.That (downscale.Size.ToString (), Is.EqualTo ("{Width=32, Height=32}"), "downscale");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
