//
// Unit tests for MKLocalSearchRequest
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
using CoreLocation;
using MapKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MapKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LocalSearchRequestTest {

		[Test]
		public void Default ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 6, 1, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.TVOS, 9, 2, throwIfOtherPlatform: false);

			using (var lsr = new MKLocalSearchRequest ()) {
				Assert.Null (lsr.NaturalLanguageQuery, "NaturalLanguageQuery");
				Assert.That (lsr.Region.Center.Latitude, Is.EqualTo (0.0d), "Latitude");
				Assert.That (lsr.Region.Center.Longitude, Is.EqualTo (0.0d), "Longitude");
				Assert.That (lsr.Region.Span.LatitudeDelta, Is.EqualTo (0.0d), "LatitudeDelta");
				Assert.That (lsr.Region.Span.LongitudeDelta, Is.EqualTo (0.0d), "LongitudeDelta");

				lsr.NaturalLanguageQuery = "restaurants";
				lsr.Region = new MKCoordinateRegion (new CLLocationCoordinate2D (47, -71), new MKCoordinateSpan (1, 1));

				// NaturalLanguageQuery is nullable, Region is not (value-type)
				lsr.NaturalLanguageQuery = null;
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
