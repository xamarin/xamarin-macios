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
using CoreGraphics;
using Foundation;
using MapKit;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MapKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PinAnnotationViewTest {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
		}

		[Test]
		public void Ctor_Annotation ()
		{
			using (var a = new MKPolyline ())
			using (MKPinAnnotationView av = new MKPinAnnotationView (a, "reuse")) {
				Assert.AreSame (a, av.Annotation, "Annotation");

#if !MONOMAC
				if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 7, 0)) // Crashes with EXC_BAD_ACCESS (SIGABRT) if < iOS 7.0
					Assert.False (av.AnimatesDrop, "AnimatesDrop");

				if (!TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 9, 0))
					return;
#endif

				Assert.That (av.PinColor, Is.EqualTo (MKPinAnnotationColor.Red), "PinColor");

				if (TestRuntime.CheckXcodeVersion (7, 0))
					Assert.NotNull (av.PinTintColor, "PinTintColor");
			}
		}

		[Test]
		public void InitWithFrame ()
		{
#if !MONOMAC
			// Crashes with EXC_BAD_ACCESS (SIGABRT) if < iOS 7.0
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
#endif

			var frame = new CGRect (10, 10, 100, 100);
			using (var av = new MKPinAnnotationView (frame)) {
				Assert.That (av.Frame.ToString (), Is.EqualTo (frame.ToString ()), "Frame"); // fp comparison fails
				Assert.Null (av.Annotation, "Annotation");
				Assert.False (av.AnimatesDrop, "AnimatesDrop");

				if (!TestRuntime.CheckXcodeVersion (7, 0))
					return;

				Assert.That (av.PinColor, Is.EqualTo (MKPinAnnotationColor.Red), "PinColor");
#if MONOMAC
				if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 12)) {
					Assert.That (av.PinTintColor, Is.EqualTo (NSColor.SystemRed), "PinTintColor");
				} else {
					Assert.Null (av.PinTintColor, "PinTintColor"); // differs from the other init call
				}
#else
				bool not_null = TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 10, 0);
				if (not_null)
					Assert.NotNull (av.PinTintColor, "PinTintColor");
				else
					Assert.Null (av.PinTintColor, "PinTintColor"); // differs from the other init call
#endif
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
