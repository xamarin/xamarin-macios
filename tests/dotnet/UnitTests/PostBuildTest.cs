using Microsoft.Build.Framework;
using Microsoft.Build.Logging.StructuredLogger;
using Mono.Cecil;

namespace Xamarin.Tests {
	[TestFixture]
	public class PostBuildTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void ArchiveTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["ArchiveOnBuild"] = "true";
			properties ["Configuration"] = configuration;

			var result = DotNet.AssertBuild (project_path, properties);
			var recordArgs = BinLog.ReadBuildEvents (result.BinLogPath).ToList ();
			var findString = "Output Property: ArchiveDir";
			var archiveDirRecord = recordArgs.Where (v => v?.Message?.Contains (findString) == true).ToList ();
			Assert.That (archiveDirRecord.Count, Is.GreaterThan (0), "ArchiveDir");
			var archiveDir = archiveDirRecord [0].Message?.Substring (findString.Length + 1)?.Trim ();
			Assert.That (archiveDir, Does.Exist, "Archive directory existence");
			AssertDSymDirectory (appPath);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void BuildIpaTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["BuildIpa"] = "true";
			properties ["Configuration"] = configuration;

			DotNet.AssertBuild (project_path, properties);

			var pkgPath = Path.Combine (appPath, "..", $"{project}.ipa");
			Assert.That (pkgPath, Does.Exist, "pkg creation");

			AssertBundleAssembliesStripStatus (appPath, true);
			AssertDSymDirectory (appPath);
		}

		[Test]
		[TestCase ("MySimpleApp", ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase ("MySimpleApp", ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase ("MySimpleAppWithSatelliteReference", ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase ("MySimpleAppWithSatelliteReference", ApplePlatform.iOS, "ios-arm64", false)]
		public void AssemblyStripping (string project, ApplePlatform platform, string runtimeIdentifiers, bool shouldStrip)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			// Force EnableAssemblyILStripping since we are building debug which never will by default
			properties ["EnableAssemblyILStripping"] = shouldStrip ? "true" : "false";

			DotNet.AssertBuild (project_path, properties);

			AssertBundleAssembliesStripStatus (appPath, shouldStrip);
			Assert.That (Path.Combine (appPath, $"{project}.dll"), Does.Exist, "Application Assembly");
			Assert.That (Path.Combine (appPath, "Microsoft.iOS.dll"), Does.Exist, "Platform Assembly");
		}

		[Test]
		[TestCase ("MySimpleApp", ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase ("MySimpleApp", ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase ("MySimpleApp", ApplePlatform.MacOSX, "osx-x64")]
		[TestCase ("MySimpleApp", ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		[TestCase ("MySimpleAppWithSatelliteReference", ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase ("MySimpleAppWithSatelliteReference", ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase ("MySimpleAppWithSatelliteReference", ApplePlatform.MacOSX, "osx-x64")]
		[TestCase ("MySimpleAppWithSatelliteReference", ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void BuildPackageTest (string project, ApplePlatform platform, string runtimeIdentifiers)
		{
			var projectVersion = "3.14";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["CreatePackage"] = "true";

			DotNet.AssertBuild (project_path, properties);

			var pkgPath = Path.Combine (appPath, "..", $"{project}-{projectVersion}.pkg");
			Assert.That (pkgPath, Does.Exist, "pkg creation");
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.iOS, "ios-arm64;ios-arm")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void PublishTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			string packageExtension;
			string pathVariable;
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				packageExtension = "ipa";
				pathVariable = "IpaPackagePath";
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				packageExtension = "pkg";
				pathVariable = "PkgPackagePath";
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var pkgPath = Path.Combine (tmpdir, $"MyPackage.{packageExtension}");

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties [pathVariable] = pkgPath;

			DotNet.AssertPublish (project_path, properties);

			Assert.That (pkgPath, Does.Exist, "ipa/pkg creation");
		}


		[TestCase (ApplePlatform.iOS, "iossimulator-x64")]
		[TestCase (ApplePlatform.iOS, "iossimulator-x86")]
		[TestCase (ApplePlatform.iOS, "iossimulator-x64;iossimulator-x64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64")]
		public void PublishFailureTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			string packageExtension;
			string pathVariable;
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				packageExtension = "ipa";
				pathVariable = "IpaPackagePath";
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				packageExtension = "pkg";
				pathVariable = "PkgPackagePath";
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var pkgPath = Path.Combine (tmpdir, $"MyPackage.{packageExtension}");

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties [pathVariable] = pkgPath;

			var rv = DotNet.AssertPublishFailure (project_path, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			Assert.AreEqual (1, errors.Length, "Error Count");
			string expectedErrorMessage;
			if (runtimeIdentifiers.IndexOf (';') >= 0) {
				expectedErrorMessage = $"A runtime identifier for a device architecture must be specified in order to publish this project. '{runtimeIdentifiers}' are simulator architectures.";
			} else {
				expectedErrorMessage = $"A runtime identifier for a device architecture must be specified in order to publish this project. '{runtimeIdentifiers}' is a simulator architecture.";
			}
			Assert.AreEqual (expectedErrorMessage, errors [0].Message, "Error Message");

			Assert.That (pkgPath, Does.Not.Exist, "ipa/pkg creation");
		}
	}
}
