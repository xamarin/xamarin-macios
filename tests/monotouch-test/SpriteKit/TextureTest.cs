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
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif
using SpriteKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

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
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.TvOS, 9, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.WatchOS, 3, 0, throwIfOtherPlatform: false);

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
					if (!TestRuntime.CheckSystemVersion (PlatformName.iOS, 9, 0, throwIfOtherPlatform: false))
						Assert.That (texture.Size, Is.EqualTo (new SizeF (128, 128)), "Size");
#endif
					
					Assert.That (texture.TextureRect, Is.EqualTo (new RectangleF (0, 0, 1, 1)), "TextureRect");
					Assert.False (texture.UsesMipmaps, "UsesMipmaps");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
