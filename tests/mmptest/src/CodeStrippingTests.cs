using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	public class CodeStrippingTests
	{
		Func<string, bool> DidAnyLipoStrip = output => output.SplitLines ().Any (x => x.Contains ("lipo") && x.Contains ("-remove"));

		static TI.UnifiedTestConfig CreateStripTestConfig (string nativeStrip, string tmpDir, string additionalMMPArgs = "")
		{
			TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { };

			if (!string.IsNullOrEmpty (nativeStrip))
				test.CSProjConfig = $"<MonoBundlingExtraArgs>--native-strip={nativeStrip} {additionalMMPArgs}</MonoBundlingExtraArgs>";
			else if (!string.IsNullOrEmpty (additionalMMPArgs))
				test.CSProjConfig = $"<MonoBundlingExtraArgs>{additionalMMPArgs}</MonoBundlingExtraArgs>";
			
			return test;
		}

		void AssertStrip (string libPath, bool shouldStrip)
		{
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

		void StripTestCore (TI.UnifiedTestConfig test, bool debugStrips, bool releaseStrips, string libPath, bool shouldWarn)
		{
			string buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
			Assert.AreEqual (debugStrips, DidAnyLipoStrip (buildOutput), "Debug lipo usage did not match expectations");
			AssertStrip (Path.Combine (test.TmpDir, "bin/Debug/UnifiedExample.app/", libPath), shouldStrip: debugStrips);
			Assert.AreEqual (shouldWarn && debugStrips, buildOutput.Contains ("MM2106"), "Debug warning did not match expectations");

			test.Release = true;
			buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
			Assert.AreEqual (releaseStrips, DidAnyLipoStrip (buildOutput), "Release lipo usage did not match expectations");
			AssertStrip (Path.Combine (test.TmpDir, "bin/Release/UnifiedExample.app/", libPath), shouldStrip: releaseStrips);
			Assert.AreEqual (shouldWarn && releaseStrips, buildOutput.Contains ("MM2106"), "Release warning did not match expectations");
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

				StripTestCore (test, debugStrips, releaseStrips, "Contents/MonoBundle/libMonoPosixHelper.dylib", shouldWarn: false);
			});
		}

		[TestCase ("", false, true)]
		[TestCase ("default", false, true)]
		[TestCase ("strip", true, true)]
		[TestCase ("skip", false, false)]
		public void ShouldStripUserFramework (string nativeStrip, bool debugStrips, bool releaseStrips)
		{
			MMPTests.RunMMPTest (tmpDir =>
			{
				var frameworkPath = FrameworkBuilder.CreateFatFramework (tmpDir);
				TI.UnifiedTestConfig test = CreateStripTestConfig (nativeStrip, tmpDir, $"--native-reference={frameworkPath}");

				StripTestCore (test, debugStrips, releaseStrips, "Contents/Frameworks/Foo.framework/Foo", shouldWarn: true);
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
				Assert.AreEqual (shouldStrip, DidAnyLipoStrip (buildOutput), "lipo usage did not match expectations");
				Assert.AreEqual (shouldStrip, buildOutput.Contains ("MM2106"), "Warning did not match expectations");
			});
		}
	}
}
