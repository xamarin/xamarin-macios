using System;
using NUnit.Framework;
using System.Linq;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class CustomKeyboardTests : ExtensionTestBase {
		public CustomKeyboardTests (string platform) : base (platform)
		{
			ExpectedAppFiles = new string [] {
				"MainStoryboard_iPad.storyboardc",
				"MainStoryboard_iPhone.storyboardc",
				"default.metallib"
			};
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			this.BuildExtension ("MyMetalGame", "MyKeyboardExtension");
			this.TestStoryboardC (AppBundlePath);
		}

	}
}
