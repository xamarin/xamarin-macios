// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
#if XAMCORE_2_0
using Foundation;
using MapKit;
#else
using MonoTouch.Foundation;
using MonoTouch.MapKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.MapKit {
	
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

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PolygonViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (MKPolygonView pv = new MKPolygonView (frame)) {
				Assert.That (pv.Frame, Is.EqualTo (frame), "Frame");
			}
		}
		
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
	}
}

#endif // !__TVOS__ && !__WATCHOS__
