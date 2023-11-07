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
	class PolygonViewPoker : MKPolygonView {

		static FieldInfo bkPolygon;

		static PolygonViewPoker ()
		{
			var t = typeof (MKPolygonView);
			bkPolygon = t.GetField ("__mt_Polygon_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}

		public PolygonViewPoker ()
		{
		}

		public PolygonViewPoker (MKPolygon polygon) : base (polygon)
		{
		}

		public MKPolygon PolygonBackingField {
			get {
				return (MKPolygon) bkPolygon.GetValue (this);
			}
		}
	}
#endif // !XAMCORE_3_0

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

#if !XAMCORE_3_0
		[Test]
		public void Defaults_BackingFields ()
		{
			if (PolygonViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var pv = new PolygonViewPoker ()) {
				Assert.Null (pv.PolygonBackingField, "1a");
				Assert.Null (pv.Polygon, "2a");
			}
		}

		[Test]
		public void Polygon_BackingFields ()
		{
			if (PolygonViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var p = new MKPolygon ())
			using (var pv = new PolygonViewPoker (p)) {
				Assert.AreSame (p, pv.PolygonBackingField, "1a");
				Assert.AreSame (p, pv.Polygon, "2a");
			}
		}
#endif // !XAMCORE_3_0
	}
}

#endif // !__TVOS__ && !__WATCHOS__
