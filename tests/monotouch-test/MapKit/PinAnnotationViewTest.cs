//
// MKPinAnnotationView Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012, 2015 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using MapKit;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MapKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PinAnnotationViewTest {
		
		[Test]
		public void Ctor_Annotation ()
		{
			using (var a = new MKPolyline ())
			using (MKPinAnnotationView av = new MKPinAnnotationView (a, "reuse")) {
				Assert.AreSame (a, av.Annotation, "Annotation");

				if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) // Crashes with EXC_BAD_ACCESS (SIGABRT) if < iOS 7.0
					Assert.False (av.AnimatesDrop, "AnimatesDrop");

				if (!UIDevice.CurrentDevice.CheckSystemVersion (9, 0))
					return;
				Assert.That (av.PinColor, Is.EqualTo (MKPinAnnotationColor.Red), "PinColor");
				Assert.NotNull (av.PinTintColor, "PinTintColor");
			}
		}

		[Test]
		public void InitWithFrame ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) // Crashes with EXC_BAD_ACCESS (SIGABRT) if < iOS 7.0
				Assert.Inconclusive ("Crashes with EXC_BAD_ACCESS (SIGABRT) if < iOS 7.0");
			
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (var av = new MKPinAnnotationView (frame)) {
				Assert.That (av.Frame.ToString (), Is.EqualTo (frame.ToString ()), "Frame"); // fp comparison fails
				Assert.Null (av.Annotation, "Annotation");
				Assert.False (av.AnimatesDrop, "AnimatesDrop");

				if (!UIDevice.CurrentDevice.CheckSystemVersion (9, 0))
					return;
				Assert.That (av.PinColor, Is.EqualTo (MKPinAnnotationColor.Red), "PinColor");
				if (UIDevice.CurrentDevice.CheckSystemVersion (10,0))
					Assert.That (av.PinTintColor.ToString (), Is.EqualTo (UIColor.FromRGBA (255, 59, 48, 255).ToString ()), "PinTintColor");
				else
					Assert.Null (av.PinTintColor, "PinTintColor"); // differs from the other init call
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
