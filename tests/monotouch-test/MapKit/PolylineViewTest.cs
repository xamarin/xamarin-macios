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

#if !XAMCORE_3_0
	class PolylineViewPoker : MKPolylineView {

		static FieldInfo bkPolyline;

		static PolylineViewPoker ()
		{
			var t = typeof (MKPolylineView);
			bkPolyline = t.GetField ("__mt_Polyline_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}

		public PolylineViewPoker ()
		{
		}

		public PolylineViewPoker (MKPolyline polyline) : base (polyline)
		{
		}

		public MKPolyline PolylineBackingField {
			get {
				return (MKPolyline) bkPolyline.GetValue (this);
			}
		}
	}
#endif // !XAMCORE_3_0

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PolylineViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (MKPolylineView pl = new MKPolylineView (frame)) {
				Assert.That (pl.Frame, Is.EqualTo (frame), "Frame");
			}
		}

#if !XAMCORE_3_0
		[Test]
		public void Defaults_BackingFields ()
		{
			if (PolylineViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var pv = new PolylineViewPoker ()) {
				Assert.Null (pv.PolylineBackingField, "1a");
				Assert.Null (pv.Polyline, "2a");
			}
		}

		[Test]
		public void Polygon_BackingFields ()
		{
			if (PolylineViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var p = new MKPolyline ())
			using (var pv = new PolylineViewPoker (p)) {
				Assert.AreSame (p, pv.PolylineBackingField, "1a");
				Assert.AreSame (p, pv.Polyline, "2a");
			}
		}
#endif // !XAMCORE_3_0
	}
}

#endif // !__TVOS__ && !__WATCHOS__
