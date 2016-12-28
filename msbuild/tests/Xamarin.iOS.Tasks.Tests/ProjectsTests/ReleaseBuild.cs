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

		[Test]
		public void RebuildTest ()
		{
			BuildProject ("MyReleaseBuild", Platform, "Release", clean: true);

			var dsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", Path.GetFileName (AppBundlePath) + ".dSYM"));

			var timestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var dsymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			Thread.Sleep (1000);

			BuildProject ("MyReleaseBuild", Platform, "Release", clean: false);

			var newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var newDSymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys)
				Assert.AreEqual (timestamps[file], newTimestamps[file], "#1: " + file);

			foreach (var file in dsymTimestamps.Keys)
				Assert.AreEqual (dsymTimestamps[file], newDSymTimestamps[file], "#2: " + file);
		}
	}
}
