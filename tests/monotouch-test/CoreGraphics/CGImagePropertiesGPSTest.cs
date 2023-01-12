using System;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreGraphics;
using NUnit.Framework;
using System.IO;

namespace monotouchtest.CoreGraphics {
	[TestFixture]
	public class CGImagePropertiesGPSTest {
		[Test]
		public void LongitudeRefAndLattitudeRefTest ()
		{
			var keys = new NSString [] {
				new NSString ("LatitudeRef"),
				new NSString ("Latitude"),
				new NSString ("LongitudeRef"),
				new NSString ("Longitude")
			};
			var values = new object [] {
				new NSString ("N"),
				47.6422f,
				new NSString ("W"),
				-122.1367f
			};
			CGImagePropertiesGps gps = new (NSDictionary.FromObjectsAndKeys (values, keys));
			Assert.AreEqual (gps.LatitudeRef, values [0]);
			Assert.AreEqual (gps.Latitude, values [1]);
			Assert.AreEqual (gps.LongitudeRef, values [2]);
			Assert.AreEqual (gps.Longitude, values [3]);
		}
	}
}

