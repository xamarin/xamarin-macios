using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture("TV", "iPhone")]
	[TestFixture("TVSimulator", "iPhoneSimulator")]
	public class MyTVAppTests : ProjectTest
	{
		public MyTVAppTests(string bundlePath, string platform) : base(bundlePath, platform)
		{
		}

		[Test]
		public void BasicTest()
		{
			BuildProject("MyTVApp", BundlePath, Platform);
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}
	}
}

