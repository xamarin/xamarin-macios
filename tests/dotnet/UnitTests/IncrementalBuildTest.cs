using Microsoft.Build.Logging.StructuredLogger;

#nullable enable

namespace Xamarin.Tests {
	public class IncrementalBuildTest : TestBaseClass {
		[Test]
		// this test is fairly slow, so execute on one arch only
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void Link (ApplePlatform platform, string runtimeIdentifiers)
		{
			LinkImpl (platform, runtimeIdentifiers);
		}

		void LinkImpl (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "IncrementalTestApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			properties ["UseMonoRuntime"] = "false";

			// Build the first time
			var rv = DotNet.AssertBuild (project_path, properties);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_LinkNativeExecutable", "A");

			// Make sure it runs successfully (if on desktop)
			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// Must not link with the frameworks from the nuget
			var lc_load_dylib = GetLoadCommands (appExecutable).ToArray ();
			Assert.That (lc_load_dylib, Does.Not.Contain ("@rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1"), "A: Should not link with @rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1");
			Assert.That (lc_load_dylib, Does.Not.Contain ("@rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2"), "A: Should not link with @rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2");

			// Capture when executable was created
			var appExecutableTimestamp = File.GetLastWriteTimeUtc (appExecutable);

			// Build again, adding a package with frameworks
			properties ["IncludeFwInRuntimesNativeDirectory"] = "true";
			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_LinkNativeExecutable", "B");

			// Executing should work just fine
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// Must link with the frameworks from the nuget
			lc_load_dylib = GetLoadCommands (appExecutable).ToArray ();
			Assert.That (lc_load_dylib, Does.Contain ("@rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1"), "B: Should link with @rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1");
			Assert.That (lc_load_dylib, Does.Contain ("@rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2"), "B: Should link with @rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2");

			// The main executable must be modified
			Assert.That (File.GetLastWriteTimeUtc (appExecutable), Is.GreaterThan (appExecutableTimestamp), "Modified B");

			// Capture when executable was rebuilt
			appExecutableTimestamp = File.GetLastWriteTimeUtc (appExecutable);

			// Build again, not doing anything
			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			// With CoreCLR, the app executable is re-linked because the generated R2R
			// framework participates in the native link inputs and is refreshed each build.
			AssertTargetExecuted (allTargets, "_LinkNativeExecutable", "C");

			// Executing should work just fine
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// Must still link with the frameworks from the nuget
			lc_load_dylib = GetLoadCommands (appExecutable).ToArray ();
			Assert.That (lc_load_dylib, Does.Contain ("@rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1"), "C: Should link with @rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1");
			Assert.That (lc_load_dylib, Does.Contain ("@rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2"), "C: Should link with @rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2");

			Assert.That (File.GetLastWriteTimeUtc (appExecutable), Is.GreaterThan (appExecutableTimestamp), "Modified C");
			appExecutableTimestamp = File.GetLastWriteTimeUtc (appExecutable);

			// Build yet again, now removing the package
			properties.Remove ("IncludeFwInRuntimesNativeDirectory");
			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_LinkNativeExecutable", "D");

			// Executing should work just fine
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);

			// Must not link with the frameworks from the nuget anymore
			lc_load_dylib = GetLoadCommands (appExecutable).ToArray ();
			Assert.That (lc_load_dylib, Does.Not.Contain ("@rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1"), "D: Should not link with @rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1");
			Assert.That (lc_load_dylib, Does.Not.Contain ("@rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2"), "D: Should not link with @rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2");

			// The main executable must be modified
			Assert.That (File.GetLastWriteTimeUtc (appExecutable), Is.GreaterThan (appExecutableTimestamp), "Modified D");
		}

		static IEnumerable<string> GetLoadCommands (string dylib)
		{
			var file = MachO.Read (dylib).Single ();
			foreach (var lc in file.load_commands) {
				if (lc is DylibLoadCommand dlc)
					yield return dlc.name;
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		public void AppManifest (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var projectPath = GenerateProject (platform, nameof (AppManifest), runtimeIdentifiers, out var appPath);
			var projectDirectory = Path.GetDirectoryName (projectPath)!;
			var appManifestPath = Path.Combine (projectDirectory, "Info.plist");
			var mainFile = Path.Combine (projectDirectory, "Main.cs");
			var properties = GetDefaultProperties (runtimeIdentifiers);

			File.WriteAllText (mainFile, "class MainClass { static int Main () => 0; }");
			WriteAppManifest (appManifestPath, "Initial");

			DotNet.AssertBuild (projectPath, properties);
			var bundledAppManifestPath = GetInfoPListPath (platform, appPath!);
			var appManifest = PDictionary.OpenFile (bundledAppManifestPath);
			Assert.That (appManifest.GetString ("IncrementalBuildValue").Value, Is.EqualTo ("Initial"), "Initial value");

			WriteAppManifest (appManifestPath, "Updated");
			var rv = DotNet.AssertBuild (projectPath, properties);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_CompileAppManifest", "Updated manifest");

			appManifest = PDictionary.OpenFile (bundledAppManifestPath);
			Assert.That (appManifest.GetString ("IncrementalBuildValue").Value, Is.EqualTo ("Updated"), "Updated value");

			rv = DotNet.AssertBuild (projectPath, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_CompileAppManifest", "Unchanged manifest");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		public void CompiledEntitlementsTriggerNativeLink (ApplePlatform platform, string runtimeIdentifiers)
		{
			CompiledEntitlementsTriggerNativeLinkImpl (platform, runtimeIdentifiers);
		}

		[Test]
		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		public void CompiledEntitlementsTriggerNativeLinkOnRemoteWindows (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfNotOnWindows ();
			CompiledEntitlementsTriggerNativeLinkImpl (platform, runtimeIdentifiers);
		}

		void CompiledEntitlementsTriggerNativeLinkImpl (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var projectPath = GenerateProject (platform, TestName, runtimeIdentifiers, out _);
			var projectDirectory = Path.GetDirectoryName (projectPath)!;
			var mainFile = Path.Combine (projectDirectory, "Main.cs");
			var entitlementsPath = Path.Combine (projectDirectory, "Entitlements.plist");
			var projectContents = File.ReadAllText (projectPath);

			projectContents = projectContents.Replace (
				"	</PropertyGroup>",
				"		<CodesignEntitlements>Entitlements.plist</CodesignEntitlements>\n" +
				"		<CodesignRequireProvisioningProfile>false</CodesignRequireProvisioningProfile>\n" +
				"	</PropertyGroup>");
			File.WriteAllText (projectPath, projectContents);
			File.WriteAllText (mainFile, "class MainClass { static int Main () => 0; }");
			WriteEntitlements (entitlementsPath, "example.com");

			var properties = GetDefaultProperties (runtimeIdentifiers);
			DotNet.AssertBuild (projectPath, properties);

			var rv = DotNet.AssertBuild (projectPath, properties);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_ForceLinkNativeExecutable", "Unchanged entitlements");

			WriteEntitlements (entitlementsPath, "example.org");
			rv = DotNet.AssertBuild (projectPath, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_ForceLinkNativeExecutable", "Updated entitlements");
			AssertTargetExecuted (allTargets, "_LinkNativeExecutable", "Updated entitlements");

			rv = DotNet.AssertBuild (projectPath, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_ForceLinkNativeExecutable", "Unchanged entitlements after update");
		}

		static void WriteEntitlements (string path, string domain)
		{
			File.WriteAllText (path, $"""
				<?xml version="1.0" encoding="UTF-8"?>
				<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
				<plist version="1.0">
				<dict>
					<key>com.apple.developer.associated-domains</key>
					<array>
						<string>applinks:{domain}</string>
					</array>
				</dict>
				</plist>
				""");
		}

		static void WriteAppManifest (string path, string value)
		{
			File.WriteAllText (path, $"""
				<?xml version="1.0" encoding="UTF-8"?>
				<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
				<plist version="1.0">
				<dict>
					<key>IncrementalBuildValue</key>
					<string>{value}</string>
				</dict>
				</plist>
				""");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		public void CodeChangeSkipsTargets (ApplePlatform platform, string runtimeIdentifiers)
		{
			CodeChangeSkipsTargetsImpl (platform, runtimeIdentifiers);
		}

		[Test]
		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		public void CodeChangeSkipsTargetsOnRemoteWindows (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfNotOnWindows ();
			CodeChangeSkipsTargetsImpl (platform, runtimeIdentifiers);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void MetalShadersNotRecompiled (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GenerateProject (platform, name: nameof (MetalShadersNotRecompiled), runtimeIdentifiers: runtimeIdentifiers, out var appPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			var projectDir = Path.GetDirectoryName (project_path)!;

			// Add a Main.cs so the project compiles
			File.WriteAllText (Path.Combine (projectDir, "Main.cs"), @"
class MainClass {
	static int Main ()
	{
		return 0;
	}
}
");

			// Add a Metal shader file to the project
			File.WriteAllText (Path.Combine (projectDir, "Shaders.metal"), @"
#include <metal_stdlib>
using namespace metal;

kernel void myKernel (texture2d<half, access::read> inTexture [[texture(0)]],
                      texture2d<half, access::write> outTexture [[texture(1)]],
                      uint2 gid [[thread_position_in_grid]])
{
}
");

			// Build the first time
			var rv = DotNet.AssertBuild (project_path, properties);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_SmeltMetal", "First build");
			AssertTargetExecuted (allTargets, "_TemperMetal", "First build");

			// Build again without any changes
			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);

			// _SmeltMetal should NOT execute on the second build since nothing changed
			AssertTargetNotExecuted (allTargets, "_SmeltMetal", "Second build");
			AssertTargetNotExecuted (allTargets, "_TemperMetal", "Second build");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void UserCodeChangeSkipsR2RCompilation_CoreCLR (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "IncrementalTestApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			var rv = DotNet.AssertBuild (project_path, properties);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_SelectR2RAssemblies", "First build");
			AssertTargetExecuted (allTargets, "_CreateR2RImages", "First build");
			var r2rInputHashesCachePath = Path.Combine (GetObjDir (project_path, platform, runtimeIdentifiers), "r2r-input-hashes.json");
			Assert.That (r2rInputHashesCachePath, Does.Exist, "R2R input hash cache");
			Assert.That (BinLog.TryFindPropertyValue (rv.BinLogPath, "_R2RInputFilesHashed", out var initialFilesHashed), Is.True, "Initial hash count");
			Assert.That (int.TryParse (initialFilesHashed, out var initialHashCount), Is.True, "Initial hash count value");
			Assert.That (initialHashCount, Is.GreaterThan (0), "Initial input files hashed");

			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_CreateR2RImages", "Unchanged build");
			Assert.That (BinLog.TryFindPropertyValue (rv.BinLogPath, "_R2RInputFilesHashed", out var unchangedFilesHashed), Is.True, "Unchanged hash count");
			Assert.That (int.TryParse (unchangedFilesHashed, out var unchangedHashCount), Is.True, "Unchanged hash count value");
			Assert.That (unchangedHashCount, Is.LessThan (initialHashCount), "Unchanged build rehashes fewer files");

			var coreLibPath = Path.Combine (GetObjDir (project_path, platform, runtimeIdentifiers), "postprocessed-assemblies", "System.Private.CoreLib.dll");
			Assert.That (coreLibPath, Does.Exist, "R2R input assembly");
			File.SetLastWriteTimeUtc (coreLibPath, DateTime.UtcNow.AddSeconds (-2));

			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_TouchR2ROutputs", "Timestamp-only input change");
			AssertTargetNotExecuted (allTargets, "_CreateR2RImages", "Timestamp-only input change");
			Assert.That (BinLog.TryFindPropertyValue (rv.BinLogPath, "_R2RInputFilesHashed", out var touchedFilesHashed), Is.True, "Timestamp-only hash count");
			Assert.That (touchedFilesHashed, Is.EqualTo ("1"), "Only the touched input was rehashed");

			var r2rInputHashPath = Path.Combine (GetObjDir (project_path, platform, runtimeIdentifiers), "r2r-input.hash");
			Assert.That (r2rInputHashPath, Does.Exist, "R2R input hash");
			File.SetLastWriteTimeUtc (r2rInputHashPath, DateTime.UtcNow.AddMinutes (1));

			properties ["AdditionalDefineConstants"] = "INCLUDED_ADDITIONAL_CODE";

			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetExecuted (allTargets, "_TouchR2ROutputs", "User code change");
			AssertTargetNotExecuted (allTargets, "_CreateR2RImages", "User code change");

			File.WriteAllText (r2rInputHashPath, "changed");
			properties.Remove ("AdditionalDefineConstants");

			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);
			AssertTargetNotExecuted (allTargets, "_TouchR2ROutputs", "R2R input change");
			AssertTargetExecuted (allTargets, "_CreateR2RImages", "R2R input change");
		}

		void CodeChangeSkipsTargetsImpl (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "IncrementalTestApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);

			properties ["UseMonoRuntime"] = "false";
			properties ["MtouchLink"] = "None";

			// Build the first time
			var rv = DotNet.AssertBuild (project_path, properties);
			var allTargets = BinLog.GetAllTargets (rv.BinLogPath);

			// Verify these targets executed on first build
			AssertTargetExecuted (allTargets, "_CreatePkgInfo", "A");
			AssertTargetExecuted (allTargets, "_CompileNativeExecutable", "A");
			AssertTargetExecuted (allTargets, "_LinkNativeExecutable", "A");

			// Make a code change
			properties ["AdditionalDefineConstants"] = "INCLUDED_ADDITIONAL_CODE";

			// Build again after modifying the helper C# file
			rv = DotNet.AssertBuild (project_path, properties);
			allTargets = BinLog.GetAllTargets (rv.BinLogPath);

			// Verify these targets did NOT execute on incremental build after C# change
			AssertTargetNotExecuted (allTargets, "_CreatePkgInfo", "B");
			AssertTargetNotExecuted (allTargets, "_CompileNativeExecutable", "B");
			// With the partial static registrar on CoreCLR, _LinkNativeExecutable should be skipped.
			AssertTargetNotExecuted (allTargets, "_LinkNativeExecutable", "B");
		}
	}
}
