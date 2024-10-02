// #define TRACE

using System.IO;
using System.IO.Compression;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class WindowsTest : TestBaseClass {

		[Category ("Windows")]
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
			AddHotRestartProperties (properties);

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
			var hotRestartAppBundleFilesWithoutPrebuiltFiles = hotRestartAppBundleFiles
				.Except (excludedPrebuiltAppEntries)
				.ToList ();

			var merged = hotRestartAppBundleFilesWithoutPrebuiltFiles
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

			// The reference to the bindings-xcframework-test project is skipped on Windows, because we can't build binding projects unless we're connected to a Mac.
			AddOrAssert (merged, "bindings-framework-test.dll");
			AddOrAssert (merged, "bindings-framework-test.pdb");
			AddOrAssert (merged, Path.Combine ("Frameworks", "XTest.framework")); // XTest.framework comes from bindings-framework-test.csproj
			AddOrAssert (merged, Path.Combine ("Frameworks", "XTest.framework", "Info.plist"));
			AddOrAssert (merged, Path.Combine ("Frameworks", "XTest.framework", "XTest"));

			// The name of the executable is different.
			AddOrAssert (merged, project);

			var rids = runtimeIdentifiers.Split (';');
			BundleStructureTest.CheckAppBundleContents (platform, merged, rids, BundleStructureTest.CodeSignature.None, configuration == "Release");

			// Assert that no files were copied to the signed directory after the app was signed.
			// https://github.com/xamarin/xamarin-macios/issues/19278
			var signedAppBundleFilesWithInfo = hotRestartAppBundleFiles.Select (v => new { Name = v, Info = new FileInfo (v) });
			Console.WriteLine ($"{signedAppBundleFilesWithInfo.Count ()} files in app bundle:");
			foreach (var fileWithInfo2 in signedAppBundleFilesWithInfo) {
				Console.WriteLine ($"    {fileWithInfo2.Info.LastWriteTimeUtc.ToString ("O")} {fileWithInfo2.Name}");
			}

			var codesignInfo = signedAppBundleFilesWithInfo.Single (v => v.Name.EndsWith ("_CodeSignature\\CodeResources"));
			var modifiedAfterSignature = signedAppBundleFilesWithInfo.Where (v => v.Info.LastWriteTimeUtc > codesignInfo.Info.LastWriteTimeUtc);
			if (modifiedAfterSignature.Any ()) {
				Console.WriteLine ($"{modifiedAfterSignature.Count ()} files were modified after the app was signed. Full list:");
				foreach (var fileWithInfo in signedAppBundleFilesWithInfo) {
					Console.WriteLine ($"    {fileWithInfo.Info.LastWriteTimeUtc.ToString ("O")} {(fileWithInfo.Info.LastWriteTimeUtc > codesignInfo.Info.LastWriteTimeUtc ? "MODIFIED " : "unchanged")} {fileWithInfo.Name}");
				}
				Assert.That (modifiedAfterSignature, Is.Empty, "Files modified after the app was signed");
			}
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

		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, "ios-arm64", BundleStructureTest.CodeSignature.All, "Debug")]
		public void BundleStructureWithRemoteMac (ApplePlatform platform, string runtimeIdentifiers, BundleStructureTest.CodeSignature signature, string configuration)
		{
			var project = "BundleStructure";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			Configuration.IgnoreIfNotOnWindows ();

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			var project_dir = Path.GetDirectoryName (Path.GetDirectoryName (project_path))!;
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["_IsAppSigned"] = signature != BundleStructureTest.CodeSignature.None ? "true" : "false";
			if (!string.IsNullOrWhiteSpace (configuration))
				properties ["Configuration"] = configuration;

			// Copy the app bundle to Windows so that we can inspect the results.
			properties ["CopyAppBundleToWindows"] = "true";

			var rv = DotNet.AssertBuild (project_path, properties);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			var warningMessages = BundleStructureTest.FilterWarnings (warnings, canonicalizePaths: true);

			var isReleaseBuild = string.Equals (configuration, "Release", StringComparison.OrdinalIgnoreCase);
			var platformString = platform.AsString ();
			var tfm = platform.ToFramework ();
			var testsDirectory = Path.GetDirectoryName (Path.GetDirectoryName (project_dir))!;
			var expectedWarnings = new List<string> {
				$"The 'PublishFolderType' metadata value 'Unknown' on the item '{Path.Combine (project_dir, platformString, "SomewhatUnknownI.bin")}' is not recognized. The file will not be copied to the app bundle. If the file is not supposed to be copied to the app bundle, remove the 'CopyToOutputDirectory' metadata on the item.",
				$"The 'PublishFolderType' metadata value 'Unknown' on the item '{Path.Combine (project_dir, platformString, "UnknownI.bin")}' is not recognized. The file will not be copied to the app bundle. If the file is not supposed to be copied to the app bundle, remove the 'CopyToOutputDirectory' metadata on the item.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneA.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneI.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneJ.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneK.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "NoneM.unknown")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, platformString, "Sub", "NoneG.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, "NoneH.txt")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
				$"The file '{Path.Combine (project_dir, "NoneO.xml")}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle.",
			};

			var rids = runtimeIdentifiers.Split (';');
			if (rids.Length > 1) {
				// All warnings show up twice if we're building for multiple architectures
				expectedWarnings.AddRange (expectedWarnings);
			}

			var zippedFrameworks = platform == ApplePlatform.MacCatalyst || platform == ApplePlatform.MacOSX;
			foreach (var rid in rids) {
				if (zippedFrameworks) {
					expectedWarnings.Add ($"The framework {Path.Combine ("obj", configuration, tfm, rid, "bindings-xcframework-test.resources.zip", "XStaticObjectTest.framework")} is a framework of static libraries, and will not be copied to the app.");
					expectedWarnings.Add ($"The framework {Path.Combine ("obj", configuration, tfm, rid, "bindings-xcframework-test.resources.zip", "XStaticArTest.framework")} is a framework of static libraries, and will not be copied to the app.");
				} else {
					expectedWarnings.Add ($"The framework {Path.Combine (testsDirectory, "bindings-xcframework-test", "dotnet", platformString, "bin", configuration, tfm, "bindings-framework-test.resources", "XStaticObjectTest.xcframework", runtimeIdentifiers, "XStaticObjectTest.framework")} is a framework of static libraries, and will not be copied to the app.");
					expectedWarnings.Add ($"The framework {Path.Combine (testsDirectory, "bindings-xcframework-test", "dotnet", platformString, "bin", configuration, tfm, "bindings-framework-test.resources", "XStaticArTest.xcframework", runtimeIdentifiers, "XStaticArTest.framework")} is a framework of static libraries, and will not be copied to the app.");
				}
			}

			if (signature == BundleStructureTest.CodeSignature.None && (platform == ApplePlatform.MacCatalyst || platform == ApplePlatform.MacOSX)) {
				expectedWarnings.Add ($"Found files in the root directory of the app bundle. This will likely cause codesign to fail. Files:\n{Path.Combine ("bin", configuration, tfm, runtimeIdentifiers.IndexOf (';') >= 0 ? string.Empty : runtimeIdentifiers, "BundleStructure.app", "UnknownJ.bin")}");
			}

			// Sort the messages so that comparison against the expected array is faster
			expectedWarnings = expectedWarnings
				.Select (v => v.Replace (Path.DirectorySeparatorChar, '/')) // warnings we get are from macOS, so make sure we expect macOS-style paths.
				.OrderBy (v => v)
				.ToList ();

			var appExecutable = GetNativeExecutable (platform, appPath);

			var objDir = GetObjDir (project_path, platform, runtimeIdentifiers, configuration);
			var zippedAppBundlePath = Path.Combine (objDir, "AppBundle.zip");
			Assert.That (zippedAppBundlePath, Does.Exist, "AppBundle.zip");

			BundleStructureTest.CheckZippedAppBundleContents (platform, zippedAppBundlePath, rids, signature, isReleaseBuild);
			AssertWarningsEqual (expectedWarnings, warningMessages, "Warnings");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// touch AppDelegate.cs, and rebuild should succeed and do the right thing
			var appDelegatePath = Path.Combine (project_dir, "AppDelegate.cs");
			Configuration.Touch (appDelegatePath);

			rv = DotNet.AssertBuild (project_path, properties);
			warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			warningMessages = BundleStructureTest.FilterWarnings (warnings, canonicalizePaths: true);

			BundleStructureTest.CheckZippedAppBundleContents (platform, zippedAppBundlePath, rids, signature, isReleaseBuild);
			AssertWarningsEqual (expectedWarnings, warningMessages, "Warnings Rebuild 1");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// remove the bin directory, and rebuild should succeed and do the right thing
			var binDirectory = Path.Combine (Path.GetDirectoryName (project_path)!, "bin");
			Directory.Delete (binDirectory, true);

			rv = DotNet.AssertBuild (project_path, properties);
			warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			warningMessages = BundleStructureTest.FilterWarnings (warnings, canonicalizePaths: true);

			BundleStructureTest.CheckZippedAppBundleContents (platform, zippedAppBundlePath, rids, signature, isReleaseBuild);
			AssertWarningsEqual (expectedWarnings, warningMessages, "Warnings Rebuild 2");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// a simple rebuild should succeed
			rv = DotNet.AssertBuild (project_path, properties);
			warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).ToArray ();
			warningMessages = BundleStructureTest.FilterWarnings (warnings, canonicalizePaths: true);

			BundleStructureTest.CheckZippedAppBundleContents (platform, zippedAppBundlePath, rids, signature, isReleaseBuild);
			AssertWarningsEqual (expectedWarnings, warningMessages, "Warnings Rebuild 3");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);
		}

		[Category ("Windows")]
		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void PluralRuntimeIdentifiersWithHotRestart (ApplePlatform platform, string runtimeIdentifiers)
		{
			var properties = AddHotRestartProperties ();
			DotNetProjectTest.PluralRuntimeIdentifiersImpl (platform, runtimeIdentifiers, properties, isUsingHotRestart: true);
		}

		static void AssertWarningsEqual (IList<string> expected, IList<string> actual, string message)
		{
			if (expected.Count == actual.Count) {
				var equal = true;
				for (var i = 0; i < actual.Count; i++) {
					if (expected [i] != actual [i]) {
						equal = false;
						break;
					}
				}
				if (equal)
					return;
			}

			var sb = new StringBuilder ();
			sb.AppendLine ($"Incorrect warnings: {message}");
			sb.AppendLine ($"Expected {expected.Count} warnings:");
			for (var i = 0; i < expected.Count; i++)
				sb.AppendLine ($"\t#{i + 1}: {expected [i]}");
			sb.AppendLine ($"Got {actual.Count} warnings:");
			for (var i = 0; i < actual.Count; i++) {
				if (i < expected.Count && actual [i] == expected [i]) {
					sb.AppendLine ($"\t#{i + 1}: {actual [i]}");
				} else {
					sb.AppendLine ($"\t!{i + 1}: {actual [i]}");
				}
			}

			Console.WriteLine (sb);
			Assert.Fail (sb.ToString ());
		}

		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void RemoteTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			var configuration = "Debug";

			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);
			Configuration.IgnoreIfNotOnWindows ();

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			var project_dir = Path.GetDirectoryName (project_path)!;
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);

			// Copy the app bundle to Windows so that we can inspect the results.
			properties ["CopyAppBundleToWindows"] = "true";

			var result = DotNet.AssertBuild (project_path, properties, timeout: TimeSpan.FromMinutes (15));
			AssertThatLinkerExecuted (result);

			var objDir = GetObjDir (project_path, platform, runtimeIdentifiers, configuration);
			var zippedAppBundlePath = Path.Combine (objDir, "AppBundle.zip");
			Assert.That (zippedAppBundlePath, Does.Exist, "AppBundle.zip");

			// Open the zipped app bundle and get the Info.plist
			using var zip = ZipFile.OpenRead (zippedAppBundlePath);
			ZipHelpers.DumpZipFile (zip, zippedAppBundlePath);
			var infoPlistEntry = zip.Entries.SingleOrDefault (v => v.Name == "Info.plist")!;
			Assert.NotNull (infoPlistEntry, "Info.plist");

			// Parse the Info.plist
			// PDictionary.FromStream requires a seekable stream, but the zip stream isn't seekable, so copy to a
			// MemoryStream and use that. Info.plist files aren't big, so this shouldn't become a memory consumption problem.
			using var memoryStream = new MemoryStream ((int) infoPlistEntry.Length);
			using var plistStream = infoPlistEntry.Open ();
			plistStream.CopyTo (memoryStream);

			var infoPlist = (PDictionary) PDictionary.FromStream (memoryStream)!;
			Assert.AreEqual ("com.xamarin.mysimpleapp", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MySimpleApp", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleVersion").Value, "CFBundleVersion");
			Assert.AreEqual ("3.14", infoPlist.GetString ("CFBundleShortVersionString").Value, "CFBundleShortVersionString");
		}

		protected Dictionary<string, string> AddHotRestartProperties (Dictionary<string, string>? properties = null)
		{
			properties ??= new Dictionary<string, string> ();
			properties ["IsHotRestartBuild"] = "true";
			properties ["IsHotRestartEnvironmentReady"] = "true";
			properties ["EnableCodeSigning"] = "false"; // Skip code signing, since that would require making sure we have code signing configured on bots.
			properties ["_IsAppSigned"] = "false";
			properties ["_AppIdentifier"] = "placeholder_AppIdentifier"; // This needs to be set to a placeholder value because DetectSigningIdentity usually does it (and we've disabled signing)
			properties ["_BundleIdentifier"] = "placeholder_BundleIdentifier"; // This needs to be set to a placeholder value because DetectSigningIdentity usually does it (and we've disabled signing)
			return properties;
		}
	}
}
