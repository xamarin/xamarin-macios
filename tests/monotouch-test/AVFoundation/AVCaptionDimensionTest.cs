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
	public class AVCaptionDimensionTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 12, 0);
		}

		[TestCase]
		public void CreateTest ()
		{
			// create a new struct, test that we do have the expected values
			nfloat val = 10;
			var units = AVCaptionUnitsType.Cells;
			var dimension = AVCaptionDimension.Create (val, units);
			Assert.AreEqual (val, dimension.Value, "Value");
			Assert.AreEqual (units, dimension.Units);
		}
	}
}
#endif
