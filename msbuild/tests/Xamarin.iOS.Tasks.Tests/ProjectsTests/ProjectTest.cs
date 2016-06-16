using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	public class ProjectTest : TestBase {
		public string BundlePath;
		public string Platform;

		public ProjectTest (string platform) {
			Platform = platform;
		}

		public ProjectTest (string bundlePath, string platform) {
			BundlePath = bundlePath;
			Platform = platform;
		}

		public void SetupPaths (string appName, string platform) 
		{
			var paths = this.SetupProjectPaths (appName, "../", true, platform);
			MonoTouchProjectPath = paths ["project_path"];
			AppBundlePath = paths ["app_bundlepath"];
		}

		[SetUp]
		public override void Setup () 
		{
			SetupEngine ();
		}

		public void BuildProject (string appName, string platform, int expectedErrorCount = 0) 
		{
			var mtouchPaths = SetupProjectPaths (appName, "../", true, platform);

			var proj = SetupProject (Engine, mtouchPaths ["project_csprojpath"]);

			AppBundlePath = mtouchPaths ["app_bundlepath"];
			Engine.GlobalProperties.SetProperty("Platform", platform);

			RunTarget (proj, "Clean");
			Assert.IsFalse (Directory.Exists (AppBundlePath), "App bundle exists after cleanup: {0} ", AppBundlePath);

			proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);
			RunTarget (proj, "Build", expectedErrorCount);

			if (expectedErrorCount > 0)
				return;

			Assert.IsTrue (Directory.Exists (AppBundlePath), "App Bundle does not exist: {0} ", AppBundlePath);

			TestFilesExists (AppBundlePath, ExpectedAppFiles);
			TestFilesDoNotExist (AppBundlePath, UnexpectedAppFiles);

			var coreFiles = platform == "iPhone" ? CoreAppFiles : CoreAppFiles.Union (new string [] { appName + ".exe", appName }).ToArray ();
			if (IsTVOS)
				TestFilesExists (platform == "iPhone" ? Path.Combine (AppBundlePath, ".monotouch-64") : AppBundlePath, coreFiles);
			else
				TestFilesExists (platform == "iPhone" ? Path.Combine (AppBundlePath, ".monotouch-32") : AppBundlePath, coreFiles);
		}
	}
}

