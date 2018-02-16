using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	public class CodeStrippingTests
	{
		Func<string, bool> didStrip = output => output.SplitLines ().Any (x => x.Contains ("lipo") && x.Contains ("-remove"));

		static TI.UnifiedTestConfig CreateStripTestConfig (string nativeStrip, string tmpDir, string additionalMMPArgs = "")
		{
			TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { };

			if (!string.IsNullOrEmpty (nativeStrip))
				test.CSProjConfig = $"<MonoBundlingExtraArgs>--native-strip={nativeStrip}{additionalMMPArgs}</MonoBundlingExtraArgs>";
			return test;
		}

		public void AssertStrip (string tmpDir, bool release, bool shouldStrip)
		{
			string libPath = string.Format ("{0}/bin/{1}/UnifiedExample.app/Contents/MonoBundle/libMonoPosixHelper.dylib", tmpDir, release ? "Release" : "Debug");

			var archsFound = Xamarin.Tests.MachO.GetArchitectures (libPath);
			if (shouldStrip) {
				Assert.AreEqual (1, archsFound.Count, "Did not contain one archs");
				Assert.True (archsFound.Contains ("x86_64"), "Did not contain x86_64");
			} else {
				Assert.AreEqual (2, archsFound.Count, "Did not contain two archs");
				Assert.True (archsFound.Contains ("i386"), "Did not contain i386");
				Assert.True (archsFound.Contains ("x86_64"), "Did not contain x86_64");
			}
		}

		[TestCase ("", false, true)]
		[TestCase ("default", false, true)]
		[TestCase ("strip", true, true)]
		[TestCase ("skip", false, false)]
		public void ShouldStripMonoPosixHelper (string nativeStrip, bool debugStrips, bool releaseStrips)
		{
			MMPTests.RunMMPTest (tmpDir =>
			{
				TI.UnifiedTestConfig test = CreateStripTestConfig (nativeStrip, tmpDir);

				string buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
				Assert.AreEqual (debugStrips, didStrip (buildOutput), "Debug lipo usage did not match expectations");
				AssertStrip (tmpDir, release: false, shouldStrip: debugStrips);

				test.Release = true;
				buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
				Assert.AreEqual (releaseStrips, didStrip (buildOutput), "Release lipo usage did not match expectations");
				AssertStrip (tmpDir, release: true, shouldStrip: releaseStrips);
			 });
		}

		const string MonoPosixOffset = "Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/libMonoPosixHelper.dylib";

		[TestCase ("strip", true)]
		[TestCase ("skip", false)]
		public void StripsThirdPartyLibrary_AndWarnsIfSo (string nativeStrip, bool shouldStrip)
		{
			MMPTests.RunMMPTest (tmpDir =>
			{
				string originalLocation = Path.Combine (TI.FindRootDirectory (), MonoPosixOffset);
				string newLibraryLocation =  Path.Combine (tmpDir, "libTest.dylib");
				File.Copy (originalLocation, newLibraryLocation);

				TI.UnifiedTestConfig test = CreateStripTestConfig (nativeStrip, tmpDir, $" --native-reference=\"{newLibraryLocation}\"");

				string buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
				Assert.AreEqual (shouldStrip, didStrip (buildOutput), "lipo usage did not match expectations");
				Assert.AreEqual (shouldStrip, buildOutput.Contains ("MM1501"), "Warning did not match expectations");
			});
		}
	}
}
