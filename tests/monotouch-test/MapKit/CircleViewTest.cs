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
	
	class CircleViewPoker : MKCircleView {
		
		static FieldInfo bkCircle;
		
		static CircleViewPoker ()
		{
			var t = typeof (MKCircleView);
			bkCircle = t.GetField ("__mt_Circle_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		
		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}
		
		public CircleViewPoker ()
		{
		}

		public CircleViewPoker (MKCircle circle) : base (circle)
		{
		}
		
		public MKCircle CircleBackingField {
			get {
				return (MKCircle) bkCircle.GetValue (this);
			}
		}
	}
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CircleViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (MKCircleView cv = new MKCircleView (frame)) {
				Assert.That (cv.Frame, Is.EqualTo (frame), "Frame");
			}
		}
		
		[Test]
		public void Defaults_BackingFields ()
		{
			if (CircleViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");
			
			using (var cv = new CircleViewPoker ()) {
				Assert.Null (cv.CircleBackingField, "1a");
				Assert.Null (cv.Circle, "2a");
			}
		}

		[Test]
		public void Circle_BackingFields ()
		{
			if (CircleViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");
			
			using (var c = new MKCircle ())
			using (var cv = new CircleViewPoker (c)) {
				Assert.AreSame (c, cv.CircleBackingField, "1a");
				Assert.AreSame (c, cv.Circle, "2a");
			}
		}
	}
}

#endif // !__TVOS_ && !__WATCHOS__
