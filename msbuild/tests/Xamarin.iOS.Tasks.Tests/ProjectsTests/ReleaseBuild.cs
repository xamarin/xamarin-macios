using System;
using System.IO;
using System.Linq;
using System.Threading;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	public class ReleaseBuild : ProjectTest
	{
		public ReleaseBuild (string platform) : base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			BuildProject ("MyReleaseBuild", Platform, "Release");
		}

		[Ignore] // requires msbuild instead of xbuild
		[Test]
		public void RebuildTest ()
		{
			var csproj = BuildProject ("MyReleaseBuild", Platform, "Release", clean: true);
			var bak = Path.Combine (Path.GetDirectoryName (csproj), "MyReleaseBuild.csproj.bak");
			var llvm = Path.Combine (Path.GetDirectoryName (csproj), "MyReleaseBuildLlvm.csproj");

			var dsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", Path.GetFileName (AppBundlePath) + ".dSYM"));

			var timestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var dsymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			EnsureFilestampChange ();

			// Rebuild w/ no changes
			BuildProject ("MyReleaseBuild", Platform, "Release", clean: false);

			var newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var newDSymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys)
				Assert.AreEqual (timestamps[file], newTimestamps[file], "#1: " + file);

			foreach (var file in dsymTimestamps.Keys)
				Assert.AreEqual (dsymTimestamps[file], newDSymTimestamps[file], "#2: " + file);

			EnsureFilestampChange ();

			// Rebuild after changing MtouchUseLlvm
			File.Copy (csproj, bak, true);
			try {
				File.Copy (llvm, csproj, true);
				File.SetLastWriteTimeUtc (csproj, DateTime.UtcNow);

				BuildProject ("MyReleaseBuild", Platform, "Release", clean: false);
			} finally {
				File.Copy (bak, csproj, true);
				File.Delete (bak);
			}

			newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			newDSymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys) {
				var dirName = Path.GetFileName (Path.GetDirectoryName (file));
				var fileName = Path.GetFileName (file);

				if (fileName == "MyReleaseBuild" || fileName == "CodeResources" || dirName == ".monotouch-32" || dirName == ".monotouch-64")
					Assert.AreNotEqual (timestamps[file], newTimestamps[file], "#3: " + file);
				else
					Assert.AreEqual (timestamps[file], newTimestamps[file], "#3: " + file);
			}

			foreach (var file in dsymTimestamps.Keys)
				Assert.AreNotEqual (dsymTimestamps[file], newDSymTimestamps[file], "#4: " + file);
		}
	}
}
