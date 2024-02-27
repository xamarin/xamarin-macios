//
// Unit tests for SKTexture
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif
using SpriteKit;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TextureTest {

		// Apple bug ?
		void CrashAvoider ()
		{
		}

		[Test]
		public void Atlas_MissingResource ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 9, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.WatchOS, 3, 0, throwIfOtherPlatform: false);

			using (var atlas = new SKTextureAtlas ()) {
				// that returns a texture, calling 'MissingResource.png' (128 x 128)
				using (var texture = atlas.TextureNamed ("ship")) {

					// this completionHandler is *NOT* optional -> crash if null
					SKTexture.PreloadTextures (new [] { texture }, CrashAvoider);

					// this completionHandler is *NOT* optional -> crash if null
					texture.Preload (CrashAvoider);

					Assert.That (texture.FilteringMode, Is.EqualTo (SKTextureFilteringMode.Linear), "FilteringMode");

#if !MONOMAC
					// FIXME: bug in iOS9 - it will randomly return 0,0 (but almost always on the first try)
					if (!TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 9, 0, throwIfOtherPlatform: false))
						Assert.That (texture.Size, Is.EqualTo (new CGSize (128, 128)), "Size");
#endif

					Assert.That (texture.TextureRect, Is.EqualTo (new CGRect (0, 0, 1, 1)), "TextureRect");
					Assert.False (texture.UsesMipmaps, "UsesMipmaps");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
