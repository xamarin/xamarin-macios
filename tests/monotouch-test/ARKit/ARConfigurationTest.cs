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

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (10,0);
		}

		[Test]
		public void SupportedVideoFormats ()
		{
			var svf = ARConfiguration.SupportedVideoFormats;
			Assert.That (svf, Is.Empty, "empty");
		}

		[Test]
		public void GetSupportedVideoFormats ()
		{
			Assert.NotNull (ARWorldTrackingConfiguration.GetSupportedVideoFormats (), "ARWorldTrackingConfiguration");
			Assert.NotNull (AROrientationTrackingConfiguration.GetSupportedVideoFormats (), "AROrientationTrackingConfiguration");
			Assert.NotNull (ARFaceTrackingConfiguration.GetSupportedVideoFormats (), "ARFaceTrackingConfiguration");
			Assert.NotNull (ARImageTrackingConfiguration.GetSupportedVideoFormats (), "ARImageTrackingConfiguration");
			Assert.NotNull (ARObjectScanningConfiguration.GetSupportedVideoFormats (), "ARObjectScanningConfiguration");
		}

		[Test]
		public void Subclasses ()
		{
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