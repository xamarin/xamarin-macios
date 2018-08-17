using System;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class RegistrarTests
	{
		[TestCase (false, "x86_64")]
		[TestCase (true, "x86_64")]
		public void SmokeTest (bool full, string arch)
		{
			if (!PlatformHelpers.CheckSystemVersion (10, 11))
				return;

			MMPTests.RunMMPTest (tmpDir => {
				// First in 64-bit
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = $"<MonoBundlingExtraArgs>--registrar=static</MonoBundlingExtraArgs><XamMacArch>{arch}</XamMacArch>",
					XM45 = full
				};
				var output = TI.TestUnifiedExecutable (test).RunOutput;

				Assert.IsTrue (!output.Contains ("Could not register the assembly"), "Could not register the assembly errors found:\n" + output);
			});
		}

		[TestCase (false, "x86_64")]
		[TestCase (true, "x86_64")]
		public void DirectoryContainsSpaces (bool full, string arch)
		{
			if (!PlatformHelpers.CheckSystemVersion (10, 11))
				return;

			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = $"<MonoBundlingExtraArgs>--registrar=static</MonoBundlingExtraArgs><XamMacArch>{arch}</XamMacArch>",
					XM45 = full
				};
			}, "test withSpace");
		}

	}
}
