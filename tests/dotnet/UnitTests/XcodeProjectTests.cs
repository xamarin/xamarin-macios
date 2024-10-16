using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Microsoft.Build.Logging.StructuredLogger;

namespace Xamarin.Tests {
	public class XcodeProjectTests : TestBaseClass {

		readonly string XCodeTestProjectDir = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Xcode");

		void AddXcodeProjectItem (string project, string path, Dictionary<string, string> metadata)
		{
			string xcProjItem = $@"
<ItemGroup>
  <XcodeProject Include=""{path}"">
    {string.Join ("\n", metadata.Select (v => $"<{v.Key}>{v.Value}</{v.Key}>"))}
  </XcodeProject>
</ItemGroup>
";
			var existingProjContent = File.ReadAllText (project);
			var newProjContent = existingProjContent.Replace ("</Project>", xcProjItem + "</Project>");
			File.WriteAllText (project, newProjContent);
		}

		void AssertXcFrameworkOutput (ApplePlatform platform, string testDir, string xcodeProjName, string config = "Debug")
		{
			if (platform == ApplePlatform.iOS || platform == ApplePlatform.TVOS) {
				var expectedXcodeFxOutput = Path.Combine (testDir, "bin", config, platform.ToFramework (), $"{TestName}.resources", $"{xcodeProjName}{platform.AsString ()}.xcframework");
				Assert.That (expectedXcodeFxOutput, Does.Exist, $"Expected xcframework output '{expectedXcodeFxOutput}' did not exist.");
			} else {
				var resourcesZip = Path.Combine (testDir, "bin", config, platform.ToFramework (), $"{TestName}.resources.zip");
				Assert.Contains ($"{xcodeProjName}{platform.AsString ()}.xcframework/Info.plist", ZipHelpers.List (resourcesZip),
					$"Expected xcframework output was not found in '{resourcesZip}'.");
			}
		}


		[Test]
		[TestCase ("Debug", "iossimulator-x64")]
		[TestCase ("Release", "ios-arm64")]
		public void BuildAppiOS (string projConfig, string rid)
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, "ios");

			var xcodeProjName = "TemplateFx";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			var projProps = new Dictionary<string, string> {
				{ "Configuration", projConfig },
				{ "RuntimeIdentifier", rid },
			};
			DotNet.AssertBuild (proj, properties: projProps);
			var appDir = Path.Combine (testDir, "bin", projConfig, platform.ToFramework (), rid, $"{TestName}.app");
			Assert.That (appDir, Does.Exist, $"Expected app dir '{appDir}' did not exist.");
			var appContent = Directory.GetFiles (appDir, "*", SearchOption.AllDirectories);
			var expectedAppOutput = Path.Combine (testDir, "bin", projConfig, platform.ToFramework (), rid, $"{TestName}.app", "Frameworks", $"{xcodeProjName}.framework", "Info.plist");
			Assert.Contains (expectedAppOutput, appContent, $"Expected framework output '{expectedAppOutput}' did not exist.");

		}

		[Test]
		[TestCase ("Release", "Release", false)]
		[TestCase ("Debug", "Relase", true)]
		[TestCase ("Release", "Debug", true)]
		public void BuildBindingiOS (string projConfig, string fxConfig, bool createNativeReference)
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, "iosbinding");

			var xcodeProjName = "TemplateFx";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "Configuration", fxConfig },
					{ "CreateNativeReference", createNativeReference.ToString () },
					{ "SchemeName", xcodeProjName },
				});

			var projProps = new Dictionary<string, string> {
				{ "Configuration", projConfig },
			};
			DotNet.AssertBuild (proj, properties: projProps);
			var expectedXcodeFxOutput = Path.Combine (testDir, "bin", projConfig, platform.ToFramework (), $"{TestName}.resources", $"{xcodeProjName}{platform.AsString ()}.xcframework");
			Assert.That (expectedXcodeFxOutput, createNativeReference ? Does.Exist : Does.Not.Exist, $"Expected xcframework output '{expectedXcodeFxOutput}' to exist when CreateNativeReference=true.");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		public void BuildBinding (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}binding");

			var xcodeProjName = "TemplateFx";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			// Add API definition to expose the API from the xcodeproj
			var apiDefinition = @"
using Foundation;
namespace XcProjBinding;
[BaseType (typeof(NSObject))]
interface SwiftBindTest {
	[Static,Export (""getStringWithMyString:"")]
	string GetString (string myString);
}";
			File.WriteAllText (Path.Combine (testDir, "ApiDefinition.cs"), apiDefinition);

			// Add class file to ensure ensure the xcframework from our xcodeproj was built and bound
			string classContent = @"
public class Binding
{
	public void BindTest () { Console.WriteLine (XcProjBinding.SwiftBindTest.GetString(""test"")); }
}
";
			File.WriteAllText (Path.Combine (testDir, "Binding.cs"), classContent);

			var rv = DotNet.AssertBuild (proj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");
			AssertXcFrameworkOutput (platform, testDir, xcodeProjName);
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		public void PackBinding (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}binding");

			var xcodeProjName = "TemplateFx";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			DotNet.AssertPack (proj);
			var expectedNupkgOutput = Path.Combine (testDir, "bin", "Release", $"{TestName}.1.0.0.nupkg");
			Assert.That (expectedNupkgOutput, Does.Exist, $"Expected pack output '{expectedNupkgOutput}' did not exist.");

			List<string> zipContent = ZipHelpers.List (expectedNupkgOutput);
			var tfm = platform.ToFrameworkWithPlatformVersion (isExecutable: false);
			var expectedFxPath = $"lib/{tfm}/{TestName}.resources/{xcodeProjName}{platform.AsString ()}.xcframework/Info.plist";
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				zipContent = ZipHelpers.ListInnerZip (expectedNupkgOutput, $"lib/{tfm}/{TestName}.resources.zip");
				expectedFxPath = $"{xcodeProjName}{platform.AsString ()}.xcframework/Info.plist";
			}
			Assert.Contains (expectedFxPath, zipContent, $"Expected xcframework output was not found in '{expectedNupkgOutput}'.");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		public void BuildIncremental (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}binding");

			var xcodeProjName = "TemplateFx";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			var xcodeProjPath = Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, xcodeProjPath,
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			// Build the first time
			var rv = DotNet.AssertBuild (proj);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_BuildXcodeProjects", "First _BuildXcodeProjects");
			var expectedXcodeFxOutput = Path.Combine (testDir, "bin", "Debug", platform.ToFramework (), $"{TestName}.resources", $"{xcodeProjName}{platform.AsString ().ToLower ()}.xcframework");
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst)
				expectedXcodeFxOutput = Path.Combine (testDir, "bin", "Debug", platform.ToFramework (), $"{TestName}.resources.zip");
			Assert.That (expectedXcodeFxOutput, Does.Exist, $"Expected xcframework output '{expectedXcodeFxOutput}' did not exist.");
			var outputFxFirstWriteTime = File.GetLastWriteTime (expectedXcodeFxOutput);

			// Build again, _BuildXcodeProjects should be skipped and outputs should not be updated
			rv = DotNet.AssertBuild (proj);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_BuildXcodeProjects", "Second _BuildXcodeProjects");
			Assert.That (expectedXcodeFxOutput, Does.Exist, $"Expected xcframework output '{expectedXcodeFxOutput}' did not exist.");
			var outputFxSecondWriteTime = File.GetLastWriteTime (expectedXcodeFxOutput);
			Assert.That (outputFxFirstWriteTime, Is.EqualTo (outputFxSecondWriteTime), $"Expected '{expectedXcodeFxOutput}' write time to be '{outputFxFirstWriteTime}', but was '{outputFxSecondWriteTime}'");

			// Update xcode project, _BuildXcodeProjects should run and outputs should be updated
			File.SetLastWriteTime (Path.Combine (xcodeProjPath, "project.pbxproj"), DateTime.Now);
			rv = DotNet.AssertBuild (proj);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_BuildXcodeProjects", "Third _BuildXcodeProjects");
			Assert.That (expectedXcodeFxOutput, Does.Exist, $"Expected xcframework output '{expectedXcodeFxOutput}' did not exist.");
			var outputFxThirdWriteTime = File.GetLastWriteTime (expectedXcodeFxOutput);
			Assert.IsTrue (outputFxThirdWriteTime > outputFxFirstWriteTime, $"Expected '{outputFxThirdWriteTime}' write time of '{outputFxThirdWriteTime}' to be greater than first write '{outputFxFirstWriteTime}'");
		}

		[Test]
		public void BuildMultipleSchemesiOS ()
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, "iosbinding");

			var xcodeProjName = "TwoSchemeFx";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			var secondSchemeName = "iOSFx";
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", secondSchemeName },
				});

			DotNet.AssertBuild (proj);
			AssertXcFrameworkOutput (platform, testDir, xcodeProjName);
			AssertXcFrameworkOutput (platform, testDir, secondSchemeName);
		}

		[Test]
		public void BuildMultipleProjectsiOS ()
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, "iosbinding");

			var xcodeProjName = "TemplateFx";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			var xcodeProjName2 = "TwoSchemeFx";
			var xcodeProjDirSrc2 = Path.Combine (XCodeTestProjectDir, xcodeProjName2);
			var xcodeProjDirDest2 = Cache.CreateTemporaryDirectory ($"{TestName}XcProjTwo");
			FileHelpers.CopyDirectory (xcodeProjDirSrc2, xcodeProjDirDest2);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest2, $"{xcodeProjName2}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName2 },
				});

			DotNet.AssertBuild (proj);
			AssertXcFrameworkOutput (platform, testDir, xcodeProjName);
			AssertXcFrameworkOutput (platform, testDir, xcodeProjName2);
		}

		[Test]
		[Category ("Multiplatform")]
		public void BuildMultiTargeting ()
		{
			var enabledPlatforms = Configuration.GetIncludedPlatforms (dotnet: true);
			var templatePlatform = enabledPlatforms.First ();
			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, $"{templatePlatform.AsString ().ToLower ()}binding");

			var tfxs = $"<TargetFrameworks>{string.Join (";", enabledPlatforms.Select (p => p.ToFramework ()))}</TargetFrameworks>";
			var existingProjContent = File.ReadAllText (proj);
			var newProjContent = existingProjContent.Replace ($"<TargetFramework>{templatePlatform.ToFramework ()}</TargetFramework>", tfxs);
			File.WriteAllText (proj, newProjContent);
			StringAssert.Contains (tfxs, File.ReadAllText (proj));

			var xcodeProjName = "TemplateFx";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			DotNet.AssertBuild (proj);
			foreach (var platform in enabledPlatforms) {
				AssertXcFrameworkOutput (platform, testDir, xcodeProjName);
			}
		}

		[Test]
		public void InvalidItemErroriOS ()
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, "ioslib");

			var xcodeProjName = "TemplateFx";
			var invalidXcodeProjPath = Path.Combine (testDir, "invalid", "path", $"{xcodeProjName}.xcodeproj");
			AddXcodeProjectItem (proj, invalidXcodeProjPath,
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			var rv = DotNet.AssertBuildFailure (proj);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			var expectedError = $"The Xcode project item: '{invalidXcodeProjPath}' could not be found. Please update the 'Include' value to a path containing a valid '.xcodeproj' file.";
			AssertErrorMessages (errors, expectedError);
		}

		[Test]
		public void InvalidSchemeErroriOS ()
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			DotNet.AssertNew (testDir, "ioslib");

			var xcodeProjName = "TemplateFx";
			var invaldSchemeName = "InvalidScheme";
			var xcodeProjDirSrc = Path.Combine (XCodeTestProjectDir, xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);

			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", invaldSchemeName },
				});

			var rv = DotNet.AssertBuildFailure (proj);
			var expectedErrorContent = $"xcodebuild: error: The project named \"{xcodeProjName}\" does not contain a scheme named \"{invaldSchemeName}\".";
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			AssertErrorMessages (errors,
				new Func<string, bool> [] {
					(msg) => msg?.Contains (expectedErrorContent) == true
				},
				new Func<string> [] {
					() => expectedErrorContent
				}
			);
		}

	}
}
