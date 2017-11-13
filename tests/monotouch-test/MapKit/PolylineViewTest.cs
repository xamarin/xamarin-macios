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

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PolylineViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (MKPolylineView pl = new MKPolylineView (frame)) {
				Assert.That (pl.Frame, Is.EqualTo (frame), "Frame");
			}
		}
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
	}
}

#endif // !__TVOS__ && !__WATCHOS__
