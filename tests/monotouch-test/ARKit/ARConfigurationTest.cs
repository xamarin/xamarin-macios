#if HAS_ARKIT

using System;
using System.Reflection;
using ARKit;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.ARKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARCondigurationTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 3);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
		}

#if !NET
		[Test]
		public void SupportedVideoFormats ()
		{
			var svf = ARConfiguration.SupportedVideoFormats;
			Assert.That (svf, Is.Empty, "empty");
		}
#endif

		[Test]
		public void GetSupportedVideoFormats_9_3 ()
		{
			Assert.NotNull (ARWorldTrackingConfiguration.GetSupportedVideoFormats (), "ARWorldTrackingConfiguration");
			Assert.NotNull (AROrientationTrackingConfiguration.GetSupportedVideoFormats (), "AROrientationTrackingConfiguration");
			Assert.NotNull (ARFaceTrackingConfiguration.GetSupportedVideoFormats (), "ARFaceTrackingConfiguration");
		}

		[Test]
		public void GetSupportedVideoFormats_10_0 ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			Assert.NotNull (ARImageTrackingConfiguration.GetSupportedVideoFormats (), "ARImageTrackingConfiguration");
			Assert.NotNull (ARObjectScanningConfiguration.GetSupportedVideoFormats (), "ARObjectScanningConfiguration");
		}
	}
}

#endif // HAS_ARKIT
