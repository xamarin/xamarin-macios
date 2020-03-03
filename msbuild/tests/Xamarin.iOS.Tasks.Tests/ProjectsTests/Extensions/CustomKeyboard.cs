using System;
using NUnit.Framework;
using System.Linq;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class CustomKeyboardTests : ExtensionTestBase {
		public CustomKeyboardTests(string platform) : base(platform) {
			ExpectedAppFiles = new string [] { 
				"MainStoryboard_iPad.storyboardc", 
				"MainStoryboard_iPhone.storyboardc", 
				"default.metallib"
			};
		}

		[Test]
		public void BasicTest () 
		{
			if (Platform == "iPhoneSimulator" && Environment.OSVersion.Version.Major < 19) // Environment.OSVersion = 19.* in macOS Catalina.
				Assert.Ignore ("Metal support is not available in the simulator until macOS 10.15.");
			this.BuildExtension ("MyMetalGame", "MyKeyboardExtension", Platform, "Debug");
			this.TestStoryboardC (AppBundlePath);
		}

	}
}

