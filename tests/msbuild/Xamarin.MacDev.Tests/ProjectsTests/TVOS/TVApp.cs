using System;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class TVAppTests : ExtensionTestBase
	{
		public TVAppTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest()
		{
			BuildExtension ("MyTVApp", "MyTVServicesExtension", Platform, "Debug");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}
	}
}

