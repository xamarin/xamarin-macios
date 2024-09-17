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
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		public void BindLibrary (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var testDir = Cache.CreateTemporaryDirectory (TestName);
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}binding", TestName);
			testDir = Path.Combine (testDir, TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
			var xcodeProjDirDest = Path.Combine (testDir, TestName);
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);

			// Add dummy class file to ensure ensure the xcframework from our xcodeproj was built and bound
			AddClassFile ("Binding", "public void BindTest () { Console.WriteLine (XcProjBinding.SwiftBindTest.GetString(\"test\")); }", testDir);

			AddTemplateApiDefinition (testDir);

			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			var rv = DotNet.AssertBuild (proj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");

			if (platform == ApplePlatform.iOS || platform == ApplePlatform.TVOS) {
				var expectedXcodeFxOutput = Path.Combine (testDir, "bin", "Debug", $"{platform.ToFramework ()}", $"{TestName}.resources", $"{xcodeProjName}{platform.AsString ()}.xcframework");
				Assert.That (expectedXcodeFxOutput, Does.Exist, $"The expected xcode project output '{expectedXcodeFxOutput}' did not exist.");
			} else {
				var resourcesZip = Path.Combine (testDir, "bin", "Debug", $"{platform.ToFramework ()}", $"{TestName}.resources.zip");
				Assert.Contains ($"{xcodeProjName}{platform.AsString ()}.xcframework/Info.plist", ZipHelpers.List (resourcesZip), $"The expected xcode project output was not found in '{resourcesZip}'.");
			}
		}

		[Test]
		public void BindPackLibrary ([Values (false, true)] bool pack)
		{
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
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}binding", TestName);
			testDir = Path.Combine (testDir, TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
			var xcodeProjDirDest = Path.Combine (testDir, TestName);
			var xcodeProjPath = Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj");
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);
			AddXcodeProjectItem (proj, xcodeProjPath,
				new Dictionary<string, string> {
					{ "SchemeName", xcodeProjName },
				});

			// Build the first time
			var rv = DotNet.AssertBuild (proj);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_BuildXcodeProjectFrameworks", "First _BuildXcodeProjectFrameworks");
			var expectedXcodeFxOutput = Path.Combine (testDir, "bin", "Debug", $"{platform.ToFramework ()}", $"{TestName}.resources", $"{xcodeProjName}{platform.AsString ().ToLower ()}.xcframework");
			if (platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst)
				expectedXcodeFxOutput = Path.Combine (testDir, "bin", "Debug", $"{platform.ToFramework ()}", $"{TestName}.resources.zip");
			Assert.That (expectedXcodeFxOutput, Does.Exist, $"The expected xcode project output '{expectedXcodeFxOutput}' did not exist.");
			var outputFxFirstWriteTime = File.GetLastWriteTime (expectedXcodeFxOutput);

			// Build again, _BuildXcodeProjects should be skipped and outputs should not be updated
			rv = DotNet.AssertBuild (proj);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_BuildXcodeProjectFrameworks", "Second _BuildXcodeProjectFrameworks");
			Assert.That (expectedXcodeFxOutput, Does.Exist, $"The expected xcode project output '{expectedXcodeFxOutput}' did not exist.");
			var outputFxSecondWriteTime = File.GetLastWriteTime (expectedXcodeFxOutput);
			Assert.That (outputFxFirstWriteTime, Is.EqualTo (outputFxSecondWriteTime), $"Expected '{expectedXcodeFxOutput}' write time to be '{outputFxFirstWriteTime}', but was '{outputFxSecondWriteTime}'");

			// Update xcode project, _BuildXcodeProjects should run and outputs should be updated
			File.SetLastWriteTime (Path.Combine (xcodeProjPath, "project.pbxproj"), DateTime.Now);
			rv = DotNet.AssertBuild (proj);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_BuildXcodeProjectFrameworks", "Third _BuildXcodeProjectFrameworks");
			Assert.That (expectedXcodeFxOutput, Does.Exist, $"The expected xcode project output '{expectedXcodeFxOutput}' did not exist.");
			var outputFxThirdWriteTime = File.GetLastWriteTime (expectedXcodeFxOutput);
			Assert.IsTrue (outputFxThirdWriteTime > outputFxFirstWriteTime, $"Expected '{outputFxThirdWriteTime}' write time of '{outputFxThirdWriteTime}' to be greater than first write '{outputFxFirstWriteTime}'");
		}

		[Test]
		public void BuildMultipleSchemes ()
		{
		}

		[Test]
		public void BuildMultipleProjects ()
		{
		}

		[Test]
		public void BuildMultipleTargeting ()
		{
		}

		[Test]
		public void SharpieBindiOS ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
		}

		[Test]
		public void InvalidItemError ()
		{
		}

		[Test]
		public void InvalidSchemeError ()
		{
		}

	}
}
