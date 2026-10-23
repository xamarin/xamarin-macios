#nullable enable

namespace Xamarin.Tests {
	public class PartialAppManifestTest : TestBaseClass {

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void ValidateSceneManifest (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var projectPath = GetProjectPath ("MyPartialAppManifestApp", platform: platform);
			Clean (projectPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			const string target = "_CompileAppManifest";

			// The shared partial manifest supplies the scene configuration.
			DotNet.AssertBuild (projectPath, properties, target: target);

			// AppManifestEntry is applied after partial manifests and must also be validated.
			properties ["ManifestKey"] = "UIApplicationSceneManifest";
			properties ["ManifestValue"] = "invalid";
			var result = DotNet.AssertBuildFailure (projectPath, properties, target: target);
			var errors = BinLog.GetBuildLogErrors (result.BinLogPath).ToArray ();
			Assert.That (errors.Length, Is.EqualTo (1), "Errors");
			Assert.That (errors [0].Message, Does.Contain ("https://aka.ms/uiscene"), "Migration instructions");

			// A failed compilation must not leave an output that bypasses validation on the next build.
			DotNet.AssertBuildFailure (projectPath, properties, target: target);

			properties ["ValidateSceneManifest"] = "false";
			DotNet.AssertBuild (projectPath, properties, target: target);

			// Re-enabling validation must invalidate the previously successful output.
			properties ["ValidateSceneManifest"] = "true";
			result = DotNet.AssertBuildFailure (projectPath, properties, target: target);
			errors = BinLog.GetBuildLogErrors (result.BinLogPath).ToArray ();
			Assert.That (errors.Length, Is.EqualTo (1), "Re-enabled errors");
			Assert.That (errors [0].Message, Does.Contain ("https://aka.ms/uiscene"), "Re-enabled validation");

			properties.Remove ("ManifestKey");
			properties.Remove ("ManifestValue");
			DotNet.AssertBuild (projectPath, properties, target: target);
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void Build (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MyPartialAppManifestApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			DotNet.AssertBuild (project_path, properties);

			var infoPlistPath = GetInfoPListPath (platform, appPath);
			var infoPlist = PDictionary.OpenFile (infoPlistPath);
			Assert.That (infoPlist.GetString ("CFBundleIdentifier").Value, Is.EqualTo ("com.xamarin.mypartialappmanifestapp"), "CFBundleIdentifier");
			Assert.That (infoPlist.GetString ("CFBundleDisplayName").Value, Is.EqualTo ("MyPartialAppManifestApp"), "CFBundleDisplayName");
			Assert.That (infoPlist.GetString ("CFBundleVersion").Value, Is.EqualTo ("3.14"), "CFBundleVersion");
			Assert.That (infoPlist.GetString ("CFBundleShortVersionString").Value, Is.EqualTo ("3.14"), "CFBundleShortVersionString");
			Assert.That (infoPlist.GetString ("Something").Value, Is.EqualTo ("SomeValue"), "Something");
			Assert.That (infoPlist.GetString ("UILaunchStoryboardName").Value, Is.EqualTo ("LaunchScreen"), "UILaunchStoryboardName");
			Assert.That (infoPlist.GetString ("a|String").Value, Is.EqualTo ("b"), "Delimiter value");

			var partialAppManifestPath = Path.Combine (Path.GetDirectoryName (project_path)!, "..", "Partial.plist");
			Configuration.Touch (partialAppManifestPath);
			var rv = DotNet.AssertBuild (project_path, GetDefaultProperties (runtimeIdentifiers));
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_CompileAppManifest", "_CompileAppManifest rebuild");

			rv = DotNet.AssertBuild (project_path, GetDefaultProperties (runtimeIdentifiers));
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_CompileAppManifest", "_CompileAppManifest rebuild 2");

			properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["ManifestKey"] = "a";
			properties ["ManifestValue"] = "String|b";
			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_CompileAppManifest", "_CompileAppManifest rebuild 3");

			infoPlist = PDictionary.OpenFile (infoPlistPath);
			Assert.That (infoPlist.GetString ("UILaunchStoryboardName").Value, Is.EqualTo ("LaunchScreen"), "UILaunchStoryboardName unchanged");
			Assert.That (infoPlist.ContainsKey ("a|String"), Is.False, "Old delimiter key");
			Assert.That (infoPlist.GetString ("a").Value, Is.EqualTo ("String|b"), "Updated delimiter value");

			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_CompileAppManifest", "_CompileAppManifest rebuild 4");
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		public void AppWithLibraryWithResourcesReference (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "AppWithLibraryWithResourcesReference";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			var library = "LibraryWithResources";
			var library_project_path = GetProjectPath (library, platform: platform);
			Clean (library_project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["BundleOriginalResources"] = "true";

			DotNet.AssertBuild (project_path, properties);

			var infoPlistPath = GetInfoPListPath (platform, appPath);
			var infoPlist = PDictionary.OpenFile (infoPlistPath);
			Assert.That (infoPlist.GetString ("LibraryWithResources").Value, Is.EqualTo ("Here I am"), "LibraryWithResources 1");

			// build again, nothing should change
			DotNet.AssertBuild (project_path, properties);

			infoPlist = PDictionary.OpenFile (infoPlistPath);
			Assert.That (infoPlist.GetString ("LibraryWithResources").Value, Is.EqualTo ("Here I am"), "LibraryWithResources 2");
		}
	}
}
