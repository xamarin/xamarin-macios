using System;

using NUnit.Framework;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public class SmokeTests {
		[TestCase (false)]
		[TestCase (true)]
		public void Unified_SmokeTest (bool full)
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					XM45 = full
				};
				TI.TestUnifiedExecutable (test);
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void FSharp_SmokeTest (bool full)
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					FSharp = true,
					XM45 = full
				};
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void Modern_SmokeTest_LinkSDK ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<LinkMode>SdkOnly</LinkMode>" };
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void Modern_SmokeTest_LinkAll ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<LinkMode>Full</LinkMode>" };
				TI.TestUnifiedExecutable (test);
			});
		}

		[TestCase ("")]
		[TestCase ("4.5")]
		[TestCase ("4.5.1")]
		[TestCase ("4.6")]
		[TestCase ("4.6.1")]
		[TestCase ("4.7")]
		public void SystemMono_SmokeTest (string version)
		{
			if (TI.FindMonoVersion () < new Version ("4.3"))
				return;

			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					SystemMonoVersion = version
				};
				TI.TestSystemMonoExecutable (test);
			});
		}

	}
}
