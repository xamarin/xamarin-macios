using System;

using NUnit.Framework;

using Microsoft.Build.Logging.StructuredLogger;

namespace Xamarin.Tests {
	public class PreBuildTest : TestBaseClass {

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		public void PrepareTarget (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var testDir = Cache.CreateTemporaryDirectory (TestName);
			DotNet.AssertNew (testDir, $"{platform.AsString ().ToLower ()}", TestName);
			testDir = Path.Combine (testDir, TestName);
			var proj = Path.Combine (testDir, $"{TestName}.csproj");
			var prepareTargetContent = @"
  <PropertyGroup>
    <MaciOSPrepareForBuildDependsOn>MyPrepareTarget</MaciOSPrepareForBuildDependsOn>
  </PropertyGroup>
  <Target Name=""MyPrepareTarget"" >
    <Message Text=""Running target: 'MyPrepareTarget'"" Importance=""high"" />
  </Target>
";
			var existingProjContent = File.ReadAllText (proj);
			var newProjContent = existingProjContent.Replace ("</Project>", prepareTargetContent + "</Project>");
			File.WriteAllText (proj, newProjContent);

			var rv = DotNet.AssertBuild (proj);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "MyPrepareTarget", "MyPrepareTarget");
		}

	}
}
