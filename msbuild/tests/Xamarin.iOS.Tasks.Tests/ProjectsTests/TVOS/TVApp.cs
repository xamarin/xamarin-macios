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
			BuildExtension("MyTVApp", "MyTVServicesExtension", BundlePath, Platform, "Debug");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}
	}
}

