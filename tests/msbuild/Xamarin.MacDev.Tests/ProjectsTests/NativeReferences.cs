using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class NativeReferencesTests : ProjectTest {

		public NativeReferencesTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var mtouchPaths = SetupProjectPaths ("MyTabbedApplication");

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);

			var include = Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "ios-fat", "XTest.framework");
			var metadata = new Dictionary<string, string> {
				{ "IsCxx", "False" },
				{ "Kind", "Framework" },
			};
			var proj = new MSBuildProject (mtouchPaths, this);
			proj.AddItem ("NativeReference", include, metadata);

			MonoTouchProject = mtouchPaths;

			RunTarget (mtouchPaths, "Clean", 0);
			RunTarget (mtouchPaths, "Build", 0);

			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework")), "Frameworks/XTest.framework");
			Assert.That (File.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework", "XTest")), "Frameworks/XTest.framework/XTest");
		}

		[Test]
		public void WithIncrementalBuilds ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			if (Platform.Contains ("Simulator"))
				return; // incremental builds on the simulator doesn't make much sense.

			var mtouchPaths = SetupProjectPaths ("MyiOSAppWithBinding");

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);

			var properties = new Dictionary<string, string> {
				{ "MtouchFastDev", "true" },
				{ "MtouchExtraArgs", "-vvvv" },
				{ "MtouchArch", "ARM64" }, // only use ARM64 to speed up the build.
				{ "MtouchLink", "Full" }, // also to speed up the build.
			};

			MonoTouchProject = mtouchPaths;

			RunTarget (mtouchPaths, "Clean", properties: properties);
			RunTarget (mtouchPaths, "Build", properties: properties);

			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework")), "Frameworks/XTest.framework");
			Assert.That (File.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework", "XTest")), "Frameworks/XTest.framework/XTest");
		}
	}
}
