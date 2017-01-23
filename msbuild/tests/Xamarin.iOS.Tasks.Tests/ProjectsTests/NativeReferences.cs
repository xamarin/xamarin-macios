using System;
using System.IO;
using System.Linq;
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

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);

			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);
			var nr = proj.AddItem ("NativeReference", Path.Combine (".", "..", "..", "..", "tests", "test-libraries", ".libs", "ios", "XTest.framework")).First ();
			nr.SetMetadataValue ("IsCxx", "False");
			nr.SetMetadataValue ("Kind", "Framework");

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

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);

			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);

			proj.SetGlobalProperty ("MtouchFastDev", "true");
			proj.SetGlobalProperty ("MtouchExtraArgs", "-vvvv");
			proj.SetGlobalProperty ("MtouchArch", "ARM64"); // only use ARM64 to speed up the build.
			proj.SetGlobalProperty ("MtouchLink", "Full"); // also to speed up the build.

			AppBundlePath = mtouchPaths.AppBundlePath;

			RunTarget (proj, "Clean", 0);
			RunTarget (proj, "Build", 0);

			Assert.That (Directory.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework")), "Frameworks/XTest.framework");
			Assert.That (File.Exists (Path.Combine (AppBundlePath, "Frameworks", "XTest.framework", "XTest")), "Frameworks/XTest.framework/XTest");
		}
	}
}

