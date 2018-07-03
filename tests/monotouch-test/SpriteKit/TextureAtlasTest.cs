//
// Unit tests for SKTextureAtlas
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using SpriteKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TextureAtlasTest {

		// Apple bug ?
		void CrashAvoider ()
		{
		}

		[Test]
		public void Empty ()
		{
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);
			TestRuntime.AsserttvOSSystemVersion (9, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertWatchOSVersion (3, 0, throwIfOtherPlatform: false);

			using (var atlas = new SKTextureAtlas ()) {
				Assert.That (atlas.TextureNames, Is.Empty, "TextureNames");

				// this completionHandler is *NOT* optional -> crash if null
				SKTextureAtlas.PreloadTextures (new [] { atlas }, CrashAvoider);

				// this completionHandler is *NOT* optional -> crash if null
				atlas.Preload (CrashAvoider);

				// that returns a texture, calling 'MissingResource.png' (128 x 128)
				using (var texture = atlas.TextureNamed ("ship")) {
					Assert.That (texture.Handle, Is.Not.EqualTo (IntPtr.Zero), "valid");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
