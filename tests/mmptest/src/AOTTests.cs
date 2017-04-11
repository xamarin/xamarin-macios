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
	public partial class MMPTests
	{
		void ValidateAOTStatus (string tmpDir, string buildResults)
		{ 
			foreach (var file in GetOutputDirInfo (tmpDir).EnumerateFiles ()) {
				bool shouldBeAOT = ShouldFileBeAOT (file);
				Assert.AreEqual (shouldBeAOT, File.Exists (file.FullName + ".dylib"), "{0} should {1} be AOT.\n{2}", file.FullName, shouldBeAOT ? "" : "not", buildResults);

			}
		}

		const string AOTTestBaseConfig = "<MonoBundlingExtraArgs>--aot=core,-Xamarin.Mac.dll</MonoBundlingExtraArgs>";
		const string AOTTestHybridConfig = "<MonoBundlingExtraArgs>--aot=core|hybrid,-Xamarin.Mac.dll</MonoBundlingExtraArgs>";
		DirectoryInfo GetOutputDirInfo (string tmpDir) => new DirectoryInfo (Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle/"));
		string GetOutputAppPath (string tmpDir) => Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MacOS/UnifiedExample");

		bool ShouldFileBeAOT (FileInfo file)
		{
			string extension = file.Extension.ToLowerInvariant ();
			if (extension == ".exe" || extension == ".dll")
				return file.Name == "System.dll" || file.Name == "mscorlib.dll";
			return false;
		}


		// AOT unit tests can be found in tools/mmp/tests
		[Test]
		public void AOT_SmokeTest () {
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = AOTTestBaseConfig
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				ValidateAOTStatus (tmpDir, buildResults);
			});
		}

		[Test]
		public void AOT_32Bit_SmokeTest ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<XamMacArch>i386</XamMacArch>" + AOTTestBaseConfig
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				ValidateAOTStatus (tmpDir, buildResults);
			});
		}

		[Test]
		public void HybridAOT_WithManualStripping_SmokeTest ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = AOTTestHybridConfig
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				foreach (var file in GetOutputDirInfo (tmpDir).EnumerateFiles ()) {
					if (ShouldFileBeAOT (file))
						TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/mono-cil-strip", file.ToString (), "Manually strip IL");

				}

				ValidateAOTStatus (tmpDir, buildResults);

				TI.RunAndAssert (GetOutputAppPath (tmpDir), "", "Run stripped app");
			});
		}
	}
}
