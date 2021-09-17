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
			this.BuildExtension ("MyMetalGame", "MyKeyboardExtension");
			this.TestStoryboardC (AppBundlePath);
		}

	}
}
