using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class PackageReferenceTests
	{
		const string PackageReference = @"<ItemGroup><PackageReference Include = ""Newtonsoft.Json"" Version = ""10.0.3"" /></ItemGroup>";
		const string TestCode = @"var output = Newtonsoft.Json.JsonConvert.SerializeObject (new int[] { 1, 2, 3 });";

		[TestCase (true)]
		[TestCase (false)]
		public void AppsWithPackageReferencs_BuildAndRun (bool full)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var config = new TI.UnifiedTestConfig (tmpDir) {
					ItemGroup = PackageReference,
					TestCode = TestCode + @"			if (output == ""[1,2,3]"")
				",
					XM45 = full
				};
				TI.AddGUIDTestCode (config);

				string project = TI.GenerateUnifiedExecutableProject (config);
				TI.NugetRestore (project);
				TI.BuildProject (project, true, useMSBuild: true);
				TI.RunGeneratedUnifiedExecutable (config);
			});
		}

		[Test]
		public void ExtensionProjectPackageReferencs_Build ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.CopyDirectory (Path.Combine (TI.FindSourceDirectory (), @"Today"), tmpDir);

				string project = Path.Combine (tmpDir, "Today/TodayExtensionTest.csproj");
				string main = Path.Combine (tmpDir, "Today/TodayViewController.cs");

				TI.CopyFileWithSubstitutions (project, project, s => s.Replace ("%ITEMGROUP%", PackageReference));
				TI.CopyFileWithSubstitutions (main, main, s => s.Replace ("%TESTCODE%", TestCode));

				TI.NugetRestore (project);
				TI.BuildProject (Path.Combine (tmpDir, "Today/TodayExtensionTest.csproj"), isUnified: true, useMSBuild: true);
			});
		}
	}
}
