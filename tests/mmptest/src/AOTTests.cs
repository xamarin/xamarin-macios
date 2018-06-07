using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

using Xamarin.Tests;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public partial class MMPTests
	{
		void ValidateAOTStatus (string tmpDir, Func<FileInfo, bool> shouldAOT, string buildResults)
		{ 
			foreach (var file in GetOutputDirInfo (tmpDir).EnumerateFiles ()) {
				bool shouldBeAOT = shouldAOT (file);
				Assert.AreEqual (shouldBeAOT, File.Exists (file.FullName + ".dylib"), "{0} should {1}be AOT.\n{2}", file.FullName, shouldBeAOT ? "" : "not ", buildResults);

			}
		}

		enum TestType { Base, Hybrid }

		string GetTestConfig (TestType type, bool useProjectTags)
		{
			if (useProjectTags) {
				switch (type) {
				case TestType.Base:
					return "<AOTMode>Core</AOTMode>";
				case TestType.Hybrid:
					return "<AOTMode>All</AOTMode><HybridAOT>true</HybridAOT>";
				}
			} else {
				switch (type) {
				case TestType.Base:
					return "<MonoBundlingExtraArgs>--aot=core</MonoBundlingExtraArgs>";
				case TestType.Hybrid:
					return "<MonoBundlingExtraArgs>--aot=all|hybrid</MonoBundlingExtraArgs>";
				}
			}
			throw new NotImplementedException ();
		}

		DirectoryInfo GetOutputDirInfo (string tmpDir) => new DirectoryInfo (Path.Combine (tmpDir, GetOutputBundlePath (tmpDir)));
		string GetOutputAppPath (string tmpDir) => Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MacOS/UnifiedExample");
		string GetOutputBundlePath (string tmpDir) => Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle");

		bool IsFileManagedCode (FileInfo file) => file.Extension.ToLowerInvariant () == ".exe" || file.Extension.ToLowerInvariant () == ".dll";
		bool ShouldBaseFilesBeAOT (FileInfo file) => file.Name == "Xamarin.Mac.dll" || file.Name == "System.dll" || file.Name == "mscorlib.dll";

		// AOT unit tests can be found in tools/mmp/tests
		[TestCase (false)]
		[TestCase (true)]
		public void AOT_SmokeTest (bool useProjectTags) {
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = GetTestConfig (TestType.Base, useProjectTags)
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				ValidateAOTStatus (tmpDir, f => ShouldBaseFilesBeAOT (f), buildResults);
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void AOT_32Bit_SmokeTest (bool useProjectTags)
		{
			Configuration.AssertXcodeSupports32Bit ();

			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<XamMacArch>i386</XamMacArch>" + GetTestConfig (TestType.Base, useProjectTags)
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				ValidateAOTStatus (tmpDir, f => ShouldBaseFilesBeAOT (f), buildResults);
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void HybridAOT_WithManualStrippingOfAllLibs_SmokeTest (bool useProjectTags)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = GetTestConfig (TestType.Hybrid, useProjectTags)
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

		[TestCase (false)]
		[TestCase (true)]
		public void HybridAOT_WithManualStrippingOfJustMainExe (bool useProjectTags)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = GetTestConfig (TestType.Hybrid, useProjectTags)
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/mono-cil-strip", Path.Combine (GetOutputBundlePath (tmpDir), "UnifiedExample.exe"), "Manually strip IL");

				ValidateAOTStatus (tmpDir, IsFileManagedCode, buildResults);

				TI.RunEXEAndVerifyGUID (tmpDir, test.guid, GetOutputAppPath (tmpDir));
			});
		}
	}
}
