// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;

using CoreGraphics;

using Foundation;

using MediaPlayer;

using NUnit.Framework;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VolumeViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (MPVolumeView vv = new MPVolumeView (frame)) {
				Assert.That (vv.Frame, Is.EqualTo (frame), "Frame");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
