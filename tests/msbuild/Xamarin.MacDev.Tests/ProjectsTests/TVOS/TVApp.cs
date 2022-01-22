using System;

using NUnit.Framework;

using Xamarin.Tests;

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
			BuildExtension ("MyTVApp", "MyTVServicesExtension");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}
	}
}
