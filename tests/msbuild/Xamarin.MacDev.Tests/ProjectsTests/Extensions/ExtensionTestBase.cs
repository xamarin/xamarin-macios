using System.IO;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.MacDev.Tasks {
	public class ExtensionTestBase : TestBase {
		public ExtensionTestBase () { }

		public ExtensionTestBase (string platform)
			: base (platform)
		{
		}

		public ProjectPaths BuildExtension (string hostAppName, string extensionName, int expectedErrorCount = 0)
		{
			var mtouchPaths = SetupProjectPaths (hostAppName);
			MonoTouchProject = mtouchPaths;

			string extensionPath = Path.Combine (AppBundlePath, "PlugIns", extensionName + ".appex");
			var proj = new ProjectPaths {
				AppBundlePath = extensionPath,
			};

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);
			Engine.ProjectCollection.SetGlobalProperty ("Configuration", Config);

			RunTarget (mtouchPaths, "Clean");
			Assert.IsFalse (Directory.Exists (AppBundlePath), "App bundle exists after cleanup: {0} ", AppBundlePath);

			RunTarget (mtouchPaths, "Build", expectedErrorCount: expectedErrorCount);

			if (expectedErrorCount > 0)
				return proj;

			Assert.IsTrue (Directory.Exists (AppBundlePath), "App Bundle does not exist: {0} ", AppBundlePath);

			TestPList (AppBundlePath, new string [] { "CFBundleExecutable", "CFBundleVersion" });

			Assert.IsTrue (Directory.Exists (extensionPath), "Appex directory does not exist: {0} ", extensionPath);

			TestPList (extensionPath, new string [] { "CFBundleExecutable", "CFBundleVersion" });

			TestFilesExists (AppBundlePath, ExpectedAppFiles);
			TestFilesDoNotExist (AppBundlePath, UnexpectedAppFiles);

			string [] coreFiles;
			var basedirs = new List<string> ();
			if (IsWatchOS) {
				basedirs.Add (extensionPath);
				coreFiles = GetCoreAppFiles (extensionName + ".dll", Path.GetFileNameWithoutExtension (extensionPath));
			} else {
				basedirs.Add (AppBundlePath);
				if (Platform == "iPhone") {
					basedirs.Add (Path.Combine (AppBundlePath, ".monotouch-32"));
					basedirs.Add (Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle"));
					basedirs.Add (Path.Combine (AppBundlePath, "Frameworks", "Xamarin.Sdk.framework", "MonoBundle", ".monotouch-32"));
				}
				coreFiles = GetCoreAppFiles (hostAppName + ".exe", hostAppName);
			}
			TestFilesExists (basedirs.ToArray (), coreFiles);

			return proj;
		}
	}
}
