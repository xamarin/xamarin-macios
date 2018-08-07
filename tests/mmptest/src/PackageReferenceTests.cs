using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class PackageReferenceTests {

		[TestCase (true)]
		[TestCase (false)]
		public void PackageReferencs_ResolveLibAndRefCorrectly (bool full)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var config = new TI.UnifiedTestConfig (tmpDir) {
					ItemGroup = @"<ItemGroup><PackageReference Include = ""Newtonsoft.Json"" Version = ""10.0.3"" /></ItemGroup>",
					TestCode = @"var output = Newtonsoft.Json.JsonConvert.SerializeObject (new int[] { 1, 2, 3 });
			if (output == ""[1,2,3]"")
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
	}
}