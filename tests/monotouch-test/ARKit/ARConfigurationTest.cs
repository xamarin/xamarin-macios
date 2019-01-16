#if XAMCORE_2_0 && __IOS__

using System;
using System.Reflection;
using ARKit;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.ARKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARCondigurationTest {

		[Test]
		public void SupportedVideoFormats ()
		{
			TestRuntime.AssertXcodeVersion (9, 3);
			var svf = ARConfiguration.SupportedVideoFormats;
			Assert.That (svf, Is.Empty, "empty");
		}

		[Test]
		public void GetSupportedVideoFormats_9_3 ()
		{
			TestRuntime.AssertXcodeVersion (9, 3);
			Assert.NotNull (ARWorldTrackingConfiguration.GetSupportedVideoFormats (), "ARWorldTrackingConfiguration");
			Assert.NotNull (AROrientationTrackingConfiguration.GetSupportedVideoFormats (), "AROrientationTrackingConfiguration");
			Assert.NotNull (ARFaceTrackingConfiguration.GetSupportedVideoFormats (), "ARFaceTrackingConfiguration");
			Assert.NotNull (ARObjectScanningConfiguration.GetSupportedVideoFormats (), "ARObjectScanningConfiguration");
		}

		[Test]
		public void GetSupportedVideoFormats_10_0 ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			Assert.NotNull (ARImageTrackingConfiguration.GetSupportedVideoFormats (), "ARImageTrackingConfiguration");
		}

		[Test]
		public void Subclasses ()
		{
			// note: this can be run on any xcode / OS version since it's reflection only
			// all subclasses of ARConfiguration must (re)export 'GetSupportedVideoFormats'
			var c = typeof (ARConfiguration);
			foreach (var sc in c.Assembly.GetTypes ()) {
				if (!sc.IsSubclassOf (c))
					continue;
				var m = sc.GetMethod ("GetSupportedVideoFormats", BindingFlags.Static | BindingFlags.Public);
				Assert.NotNull (m, sc.FullName);
			}
		}
	}
}

#endif
