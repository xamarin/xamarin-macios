// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class EnableCrashReportTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		public void Enabled (ApplePlatform platform, string runtimeIdentifiers)
		{
			// When $(EnableCrashReport) is set to true, the DOTNET_EnableCrashReport=1 environment
			// variable is set at startup, which makes the .NET runtime's in-process crash reporter
			// write a JSON crash report when the app crashes. Verify this by crashing the app on
			// launch and checking that a crash report file was created.
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["EnableCrashReport"] = "true";

			DotNet.AssertBuild (project_path, properties);

			if (!CanExecute (platform, runtimeIdentifiers))
				return;

			var appExecutable = GetNativeExecutable (platform, appPath);

			// Run with DOTNET_CrashReportRootPath set to a known, writable location. The runtime
			// only sets DOTNET_CrashReportRootPath if it's not already set (it uses setenv with
			// overwrite=0), so setting it here takes precedence and makes it easy to find the report.
			var customReportDir = Cache.CreateTemporaryDirectory ("crash-reports");
			AssertCrashReport (appExecutable, new Dictionary<string, string?> {
				{ "CRASH_ON_LAUNCH", "1" },
				{ "DOTNET_CrashReportRootPath", customReportDir },
			}, customReportDir);

			// Run without DOTNET_CrashReportRootPath: the runtime picks a default location and
			// verify a crash report shows up there too. MySimpleApp isn't sandboxed (it has no
			// app-sandbox entitlement), so for a non-sandboxed app the runtime uses
			// ~/Library/Caches/<bundleId> (a sandboxed app would instead use the caches directory
			// inside its container, ~/Library/Containers/<bundleId>/Data/Library/Caches).
			var bundleIdentifier = GetBundleIdentifier (platform, appPath);
			var cachesReportDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Caches", bundleIdentifier);
			var reportsDir = Path.Combine (cachesReportDir, ".dotnet", "crash-reports");
			if (Directory.Exists (reportsDir))
				Directory.Delete (reportsDir, true);
			AssertCrashReport (appExecutable, new Dictionary<string, string?> {
				{ "CRASH_ON_LAUNCH", "1" },
			}, cachesReportDir);
		}

		string GetBundleIdentifier (ApplePlatform platform, string appPath)
		{
			var infoPlistPath = Path.Combine (appPath, GetRelativeCodesignDirectory (platform), "Info.plist");
			var infoPlist = PDictionary.OpenFile (infoPlistPath);
			var bundleIdentifier = infoPlist?.GetString ("CFBundleIdentifier")?.Value;
			if (string.IsNullOrEmpty (bundleIdentifier))
				throw new InvalidOperationException ($"Could not read the bundle identifier from '{infoPlistPath}'.");
			return bundleIdentifier;
		}

		void AssertCrashReport (string executable, Dictionary<string, string?> environment, string crashReportDir)
		{
			var rv = Execute (executable, out var output, out var _, environment);
			Assert.That (rv.TimedOut, Is.False, $"The app timed out instead of crashing:\n{output}");
			Assert.That (rv.ExitCode, Is.Not.EqualTo (0), $"The app should have crashed:\n{output}");

			var crashReports = Directory.GetFiles (crashReportDir, "*.crashreport.json", SearchOption.AllDirectories);
			Assert.That (crashReports, Is.Not.Empty, $"A crash report should have been created in '{crashReportDir}':\n{output}");
		}
	}
}
