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
using UIKit;
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

		[Test]
		public void Empty ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("Requires iOS7");

			using (var atlas = new SKTextureAtlas ()) {
				Assert.That (atlas.TextureNames, Is.Empty, "TextureNames");

				// completionHandler is optional
				SKTextureAtlas.PreloadTextures (new [] { atlas }, null);

				// completionHandler is optional
				atlas.Preload (null);

				// that returns a texture, calling 'MissingResource.png' (128 x 128)
				using (var texture = atlas.TextureNamed ("ship")) {
					Assert.That (texture.Handle, Is.Not.EqualTo (IntPtr.Zero), "valid");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
