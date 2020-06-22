using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

using Xamarin.Utils;
using Xamarin.Tests;

namespace Xamarin.MMP.Tests
{
	public class CodeStrippingTests
	{
		static Func<string, bool> LipoStripConditional = s => s.Contains ("lipo") && s.Contains ("-thin");
		static Func<string, bool> LipoStripSkipPosixAndMonoNativeConditional = s => LipoStripConditional (s) && !s.Contains ("libMonoPosixHelper.dylib") && !s.Contains ("libmono-native.dylib");

		static Func<string, bool> DidAnyLipoStrip = output => output.SplitLines ().Any (LipoStripConditional);
		static Func<string, bool> DidAnyLipoStripSkipPosixAndMonoNative = output => output.SplitLines ().Any (LipoStripSkipPosixAndMonoNativeConditional);

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
			var archsFound = MachO.GetArchitectures (libPath);
			if (shouldStrip) {
				Assert.AreEqual (1, archsFound.Count, "Did not contain one archs");
				Assert.True (archsFound.Contains (Abi.x86_64), "Did not contain x86_64");
			} else {
				Assert.AreEqual (2, archsFound.Count, "Did not contain two archs");
				Assert.True (archsFound.Contains (Abi.i386), "Did not contain i386");
				Assert.True (archsFound.Contains (Abi.x86_64), "Did not contain x86_64");
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
			var posixHelper = Path.Combine (Configuration.SdkRootXM, "SDKs","Xamarin.macOS.sdk", "lib", "libMonoPosixHelper.dylib");
			if (Xamarin.MachO.GetArchitectures (posixHelper).Count < 2)
				Assert.Ignore ($"libMonoPosixHelper.dylib is not a fat library.");

			MMPTests.RunMMPTest (tmpDir =>
			{
				TI.UnifiedTestConfig test = CreateStripTestConfig (strip, tmpDir);
				// Mono's linker is smart enough to remove libMonoPosixHelper unless used (DeflateStream uses it)
				test.TestCode = "using (var ms = new System.IO.MemoryStream ()) { using (var gz = new System.IO.Compression.DeflateStream (ms, System.IO.Compression.CompressionMode.Compress)) { }}";
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
				string originalLocation = Path.Combine (Configuration.SourceRoot, "tests", "test-libraries", "libtest-fat.macos.dylib");
				string newLibraryLocation =  Path.Combine (tmpDir, "libTest.dylib");
				File.Copy (originalLocation, newLibraryLocation);

				TI.UnifiedTestConfig test = CreateStripTestConfig (strip, tmpDir, $" --native-reference=\"{newLibraryLocation}\"");
				test.Release = true;

				var testOutput = TI.TestUnifiedExecutable (test);
				string buildOutput = testOutput.BuildOutput;
				Assert.AreEqual (shouldStrip, DidAnyLipoStrip (buildOutput), "lipo usage did not match expectations");
				if (shouldStrip) {
					testOutput.Messages.AssertWarning (2108, "libTest.dylib was stripped of architectures except x86_64 to comply with App Store restrictions. This could break existing codesigning signatures. Consider stripping the library with lipo or disabling with --optimize=-trim-architectures");
				} else {
					testOutput.Messages.AssertWarningCount (0);
				}
			});
		}

		void AssertNoLipoOrWarning (string buildOutput, string context)
		{
			Assert.False (DidAnyLipoStrip (buildOutput), "lipo incorrectly run in context: " + context);
			Assert.False (buildOutput.Contains ("MM2108"), "MM2108 incorrectly given in in context: " + context);
		}

		void AssertLipoOnlyMonoPosixAndMonoNative (string buildOutput, string context)
		{
			Assert.False (DidAnyLipoStripSkipPosixAndMonoNative (buildOutput), "lipo incorrectly run in context outside of libMonoPosixHelper/libmono-native: " + context);
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
				Assert.True (DidAnyLipoStrip (buildOutput), $"lipo did not run in release");
				Assert.True (buildOutput.Contains ("MM2108"), $"MM2108 not given in release");

			});
		}

		[TestCase]
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
				AssertLipoOnlyMonoPosixAndMonoNative (buildOutput, "Release"); // libMonoPosixHelper.dylib and libmono-native.dylib will lipo in Release
			});
		}
	}
}
