using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public class NetStandardTests {
		[Test]
		public void ModernUsingNetStandardLib_SmokeTest ()
		{
			MMPTests.RunMMPTest(tmpDir => {
				NetStandardTestCore (tmpDir, false);
			});
		}

		[Test]
		public void FullUsingNetStandardLib_SmokeTest ()
		{
			MMPTests.RunMMPTest(tmpDir => {
				NetStandardTestCore (tmpDir, true);
			});
		}

		static void NetStandardTestCore (string tmpDir, bool full)
		{
			if (!File.Exists ("/usr/local/share/dotnet/dotnet"))
				Assert.Ignore (".NET Core SDK not installed");

			TI.UnifiedTestConfig config = new TI.UnifiedTestConfig (tmpDir);

			string netStandardProject = TI.GenerateNetStandardProject (config);

			var environment = new Dictionary<string, string> {
				{ "MSBUILD_EXE_PATH", null },
				{ "MSBuildExtensionsPathFallbackPathsOverride", null },
				{ "MSBuildSDKsPath", null },
			};

			TI.RunAndAssert("/usr/local/share/dotnet/dotnet", new [] { "restore", netStandardProject }, "Restore", environment: environment);
			TI.BuildProject(netStandardProject);

			config.ItemGroup = $@"
<ItemGroup>
	<ProjectReference Include=""NetStandard\NetStandardLib.csproj"">
		<Project>{{BEC2D25A-A590-4858-8E07-849539D168B1}}</Project>
		<Name>NetStandardLib</Name>
	</ProjectReference>
</ItemGroup>
";

			if (full)
				config.TargetFrameworkVersion = "<TargetFrameworkVersion>4.6.1</TargetFrameworkVersion>";
			
			config.TestCode = "var test = new NetStandardLib.Class1 ().TestMethod ();";
			config.CSProjConfig = "<MonoBundlingExtraArgs>--registrar=dynamic</MonoBundlingExtraArgs>";
			config.XM45 = full;

			TI.TestUnifiedExecutable(config);
		}
	}
}
