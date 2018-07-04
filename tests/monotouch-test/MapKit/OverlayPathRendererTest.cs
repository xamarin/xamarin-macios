//
// MKOverlayPathRenderer Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using CoreLocation;
using MapKit;
using ObjCRuntime;
#else
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MapKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OverlayPathRendererTest {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);
		}

		[Test]
		public void DefaultCtor ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var opr = new MKOverlayPathRenderer ()) {
				Assert.Null (opr.Path, "Path");
			}
		}

		[Test]
		public void CtorOverlay ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			var loc = new CLLocationCoordinate2D (40, 70);
			using (var overlay = MKCircle.Circle (loc, 2000))
			using (var opr = new MKOverlayPathRenderer (overlay)) {
				Assert.Null (opr.Path, "Path");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
