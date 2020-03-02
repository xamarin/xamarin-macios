using System;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture("TV", "iPhone")]
	[TestFixture("TVSimulator", "iPhoneSimulator")]
	public class TVAppTests : ExtensionTestBase
	{
		public TVAppTests (string bundlePath, string platform) : base(bundlePath, platform)
		{
		}

		[Test]
		public void BasicTest()
		{
			if (Platform == "iPhoneSimulator" && Environment.OSVersion.Version.Major < 19) // Environment.OSVersion = 19.* in macOS Catalina.
				Assert.Ignore ("Metal support is not available in the simulator until macOS 10.15.");
			BuildExtension ("MyTVApp", "MyTVServicesExtension", BundlePath, Platform, "Debug");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}
	}
}

