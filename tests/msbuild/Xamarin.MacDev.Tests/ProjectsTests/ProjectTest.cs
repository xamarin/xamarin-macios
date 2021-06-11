using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.iOS.Tasks
{
	public class ProjectTest : TestBase {
		public ProjectTest (string platform)
			: base (platform)
		{
		}

		public ProjectTest (string platform, string config)
			: base (platform, config)
		{
		}

		public ProjectPaths BuildProject (string appName, int expectedErrorCount = 0, bool clean = true, bool nuget_restore = false)
		{
			var mtouchPaths = SetupProjectPaths (appName);
			var csproj = mtouchPaths.ProjectCSProjPath;

			MonoTouchProject = mtouchPaths;
			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);
			Engine.ProjectCollection.SetGlobalProperty ("Configuration", Config);

			if (nuget_restore)
				NugetRestore (csproj);

			if (clean) {
				RunTarget (mtouchPaths, "Clean", Mode);
				Assert.IsFalse (Directory.Exists (AppBundlePath), "App bundle exists after cleanup: {0} ", AppBundlePath);
				Assert.IsFalse (Directory.Exists (AppBundlePath + ".dSYM"), "App bundle .dSYM exists after cleanup: {0} ", AppBundlePath + ".dSYM");
				Assert.IsFalse (Directory.Exists (AppBundlePath + ".mSYM"), "App bundle .mSYM exists after cleanup: {0} ", AppBundlePath + ".mSYM");

				var baseDir = Path.GetDirectoryName (csproj);
				var objDir = MonoTouchProject.ProjectObjPath;
				var binDir = MonoTouchProject.ProjectBinPath;

				if (Directory.Exists (objDir)) {
					var paths = Directory.EnumerateFiles (objDir, "*.*", SearchOption.AllDirectories)
							.Where (v => !v.EndsWith (".FileListAbsolute.txt", StringComparison.Ordinal))
							.Where (v => !v.EndsWith (".assets.cache", StringComparison.Ordinal));
					Assert.IsEmpty (paths, "Files not cleaned:\n\t{0}", string.Join ("\n\t", paths));
				}

				if (Directory.Exists (binDir)) {
					var paths = Directory.EnumerateFiles (binDir, "*.*", SearchOption.AllDirectories);
					Assert.IsEmpty (paths, "Files not cleaned:\n\t{0}", string.Join ("\n\t", paths));
				}
			}

			RunTarget (mtouchPaths, "Build", Mode, expectedErrorCount);

			if (expectedErrorCount > 0)
				return mtouchPaths;

			Assert.IsTrue (Directory.Exists (AppBundlePath), "App Bundle does not exist: {0} ", AppBundlePath);

			TestFilesExists (AppBundlePath, ExpectedAppFiles);
			TestFilesDoNotExist (AppBundlePath, UnexpectedAppFiles);

			if (Mode != ExecutionMode.DotNet) {
				var coreFiles = GetCoreAppFiles (Path.GetFileNameWithoutExtension (mtouchPaths.AppBundlePath) + ".exe", Path.GetFileNameWithoutExtension (mtouchPaths.AppBundlePath));
				string assemblyBundlePath;
				string frameworksBundlePath;
				string executableBundlePath;

				switch (ApplePlatform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					assemblyBundlePath = AppBundlePath;
					frameworksBundlePath = AppBundlePath;
					executableBundlePath = AppBundlePath;
					break;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					assemblyBundlePath = Path.Combine (AppBundlePath, "Contents", "MonoBundle");
					frameworksBundlePath = Path.Combine (AppBundlePath, "Contents");
					executableBundlePath = Path.Combine (AppBundlePath, "Contents", "MacOS");
					break;
				default:
					throw new InvalidOperationException ($"Unknown platform: {ApplePlatform}");
				}

				var baseDirs = new string [] {
					Path.Combine (assemblyBundlePath, ".monotouch-32"),
					Path.Combine (assemblyBundlePath, ".monotouch-64"),
					assemblyBundlePath,
					Path.Combine (frameworksBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle", ".monotouch-32"),
					Path.Combine (frameworksBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle", ".monotouch-64"),
					Path.Combine (frameworksBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle"),
					executableBundlePath,
				};
				TestFilesExists (baseDirs, coreFiles);
			}

			if (Platform == "iPhone") {
				var dSYMInfoPlist = Path.Combine (AppBundlePath + ".dSYM", "Contents", "Info.plist");
				var nativeExecutable = Path.Combine (AppBundlePath, appName);

				Assert.IsTrue (File.Exists (dSYMInfoPlist), "dSYM Info.plist file does not exist");
				Assert.IsTrue (File.GetLastWriteTimeUtc (dSYMInfoPlist) >= File.GetLastWriteTimeUtc (nativeExecutable), "dSYM Info.plist should be newer than the native executable");
			}

			return mtouchPaths;
		}

		protected void BuildCommonProject (string name)
		{
			BuildProject (Configuration.GetCommonProjectPath (name, isDotNet: false, platform: ApplePlatform));
		}

		protected void ExecuteProject ()
		{
			Configuration.ExecuteWithMagicWordAndAssert (MonoTouchProject.ProjectCSProjPath, ApplePlatform, isDotNetProject: false, projectConfiguration: Config, projectPlatform: Platform);
		}
	}
}
