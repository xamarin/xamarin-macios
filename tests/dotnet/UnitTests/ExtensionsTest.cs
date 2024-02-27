using System.Collections.Generic;

using Microsoft.Build.Logging.StructuredLogger;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class ExtensionsTest : TestBaseClass {
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void AdditionalAppExtensionTest (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "AdditionalAppExtensionConsumer";
			var extensionProject = "NativeIntentsExtension";
			var configuration = "Debug";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers, platform, out var appPath);

			// Manually build the native extension project first using xcodebuild
			var xcodeProjectFolder = Path.Combine (Path.GetDirectoryName (Path.GetDirectoryName (project_path))!, "nativeextension", platform.AsString ());
			var xcodeBuildArgs = new [] {
				"-configuration", configuration,
				"-target", "NativeIntentsExtension",
				"-project", Path.Combine (xcodeProjectFolder, "NativeContainer.xcodeproj"),
			};
			var env = new Dictionary<string, string> {
				{ "DEVELOPER_DIR", Configuration.XcodeLocation },
			};
			foreach (var action in new string [] { "clean", "build" })
				ExecutionHelper.Execute ("/usr/bin/xcodebuild", xcodeBuildArgs.Concat (new [] { action }).ToArray (), environmentVariables: env, timeout: TimeSpan.FromMinutes (1), throwOnError: true);

			string buildPlatform;
			switch (platform) {
			case ApplePlatform.iOS:
				buildPlatform = "-iphoneos";
				break;
			case ApplePlatform.TVOS:
				buildPlatform = "-appletvos";
				break;
			default:
				buildPlatform = string.Empty;
				break;
			}

			// Now build the containing project
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties.Add ("AdditionalAppExtensionPath", xcodeProjectFolder);
			properties.Add ("AdditionalAppExtensionBuildOutput", $"build/{configuration}{buildPlatform}");
			var rv = DotNet.AssertBuild (project_path, properties);

			var expectedDirectories = new List<string> ();
			if (IsRuntimeIdentifierSigned (runtimeIdentifiers)) {
				expectedDirectories.Add (Path.Combine (appPath, "_CodeSignature"));
				expectedDirectories.Add (Path.Combine (appPath, "PlugIns", extensionProject + ".appex", "_CodeSignature"));
			}

			foreach (var dir in expectedDirectories)
				Assert.That (dir, Does.Exist, "Directory should exist.");
		}
	}
}
