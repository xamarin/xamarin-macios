using System.IO;
using System.Linq;
using System.Threading;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	public class CodesignAppBundle : ProjectTest
	{
		public CodesignAppBundle (string platform) : base (platform)
		{
		}

		[Ignore] // requires msbuild instead of xbuild
		[Test]
		public void RebuildNoChanges ()
		{
			BuildProject ("MyTabbedApplication", Platform, "Release", clean: true);

			var dsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", "MyTabbedApplication.app.dSYM"));
			var appexDsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", "MyActionExtension.appex.dSYM"));

			var timestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var dsymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var appexDsymTimestamps = Directory.EnumerateFiles (appexDsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			Thread.Sleep (1000);

			// Rebuild w/ no changes
			BuildProject ("MyTabbedApplication", Platform, "Release", clean: false);

			var newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var newDsymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var newAppexDsymTimestamps = Directory.EnumerateFiles (appexDsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys)
				Assert.AreEqual (timestamps[file], newTimestamps[file], "App Bundle timestamp changed: " + file);

			foreach (var file in dsymTimestamps.Keys)
				Assert.AreEqual (dsymTimestamps[file], newDsymTimestamps[file], "App Bundle DSym timestamp changed: " + file);

			foreach (var file in appexDsymTimestamps.Keys)
				Assert.AreEqual (appexDsymTimestamps[file], newAppexDsymTimestamps[file], "App Extension DSym timestamp changed: " + file);
		}

		[Ignore] // requires msbuild instead of xbuild
		[Test]
		public void CodesignAfterModifyingAppExtensionTest ()
		{
			var csproj = BuildProject ("MyTabbedApplication", Platform, "Release", clean: true);
			var testsDir = Path.GetDirectoryName (Path.GetDirectoryName (csproj));
			var appexProjectDir = Path.Combine (testsDir, "MyActionExtension");
			var viewController = Path.Combine (appexProjectDir, "ActionViewController.cs");
			var mainExecutable = Path.Combine (AppBundlePath, "MyTabbedApplication");
			var timestamp = File.GetLastWriteTimeUtc (mainExecutable);
			var text = File.ReadAllText (viewController);

			Thread.Sleep (1000);

			// replace "bool imageFound = false;" with "bool imageFound = true;" so that we force the appex to get rebuilt
			text = text.Replace ("bool imageFound = false;", "bool imageFound = true;");
			File.WriteAllText (viewController, text);

			try {
				BuildProject ("MyTabbedApplication", Platform, "Release", clean: false);
				var newTimestamp = File.GetLastWriteTimeUtc (mainExecutable);

				// make sure that the main app bundle was codesigned due to the changes in the appex
				Assert.IsTrue (newTimestamp > timestamp, "The main app bundle does not seem to have been re-codesigned");
			} finally {
				// restore the original ActionViewController.cs code...
				text = text.Replace ("bool imageFound = true;", "bool imageFound = false;");
				File.WriteAllText (viewController, text);
			}
		}

		[Ignore] // requires msbuild instead of xbuild
		[Test]
		public void CodesignAfterModifyingWatchApp2Test ()
		{
			var csproj = BuildProject ("MyWatch2Container", Platform, "Release", clean: true);
			var testsDir = Path.GetDirectoryName (Path.GetDirectoryName (csproj));
			var appexProjectDir = Path.Combine (testsDir, "MyWatchKit2Extension");
			var viewController = Path.Combine (appexProjectDir, "InterfaceController.cs");
			var mainExecutable = Path.Combine (AppBundlePath, "MyWatch2Container");
			var timestamp = File.GetLastWriteTimeUtc (mainExecutable);
			var text = File.ReadAllText (viewController);

			Thread.Sleep (1000);

			// replace "bool imageFound = false;" with "bool imageFound = true;" so that we force the appex to get rebuilt
			text = text.Replace ("{0} awake with context", "{0} The Awakening...");
			File.WriteAllText (viewController, text);

			try {
				BuildProject ("MyWatch2Container", Platform, "Release", clean: false);
				var newTimestamp = File.GetLastWriteTimeUtc (mainExecutable);

				// make sure that the main app bundle was codesigned due to the changes in the appex
				Assert.IsTrue (newTimestamp > timestamp, "The main app bundle does not seem to have been re-codesigned");
			} finally {
				// restore the original ActionViewController.cs code...
				text = text.Replace ("{0} The Awakening...", "{0} awake with context");
				File.WriteAllText (viewController, text);
			}
		}
	}
}
