// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if !__WATCHOS__
using System.Drawing;
#endif
using System.Reflection;
using CoreGraphics;
using Foundation;
using MapKit;
using NUnit.Framework;

namespace MonoTouchFixtures.MapKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OverlayViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (MKOverlayView ov = new MKOverlayView (frame)) {
				Assert.That (ov.Frame, Is.EqualTo (frame), "Frame");
				Assert.Null (ov.Overlay, "Overlay");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
