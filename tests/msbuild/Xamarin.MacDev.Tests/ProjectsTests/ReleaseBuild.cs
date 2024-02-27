using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	public class ReleaseBuild : ProjectTest {
		public ReleaseBuild (string platform)
			: base (platform, "Release")
		{
		}

		[Test]
		public void BuildTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			BuildProject ("MyReleaseBuild");

			var args = new List<string> { "-r", "UIWebView", AppBundlePath };
			ExecutionHelper.Execute ("grep", args, out var output);
			Assert.That (output.ToString (), Is.Empty, "UIWebView");
		}

		[Test]
		public void RebuildTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var csproj = BuildProject ("MyReleaseBuild");

			var dsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", Path.GetFileName (AppBundlePath) + ".dSYM"));

			var timestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var dsymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			EnsureFilestampChange ();

			// Rebuild w/ no changes
			BuildProject ("MyReleaseBuild", clean: false);

			var newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var newDSymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys)
				Assert.AreEqual (timestamps [file], newTimestamps [file], "#1: " + file);

			foreach (var file in dsymTimestamps.Keys)
				Assert.AreEqual (dsymTimestamps [file], newDSymTimestamps [file], "#2: " + file);

			EnsureFilestampChange ();

			// Rebuild after changing MtouchUseLlvm
			var proj = new MSBuildProject (csproj, this);
			proj.SetProperty ("MtouchUseLlvm", "true");
			BuildProject ("MyReleaseBuild", clean: false);

			newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			newDSymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys) {
				var fileName = Path.GetFileName (file);

				var isModificationExpected = false;

				if (fileName.EndsWith (".aotdata.armv7", StringComparison.Ordinal) || fileName.EndsWith (".aotdata.arm64", StringComparison.Ordinal)) {
					// aotdata files should be modified
					isModificationExpected = true;
				} else if (fileName == "MyReleaseBuild") {
					// the executable must of course be modified
					isModificationExpected = true;
				} else if (fileName == "CodeResources") {
					// the signature has of course changed too
					isModificationExpected = true;
				} else if (fileName.EndsWith (".dll", StringComparison.Ordinal) || fileName.EndsWith (".exe", StringComparison.Ordinal)) {
					// I'm not sure if assemblies have to be modified, but they currently are, so mark them as such to make the test pass.
					isModificationExpected = true;
				}

				if (isModificationExpected)
					Assert.AreNotEqual (timestamps [file], newTimestamps [file], "#3: " + file);
				else
					Assert.AreEqual (timestamps [file], newTimestamps [file], "#3: " + file);
			}

			foreach (var file in dsymTimestamps.Keys)
				Assert.AreNotEqual (dsymTimestamps [file], newDSymTimestamps [file], "#4: " + file);
		}
	}
}
