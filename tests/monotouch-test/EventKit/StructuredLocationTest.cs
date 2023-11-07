//
// Unit tests for EKStructuredLocation
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;
using Foundation;
using EventKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.EventKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StructureLocationTest {
		[Test]
		public void DefaultValues ()
		{
			if (!TestRuntime.CheckXcodeVersion (4, 5))
				Assert.Inconclusive ("EKStructuredLocation is new in 6.0");

			var sl = new EKStructuredLocation ();
			Assert.IsNull (sl.GeoLocation, "GeoLocation");
			Assert.AreEqual (0, sl.Radius, "Radius");
			Assert.IsNull (sl.Title, "Title");
		}

		[Test]
		public void FromTitle ()
		{
			if (!TestRuntime.CheckXcodeVersion (4, 5))
				Assert.Inconclusive ("EKStructuredLocation is new in 6.0");

			var sl = EKStructuredLocation.FromTitle ("my title");
			Assert.IsNull (sl.GeoLocation, "GeoLocation");
			Assert.AreEqual (0, sl.Radius, "Radius");
			Assert.AreEqual ("my title", sl.Title, "Title");
		}
	}
}

#endif // !__TVOS__
