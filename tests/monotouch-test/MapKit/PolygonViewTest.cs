// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
using CoreGraphics;
using Foundation;
using MapKit;
using NUnit.Framework;

namespace MonoTouchFixtures.MapKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PolygonViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (MKPolygonView pv = new MKPolygonView (frame)) {
				Assert.That (pv.Frame, Is.EqualTo (frame), "Frame");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
