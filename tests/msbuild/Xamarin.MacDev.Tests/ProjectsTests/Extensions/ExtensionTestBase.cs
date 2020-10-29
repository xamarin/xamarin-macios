using System.IO;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using Microsoft.Build.Evaluation;

namespace Xamarin.iOS.Tasks
{
	public class ExtensionTestBase : TestBase {
		public string Platform;

		public ExtensionTestBase () { }

		public ExtensionTestBase (string platform)
		{
			Platform = platform;
		}

		public Project BuildExtension (string hostAppName, string extensionName, string platform, string config, int expectedErrorCount = 0, System.Action<ProjectPaths> additionalAsserts = null)
		{
			var bundlePath = platform;
			var mtouchPaths = SetupProjectPaths (hostAppName, "../", true, bundlePath, config);

			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);

			AppBundlePath = mtouchPaths.AppBundlePath;
			string extensionPath = Path.Combine(AppBundlePath, "PlugIns", extensionName + ".appex");
			Engine.ProjectCollection.SetGlobalProperty ("Platform", platform);
			Engine.ProjectCollection.SetGlobalProperty ("Configuration", config);

			RunTarget (proj, "Clean");
			Assert.IsFalse (Directory.Exists (AppBundlePath), "{1}: App bundle exists after cleanup: {0} ", AppBundlePath, bundlePath);
		
			proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);
			RunTarget (proj, "Build", expectedErrorCount);

			if (expectedErrorCount > 0)
				return proj;

			Assert.IsTrue (Directory.Exists (AppBundlePath), "{1} App Bundle does not exist: {0} ", AppBundlePath, bundlePath);

			TestPList (AppBundlePath, new string[] {"CFBundleExecutable", "CFBundleVersion"});

			Assert.IsTrue (Directory.Exists (extensionPath), "Appex directory does not exist: {0} ", extensionPath);

			TestPList (extensionPath, new string[] {"CFBundleExecutable", "CFBundleVersion"});

			TestFilesExists (AppBundlePath, ExpectedAppFiles);
			TestFilesDoNotExist (AppBundlePath, UnexpectedAppFiles);

			string [] coreFiles;
			var basedirs = new List<string> ();
			if (IsWatchOS) {
				basedirs.Add (extensionPath);
				coreFiles = GetCoreAppFiles (platform, config, extensionName + ".dll", Path.GetFileNameWithoutExtension (extensionPath));
			} else {
				basedirs.Add (AppBundlePath);
				if (platform == "iPhone") {
					basedirs.Add (Path.Combine (AppBundlePath, ".monotouch-32"));
					basedirs.Add (Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle"));
					basedirs.Add (Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle", ".monotouch-32"));
				}
				coreFiles = GetCoreAppFiles (platform, config, hostAppName + ".exe", hostAppName);
			}
			TestFilesExists (basedirs.ToArray (), coreFiles);

			if (additionalAsserts != null)
				additionalAsserts (mtouchPaths);

			return proj;
		}

		public void SetupPaths (string appName, string platform) 
		{
			var paths = this.SetupProjectPaths (appName, "../", true, platform);
			MonoTouchProjectPath = paths.ProjectPath;
			AppBundlePath = paths.AppBundlePath;
		}
			
		[SetUp]
		public override void Setup () 
		{
			AssertValidDeviceBuild (Platform);
			SetupEngine ();
		}
	}
}

