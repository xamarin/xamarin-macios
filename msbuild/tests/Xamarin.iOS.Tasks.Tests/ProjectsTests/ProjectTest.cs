using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	public class ProjectTest : TestBase {
		public string BundlePath;
		public string Platform;

		public ProjectTest (string platform)
		{
			Platform = platform;
		}

		public ProjectTest (string bundlePath, string platform)
		{
			BundlePath = bundlePath;
			Platform = platform;
		}

		//public void SetupPaths (string appName, string platform) 
		//{
		//	var paths = SetupProjectPaths (appName, "../", true, platform);
		//	MonoTouchProjectPath = paths ["project_path"];
		//	AppBundlePath = paths ["app_bundlepath"];
		//}

		[SetUp]
		public override void Setup () 
		{
			AssertValidDeviceBuild (Platform);
			SetupEngine ();
		}

		public string BuildProject (string appName, string platform, string config, int expectedErrorCount = 0, bool clean = true)
		{
			var mtouchPaths = SetupProjectPaths (appName, "../", true, platform, config);
			var csproj = mtouchPaths["project_csprojpath"];

			var proj = SetupProject (Engine, csproj);

			AppBundlePath = mtouchPaths ["app_bundlepath"];
			Engine.ProjectCollection.SetGlobalProperty("Platform", platform);
			Engine.ProjectCollection.SetGlobalProperty("Configuration", config);

			if (clean) {
				RunTarget (proj, "Clean");
				Assert.IsFalse (Directory.Exists (AppBundlePath), "App bundle exists after cleanup: {0} ", AppBundlePath);
				Assert.IsFalse (Directory.Exists (AppBundlePath + ".dSYM"), "App bundle .dSYM exists after cleanup: {0} ", AppBundlePath + ".dSYM");
				Assert.IsFalse (Directory.Exists (AppBundlePath + ".mSYM"), "App bundle .mSYM exists after cleanup: {0} ", AppBundlePath + ".mSYM");

				var baseDir = Path.GetDirectoryName (csproj);
				var objDir = Path.Combine (baseDir, "obj", platform, config);
				var binDir = Path.Combine (baseDir, "bin", platform, config);

				if (Directory.Exists (objDir)) {
					var path = Directory.EnumerateFiles (objDir, "*.*", SearchOption.AllDirectories).FirstOrDefault ();
					Assert.IsNull (path, "File not cleaned: {0}", path);
				}

				if (Directory.Exists (binDir)) {
					// Note: the .dSYM/Contents string match is a work-around for xbuild which is broken (wrongly interprets %(Directory))
					var path = Directory.EnumerateFiles (binDir, "*.*", SearchOption.AllDirectories).FirstOrDefault (x => x.IndexOf (".dSYM/Contents/", StringComparison.Ordinal) == -1);
					Assert.IsNull (path, "File not cleaned: {0}", path);
				}
			}

			proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);
			RunTarget (proj, "Build", expectedErrorCount);

			if (expectedErrorCount > 0)
				return csproj;

			Assert.IsTrue (Directory.Exists (AppBundlePath), "App Bundle does not exist: {0} ", AppBundlePath);

			TestFilesExists (AppBundlePath, ExpectedAppFiles);
			TestFilesDoNotExist (AppBundlePath, UnexpectedAppFiles);

			var coreFiles = GetCoreAppFiles (platform, config, appName.Replace (" ", "") + ".exe", appName.Replace (" ", ""));
			var baseDirs = new string [] {
				Path.Combine (AppBundlePath, ".monotouch-32"),
				Path.Combine (AppBundlePath, ".monotouch-64"),
				AppBundlePath,
				Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle", ".monotouch-32"),
				Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle", ".monotouch-64"),
				Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle"),
			};
			TestFilesExists (baseDirs, coreFiles);

			if (platform == "iPhone") {
				var dSYMInfoPlist = Path.Combine (AppBundlePath + ".dSYM", "Contents", "Info.plist");
				var nativeExecutable = Path.Combine (AppBundlePath, appName);

				Assert.IsTrue (File.Exists (dSYMInfoPlist), "dSYM Info.plist file does not exist");
				Assert.IsTrue (File.GetLastWriteTimeUtc (dSYMInfoPlist) >= File.GetLastWriteTimeUtc (nativeExecutable), "dSYM Info.plist should be newer than the native executable");
			}

			return csproj;
		}
	}
}

