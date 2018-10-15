using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public class NetStandardTests {
		// [Test] https://bugzilla.xamarin.com/show_bug.cgi?id=53164
		public void ModernUsingNetStandardLib_SmokeTest ()
		{
			MMPTests.RunMMPTest(tmpDir => {
				NetStandardTestCore (tmpDir, false);
			});
		}

		// [Test] https://bugzilla.xamarin.com/show_bug.cgi?id=53164
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
			TI.RunAndAssert("/usr/local/share/dotnet/dotnet", $"restore {netStandardProject}", "Restore");
			TI.BuildProject(netStandardProject, true);

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
