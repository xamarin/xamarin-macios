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

#if !XAMCORE_3_0
	class MapViewPoker : MKMapView {

		static FieldInfo bkAnnotations;
		static FieldInfo bkSelectedAnnotations;
		static FieldInfo bkOverlays;

		static MapViewPoker ()
		{
			var t = typeof (MKMapView);
			bkAnnotations = t.GetField ("__mt_Annotations_var", BindingFlags.Instance | BindingFlags.NonPublic);
			bkSelectedAnnotations = t.GetField ("__mt_SelectedAnnotations_var", BindingFlags.Instance | BindingFlags.NonPublic);
			bkOverlays = t.GetField ("__mt_Overlays_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public MapViewPoker ()
		{
		}

		// if created (and even if unused) iOS will call it back later (retain)
		public MapViewPoker (IntPtr p) : base (p)
		{
		}

		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}

		public NSObject [] AnnotationsBackingField {
			get {
				return (NSObject []) bkAnnotations.GetValue (this);
			}
		}

		public NSObject [] SelectedAnnotationsBackingField {
			get {
				return (NSObject []) bkSelectedAnnotations.GetValue (this);
			}
		}

		public NSObject [] OverlaysBackingField {
			get {
				return (NSObject []) bkOverlays.GetValue (this);
			}
		}
	}
#endif // !XAMCORE_3_0

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

#if !XAMCORE_3_0
		[Test]
		public void Annotations_BackingFields ()
		{
			if (MapViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var a = new MKCircle ())     // MKAnnotation is abstract
			using (var o1 = new MKPolygon ())   // it must export 'coordinate' or this will fail
			using (var o2 = new MKPolyline ())
			using (var mv = new MapViewPoker ()) {
				Assert.Null (mv.AnnotationsBackingField, "1a");
				Assert.That (mv.Annotations, Is.Empty, "1b");

				mv.AddAnnotation (a);
				Assert.AreSame (a, mv.AnnotationsBackingField [0], "2a");
				Assert.AreSame (a, mv.Annotations [0], "2b");

				mv.RemoveAnnotation (a);
				Assert.That (mv.AnnotationsBackingField, Is.Empty, "3a");
				Assert.That (mv.Annotations, Is.Empty, "3b");

				mv.AddAnnotation (o1);
				Assert.AreSame (o1, mv.AnnotationsBackingField [0], "4a");
				Assert.AreSame (o1, mv.Annotations [0], "4b");

				mv.RemoveAnnotation (o1);
				Assert.That (mv.AnnotationsBackingField, Is.Empty, "5a");
				Assert.That (mv.Annotations, Is.Empty, "5b");

				mv.AddAnnotations (new IMKAnnotation [] { o1, o2 });
				// don't assume ordering
				Assert.That (mv.AnnotationsBackingField.Length, Is.EqualTo (2), "6a");
				Assert.That (mv.Annotations.Length, Is.EqualTo (2), "6b");

				mv.RemoveAnnotations (new IMKAnnotation [] { o2, o1 });
				Assert.That (mv.AnnotationsBackingField, Is.Empty, "7a");
				Assert.That (mv.Annotations, Is.Empty, "7b");
			}
		}

		[Test]
		public void SelectedAnnotations_BackingFields ()
		{
			if (MapViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

#if !MONOMAC
			if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0)) {
				// This test selects annotations on a map view, but according to apple's docs
				// and a lot of googling this will not necessarily work until the map view is
				// show. Since we can't relinquish control of the UI thread, we have no option
				// but ignoring this test. For now I've only seen it fail on iOS 7 DP4.
				Assert.Inconclusive ("This test is not deterministic on iOS7 DP4.");
			}
#endif

			using (var a = new MKCircle ())     // MKAnnotation is abstract
			using (var o1 = new MKPolygon ())   // it must export 'coordinate' or this will fail
			using (var o2 = new MKPolyline ())
			using (var mv = new MapViewPoker ()) {
				Assert.Null (mv.SelectedAnnotationsBackingField, "1a");
				Assert.Null (mv.SelectedAnnotations, "1b"); // not an empty array

				mv.SelectAnnotation (a, false);
				Assert.AreSame (a, mv.SelectedAnnotationsBackingField [0], "2a");
				Assert.AreSame (a, mv.SelectedAnnotations [0], "2b");

				// sanity
				Assert.Null (mv.AnnotationsBackingField, "3a");
				Assert.That (mv.Annotations, Is.Empty, "3b");

				mv.SelectedAnnotations = new IMKAnnotation [] { o1, o2 };
				// note: when assigning the property only the first item is selected (by design)
				// so we're not exactly backing up correctly (we still hold 'o2')
				// OTOH we do not want to recursively [PostGet] the same property (unless handled by the generator)
				Assert.That (mv.SelectedAnnotationsBackingField.Length, Is.EqualTo (2), "4a");
				Assert.That (mv.SelectedAnnotations.Length, Is.EqualTo (1), "4b");

				mv.DeselectAnnotation (o1, false);
				// since only 'o1' was really selected, unselecting it will return null
				Assert.Null (mv.SelectedAnnotationsBackingField, "5a");
				Assert.Null (mv.SelectedAnnotations, "5b"); // not an empty array
			}
		}

		[Test]
		public void Overlays_BackingFields ()
		{
			if (MapViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var o1 = new MKPolygon ())   // it must export 'boundingMapRect' or this will fail
			using (var o2 = new MKPolyline ())
			using (var mv = new MapViewPoker ()) {
				var overlays = new IMKOverlay [] { o1, o2 };
				Assert.Null (mv.OverlaysBackingField, "1a");
				Assert.Null (mv.Overlays, "1b"); // not an empty array

				mv.AddOverlay (o1);
				Assert.AreSame (o1, mv.OverlaysBackingField [0], "2a");
				Assert.AreSame (o1, mv.Overlays [0], "2b");

				mv.InsertOverlay (o2, 0);
				Assert.That (mv.OverlaysBackingField.Length, Is.EqualTo (2), "3a");
				Assert.That (mv.Overlays.Length, Is.EqualTo (2), "3b");

				mv.RemoveOverlay (o1);
				Assert.AreSame (o2, mv.OverlaysBackingField [0], "4a");
				Assert.AreSame (o2, mv.Overlays [0], "4b");

				mv.InsertOverlayAbove (o1, o2);
				Assert.That (mv.OverlaysBackingField.Length, Is.EqualTo (2), "5a");
				Assert.That (mv.Overlays.Length, Is.EqualTo (2), "5b");

				mv.RemoveOverlay (o2);
				Assert.AreSame (o1, mv.OverlaysBackingField [0], "6a");
				Assert.AreSame (o1, mv.Overlays [0], "6b");

				mv.InsertOverlayBelow (o2, o1);
				Assert.That (mv.OverlaysBackingField.Length, Is.EqualTo (2), "7a");
				Assert.That (mv.Overlays.Length, Is.EqualTo (2), "7b");

				mv.RemoveOverlays (overlays);
				Assert.That (mv.OverlaysBackingField, Is.Empty, "8a");
				Assert.That (mv.Overlays, Is.Empty, "8b");

				mv.AddOverlays (overlays);
				Assert.That (mv.OverlaysBackingField.Length, Is.EqualTo (2), "9a");
				Assert.That (mv.Overlays.Length, Is.EqualTo (2), "9b");
			}
		}
#endif // !XAMCORE_3_0

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
