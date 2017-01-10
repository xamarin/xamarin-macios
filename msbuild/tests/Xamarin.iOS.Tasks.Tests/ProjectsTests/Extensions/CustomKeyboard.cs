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
			if (Platform == "iPhoneSimulator") {
				// Note: we expect an error due to Metal not being supported on iPhoneSimulator
				// Note 2: msbuild now is aware that Metal is device-only and throws an error thus this test becomes device-only for time being
				//this.BuildExtension ("MyMetalGame", "MyKeyboardExtension", Platform, 1);
				return;
			}

			this.BuildExtension ("MyMetalGame", "MyKeyboardExtension", Platform, "Debug");
			this.TestStoryboardC (AppBundlePath);
		}

	}
}

