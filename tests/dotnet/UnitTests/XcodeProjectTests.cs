using System;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Microsoft.Build.Logging.StructuredLogger;

namespace Xamarin.Tests
{
	public class XcodeProjectTests : TestBaseClass
	{
		string TestDir = string.Empty;

		[SetUp]
		public void SetUp ()
		{
			TestDir = Cache.CreateTemporaryDirectory ();
		}

		[TearDown]
		public void TearDown ()
		{
			var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
			if (testStatus == TestStatus.Passed || testStatus == TestStatus.Skipped) {
				try {
					Directory.Delete (TestDir, recursive: true);
				} catch (Exception) {
					// Ignore cleanup failure
				}
			}
		}

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

		void AddXcodeProjectItem (string project, string path, Dictionary<string,string> metadata)
		{
			string xcProjItem = $@"
<ItemGroup>
  <MaciOSXcodeProject Include=""{path}"">
    {string.Join ("\n", metadata.Select (v => $"<{v.Key}>{v.Value}</{v.Key}>"))}
  </MaciOSXcodeProject>
</ItemGroup>
";
			var existingProjContent = File.ReadAllText (project);
			var newProjContent = existingProjContent.Replace ("</Project>", xcProjItem + "</Project>");
			File.WriteAllText (project, newProjContent);
		}

		[Test]
		public void BindLibraryiOS ()
		{
			DotNet.AssertNew (TestDir, "iosbinding", TestName);
			TestDir = Path.Combine (TestDir, TestName);
			var proj = Path.Combine (TestDir, $"{TestName}.csproj");

			var xcodeProjName = "XcodeFxTemplate";
			var xcodeProjDirSrc = Path.Combine (Configuration.SourceRoot, "tests", "common", "TestProjects", "Templates", xcodeProjName);
			var xcodeProjDirDest = Path.Combine (TestDir, TestName);
			FileHelpers.CopyDirectory (xcodeProjDirSrc, xcodeProjDirDest);

			// Add dummy class file to ensure ensure the xcframework from our xcodeproj was built and bound
			AddClassFile ("Binding", "public void BindTest () { Console.WriteLine (XcProjBinding.SwiftBindTest.GetString(\"test\")); }", TestDir);

			AddTemplateApiDefinition (TestDir);

			AddXcodeProjectItem (proj, Path.Combine (xcodeProjDirDest, $"{xcodeProjName}.xcodeproj"),
				new Dictionary<string,string> {
					{ "SchemeName", xcodeProjName }
				});

			var rv = DotNet.AssertBuild (proj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");

			var expectedXcodeFxOutput = Path.Combine (TestDir, "bin", "Debug", $"{Configuration.DotNetTfm}-ios", $"{TestName}.resources", $"{xcodeProjName}iOS.xcframework");
			Assert.IsTrue (Directory.Exists (expectedXcodeFxOutput), $"The expected xcode project output '{expectedXcodeFxOutput}' did not exist.");
		}

		[Test]
		public void BindPackLibrary ([Values (false, true)] bool pack)
		{
		}

		[Test]
		public void BuildIncremental ()
		{
			DotNet.AssertNew (TestDir, "iosbinding", TestName);
			TestDir = Path.Combine (TestDir, TestName);
			var proj = Path.Combine (TestDir, $"{TestName}.csproj");

			// Build the first time
			var rv = DotNet.AssertBuild (proj);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_BuildMaciOSXcodeProjects", "First _BuildMaciOSXcodeProjects");

			// Build again, _BuildMaciOSXcodeProjects should be skipped and outputs should not be updated
			rv = DotNet.AssertBuild (proj);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_BuildMaciOSXcodeProjects", "Second _BuildMaciOSXcodeProjects");

			// Update xcode project, _BuildMaciOSXcodeProjects should run and outputs should be updated
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
		public void InvalidItemError ()
		{
		}

		[Test]
		public void InvalidSchemeError ()
		{
		}

	}
}
