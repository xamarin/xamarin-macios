using System;
using System.IO;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class ProjectWithFrameworksTests : ExtensionTestBase {
		public ProjectWithFrameworksTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			this.BuildExtension ("MyMasterDetailApp", "MyShareExtension");

			// Verify that Mono.frameworks is in the app
			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks")), "Frameworks exists");
			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks", "Mono.framework")), "Mono.framework exists");
			Assert.That (File.Exists (Path.Combine (AppBundlePath, "Frameworks", "Mono.framework", "Mono")), "Mono.framework/Mono exists");
		}
	}
}
