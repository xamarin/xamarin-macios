#if !__WATCHOS__

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
using CoreImage;

namespace monotouchtest.CoreGraphics {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CGImagePropertiesGPSTest {
		[Test]
		public void LongitudeRefAndLatitudeRefTest ()
		{
			float expectedLatitude = 47.64248f;
			float expectedLongitude = 122.136986f;
			string expectedLatitudeRef = "N";
			string expectedLongitudeRef = "W";
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08_with_loc.png");

			using (var url = NSUrl.FromFilename (file))
			using (var ci = CIImage.FromUrl (url)) {
				var gps = ci.Properties.Gps;
				Assert.AreEqual (expectedLatitude, gps.Latitude, 0.0001f, "Invalid or no Latitude value found.");
				Assert.AreEqual (expectedLongitude, gps.Longitude, 0.0001f, "Invalid or no Longitude value found.");
				Assert.AreEqual (expectedLatitudeRef, gps.LatitudeRef, "Invalid or no LatitudeRef value found.");
				Assert.AreEqual (expectedLongitudeRef, gps.LongitudeRef, "Invalid or no LongitudeRef value found.");
			}
		}
	}
}

#endif // !__WATCHOS__
