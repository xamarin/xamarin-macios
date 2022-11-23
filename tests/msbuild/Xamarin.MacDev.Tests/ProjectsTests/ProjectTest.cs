using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.MacDev.Tasks {
	public class ProjectTest : TestBase {
		public ProjectTest (string platform)
			: base (platform)
		{
		}

		public ProjectTest (string platform, string config)
			: base (platform, config)
		{
		}

		public ProjectPaths BuildProject (string appName, int expectedErrorCount = 0, bool clean = true, bool nuget_restore = false, bool is_library = false, Dictionary<string, string> properties = null)
		{
			var mtouchPaths = SetupProjectPaths (appName);
			var csproj = mtouchPaths.ProjectCSProjPath;

			MonoTouchProject = mtouchPaths;
			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);
			Engine.ProjectCollection.SetGlobalProperty ("Configuration", Config);

			if (nuget_restore)
				NugetRestore (csproj);

			if (clean) {
				RunTarget (mtouchPaths, "Clean", Mode, properties: properties);
				Assert.IsFalse (Directory.Exists (AppBundlePath), "App bundle exists after cleanup: {0} ", AppBundlePath);
				Assert.IsFalse (Directory.Exists (AppBundlePath + ".dSYM"), "App bundle .dSYM exists after cleanup: {0} ", AppBundlePath + ".dSYM");
				Assert.IsFalse (Directory.Exists (AppBundlePath + ".mSYM"), "App bundle .mSYM exists after cleanup: {0} ", AppBundlePath + ".mSYM");

				var baseDir = Path.GetDirectoryName (csproj);
				var objDir = Path.Combine (baseDir, "obj", Platform, Config);
				var binDir = Path.Combine (baseDir, "bin", Platform, Config);

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

			RunTarget (mtouchPaths, "Build", Mode, expectedErrorCount, properties: properties);

			if (expectedErrorCount > 0 || is_library)
				return mtouchPaths;

			Assert.IsTrue (Directory.Exists (AppBundlePath), "App Bundle does not exist: {0} ", AppBundlePath);

			TestFilesExists (AppBundlePath, ExpectedAppFiles);
			TestFilesDoNotExist (AppBundlePath, UnexpectedAppFiles);

			if (Mode != ExecutionMode.DotNet) {
				var coreFiles = GetCoreAppFiles (appName.Replace (" ", "") + ".exe", appName.Replace (" ", ""));
				var baseDirs = new string [] {
				Path.Combine (AppBundlePath, ".monotouch-32"),
				Path.Combine (AppBundlePath, ".monotouch-64"),
				AppBundlePath,
				Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle", ".monotouch-32"),
				Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle", ".monotouch-64"),
				Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle"),
			};
				TestFilesExists (baseDirs, coreFiles);
			}

			if (Platform == "iPhone") {
				var dSYMInfoPlist = Path.Combine (AppBundlePath + ".dSYM", "Contents", "Info.plist");
				var nativeExecutable = Path.Combine (AppBundlePath, appName);

				Assert.That (dSYMInfoPlist, Does.Exist, "dSYM Info.plist file does not exist");
				Assert.That (File.GetLastWriteTimeUtc (dSYMInfoPlist), Is.GreaterThanOrEqualTo (File.GetLastWriteTimeUtc (nativeExecutable)), $"dSYM Info.plist ({dSYMInfoPlist}) should be newer than the native executable ({nativeExecutable})");
			}

			return mtouchPaths;
		}
	}
}
