using System;
using System.IO;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class NativeReferencesTests : ProjectTest {
		
		public NativeReferencesTests (string platform) : base (platform)      
		{
		}

		[Test]
		public void BasicTest ()
		{
			var mtouchPaths = SetupProjectPaths ("MyTabbedApplication", "../", true, Platform);

			Engine.GlobalProperties.SetProperty ("Platform", Platform);

			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);
			var nr = proj.AddNewItem ("NativeReference", Path.Combine (".", "..", "..", "..", "tests", "test-libraries", ".libs", "ios", "XTest.framework"));
			nr.SetMetadata ("IsCxx", "False");
			nr.SetMetadata ("Kind", "Framework");

			AppBundlePath = mtouchPaths.AppBundlePath;

			RunTarget (proj, "Clean", 0);
			RunTarget (proj, "Build", 0);

			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework")), "Frameworks/XTest.framework");
			Assert.That (File.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework", "XTest")), "Frameworks/XTest.framework/XTest");
		}

		[Test]
		public void WithIncrementalBuilds ()
		{
			if (Platform.Contains ("Simulator"))
				return; // incremental builds on the simulator doesn't make much sense.

			var mtouchPaths = SetupProjectPaths ("MyiOSAppWithBinding", "../", true, Platform);

			Engine.GlobalProperties.SetProperty ("Platform", Platform);

			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);

			proj.GlobalProperties.SetProperty ("MtouchFastDev", "true");
			proj.GlobalProperties.SetProperty ("MtouchExtraArgs", "-vvvv");
			proj.GlobalProperties.SetProperty ("MtouchArch", "ARM64"); // only use ARM64 to speed up the build.
			proj.GlobalProperties.SetProperty ("MtouchLink", "Full"); // also to speed up the build.

			AppBundlePath = mtouchPaths.AppBundlePath;

			RunTarget (proj, "Clean", 0);
			RunTarget (proj, "Build", 0);

			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework")), "Frameworks/XTest.framework");
			Assert.That (File.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework", "XTest")), "Frameworks/XTest.framework/XTest");
		}
	}
}

