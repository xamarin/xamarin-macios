using System;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class TVMetalGameTests : ProjectTest {
		public TVMetalGameTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.TVOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			BuildProject ("MyTVMetalGame");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}
	}
}
