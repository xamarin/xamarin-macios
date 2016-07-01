using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	public class ExtensionTestBase : TestBase {
		public string BundlePath;
		public string Platform;

		public ExtensionTestBase () { }

		public ExtensionTestBase (string platform) {
			Platform = platform;
		}

		public ExtensionTestBase (string bundlePath, string platform)
		{
			BundlePath = bundlePath;
			Platform = platform;
		}

		public void BuildExtension (string hostAppName, string extensionName, string platform, int expectedErrorCount = 0)
		{
			BuildExtension (hostAppName, extensionName, platform, platform, expectedErrorCount);
		}

		public void BuildExtension (string hostAppName, string extensionName, string bundlePath, string platform, int expectedErrorCount = 0)
		{
			var mtouchPaths = SetupProjectPaths (hostAppName, "../", true, bundlePath);

			var proj = SetupProject (Engine, mtouchPaths ["project_csprojpath"]);

			AppBundlePath = mtouchPaths ["app_bundlepath"];
			string extensionPath = Path.Combine(AppBundlePath, "PlugIns", extensionName + ".appex");
			Engine.GlobalProperties.SetProperty ("Platform", platform);

			RunTarget (proj, "Clean");
			Assert.IsFalse (Directory.Exists (AppBundlePath), "{1}: App bundle exists after cleanup: {0} ", AppBundlePath, bundlePath);
		
			proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);
			RunTarget (proj, "Build", expectedErrorCount);

			if (expectedErrorCount > 0)
				return;

			Assert.IsTrue (Directory.Exists (AppBundlePath), "{1} App Bundle does not exist: {0} ", AppBundlePath, bundlePath);

			TestPList (AppBundlePath, new string[] {"CFBundleExecutable", "CFBundleVersion"});

			Assert.IsTrue (Directory.Exists (extensionPath), "Appex directory does not exist: {0} ", extensionPath);

			TestPList (extensionPath, new string[] {"CFBundleExecutable", "CFBundleVersion"});

			TestFilesExists (AppBundlePath, ExpectedAppFiles);
			TestFilesDoNotExist (AppBundlePath, UnexpectedAppFiles);

			var coreFiles = platform == "iPhone" ? CoreAppFiles : CoreAppFiles.Union (new string [] { hostAppName + ".exe", hostAppName }).ToArray ();
			if (IsTVOS)
				TestFilesExists (platform == "iPhone" ? Path.Combine (AppBundlePath, ".monotouch-64") : AppBundlePath, coreFiles);
			else if (IsWatchOS) {
				coreFiles = platform == "iPhone" ? CoreAppFiles : CoreAppFiles.Union (new string [] { extensionName + ".dll", Path.GetFileNameWithoutExtension (extensionPath) }).ToArray ();
				TestFilesExists (platform == "iPhone" ? Path.Combine (extensionPath, ".monotouch-32") : extensionPath, coreFiles);
			} else
				TestFilesExists (platform == "iPhone" ? Path.Combine (AppBundlePath, ".monotouch-32") : AppBundlePath, coreFiles);
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
			AssertValidDeviceBuild (Platform);
			SetupEngine ();
		}
	}
}

