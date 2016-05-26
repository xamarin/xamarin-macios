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
#if XAMCORE_2_0
using Foundation;
using EventKit;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.EventKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StructureLocationTest
	{
		[Test]
		public void DefaultValues ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Inconclusive ("EKStructuredLocation is new in 6.0");

			var sl = new EKStructuredLocation ();
			Assert.IsNull (sl.GeoLocation, "GeoLocation");
			Assert.AreEqual (0, sl.Radius, "Radius");
			Assert.IsNull (sl.Title, "Title");
		}

		[Test]
		public void FromTitle ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Inconclusive ("EKStructuredLocation is new in 6.0");

			var sl = EKStructuredLocation.FromTitle ("my title");
			Assert.IsNull (sl.GeoLocation, "GeoLocation");
			Assert.AreEqual (0, sl.Radius, "Radius");
			Assert.AreEqual ("my title", sl.Title, "Title");
		}
	}
}

#endif // !__TVOS__
