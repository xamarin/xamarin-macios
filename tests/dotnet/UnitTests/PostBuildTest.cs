using System.IO.Compression;
using System.Text.Json;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging.StructuredLogger;
using Mono.Cecil;

namespace Xamarin.Tests {
	[TestFixture]
	public class PostBuildTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
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
			var findString2 = "TaskOutput: ArchiveDir";
			var archiveDirs = recordArgs.
				Select (v => v?.Message).
				Select (v => {
					if (v is null)
						return null;
					if (v.Contains (findString))
						return v.Substring (findString.Length + 1);
					if (v.Contains (findString2))
						return v.Substring (findString2.Length + 1);
					return null;
				}).
				Where (v => v is not null).
				ToList ();
			Assert.That (archiveDirs.Count, Is.GreaterThan (0), "ArchiveDir");
			var archiveDir = archiveDirs [0]!.Trim ();
			Assert.That (archiveDir, Does.Exist, "Archive directory existence");
			AssertApplicationArtifact (result.BinLogPath, archiveDir, platform, "xcarchive", isDirectory: true);
			AssertDSymDirectory (appPath);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void BuildIpaTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			BuildIpaTestImpl (platform, runtimeIdentifiers);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void CoreCLR_ConvertedFrameworks_HaveInfoPlist (ApplePlatform platform, string runtimeIdentifiers)
		{
			// Ref: https://github.com/dotnet/macios/issues/25248
			// Verify that converted CoreCLR dylib frameworks have a valid Info.plist
			// with the correct CFBundleIdentifier, so that codesign signs them as
			// framework bundles and uses the plist bundle identifier.
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["BuildIpa"] = "true";
			properties ["Configuration"] = configuration;
			properties ["UseMonoRuntime"] = "false";

			DotNet.AssertBuild (project_path, properties);

			var frameworksDir = Path.Combine (appPath, "Frameworks");
			Assert.That (frameworksDir, Does.Exist, "Frameworks directory should exist for CoreCLR device builds");

			var frameworkDirs = Directory.GetDirectories (frameworksDir, "*.framework");
			Assert.That (frameworkDirs.Length, Is.GreaterThan (0), "Expected at least one .framework directory");

			// Read the main app's bundle identifier
			var appInfoPlistPath = Path.Combine (appPath, "Info.plist");
			Assert.That (appInfoPlistPath, Does.Exist, "App Info.plist should exist");
			var appPlist = PDictionary.FromFile (appInfoPlistPath);
			Assert.That (appPlist, Is.Not.Null, $"Failed to parse Info.plist at '{appInfoPlistPath}'");
			var bundleIdentifierValue = appPlist!.GetString ("CFBundleIdentifier");
			Assert.That (bundleIdentifierValue, Is.Not.Null, $"CFBundleIdentifier should exist in '{appInfoPlistPath}'");
			var bundleIdentifier = bundleIdentifierValue!.Value;

			foreach (var fwDir in frameworkDirs) {
				var fwName = Path.GetFileNameWithoutExtension (fwDir);
				var infoPlistPath = Path.Combine (fwDir, "Info.plist");
				Assert.That (infoPlistPath, Does.Exist, $"Info.plist should exist in {fwName}.framework");

				var plist = PDictionary.FromFile (infoPlistPath);
				Assert.That (plist, Is.Not.Null, $"Failed to parse Info.plist at '{infoPlistPath}'");
				var fwBundleId = plist!.GetString ("CFBundleIdentifier")?.Value;
				Assert.That (fwBundleId, Does.StartWith (bundleIdentifier + "."), $"CFBundleIdentifier for {fwName}.framework should start with the app bundle identifier");

				var bundleExe = plist.GetString ("CFBundleExecutable")?.Value;
				Assert.That (bundleExe, Is.EqualTo (fwName), $"CFBundleExecutable for {fwName}.framework");

				var packageType = plist.GetString ("CFBundlePackageType")?.Value;
				Assert.That (packageType, Is.EqualTo ("FMWK"), $"CFBundlePackageType for {fwName}.framework");

				// Verify the executable binary actually exists
				Assert.That (Path.Combine (fwDir, fwName), Does.Exist, $"Executable should exist in {fwName}.framework");

				// If the framework was signed, verify it was signed as a bundle (not a bare Mach-O)
				// and that the codesign identifier matches the Info.plist CFBundleIdentifier
				var codeSignatureDir = Path.Combine (fwDir, "_CodeSignature");
				if (Directory.Exists (codeSignatureDir)) {
					var exitCode = ExecutionHelper.Execute ("/usr/bin/codesign", new string [] { "-dvvv", fwDir }, out var codesignOutput);
					var output = codesignOutput.ToString ();
					Assert.That (exitCode, Is.EqualTo (0), $"codesign failed for framework {fwName}. Codesign output:\n{output}");
					Assert.That (output, Does.Contain ("Format=bundle with Mach-O"), $"Framework {fwName} should be signed as a bundle, not a bare Mach-O. Codesign output:\n{output}");
					Assert.That (output, Does.Contain ($"Identifier={fwBundleId}"), $"Framework {fwName} codesign identifier should match its Info.plist CFBundleIdentifier. Codesign output:\n{output}");
				}
			}
		}

		void BuildIpaTestImpl (ApplePlatform platform, string runtimeIdentifiers)
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
			properties ["UseMonoRuntime"] = "false";

			var result = DotNet.AssertBuild (project_path, properties);

			var pkgPath = Path.Combine (appPath, "..", $"{project}.ipa");
			Assert.That (pkgPath, Does.Exist, "pkg creation");
			AssertApplicationArtifact (result.BinLogPath, appPath, platform, "app", isDirectory: true);
			AssertApplicationArtifact (result.BinLogPath, pkgPath, platform, "ipa", isDirectory: false);

			// With CoreCLR (R2R), assemblies retain their IL bodies.
			AssertBundleAssembliesStripStatus (appPath, false);
			AssertDSymDirectory (appPath);

			// IpaIncludeSymbols defaults to true, so the .ipa must contain a populated 'Symbols' directory.
			AssertIpaSymbols (pkgPath, shouldContainSymbols: true);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void BuildIpaWithoutSymbolsTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["BuildIpa"] = "true";
			properties ["IpaIncludeSymbols"] = "false";
			properties ["Configuration"] = configuration;

			var result = DotNet.AssertBuild (project_path, properties);

			var pkgPath = Path.Combine (appPath, "..", $"{project}.ipa");
			Assert.That (pkgPath, Does.Exist, "pkg creation");

			// IpaIncludeSymbols=false, so the .ipa must not contain a 'Symbols' directory.
			AssertIpaSymbols (pkgPath, shouldContainSymbols: false);
		}

		static void AssertIpaSymbols (string ipaPath, bool shouldContainSymbols)
		{
			using var archive = ZipFile.OpenRead (ipaPath);
			var symbolFiles = archive.Entries.
				Where (entry => entry.FullName.StartsWith ("Symbols/", StringComparison.Ordinal) && entry.FullName.EndsWith (".symbols", StringComparison.Ordinal)).
				ToList ();
			if (shouldContainSymbols) {
				Assert.That (symbolFiles, Is.Not.Empty, "The .ipa should contain Symbols/*.symbols files.");
			} else {
				Assert.That (symbolFiles, Is.Empty, "The .ipa should not contain any Symbols/*.symbols files.");
			}
		}

		[Test]
		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void BuildIpaAndArchiveOnRemoteWindowsTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			Configuration.IgnoreIfNotOnWindows ();

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			// Set both BuildIpa and ArchiveOnBuild so the (slow) remote build to the paired Mac runs only
			// once while still verifying that both the .ipa and the .xcarchive are surfaced.
			properties ["BuildIpa"] = "true";
			properties ["ArchiveOnBuild"] = "true";
			properties ["Configuration"] = configuration;

			var result = DotNet.AssertBuild (project_path, properties, timeout: TimeSpan.FromMinutes (25));

			// The .ipa is built on the paired Mac and copied back to this Windows machine, so it must be
			// surfaced in @(ApplicationArtifact) with the local (Windows) path. The .app bundle stays on the
			// Mac, so it's intentionally not surfaced as an artifact on Windows.
			var ipaPath = Path.Combine (appPath, "..", $"{project}.ipa");
			Assert.That (ipaPath, Does.Exist, "ipa creation");
			AssertApplicationArtifact (result.BinLogPath, ipaPath, platform, "ipa", isDirectory: false);

			// The .xcarchive is produced on the paired Mac and copied back to this Windows machine by
			// CopyArchiveFromMac, which sets $(ArchivePath) to the local (Windows) path while $(ArchiveDir)
			// keeps pointing at the Mac path. The Windows archive path is date-based, so match on the
			// package format rather than an exact path.
			AssertApplicationArtifact (result.BinLogPath, platform, "xcarchive", isDirectory: true);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void GetApplicationArtifactsIpaTest (ApplePlatform platform, string runtimeIdentifiers)
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

			var outputs = GetApplicationArtifacts (project_path, properties);
			var pkgPath = Path.Combine (appPath, "..", $"{project}.ipa");

			Assert.That (pkgPath, Does.Exist, "pkg creation");
			AssertApplicationArtifact (outputs, appPath, platform, "app", isDirectory: true);
			AssertApplicationArtifact (outputs, pkgPath, platform, "ipa", isDirectory: false);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", null, true)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", null, true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", null, true)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64", null, true)]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", "Info.plist", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64", "Info.plist", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", "InfoWithoutDisplayName.plist", false)]
		public void ApplicationArtifactMetadataTest (ApplePlatform platform, string runtimeIdentifiers, string? customApplicationManifest, bool generateApplicationManifest)
		{
			var project = "MySimpleAppWithArtifactMetadata";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["GenerateApplicationManifest"] = generateApplicationManifest ? "true" : "false";
			if (customApplicationManifest is not null)
				properties ["CustomApplicationManifest"] = customApplicationManifest;

			var outputs = GetApplicationArtifacts (project_path, properties);
			var appOutput = AssertApplicationArtifact (outputs, appPath, platform, "app", isDirectory: true);

			if (customApplicationManifest is null) {
				AssertApplicationMetadata (
					appOutput,
					"com.xamarin.mysimpleappwithartifactmetadata",
					"MySimpleAppWithArtifactMetadata",
					"MySimpleAppWithArtifactMetadata",
					"3.14",
					"3.14");
			} else if (customApplicationManifest == "InfoWithoutDisplayName.plist") {
				AssertApplicationMetadata (
					appOutput,
					"com.xamarin.customartifactmetadata",
					"",
					"Fallback Bundle Name",
					"9.8.7",
					"123");
			} else {
				AssertApplicationMetadata (
					appOutput,
					"com.xamarin.customartifactmetadata",
					"$(PRODUCT_NAME)",
					"$(PRODUCT_NAME)",
					"9.8.7",
					"123");
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "ipa")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", "pkg")]
		public void GetApplicationArtifactsDependsOnTest (ApplePlatform platform, string runtimeIdentifiers, string packageFormat)
		{
			var project = "MySimpleAppWithArtifactMetadata";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["BuildIpa"] = packageFormat == "ipa" ? "true" : "false";
			properties ["CreatePackage"] = packageFormat == "pkg" ? "true" : "false";
			properties ["Configuration"] = configuration;
			properties ["ExpectedAugmentedPackageFormat"] = packageFormat;

			var outputs = GetApplicationArtifacts (project_path, properties);
			var appOutput = AssertApplicationArtifact (outputs, appPath, platform, "app", isDirectory: true);
			var packageOutput = AssertApplicationArtifact (outputs, platform, packageFormat, isDirectory: false);
			Assert.That (GetMetadata (appOutput, "ApplicationTitle"), Is.EqualTo ("My MAUI App"), "ApplicationTitle");
			Assert.That (GetMetadata (appOutput, "MauiObservedPackageFormat"), Is.EqualTo ("app"), "MauiObservedPackageFormat");
			Assert.That (GetMetadata (packageOutput, "ApplicationTitle"), Is.EqualTo ("My MAUI App"), "ApplicationTitle");
			Assert.That (GetMetadata (packageOutput, "MauiObservedPackageFormat"), Is.EqualTo (packageFormat), "MauiObservedPackageFormat");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void GetApplicationArtifactsArchiveTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform, out _, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["ArchiveOnBuild"] = "true";
			properties ["Configuration"] = configuration;

			var outputs = GetApplicationArtifacts (project_path, properties);
			AssertApplicationArtifact (outputs, platform, "xcarchive", isDirectory: true);
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

			var result = DotNet.AssertBuild (project_path, properties);

			var pkgPath = Path.Combine (appPath, "..", $"{project}-{projectVersion}.pkg");
			Assert.That (pkgPath, Does.Exist, "pkg creation");
			AssertApplicationArtifact (result.BinLogPath, appPath, platform, "app", isDirectory: true);
			AssertApplicationArtifact (result.BinLogPath, pkgPath, platform, "pkg", isDirectory: false);
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		public void GetApplicationArtifactsPkgTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			var projectVersion = "3.14";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["CreatePackage"] = "true";

			var outputs = GetApplicationArtifacts (project_path, properties);
			var pkgPath = Path.Combine (appPath, "..", $"{project}-{projectVersion}.pkg");

			Assert.That (pkgPath, Does.Exist, "pkg creation");
			AssertApplicationArtifact (outputs, appPath, platform, "app", isDirectory: true);
			AssertApplicationArtifact (outputs, pkgPath, platform, "pkg", isDirectory: false);
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64;maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64;osx-x64")]
		public void PublishTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			PublishTestImpl (platform, runtimeIdentifiers);
		}

		void PublishTestImpl (ApplePlatform platform, string runtimeIdentifiers)
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
			properties ["UseMonoRuntime"] = "false";
			properties [pathVariable] = pkgPath;

			var result = DotNet.AssertPublish (project_path, properties);

			Assert.That (pkgPath, Does.Exist, "ipa/pkg creation");
			AssertApplicationArtifact (result.BinLogPath, pkgPath, platform, packageExtension, isDirectory: false);
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64", "ipa", "IpaPackagePath")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", "pkg", "PkgPackagePath")]
		public void PublishApplicationArtifactsDependsOnTest (ApplePlatform platform, string runtimeIdentifiers, string packageFormat, string pathVariable)
		{
			var project = "MySimpleAppWithArtifactMetadata";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var pkgPath = Path.Combine (tmpdir, $"MyPackage.{packageFormat}");

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties [pathVariable] = pkgPath;
			properties ["Configuration"] = configuration;
			properties ["ExpectedAugmentedPackageFormat"] = packageFormat;

			var outputs = GetApplicationArtifacts (project_path, properties, target: "Publish");
			var appOutput = AssertApplicationArtifact (outputs, appPath, platform, "app", isDirectory: true);
			var packageOutput = AssertApplicationArtifact (outputs, pkgPath, platform, packageFormat, isDirectory: false);
			Assert.That (GetMetadata (appOutput, "ApplicationTitle"), Is.EqualTo ("My MAUI App"), "ApplicationTitle");
			Assert.That (GetMetadata (appOutput, "MauiObservedPackageFormat"), Is.EqualTo ("app"), "MauiObservedPackageFormat");
			Assert.That (GetMetadata (packageOutput, "ApplicationTitle"), Is.EqualTo ("My MAUI App"), "ApplicationTitle");
			Assert.That (GetMetadata (packageOutput, "MauiObservedPackageFormat"), Is.EqualTo (packageFormat), "MauiObservedPackageFormat");
		}


		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64;iossimulator-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
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
			Assert.That (errors.Length, Is.EqualTo (1), "Error Count");
			string expectedErrorMessage;
			if (runtimeIdentifiers.IndexOf (';') >= 0) {
				expectedErrorMessage = $"A runtime identifier for a device architecture must be specified in order to publish this project. '{runtimeIdentifiers}' are simulator architectures.";
			} else {
				expectedErrorMessage = $"A runtime identifier for a device architecture must be specified in order to publish this project. '{runtimeIdentifiers}' is a simulator architecture.";
			}
			Assert.That (errors [0].Message, Is.EqualTo (expectedErrorMessage), "Error Message");

			Assert.That (pkgPath, Does.Not.Exist, "ipa/pkg creation");
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void DefaultPublishRid (ApplePlatform platform, string expectedRuntimeIdentifier)
		{
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, expectedRuntimeIdentifier);

			var project_path = GetProjectPath (project, expectedRuntimeIdentifier, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);

			var properties = GetDefaultProperties ();
			var rv = DotNet.AssertPublish (project_path, properties);
			Assert.That (appPath, Does.Exist, "App existence");
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void PublishRuntimeIdentifierNotAppendedToRuntimeIdentifiers (ApplePlatform platform, string expectedPublishRuntimeIdentifier)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);

			var properties = GetDefaultProperties ();
			var publishRuntimeIdentifier = DotNet.GetProperty (project_path, "PublishRuntimeIdentifier", properties);
			var runtimeIdentifiers = DotNet.GetProperty (project_path, "RuntimeIdentifiers", properties);

			Assert.That (publishRuntimeIdentifier, Is.EqualTo (expectedPublishRuntimeIdentifier), "PublishRuntimeIdentifier");
			// We use RuntimeIdentifiers to mean "build for all these RIDs", so PublishRuntimeIdentifier must not be
			// appended to RuntimeIdentifiers (this used to confuse our build): https://github.com/dotnet/macios/issues/24547
			Assert.That (runtimeIdentifiers, Does.Not.Contain (expectedPublishRuntimeIdentifier), "RuntimeIdentifiers");
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void PublishRuntimeIdentifierEscapeHatchesAreAlwaysSet (ApplePlatform platform, string runtimeIdentifier)
		{
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);

			var properties = GetDefaultProperties ();
			properties ["RuntimeIdentifier"] = runtimeIdentifier;

			// These properties opt out of SDK behavior around PublishRuntimeIdentifier, and must be set even when the
			// user specified a RuntimeIdentifier, otherwise the SDK might compute a default PublishRuntimeIdentifier
			// (the host portable RID) and append it to RuntimeIdentifiers, which confuses our build:
			// https://github.com/dotnet/macios/issues/24547
			var useDefaultPublishRuntimeIdentifier = DotNet.GetProperty (project_path, "UseDefaultPublishRuntimeIdentifier", properties);
			var appendPublishRuntimeIdentifierToRuntimeIdentifiers = DotNet.GetProperty (project_path, "AppendPublishRuntimeIdentifierToRuntimeIdentifiers", properties);

			Assert.That (useDefaultPublishRuntimeIdentifier, Is.EqualTo ("false"), "UseDefaultPublishRuntimeIdentifier");
			Assert.That (appendPublishRuntimeIdentifierToRuntimeIdentifiers, Is.EqualTo ("false"), "AppendPublishRuntimeIdentifierToRuntimeIdentifiers");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		public void DylibPostProcessingItems (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "NativeDynamicLibraryReferencesApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			var result = DotNet.AssertBuild (project_path, properties);
			var postProcessingItems = GetPostProcessingItems (result.BinLogPath);

			// Find the user's dylib item (not SDK runtime dylibs)
			var dylibItems = postProcessingItems.Where (i => i.ItemSpec.Contains ("libframework.dylib")).ToList ();
			Assert.That (dylibItems.Count, Is.EqualTo (1), $"Expected 1 libframework.dylib post-processing item, got {dylibItems.Count}. All items:\n\t{string.Join ("\n\t", postProcessingItems.Select (i => i.ItemSpec))}");
			var dylibItem = dylibItems [0];

			// Verify the path does NOT contain ".framework/" (the bug was that dylibs were treated as frameworks)
			Assert.That (dylibItem.ItemSpec, Does.Not.Contain (".framework/"), "Dylib path should not contain .framework/");

			// Verify the path contains the full dylib filename
			Assert.That (dylibItem.ItemSpec, Does.Contain ("libframework.dylib"), "Dylib path should contain the full dylib filename");

			// Verify the DSymName is correct for a dylib (should be "libframework.dSYM", not "libframework.dylib.dSYM")
			var dSymName = dylibItem.GetMetadata ("DSymName");
			Assert.That (dSymName, Is.EqualTo ("libframework.dSYM"), "DSymName for dylib");

			// Verify dSYMSourcePath points to where a pre-existing dSYM would be for the dylib.
			// For a dylib at /path/to/libfoo.dylib, the dSYMSourcePath should be /path/to/libfoo.dylib.dSYM
			var dSYMSourcePath = dylibItem.GetMetadata ("dSYMSourcePath");
			var itemSourcePath = dylibItem.GetMetadata ("ItemSourcePath");
			Assert.That (dSYMSourcePath, Is.EqualTo (itemSourcePath + ".dSYM"), "dSYMSourcePath for dylib");

			// Debug builds don't generate dSYMs, verify none exist
			var appContainerDir = Path.GetDirectoryName (appPath)!;
			var dSymDirs = Directory.GetDirectories (appContainerDir, "*.dSYM");
			Assert.That (dSymDirs, Is.Empty, "No dSYMs should exist for Debug builds");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		public void FrameworkPostProcessingItems (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "NativeFrameworkReferencesApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			var result = DotNet.AssertBuild (project_path, properties);
			var postProcessingItems = GetPostProcessingItems (result.BinLogPath);

			// Find the framework item (XTest.framework is the dynamic framework)
			var frameworkItems = postProcessingItems.Where (i => i.ItemSpec.Contains ("XTest.framework/XTest")).ToList ();
			Assert.That (frameworkItems.Count, Is.EqualTo (1), $"Expected 1 XTest framework post-processing item, got {frameworkItems.Count}. All items:\n\t{string.Join ("\n\t", postProcessingItems.Select (i => i.ItemSpec))}");
			var frameworkItem = frameworkItems [0];

			// Verify the DSymName is correct for a framework (should be "XTest.framework.dSYM")
			var dSymName = frameworkItem.GetMetadata ("DSymName");
			Assert.That (dSymName, Is.EqualTo ("XTest.framework.dSYM"), "DSymName for framework");

			// Verify dSYMSourcePath points to where a pre-existing dSYM would be for the framework.
			// For a framework at /path/to/XTest.framework/XTest, the dSYMSourcePath should be /path/to/XTest.framework.dSYM
			var dSYMSourcePath = frameworkItem.GetMetadata ("dSYMSourcePath");
			var itemSourcePath = frameworkItem.GetMetadata ("ItemSourcePath");
			Assert.That (dSYMSourcePath, Is.EqualTo (Path.GetDirectoryName (itemSourcePath) + ".dSYM"), "dSYMSourcePath for framework");

			// Debug builds don't generate dSYMs, verify none exist
			var appContainerDir = Path.GetDirectoryName (appPath)!;
			var dSymDirs = Directory.GetDirectories (appContainerDir, "*.dSYM");
			Assert.That (dSymDirs, Is.Empty, "No dSYMs should exist for Debug builds");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64", "Release")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64", "Release")]
		public void BundleStructureDSyms (ApplePlatform platform, string runtimeIdentifiers, string configuration)
		{
			var project = "BundleStructure";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			properties ["_IsAppSigned"] = "true";
			// macOS and Mac Catalyst default to NoDSymUtil=true (dSYMs only generated when archiving),
			// so explicitly disable it to test dSYM generation.
			properties ["NoDSymUtil"] = "false";

			DotNet.AssertBuild (project_path, properties);

			AssertExpectedDSyms (platform, appPath);
		}

		[Test]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void NativeAotGeneratesDSymsByDefault (ApplePlatform platform, string runtimeIdentifiers)
		{
			// https://github.com/dotnet/macios/issues/21553
			// macOS and Mac Catalyst don't generate dSYMs by default, except when using Native AOT.
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			properties ["PublishAot"] = "true";
			properties ["_IsPublishing"] = "true";
			// Note: NoDSymUtil is intentionally not set, to verify dSYMs are generated by default with Native AOT.

			DotNet.AssertPublish (project_path, properties);

			AssertExpectedDSyms (platform, appPath);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		public void StaticFrameworksNotInPostProcessing (ApplePlatform platform, string runtimeIdentifiers)
		{
			// https://github.com/dotnet/macios/issues/24840
			// This test does a Release build, which enables dsymutil/strip post-processing.
			// Without the fix, the build would fail because dsymutil would try to process a
			// static framework that is not present in the app bundle.
			var project = "StaticFrameworkFilterApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			// macOS and Mac Catalyst default to NoDSymUtil=true (dSYMs only generated when archiving),
			// so explicitly disable it to test dSYM generation.
			properties ["NoDSymUtil"] = "false";

			var result = DotNet.AssertBuild (project_path, properties);
			var postProcessingItems = GetPostProcessingItems (result.BinLogPath);

			// The dynamic framework (XTest) should be in the post-processing items
			var dynamicFrameworkItems = postProcessingItems.Where (i => i.ItemSpec.Contains ("XTest.framework/XTest")).ToList ();
			Assert.That (dynamicFrameworkItems.Count, Is.EqualTo (1), $"Expected 1 XTest framework post-processing item, got {dynamicFrameworkItems.Count}. All items:\n\t{string.Join ("\n\t", postProcessingItems.Select (i => i.ItemSpec))}");

			// The static framework (XStaticArTest) should NOT be in the post-processing items,
			// because it's a static library and won't be in the app bundle.
			var staticFrameworkItems = postProcessingItems.Where (i => i.ItemSpec.Contains ("XStaticArTest")).ToList ();
			Assert.That (staticFrameworkItems, Is.Empty, $"Static framework XStaticArTest should not be in post-processing items. All items:\n\t{string.Join ("\n\t", postProcessingItems.Select (i => i.ItemSpec))}");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		public void PublishDSymToPublishDirectory (ApplePlatform platform, string runtimeIdentifiers, bool copyDSym)
		{
			// https://github.com/dotnet/macios/issues/15384
			// When publishing, the generated *.dSYM directories should be copied to the publish
			// directory (unless CopyDSYMToPublishDirectory=false).
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			if (!copyDSym)
				properties ["CopyDSYMToPublishDirectory"] = "false";

			DotNet.AssertPublish (project_path, properties);

			var appContainerDir = Path.GetDirectoryName (appPath)!;
			var appBundleName = Path.GetFileName (appPath);
			var publishDir = Path.Combine (appContainerDir, "publish");

			// The dSYM must have been generated next to the app bundle in the first place.
			var sourceDSym = Path.Combine (appContainerDir, appBundleName + ".dSYM");
			Assert.That (sourceDSym, Does.Exist, "Source dSYM");

			var publishedDSym = Path.Combine (publishDir, appBundleName + ".dSYM");
			if (copyDSym) {
				Assert.That (publishedDSym, Does.Exist, "Published dSYM");
				Assert.That (Path.Combine (publishedDSym, "Contents", "Info.plist"), Does.Exist, "Published dSYM Info.plist");
			} else {
				Assert.That (publishedDSym, Does.Not.Exist, "Published dSYM (disabled)");
			}
		}

		static ITaskItem AssertApplicationArtifact (string binLogPath, string path, ApplePlatform platform, string packageFormat, bool isDirectory)
		{
			var outputs = GetItems (binLogPath, "ApplicationArtifact");
			var fullPath = Path.GetFullPath (path);
			var output = outputs.SingleOrDefault (v => Path.GetFullPath (v.ItemSpec) == fullPath);
			Assert.That (output, Is.Not.Null, $"Could not find {packageFormat} output for {fullPath}. All outputs:\n\t{string.Join ("\n\t", outputs.Select (v => v.ItemSpec))}");
			Assert.That (output!.GetMetadata ("PackageFormat"), Is.EqualTo (packageFormat), "PackageFormat");
			Assert.That (output.GetMetadata ("IsDirectory"), Is.EqualTo (isDirectory ? "true" : "false"), "IsDirectory");
			Assert.That (output.GetMetadata ("PlatformName"), Is.EqualTo (platform.AsString ()), "PlatformName");
			Assert.That (output.GetMetadata ("BundleIdentifier"), Is.Not.Empty, "BundleIdentifier");
			Assert.That (output.GetMetadata ("ArtifactKind"), Is.Null.Or.Empty, "ArtifactKind");
			Assert.That (output.GetMetadata ("AppBundlePath"), Is.Null.Or.Empty, "AppBundlePath");
			Assert.That (output.GetMetadata ("CodeSigned"), Is.Null.Or.Empty, "CodeSigned");
			Assert.That (output.GetMetadata ("Signed"), Is.Null.Or.Empty, "Signed");
			Assert.That (output.GetMetadata ("PackageSigned"), Is.Null.Or.Empty, "PackageSigned");
			return output;
		}

		static ITaskItem AssertApplicationArtifact (string binLogPath, ApplePlatform platform, string packageFormat, bool isDirectory)
		{
			var outputs = GetItems (binLogPath, "ApplicationArtifact");
			var matchingOutputs = outputs.Where (v => v.GetMetadata ("PackageFormat") == packageFormat).ToList ();
			Assert.That (matchingOutputs, Has.Count.EqualTo (1), $"Expected one {packageFormat} output. All outputs:\n\t{string.Join ("\n\t", outputs.Select (v => v.ItemSpec))}");

			var itemSpec = matchingOutputs [0].ItemSpec;
			Assert.That (itemSpec, Does.Exist, packageFormat);
			return AssertApplicationArtifact (binLogPath, itemSpec, platform, packageFormat, isDirectory);
		}

		static JsonElement [] GetApplicationArtifacts (string projectPath, Dictionary<string, string> properties, string target = "GetApplicationArtifacts")
		{
			using var document = JsonDocument.Parse (DotNet.GetItems (projectPath, "ApplicationArtifact", target: target, properties: properties));
			var outputs = document.RootElement.GetProperty ("Items").GetProperty ("ApplicationArtifact").EnumerateArray ().Select (v => v.Clone ()).ToArray ();
			Assert.That (outputs, Is.Not.Empty, "ApplicationArtifact items");
			return outputs;
		}

		static JsonElement AssertApplicationArtifact (JsonElement [] outputs, string path, ApplePlatform platform, string packageFormat, bool isDirectory)
		{
			var fullPath = Path.GetFullPath (path);
			var output = outputs.SingleOrDefault (v => Path.GetFullPath (GetMetadata (v, "FullPath")) == fullPath);
			Assert.That (output.ValueKind, Is.Not.EqualTo (JsonValueKind.Undefined), $"Could not find {packageFormat} output for {fullPath}. All outputs:\n\t{string.Join ("\n\t", outputs.Select (v => GetMetadata (v, "FullPath")))}");
			Assert.That (GetMetadata (output, "PackageFormat"), Is.EqualTo (packageFormat), "PackageFormat");
			Assert.That (GetMetadata (output, "IsDirectory"), Is.EqualTo (isDirectory ? "true" : "false"), "IsDirectory");
			Assert.That (GetMetadata (output, "PlatformName"), Is.EqualTo (platform.AsString ()), "PlatformName");
			Assert.That (GetMetadata (output, "BundleIdentifier"), Is.Not.Empty, "BundleIdentifier");
			Assert.That (GetMetadata (output, "ApplicationId"), Is.EqualTo (GetMetadata (output, "BundleIdentifier")), "ApplicationId");
			Assert.That (GetMetadata (output, "ApplicationName"), Is.Not.Empty, "ApplicationName");
			Assert.That (GetMetadata (output, "ApplicationDisplayVersion"), Is.Not.Empty, "ApplicationDisplayVersion");
			Assert.That (GetMetadata (output, "ApplicationVersion"), Is.Not.Empty, "ApplicationVersion");
			Assert.That (GetMetadata (output, "ArtifactKind"), Is.Empty, "ArtifactKind");
			Assert.That (GetMetadata (output, "AppBundlePath"), Is.Empty, "AppBundlePath");
			Assert.That (GetMetadata (output, "CodeSigned"), Is.Empty, "CodeSigned");
			Assert.That (GetMetadata (output, "Signed"), Is.Empty, "Signed");
			Assert.That (GetMetadata (output, "PackageSigned"), Is.Empty, "PackageSigned");
			return output;
		}

		static JsonElement AssertApplicationArtifact (JsonElement [] outputs, ApplePlatform platform, string packageFormat, bool isDirectory)
		{
			var matchingOutputs = outputs.Where (v => GetMetadata (v, "PackageFormat") == packageFormat).ToArray ();
			Assert.That (matchingOutputs, Has.Length.EqualTo (1), $"Expected one {packageFormat} output. All outputs:\n\t{string.Join ("\n\t", outputs.Select (v => GetMetadata (v, "FullPath")))}");

			var fullPath = GetMetadata (matchingOutputs [0], "FullPath");
			Assert.That (fullPath, Does.Exist, packageFormat);
			return AssertApplicationArtifact (outputs, fullPath, platform, packageFormat, isDirectory);
		}

		static void AssertApplicationMetadata (JsonElement output, string applicationId, string applicationTitle, string applicationName, string applicationDisplayVersion, string applicationVersion)
		{
			Assert.Multiple (() => {
				Assert.That (GetMetadata (output, "ApplicationId"), Is.EqualTo (applicationId), "ApplicationId");
				Assert.That (GetMetadata (output, "ApplicationTitle"), Is.EqualTo (applicationTitle), "ApplicationTitle");
				Assert.That (GetMetadata (output, "ApplicationName"), Is.EqualTo (applicationName), "ApplicationName");
				Assert.That (GetMetadata (output, "ApplicationDisplayVersion"), Is.EqualTo (applicationDisplayVersion), "ApplicationDisplayVersion");
				Assert.That (GetMetadata (output, "ApplicationVersion"), Is.EqualTo (applicationVersion), "ApplicationVersion");
			});
		}

		static string GetMetadata (JsonElement item, string name)
		{
			return item.TryGetProperty (name, out var value) ? value.GetString () ?? "" : "";
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		public void XCFrameworkDSymsCopiedToArchive (ApplePlatform platform, string runtimeIdentifiers)
		{
			// https://github.com/dotnet/macios/issues/23076
			// Verify that dSYMs from xcframeworks are copied next to the app's dSYM
			// and included in the archive.
			var project = "NativeXCFrameworkReferencesApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["ArchiveOnBuild"] = "true";
			properties ["Configuration"] = configuration;
			var archiveDir = Cache.CreateTemporaryDirectory ();
			properties ["ArchiveDir"] = archiveDir;

			DotNet.AssertBuild (project_path, properties);

			Assert.That (archiveDir, Does.Exist, "Archive directory existence");

			// Verify the xcframework's dSYM is in the archive's dSYMs directory
			var archiveDSymsDir = Path.Combine (archiveDir, "dSYMs");
			Assert.That (archiveDSymsDir, Does.Exist, "Archive dSYMs directory should exist");

			var xcframeworkDSym = Path.Combine (archiveDSymsDir, "XTest.framework.dSYM");
			Assert.That (xcframeworkDSym, Does.Exist, "XTest.framework.dSYM should be in the archive");
			Assert.That (Path.Combine (xcframeworkDSym, "Contents", "Info.plist"), Does.Exist, "Archived dSYM should contain Info.plist");
		}

		static List<ITaskItem> GetPostProcessingItems (string binLogPath)
		{
			return GetItems (binLogPath, "_PostProcessingItem");
		}

		static List<ITaskItem> GetItems (string binLogPath, string itemType)
		{
			var items = new Dictionary<string, ITaskItem> ();
			foreach (var args in BinLog.ReadBuildEvents (binLogPath)) {
				if (args is not TaskParameterEventArgs tpea)
					continue;
				if (tpea.Kind != TaskParameterMessageKind.AddItem)
					continue;
				if (tpea.ItemType != itemType)
					continue;
				foreach (var item in tpea.Items) {
					if (item is ITaskItem taskItem)
						items [taskItem.ItemSpec] = taskItem;
				}
			}
			return items.Values.ToList ();
		}
	}
}
