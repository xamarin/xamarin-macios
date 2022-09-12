using System;

using NUnit.Framework;

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
			BuildProject ("MyTVMetalGame");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}
	}
}
