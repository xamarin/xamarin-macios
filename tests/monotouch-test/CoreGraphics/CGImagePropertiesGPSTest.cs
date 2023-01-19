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
	public class CGImagePropertiesGPSTest {
		[Test]
		public void LongitudeRefAndLatitudeRefTest ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08_with_loc.png");
			using (var url = NSUrl.FromFilename (file))
			using (var ci = CIImage.FromUrl (url)) {
				var gpsA = ci.Properties.Gps;
				Assert.AreEqual (gpsA.Latitude, 47.64248f);
				Assert.AreEqual (gpsA.Longitude, 122.136986f);
				Assert.AreEqual (gpsA.LatitudeRef, "N");
				Assert.AreEqual (gpsA.LongitudeRef, "W");
			}
		}
	}
}

