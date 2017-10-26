// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if !__WATCHOS__
using System.Drawing;
#endif
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
	
	class OverlayViewPoker : MKOverlayView {
		
		static FieldInfo bkOverlay;
		
		static OverlayViewPoker ()
		{
			var t = typeof (MKOverlayView);
			bkOverlay = t.GetField ("__mt_Overlay_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		
		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}

#if XAMCORE_2_0
		public OverlayViewPoker (IMKOverlay overlay) : base (overlay)
#else
		public OverlayViewPoker (NSObject overlay) : base (overlay)
#endif
		{
		}

		public NSObject OverlayBackingField {
			get {
				return (NSObject) bkOverlay.GetValue (this);
			}
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OverlayViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (MKOverlayView ov = new MKOverlayView (frame)) {
				Assert.That (ov.Frame, Is.EqualTo (frame), "Frame");
				Assert.Null (ov.Overlay, "Overlay");
			}
		}

		[Test]
		public void Overlay_BackingFields ()
		{
			if (OverlayViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");
			
			using (var c = new MKCircle ())
			using (var ov = new OverlayViewPoker (c)) {
				Assert.AreSame (c, ov.Overlay, "1a");
				Assert.AreSame (c, ov.Overlay, "2a");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
