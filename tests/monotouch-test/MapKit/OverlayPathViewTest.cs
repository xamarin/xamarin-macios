// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using MapKit;
using NUnit.Framework;

namespace MonoTouchFixtures.MapKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OverlayPathViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (MKOverlayPathView opv = new MKOverlayPathView (frame)) {
				Assert.That (opv.Frame, Is.EqualTo (frame), "Frame");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
