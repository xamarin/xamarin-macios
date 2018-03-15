using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class AOTTests
	{
		void ValidateAOTStatus (string tmpDir, Func<FileInfo, bool> shouldAOT, string buildResults)
		{ 
			foreach (var file in GetOutputDirInfo (tmpDir).EnumerateFiles ()) {
				bool shouldBeAOT = shouldAOT (file);
				Assert.AreEqual (shouldBeAOT, File.Exists (file.FullName + ".dylib"), "{0} should {1}be AOT.\n{2}", file.FullName, shouldBeAOT ? "" : "not ", buildResults);

			}
		}

		const string AOTTestBaseConfig = "<MonoBundlingExtraArgs>--aot=core,-Xamarin.Mac.dll</MonoBundlingExtraArgs>";
		const string AOTTestHybridConfig = "<MonoBundlingExtraArgs>--aot=all|hybrid</MonoBundlingExtraArgs>";

		DirectoryInfo GetOutputDirInfo (string tmpDir) => new DirectoryInfo (Path.Combine (tmpDir, GetOutputBundlePath (tmpDir)));
		string GetOutputAppPath (string tmpDir) => Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MacOS/UnifiedExample");
		string GetOutputBundlePath (string tmpDir) => Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle");

		bool IsFileManagedCode (FileInfo file) => file.Extension.ToLowerInvariant () == ".exe" || file.Extension.ToLowerInvariant () == ".dll";
		bool ShouldBaseFilesBeAOT (FileInfo file) => file.Name == "System.dll" || file.Name == "mscorlib.dll";

		// AOT unit tests can be found in tools/mmp/tests
		[Test]
		public void AOT_SmokeTest () {
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = AOTTestBaseConfig
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				ValidateAOTStatus (tmpDir, f => ShouldBaseFilesBeAOT (f), buildResults);
			});
		}

		[Test]
		public void AOT_32Bit_SmokeTest ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<XamMacArch>i386</XamMacArch>" + AOTTestBaseConfig
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				ValidateAOTStatus (tmpDir, f => ShouldBaseFilesBeAOT (f), buildResults);
			});
		}

		[Test]
		public void HybridAOT_WithManualStrippingOfAllLibs_SmokeTest ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = AOTTestHybridConfig
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				foreach (var file in GetOutputDirInfo (tmpDir).EnumerateFiles ()) {
					if (IsFileManagedCode (file))
						TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/mono-cil-strip", file.ToString (), "Manually strip IL");

				}

				ValidateAOTStatus (tmpDir, IsFileManagedCode, buildResults);

				TI.RunEXEAndVerifyGUID (tmpDir, test.guid, GetOutputAppPath (tmpDir));
			});
		}

		[Test]
		public void HybridAOT_WithManualStrippingOfJustMainExe ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = AOTTestHybridConfig
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/mono-cil-strip", Path.Combine (GetOutputBundlePath (tmpDir), "UnifiedExample.exe"), "Manually strip IL");

				ValidateAOTStatus (tmpDir, IsFileManagedCode, buildResults);

				TI.RunEXEAndVerifyGUID (tmpDir, test.guid, GetOutputAppPath (tmpDir));
			});
		}
	}
}
