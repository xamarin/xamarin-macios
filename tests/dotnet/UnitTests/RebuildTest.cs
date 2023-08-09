namespace Xamarin.Tests {
	[TestFixture]
	public class RebuildTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void ReAotTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "RebuildTestAppWithLibraryReference";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["BuildCounter"] = "First";

			DotNet.AssertBuild (project_path, properties);

			var appExecutable = Path.Combine (appPath, "Contents", "MacOS", Path.GetFileNameWithoutExtension (project_path));

			Assert.That (appExecutable, Does.Exist, "There is an executable");
			if (CanExecute (platform, runtimeIdentifiers)) {
				var output = ExecuteWithMagicWordAndAssert (appExecutable);
				Assert.That (output, Does.Contain ("FIRSTBUILD"), "First build");
			}

			// Build again, changing something
			properties ["BuildCounter"] = "Second";
			DotNet.AssertBuild (project_path, properties);
			Assert.That (appExecutable, Does.Exist, "There is an executable");
			if (CanExecute (platform, runtimeIdentifiers)) {
				var output = ExecuteWithMagicWordAndAssert (appExecutable);
				Assert.That (output, Does.Contain ("SECONDBUILD"), "Second build");
			}
		}
	}
}

