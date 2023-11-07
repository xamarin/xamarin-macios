namespace Xamarin.Tests {
	[TestFixture]
	public class PublishTrimmedTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void DisableLinker (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["PublishTrimmed"] = "false";

			var rv = DotNet.AssertBuildFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			Assert.AreEqual (1, errors.Length, "Error count");
			var linkModeName = platform == ApplePlatform.MacOSX ? "LinkMode" : "MtouchLink";
			Assert.AreEqual ($"{platform.AsString ()} projects must build with PublishTrimmed=true. Current value: false. Set '{linkModeName}=None' instead to disable trimming for all assemblies.", errors [0].Message, "Error message");
		}
	}
}
