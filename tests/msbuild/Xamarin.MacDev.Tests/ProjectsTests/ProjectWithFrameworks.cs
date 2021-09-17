using System;
using System.IO;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class ProjectWithFrameworksTests : ExtensionTestBase {
		public ProjectWithFrameworksTests (string platform) : base (platform)      
		{
		}

		[Test]
		public void BasicTest ()
		{
			this.BuildExtension ("MyMasterDetailApp", "MyShareExtension");

			// Verify that Mono.frameworks is in the app
			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks")), "Frameworks exists");
			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks", "Mono.framework")), "Mono.framework exists");
			Assert.That (File.Exists (Path.Combine (AppBundlePath, "Frameworks", "Mono.framework", "Mono")), "Mono.framework/Mono exists");
		}
	}
}
