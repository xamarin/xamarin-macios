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

#if !XAMCORE_3_0
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

		public OverlayViewPoker (IMKOverlay overlay) : base (overlay)
		{
		}

		public NSObject OverlayBackingField {
			get {
				return (NSObject) bkOverlay.GetValue (this);
			}
		}
	}
#endif // !XAMCORE_3_0

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

#if !XAMCORE_3_0
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
#endif // !XAMCORE_3_0
	}
}

#endif // !__TVOS__ && !__WATCHOS__
