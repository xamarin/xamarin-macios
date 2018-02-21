using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

using Xamarin.Utils;

namespace Xamarin.MMP.Tests
{
	public class CodeStrippingTests
	{
		Func<string, bool> DidAnyLipoStrip = output => output.SplitLines ().Any (x => x.Contains ("lipo") && x.Contains ("-thin"));

		static TI.UnifiedTestConfig CreateStripTestConfig (bool? strip, string tmpDir, string additionalMMPArgs = "")
		{
			TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { };

			if (strip.HasValue)
				test.CSProjConfig = $"<MonoBundlingExtraArgs>--optimize={(strip.Value ? "+" : "-")}trim-architectures {additionalMMPArgs}</MonoBundlingExtraArgs>";
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
			Assert.AreEqual (shouldWarn && debugStrips, buildOutput.Contains ("MM2108"), "Debug warning did not match expectations");

			test.Release = true;
			buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
			Assert.AreEqual (releaseStrips, DidAnyLipoStrip (buildOutput), "Release lipo usage did not match expectations");
			AssertStrip (Path.Combine (test.TmpDir, "bin/Release/UnifiedExample.app/", libPath), shouldStrip: releaseStrips);
			Assert.AreEqual (shouldWarn && releaseStrips, buildOutput.Contains ("MM2108"), "Release warning did not match expectations");
		}

		[TestCase (null, false, true)]
		[TestCase (true, true, true)]
		[TestCase (false, false, false)]
		public void ShouldStripMonoPosixHelper (bool? strip, bool debugStrips, bool releaseStrips)
		{
			MMPTests.RunMMPTest (tmpDir =>
			{
				TI.UnifiedTestConfig test = CreateStripTestConfig (strip, tmpDir);

				StripTestCore (test, debugStrips, releaseStrips, "Contents/MonoBundle/libMonoPosixHelper.dylib", shouldWarn: false);
			});
		}

		[TestCase (null, false, true)]
		[TestCase (true, true, true)]
		[TestCase (false, false, false)]
		public void ShouldStripUserFramework (bool? strip, bool debugStrips, bool releaseStrips)
		{
			MMPTests.RunMMPTest (tmpDir =>
			{
				var frameworkPath = FrameworkBuilder.CreateFatFramework (tmpDir);
				TI.UnifiedTestConfig test = CreateStripTestConfig (strip, tmpDir, $"--native-reference={frameworkPath}");

				StripTestCore (test, debugStrips, releaseStrips, "Contents/Frameworks/Foo.framework/Foo", shouldWarn: true);
			});
		}

		const string MonoPosixOffset = "Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/libMonoPosixHelper.dylib";

		[TestCase (true, true)]
		[TestCase (false, false)]
		public void ExplictStripOption_ThirdPartyLibrary_AndWarnsIfSo (bool? strip, bool shouldStrip)
		{
			MMPTests.RunMMPTest (tmpDir =>
			{
				string originalLocation = Path.Combine (TI.FindRootDirectory (), MonoPosixOffset);
				string newLibraryLocation =  Path.Combine (tmpDir, "libTest.dylib");
				File.Copy (originalLocation, newLibraryLocation);

				TI.UnifiedTestConfig test = CreateStripTestConfig (strip, tmpDir, $" --native-reference=\"{newLibraryLocation}\"");
				test.Release = true;

				string buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
				Assert.AreEqual (shouldStrip, DidAnyLipoStrip (buildOutput), "lipo usage did not match expectations");
				Assert.AreEqual (shouldStrip, buildOutput.Contains ("MM2108"), "Warning did not match expectations");
			});
		}

		void AssertNoLipoOrWarning (string buildOutput, string context)
		{
			Assert.False (DidAnyLipoStrip (buildOutput), "lipo incorrectly run in context: " + context);
			Assert.False (buildOutput.Contains ("MM2108"), "MM2108 incorrectly given in in context: " + context);
		}

		[TestCase (false)]
		[TestCase (true)]
		public void ThirdPartyLibrary_WithIncorrectBitness_ShouldWarnOnRelease (bool sixtyFourBits)
		{
			MMPTests.RunMMPTest (tmpDir =>
			{
				var frameworkPath = FrameworkBuilder.CreateFatFramework (tmpDir);

				TI.UnifiedTestConfig test = CreateStripTestConfig (null, tmpDir, $" --native-reference=\"{frameworkPath}\"");

				// Should always skip lipo/warning in Debug
				string buildOutput = TI.TestUnifiedExecutable(test).BuildOutput;
				AssertNoLipoOrWarning (buildOutput, "Debug");

				// Should always lipo/warn in Release
				test.Release = true;
				buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
				Assert.True (DidAnyLipoStrip (buildOutput), $"lipo did not run in release\n{buildOutput}");
				Assert.True (buildOutput.Contains ("MM2108"), $"MM2108 not given in release\n{buildOutput}");

			});
		}

		public void ThirdPartyLibrary_WithCorrectBitness_ShouldNotStripOrWarn ()
		{
			MMPTests.RunMMPTest (tmpDir =>
			{
				var frameworkPath = FrameworkBuilder.CreateThinFramework (tmpDir);

				TI.UnifiedTestConfig test = CreateStripTestConfig (null, tmpDir, $" --native-reference=\"{frameworkPath}\"");

				string buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
				AssertNoLipoOrWarning (buildOutput, "Debug");

				test.Release = true;
				buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
				AssertNoLipoOrWarning (buildOutput, "Release");
			});
		}
	}
}
