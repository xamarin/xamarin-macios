// #define TRACE

using System.IO;
using System.IO.Compression;

#nullable enable

namespace Xamarin.Tests {
	[Category ("Windows")]
	public class WindowsTest : TestBaseClass {

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void BundleStructureWithHotRestart (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.IgnoreIfNotOnWindows ();

			var project = "BundleStructure";
			var configuration = "Debug";
			var tmpdir = Cache.CreateTemporaryDirectory ();

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			var project_dir = Path.GetDirectoryName (Path.GetDirectoryName (project_path))!;
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			if (!string.IsNullOrWhiteSpace (configuration))
				properties ["Configuration"] = configuration;
			properties ["IsHotRestartBuild"] = "true";
			properties ["IsHotRestartEnvironmentReady"] = "true";
			properties ["EnableCodeSigning"] = "false"; // Skip code signing, since that would require making sure we have code signing configured on bots.
			properties ["_AppIdentifier"] = "placeholder_AppIdentifier"; // This needs to be set to a placeholder value because DetectSigningIdentity usually does it (and we've disabled signing)
			properties ["_BundleIdentifier"] = "placeholder_BundleIdentifier"; // This needs to be set to a placeholder value because DetectSigningIdentity usually does it (and we've disabled signing)
			properties ["_IsAppSigned"] = "false";

			// Redirect hot restart output to a place we can control from here
			var hotRestartOutputDir = Path.Combine (tmpdir, "out");
			Directory.CreateDirectory (hotRestartOutputDir);
			properties ["HotRestartSignedAppOutputDir"] = hotRestartOutputDir + Path.DirectorySeparatorChar;
			var hotRestartAppBundlePath = Path.Combine (tmpdir, "HotRestartAppBundlePath"); // Do not create this directory, it will be created and populated with default contents if it doesn't exist.
			properties ["HotRestartAppBundlePath"] = hotRestartAppBundlePath; // no trailing directory separator char for this property.
			var rv = DotNet.AssertBuild (project_path, properties);

			// Find the files in the prebuilt hot restart app
			var prebuiltAppEntries = Array.Empty<string> ().ToHashSet ();
			if (BinLog.TryFindPropertyValue (rv.BinLogPath, "MessagingAgentsDirectory", out var preBuiltAppBundleLocation)) {
				var preBuiltAppBundlePath = Path.Combine (preBuiltAppBundleLocation, "Xamarin.PreBuilt.iOS.app.zip");
				using var archive = System.IO.Compression.ZipFile.OpenRead (preBuiltAppBundlePath);
				prebuiltAppEntries = archive
					.Entries
					.Select (v => v.FullName)
					.SelectMany (v => {
						// This code has two purposes:
						// 1 - make sure the paths are using the current platform's directory separator char (instead of '/')
						// 2 - add both files and their containing directories to the list (since we check for directory presence later in this test)
						var components = v.Split (new char [] { '/', Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
						var rv = new List<string> ();
						for (var i = 0; i < components.Length; i++) {
							rv.Add (Path.Combine (components.Take (i + 1).ToArray ()));
						}
						return rv;
					})
					.ToHashSet ();

#if TRACE
				Console.WriteLine ($"Prebuilt app files:");
				foreach (var pbf in prebuiltAppEntries)
					Console.WriteLine ($"    {pbf}");
#endif
			} else {
				Assert.Fail ("Could not find the property 'MessagingAgentsDirectory' in the binlog.");
			}

			DumpDirContents (appPath);
			DumpDirContents (tmpdir);

			var hotRestartAppBundleFiles = BundleStructureTest.Find (hotRestartAppBundlePath);

			var payloadFiles = BundleStructureTest.Find (Path.Combine (hotRestartOutputDir, "Payload", "BundleStructure.app"));
			var contentFiles = BundleStructureTest.Find (Path.Combine (hotRestartOutputDir, "BundleStructure.content"));

			// Exclude most files from the prebuilt hot restart app
			var excludedPrebuiltAppEntries = prebuiltAppEntries
				.Where (v => {
					// We're not excluding some files that are common to all apps.
					switch (v) {
					case "Info.plist":
					case "MonoTouchDebugConfiguration.txt":
					case "PkgInfo":
					case "Settings.bundle":
					case "Settings.bundle\\Root.plist":
						return false;
					}
					return true;
				});
			hotRestartAppBundleFiles = hotRestartAppBundleFiles
				.Except (excludedPrebuiltAppEntries)
				.ToList ();

			var merged = hotRestartAppBundleFiles
				.Union (payloadFiles)
				.Union (contentFiles)
				.Where (v => {
					// remove files in the BundleStructure.content subdirectory
					if (v.StartsWith ("BundleStructure.content", StringComparison.Ordinal))
						return false;
					// hotrestart-specific files
					if (v == "Extracted")
						return false;
					if (v == "Entitlements.plist")
						return false;
					if (v == "BundleStructure.hotrestartapp")
						return false;
					return true;
				})
				.Distinct ()
				.OrderBy (v => v)
				.ToList ();

			// The reference to the bindings-framework-test project is skipped on Windows, because we can't build binding projects unless we're connected to a Mac.
			AddOrAssert (merged, "bindings-framework-test.dll");
			AddOrAssert (merged, "bindings-framework-test.pdb");
			AddOrAssert (merged, Path.Combine ("Frameworks", "XTest.framework")); // XTest.framework comes from bindings-framework-test.csproj
			AddOrAssert (merged, Path.Combine ("Frameworks", "XTest.framework", "Info.plist"));
			AddOrAssert (merged, Path.Combine ("Frameworks", "XTest.framework", "XTest"));

			// The name of the executable is different.
			AddOrAssert (merged, project);

			var rids = runtimeIdentifiers.Split (';');
			BundleStructureTest.CheckAppBundleContents (platform, merged, rids, BundleStructureTest.CodeSignature.None, configuration == "Release");
		}

		static void AddOrAssert (IList<string> list, string item)
		{
			Assert.That (list, Does.Not.Contain (item), $"item {item} already in list.");
			list.Add (item);
		}

		static void DumpDirContents (string dir)
		{
#if TRACE
			if (!Directory.Exists (dir)) {
				Console.WriteLine ($"The directory {dir} does not exist!");
				return;
			}
			var files = Directory.GetFileSystemEntries (dir, "*", SearchOption.AllDirectories);
			Console.WriteLine ($"Found {files.Count ()} in {dir}:");
			foreach (var entry in files.OrderBy (v => v))
				Console.WriteLine ($"    {entry}");
#endif
		}
	}
}
