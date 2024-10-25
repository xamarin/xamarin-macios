// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif
using MapKit;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MapKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MapViewTest {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
		}

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (MKMapView mv = new MKMapView (frame)) {
				Assert.That (mv.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void Overlays ()
		{
			using (var polygon = new MKPolygon ())
			using (var polyline = new MKPolyline ())
			using (var circle = new MKCircle ())
			using (MKMapView mv = new MKMapView ()) {
				// old API accepted NSObject (limited protocol support)
				mv.AddOverlay (polygon);
				Assert.That (mv.Overlays.Length, Is.EqualTo (1), "1");
				mv.RemoveOverlay (polygon);
				Assert.That (mv.Overlays, Is.Empty, "2");

				IMKOverlay [] list = { polygon, polyline, circle };
				mv.AddOverlays (list);
				Assert.That (mv.Overlays.Length, Is.EqualTo (3), "3");
				mv.RemoveOverlays (list);
				Assert.That (mv.Overlays, Is.Empty, "4");
			}
		}

		[Test]
		public void Overlays7 ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var polygon = new MKPolygon ())
			using (var polyline = new MKPolyline ())
			using (var circle = new MKCircle ())
			using (var tile = new MKTileOverlay ())
			using (MKMapView mv = new MKMapView ()) {
				// new API accepted MKOverlay (better protocol support)
				mv.AddOverlay (polygon, MKOverlayLevel.AboveLabels);
				mv.AddOverlay (tile, MKOverlayLevel.AboveLabels);
				Assert.That (mv.Overlays.Length, Is.EqualTo (2), "1");
				mv.RemoveOverlay (tile);
				mv.RemoveOverlay (polygon);
				Assert.That (mv.Overlays, Is.Empty, "2");

				IMKOverlay [] list = { polygon, polyline, circle, tile };
				mv.AddOverlays (list, MKOverlayLevel.AboveRoads);
				Assert.That (mv.Overlays.Length, Is.EqualTo (4), "3");
				mv.RemoveOverlays (list);
				Assert.That (mv.Overlays, Is.Empty, "4");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
