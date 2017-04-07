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
	public partial class MMPTests
	{
		int GetNumberOfTypesInLibrary (string path)
		{
			string output = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Versions/Current/Commands/monop", new StringBuilder ("-r:" + path), "GetNumberOfTypesInLibrary");
			string[] splitBuildOutput = output.Split (new string[] { Environment.NewLine }, StringSplitOptions.None);
			string outputLine = splitBuildOutput.First (x => x.StartsWith ("Total:"));
			string numberSize = outputLine.Split (':')[1];
			string number = numberSize.Split (' ')[1];
			return int.Parse (number);
		}

		[Test]
		public void UnifiedLinkingSDK_WithAllNonProductSkipped_Builds ()
		{
			RunMMPTest (tmpDir => {
				string[] dependencies = { "mscorlib", "System.Core", "System" };
				string config = "<LinkMode>SdkOnly</LinkMode><MonoBundlingExtraArgs>--linkskip=" + dependencies.Aggregate ((arg1, arg2) => arg1 + " --linkskip=" + arg2) + "</MonoBundlingExtraArgs>";
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = config };
				TI.TestUnifiedExecutable (test);
				foreach (string dep in dependencies) {
					string outputDep = Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle", dep + ".dll");
					string baseDep = Path.Combine (TI.FindRootDirectory (), "Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/", dep + ".dll");

					Assert.AreEqual (GetNumberOfTypesInLibrary (baseDep), GetNumberOfTypesInLibrary (outputDep), "We linked a linkskip - " + dep + " with config:\n" + config);
				}
			});
		}
	}
}
