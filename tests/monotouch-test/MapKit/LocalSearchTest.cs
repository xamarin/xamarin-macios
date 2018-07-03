//
// Unit tests for MKLocalSearch
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using CoreLocation;
using MapKit;
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
	public class LocalSearchTest {
		[Test]
		public void EmptyRequest ()
		{
			TestRuntime.AssertiOSSystemVersion (6, 1, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);
			TestRuntime.AsserttvOSSystemVersion (9, 2, throwIfOtherPlatform: false);

			using (var lsr = new MKLocalSearchRequest ())
			using (MKLocalSearch ls = new MKLocalSearch (lsr)) {
				lsr.Region = new MKCoordinateRegion (new CLLocationCoordinate2D (47,-71), new MKCoordinateSpan (1,1));
				bool wait = true;
				ls.Start ((MKLocalSearchResponse response, NSError error) => {
					wait = false;
				});
				Assert.True (ls.IsSearching, "IsSearching");

				// wait a bit before cancelling the search (so it really starts)
				// otherwise IsSearching might never complete (on iOS8) and seems very random (in earlier versions)
				NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (2));
				ls.Cancel ();

#if false
				// give it some time to cancel - but eventually time out
				int counter = 0;
				while (wait && (counter < 5)) {
					NSRunLoop.Main.RunUntil (DateTime.Now.AddSeconds (counter));
					counter++;
				}

				Assert.False (ls.IsSearching, "IsSearching/Cancel");
#endif
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
