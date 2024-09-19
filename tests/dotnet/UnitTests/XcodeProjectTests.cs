using System;

using NUnit.Framework;

using Microsoft.Build.Logging.StructuredLogger;

namespace Xamarin.Tests {
	public class XcodeProjectTests : TestBaseClass {

		void AddTemplateApiDefinition (string directory)
		{
			var apiDefinitions = @"
using Foundation;
namespace XcProjBinding;
[BaseType (typeof(NSObject))]
interface SwiftBindTest {
	[Static,Export (""getStringWithMyString:"")]
	string GetString (string myString);
}";
			var file = Path.Combine (directory, "ApiDefinition.cs");
			File.WriteAllText (file, apiDefinitions);
		}

		void AddClassFile (string name, string content, string directory)
		{
			string classContent = $@"
public class {name}
{{
	{content}
}}
";
			var file = Path.Combine (directory, $"{name}.cs");
			File.WriteAllText (file, classContent);
		}

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


		[Test]
		[TestCase ("Debug")]
		[TestCase ("Release")]
		public void BuildAppiOS (string projConfig)
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			DotNet.AssertNew (testDir, "ios");
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			var projProps = new Dictionary<string, string> {
				{ "Configuration", projConfig },
			};
			DotNet.AssertBuild (proj, properties: projProps);
			var expectedAppOutput = Path.Combine (testDir, "bin", projConfig, platform.ToFramework (), "iossimulator-arm64", $"{TestName}.app", "Frameworks", $"{xcodeProjName}.framework", "Info.plist");
			Assert.That (expectedAppOutput, Does.Exist, $"Expected framework output was not found in '{expectedAppOutput}'.");
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
			DotNet.AssertNew (testDir, "iosbinding");
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
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
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}binding");
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			// Add class file to ensure ensure the xcframework from our xcodeproj was built and bound
			AddClassFile ("Binding", "public void BindTest () { Console.WriteLine (XcProjBinding.SwiftBindTest.GetString(\"test\")); }", testDir);
			AddTemplateApiDefinition (testDir);

			var rv = DotNet.AssertBuild (proj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");

			if (platform == ApplePlatform.iOS || platform == ApplePlatform.TVOS) {
				var expectedXcodeFxOutput = Path.Combine (testDir, "bin", "Debug", platform.ToFramework (), $"{TestName}.resources", $"{xcodeProjName}{platform.AsString ()}.xcframework");
				Assert.That (expectedXcodeFxOutput, Does.Exist, $"Expected xcframework output '{expectedXcodeFxOutput}' did not exist.");
			} else {
				var resourcesZip = Path.Combine (testDir, "bin", "Debug", platform.ToFramework (), $"{TestName}.resources.zip");
				Assert.Contains ($"{xcodeProjName}{platform.AsString ()}.xcframework/Info.plist", ZipHelpers.List (resourcesZip),
					$"Expected xcframework output was not found in '{resourcesZip}'.");
			}
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
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}binding");
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
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
			var expectedFxPath = $"lib/{platform.ToFrameworkWithPlatformVersion ()}/{TestName}.resources/{xcodeProjName}{platform.AsString ()}.xcframework/Info.plist";
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst) {
				zipContent = ZipHelpers.ListInnerZip (expectedNupkgOutput, $"lib/{platform.ToFrameworkWithPlatformVersion ()}/{TestName}.resources.zip");
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
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}binding");
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
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
		}

		[Test]
		public void BuildMultipleProjectsiOS ()
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);
		}

		[Test]
		public void BuildMultiTargeting ()
		{
		}

		[Test]
		public void InvalidItemErroriOS ()
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			DotNet.AssertNew (testDir, "ioslib");
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			AddXcodeProjectItem (proj, Path.Combine (testDir, "invalid", "path", $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			var rv = DotNet.AssertBuildFailure (proj);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			AssertErrorMessages (errors, "TODO: invalid item");
		}

		[Test]
		public void InvalidSchemeErroriOS ()
		{
			var platform = ApplePlatform.iOS;
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			DotNet.AssertNew (testDir, "ioslib");
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
			var xcodeProjDirDest = Cache.CreateTemporaryDirectory ($"{TestName}XcProj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);

			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", "invalid" },
				});

			var rv = DotNet.AssertBuildFailure (proj);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			AssertErrorMessages (errors, "TODO: invalid scheme");
		}

	}
}
