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
		// AOT unit tests can be found in tools/mmp/tests
		[Test]
		public void AOT_SmokeTest () {
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<MonoBundlingExtraArgs>--aot=core,-Xamarin.Mac.dll</MonoBundlingExtraArgs>"
				};
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;

				var dirInfo = new DirectoryInfo (Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle/"));

				foreach (var file in dirInfo.EnumerateFiles ()) {
					string extension = file.Extension.ToLowerInvariant ();
					if (extension == ".exe" || extension == ".dll") {
						bool shouldBeAOT = (file.Name == "System.dll" || file.Name == "mscorlib.dll");
						Assert.AreEqual (shouldBeAOT, File.Exists (file.FullName + ".dylib"), "{0} should {1} be AOT.\n{2}", file.FullName, shouldBeAOT ? "" : "not", buildResults);
					}
				}
			});
		}
	}
}
