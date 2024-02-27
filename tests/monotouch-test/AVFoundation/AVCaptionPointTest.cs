#if __MACOS__
using System;
using Foundation;
using AVFoundation;
using NUnit.Framework;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVCaptionPointTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 12, 0);
		}

		[TestCase]
		public void CreateTest ()
		{
			nfloat val = 10;
			var units = AVCaptionUnitsType.Cells;
			var firstDimension = AVCaptionDimension.Create (val, units);
			var secondDimension = AVCaptionDimension.Create (val, units);

			var point = AVCaptionPoint.Create (firstDimension, secondDimension);
			Assert.AreEqual (val, point.X.Value, "X");
			Assert.AreEqual (val, point.Y.Value, "Y");
		}
	}
}
#endif
