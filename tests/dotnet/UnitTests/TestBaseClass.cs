#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Mono.Cecil;

using NUnit.Framework;

using Xamarin.MacDev;
using Xamarin.Tests;

namespace Xamarin.Tests {
	[TestFixture]
	public abstract class TestBaseClass {
		protected static Dictionary<string, string> verbosity = new Dictionary<string, string> {
			{ "_BundlerVerbosity", "1" },
		};

		readonly char [] invalidChars = { '{', '}', '(', ')', '$', ':', ';', '\"', '\'', ',', '=', '|' };
		protected string TestName {
			get {
				var result = TestContext.CurrentContext.Test.Name;
				foreach (var c in invalidChars.Concat (Path.GetInvalidPathChars ().Concat (Path.GetInvalidFileNameChars ()))) {
					result = result.Replace (c, '_');
				}
				return result.Replace ("_", string.Empty);
			}
		}

		protected static Dictionary<string, string> GetDefaultProperties (string? runtimeIdentifiers = null, Dictionary<string, string>? extraProperties = null, bool? includeRemoteProperties = null)
		{
			var rv = new Dictionary<string, string> (verbosity);
			if (!string.IsNullOrEmpty (runtimeIdentifiers))
				SetRuntimeIdentifiers (rv, runtimeIdentifiers);
			if (extraProperties is not null) {
				foreach (var kvp in extraProperties)
					rv [kvp.Key] = kvp.Value;
			}

			if (!includeRemoteProperties.HasValue)
				includeRemoteProperties = Configuration.IsBuildingRemotely;

			if (includeRemoteProperties == true)
				AddRemoteProperties (rv);

			// We must set 'UseFloatingTargetPlatformVersion=true' for our test projects, to avoid building them with other workloads than the current workload.
			rv ["UseFloatingTargetPlatformVersion"] = "true";

			return rv;
		}

		protected static Dictionary<string, string> AddRemoteProperties (Dictionary<string, string>? properties = null)
		{
			properties ??= new Dictionary<string, string> ();
			properties ["ServerAddress"] = Environment.GetEnvironmentVariable ("MAC_AGENT_IP") ?? string.Empty;
			properties ["ServerUser"] = Environment.GetEnvironmentVariable ("MAC_AGENT_USER") ?? string.Empty;
			properties ["ServerPassword"] = Environment.GetEnvironmentVariable ("XMA_PASSWORD") ?? string.Empty;
			if (!string.IsNullOrEmpty (properties ["ServerUser"]))
				properties ["ContinueOnDisconnected"] = "false";
			return properties;
		}

		protected static void SetRuntimeIdentifiers (Dictionary<string, string> properties, string runtimeIdentifiers)
		{
			var multiRid = runtimeIdentifiers.IndexOf (';') >= 0 ? "RuntimeIdentifiers" : "RuntimeIdentifier";
			properties [multiRid] = runtimeIdentifiers;
		}

		protected static string GetProjectPath (string project, string runtimeIdentifiers, ApplePlatform platform, out string appPath, string? subdir = null, string configuration = "Debug", string? netVersion = null, string? applicationTitle = null)
		{
			return GetProjectPath (project, null, runtimeIdentifiers, platform, out appPath, configuration, netVersion, applicationTitle);
		}

		protected static string GetProjectPath (string project, string? subdir, string runtimeIdentifiers, ApplePlatform platform, out string appPath, string configuration = "Debug", string? netVersion = null, string? applicationTitle = null)
		{
			var rv = GetProjectPath (project, subdir, platform);
			if (applicationTitle is null)
				applicationTitle = project;
			appPath = Path.Combine (GetOutputPath (project, subdir, runtimeIdentifiers, platform, configuration, netVersion), applicationTitle + ".app");
			return rv;
		}

		protected string GetAppPath (string projectPath, ApplePlatform platform, string runtimeIdentifiers, string configuration = "Debug")
		{
			return Path.Combine (GetBinDir (projectPath, platform, runtimeIdentifiers, configuration), Path.GetFileNameWithoutExtension (projectPath) + ".app");
		}

		protected string GetBinDir (string projectPath, ApplePlatform platform, string runtimeIdentifiers, string configuration = "Debug")
		{
			return GetBinOrObjDir ("bin", projectPath, platform, runtimeIdentifiers, configuration);
		}

		internal static protected string GetObjDir (string projectPath, ApplePlatform platform, string runtimeIdentifiers, string configuration = "Debug")
		{
			return GetBinOrObjDir ("obj", projectPath, platform, runtimeIdentifiers, configuration);
		}

		internal static protected string GetBinOrObjDir (string binOrObj, string projectPath, ApplePlatform platform, string runtimeIdentifiers, string configuration = "Debug")
		{
			var appPathRuntimeIdentifier = runtimeIdentifiers.IndexOf (';') >= 0 ? "" : runtimeIdentifiers;
			return Path.Combine (Path.GetDirectoryName (projectPath)!, binOrObj, configuration, platform.ToFramework (), appPathRuntimeIdentifier);
		}

		protected static string GetOutputPath (string project, string? subdir, string runtimeIdentifiers, ApplePlatform platform, string configuration = "Debug", string? netVersion = null)
		{
			var rv = GetProjectPath (project, subdir, platform);
			if (string.IsNullOrEmpty (runtimeIdentifiers))
				runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform, configuration);
			var appPathRuntimeIdentifier = runtimeIdentifiers.IndexOf (';') >= 0 ? "" : runtimeIdentifiers;
			return Path.Combine (Path.GetDirectoryName (rv)!, "bin", configuration, platform.ToFramework (netVersion), appPathRuntimeIdentifier);
		}

		protected static string GetDefaultRuntimeIdentifier (ApplePlatform platform, string configuration = "Debug")
		{
			var arch = Configuration.CanRunArm64 ? "arm64" : "x64";
			switch (platform) {
			case ApplePlatform.iOS:
				return "iossimulator-arm64";
			case ApplePlatform.TVOS:
				return "tvossimulator-arm64";
			case ApplePlatform.MacOSX:
				if ("Release".Equals (configuration, StringComparison.OrdinalIgnoreCase))
					return "osx-x64;osx-arm64";
				return $"osx-{arch}";
			case ApplePlatform.MacCatalyst:
				if ("Release".Equals (configuration, StringComparison.OrdinalIgnoreCase))
					return "maccatalyst-x64;maccatalyst-arm64";
				return $"maccatalyst-{arch}";
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		protected static string GetProjectPath (string project, string? subdir = null, ApplePlatform? platform = null)
		{
			if (TryGetTestProjectPath (project, platform ?? ApplePlatform.None, out var testProjectPath))
				return testProjectPath;

			var project_dir = Path.Combine (Configuration.SourceRoot, "tests", "dotnet", project);
			if (!string.IsNullOrEmpty (subdir))
				project_dir = Path.Combine (project_dir, subdir);

			var project_path = Path.Combine (project_dir, project + ".csproj");
			if (File.Exists (project_path))
				return project_path;

			if (platform.HasValue)
				project_dir = Path.Combine (project_dir, platform.Value.AsString ());

			project_path = Path.Combine (project_dir, project + ".csproj");
			if (!File.Exists (project_path))
				project_path = Path.ChangeExtension (project_path, "sln");

			if (!File.Exists (project_path))
				throw new FileNotFoundException ($"Could not find the project or solution {project} - {project_path} does not exist.");

			return project_path;
		}

		static bool TryGetTestProjectPath (string project, ApplePlatform platform, [NotNullWhen (true)] out string? projectPath)
		{
			projectPath = null;

			switch (project) {
			case "monotouch-test":
				projectPath = Path.Combine (Configuration.SourceRoot, "tests", project, "dotnet", platform.AsString (), project + ".csproj");
				return true;
			}
			return false;
		}

		protected string GetPlugInsRelativePath (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return "PlugIns";
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				return Path.Combine ("Contents", "PlugIns");
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		protected string GetFrameworksRelativePath (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return "Frameworks";
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				return Path.Combine ("Contents", "Frameworks");
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		protected static void Clean (string project_path)
		{
			var dirs = Directory.GetDirectories (Path.GetDirectoryName (project_path)!, "*", SearchOption.AllDirectories);
			dirs = dirs.OrderBy (v => v.Length).Reverse ().ToArray (); // If we have nested directories, make sure to delete the nested one first
			foreach (var dir in dirs) {
				var name = Path.GetFileName (dir);
				if (name != "bin" && name != "obj")
					continue;
				Directory.Delete (dir, true);
			}
		}

		protected static bool CanExecute (ApplePlatform platform, Dictionary<string, string> properties)
		{
			if (properties.TryGetValue ("RuntimeIdentifier", out var runtimeIdentifiers)) {
				return CanExecute (platform, runtimeIdentifiers);
			} else if (properties.TryGetValue ("RuntimeIdentifiers", out runtimeIdentifiers)) {
				return CanExecute (platform, runtimeIdentifiers);
			}
			return false;
		}

		protected static bool CanExecute (ApplePlatform platform, string runtimeIdentifiers)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return false;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				// If we're targetting x64, then we can execute everywhere
				if (runtimeIdentifiers.Contains ("-x64", StringComparison.Ordinal))
					return true;

				// If we're not targeting x64, and we're executing on x64, then we're out of luck
				if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
					return false;

				// Otherwise we can still execute.
				return true;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		protected string GetRelativeResourcesDirectory (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return "Resources";
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return Path.Combine ("Contents", "Resources");
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		protected string GetRelativeAssemblyDirectory (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return string.Empty;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return Path.Combine ("Contents", "MonoBundle");
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}

		protected string GetRelativeDylibDirectory (ApplePlatform platform)
		{
			return GetRelativeAssemblyDirectory (platform);
		}

		protected string GetInfoPListPath (ApplePlatform platform, string app_directory)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return Path.Combine (app_directory, "Info.plist");
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return Path.Combine (app_directory, "Contents", "Info.plist");
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}

		protected void AssertBundleAssembliesStripStatus (string appPath, bool shouldStrip)
		{
			var assemblies = Directory.GetFiles (appPath, "*.dll", SearchOption.AllDirectories);
			var assembliesWithOnlyEmptyMethods = new List<String> ();
			foreach (var assembly in assemblies) {
				ModuleDefinition definition = ModuleDefinition.ReadModule (assembly, new ReaderParameters { ReadingMode = ReadingMode.Deferred });

				// ReadyToRun images (produced by crossgen2) are marked as an IL library instead of IL-only,
				// and the IL bodies of their R2R-compiled methods may have been removed (the .NET SDK enables
				// PublishReadyToRunStripILBodies by default for iOS-like RIDs in release builds). Such methods
				// still contain code (as native code) and weren't emptied by our own IL stripper, but their
				// (removed) IL bodies can't be inspected with Mono.Cecil, so don't try - treat any method with
				// a body as non-empty.
				var isReadyToRunImage = (definition.Attributes & ModuleAttributes.ILOnly) == 0;

				var nonEmptyMethods = definition.Assembly.MainModule.Types.SelectMany (t =>
					t.Methods.Where (m => m.HasBody && (isReadyToRunImage || m.Body.Instructions.Count > 1))).ToArray ();
				var onlyHasEmptyMethods = !nonEmptyMethods.Any ();
				if (onlyHasEmptyMethods) {
					assembliesWithOnlyEmptyMethods.Add (assembly);
				} else if (shouldStrip) {
					Console.WriteLine ($"The assembly {definition.Assembly.Name} has {nonEmptyMethods.Length} non-empty methods ({assembly}):");
					foreach (var m in nonEmptyMethods)
						Console.WriteLine ($"    {m}: {m.Body.Instructions.Count} instructions.");
				}
			}

			// Some assemblies, such as Facades, will be completely empty even when not stripped
			if (shouldStrip) {
				Assert.That (assembliesWithOnlyEmptyMethods, Is.EquivalentTo (assemblies), $"Unexpected stripping status: some assemblies contains methods that weren't fully stripped.");
			} else {
				Assert.That (assembliesWithOnlyEmptyMethods, Is.Not.EquivalentTo (assemblies), $"Unexpected stripping status: no methods in any assembly contains any code.");
			}
		}

		protected void AssertDSymDirectory (string appPath)
		{
			var dSYMDirectory = appPath + ".dSYM";
			Assert.That (dSYMDirectory, Does.Exist, "dsym directory");
		}

		// Assert that the expected dSYMs exist for all binaries in the app bundle, and that no unexpected dSYMs exist.
		protected void AssertExpectedDSyms (ApplePlatform platform, string appPath)
		{
			var appContainerDir = Path.GetDirectoryName (appPath)!;
			var appBundleName = Path.GetFileName (appPath);

			// Collect expected dSYM names based on the binaries in the app bundle
			var expectedDSyms = new HashSet<string> ();

			// The app bundle itself should have a dSYM
			expectedDSyms.Add (appBundleName + ".dSYM");

			// Find frameworks in the app bundle
			var frameworksDir = Path.Combine (appPath, GetFrameworksRelativePath (platform));
			if (Directory.Exists (frameworksDir)) {
				foreach (var frameworkDir in Directory.GetDirectories (frameworksDir, "*.framework")) {
					var frameworkName = Path.GetFileNameWithoutExtension (frameworkDir);
					var frameworkBinary = Path.Combine (frameworkDir, frameworkName);
					if (File.Exists (frameworkBinary))
						expectedDSyms.Add (frameworkName + ".framework.dSYM");
				}
			}

			// Find dylibs in the app bundle
			var contentsRelativeDir = GetRelativeDylibDirectory (platform);
			var contentsDir = string.IsNullOrEmpty (contentsRelativeDir) ? appPath : Path.Combine (appPath, contentsRelativeDir);
			if (Directory.Exists (contentsDir)) {
				foreach (var dylib in Directory.GetFiles (contentsDir, "*.dylib")) {
					var fileName = Path.GetFileNameWithoutExtension (dylib);
					expectedDSyms.Add (fileName + ".dSYM");
				}
			}

			// Find actual dSYM directories
			var actualDSyms = Directory.GetDirectories (appContainerDir, "*.dSYM")
				.Select (d => Path.GetFileName (d))
				.ToHashSet ();

			var missingDSyms = expectedDSyms.Except (actualDSyms).OrderBy (v => v).ToList ();
			var unexpectedDSyms = actualDSyms.Except (expectedDSyms).OrderBy (v => v).ToList ();

			if (missingDSyms.Count > 0)
				Console.WriteLine ($"    Missing dSYMs:\n        {string.Join ("\n        ", missingDSyms)}");
			if (unexpectedDSyms.Count > 0)
				Console.WriteLine ($"    Unexpected dSYMs:\n        {string.Join ("\n        ", unexpectedDSyms)}");

			Assert.That (missingDSyms, Is.Empty, "Missing dSYMs");
			Assert.That (unexpectedDSyms, Is.Empty, "Unexpected dSYMs");
		}

		protected static string GetNativeExecutable (ApplePlatform platform, string app_directory)
		{
			var executableName = Path.GetFileNameWithoutExtension (app_directory);
			return Path.Combine (app_directory, GetRelativeExecutableDirectory (platform), executableName);
		}

		protected static string GetRelativeExecutableDirectory (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return string.Empty;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return Path.Combine ("Contents", "MacOS");
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}

		protected string GetRelativeCodesignDirectory (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return string.Empty;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return "Contents";
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}

		protected string GetResourcesDirectory (ApplePlatform platform, string app_directory)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return app_directory;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return Path.Combine (app_directory, "Contents", "Resources");
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}

		protected string GenerateProject (ApplePlatform platform, string name, string runtimeIdentifiers, out string? appPath)
		{
			var dir = Cache.CreateTemporaryDirectory (name);
			var csproj = Path.Combine (dir, $"{name}.csproj");
			var sb = new StringBuilder ();
			sb.AppendLine ($"<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			sb.AppendLine ($"<Project Sdk=\"Microsoft.NET.Sdk\">");
			sb.AppendLine ($"	<PropertyGroup>");
			sb.AppendLine ($"		<TargetFramework>{platform.ToFramework ()}</TargetFramework>");
			sb.AppendLine ($"		<OutputType>Exe</OutputType>");
			sb.AppendLine ($"		<ApplicationTitle>{name}</ApplicationTitle>");
			sb.AppendLine ($"		<ApplicationId>com.xamarin.testproject.{name}</ApplicationId>");
			sb.AppendLine ($"	</PropertyGroup>");
			sb.AppendLine ($"</Project>");

			File.WriteAllText (csproj, sb.ToString ());

			var appPathRuntimeIdentifier = runtimeIdentifiers.IndexOf (';') >= 0 ? "" : runtimeIdentifiers;
			appPath = Path.Combine (dir, "bin", "Debug", platform.ToFramework (), appPathRuntimeIdentifier, name + ".app");

			return csproj;
		}

		protected string ExecuteWithMagicWordAndAssert (ApplePlatform platform, string runtimeIdentifiers, string executable, Dictionary<string, string?>? environment = null)
		{
			if (!CanExecute (platform, runtimeIdentifiers))
				return string.Empty;

			return ExecuteWithMagicWordAndAssert (executable, environment);
		}

		protected string ExecuteWithMagicWordAndAssert (string executable, Dictionary<string, string?>? environment = null, int expectedExitCode = 0)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				Console.WriteLine ($"Not executing '{executable}' because we're on Windows.");
				return string.Empty;
			}

			var rv = Execute (executable, out var output, out string magicWord, environment);
			var outputString = output.ToString ();
			if (rv.ExitCode != expectedExitCode) {
				var msg = $"'{executable}' exited with exit code {rv.ExitCode} (expected exit code {expectedExitCode}) (timed out: {rv.TimedOut} timeout: {rv.Timeout}):" +
							"\t" + outputString.Replace ("\n", "\n\t").TrimEnd (new char [] { '\n', '\t' });
				Console.WriteLine (msg);
				Assert.Fail (msg);
			}
			if (!outputString.Contains (magicWord)) {
				var msg = $"'{executable}' exited with exit code {rv.ExitCode} as expected, but did not contain the magic word '{magicWord}' ({outputString.Length}):" +
							"\t" + outputString.Replace ("\n", "\n\t").TrimEnd (new char [] { '\n', '\t' });
				Console.WriteLine (msg);
				Assert.Fail (msg);
			}
			return outputString;
		}

		protected Execution Execute (string executable, out string output, out string magicWord, Dictionary<string, string?>? environment = null)
		{
			if (!File.Exists (executable))
				throw new FileNotFoundException ($"The executable '{executable}' does not exists.");

			magicWord = Guid.NewGuid ().ToString ();
			var env = new Dictionary<string, string?> {
				{ "MAGIC_WORD", magicWord },
				{ "DYLD_FALLBACK_LIBRARY_PATH", null }, // VSMac might set this, which may cause tests to crash.
			};
			if (environment is not null) {
				foreach (var kvp in environment)
					env [kvp.Key] = kvp.Value;
			}

			DeleteSavedState (executable, false);
			try {
				var rv = Execution.RunAsync (executable, Array.Empty<string> (), environment: env, timeout: TimeSpan.FromSeconds (30)).Result;
				output = rv.Output.MergedOutput;
				return rv;
			} finally {
				// Remove the override so it doesn't affect a later test using the same bundle identifier.
				DeleteSavedState (executable, true);
			}
		}

		// Delete the saved application state for the app being launched, to prevent
		// the "Do you want to try to reopen its windows again?" dialog from showing
		// if the app crashed during a previous test run. See https://github.com/dotnet/macios/issues/25922
		static void DeleteSavedState (string executable, bool cleanup)
		{
			var bundleIdentifier = GetBundleIdentifier (executable);
			if (string.IsNullOrEmpty (bundleIdentifier))
				return;

			var savedStateParentDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Saved Application State");
			// Mac Catalyst apps run under the "iosmac" personality, and macOS stores their saved state
			// with a "~iosmac" suffix added to the bundle identifier, so delete both variants.
			foreach (var identifier in new [] { bundleIdentifier, $"{bundleIdentifier}~iosmac" }) {
				var savedStateDir = Path.Combine (savedStateParentDir, $"{identifier}.savedState");
				try {
					if (Directory.Exists (savedStateDir)) {
						Directory.Delete (savedStateDir, true);
						Console.WriteLine ($"Deleted saved application state: {savedStateDir}");
					}

					if (cleanup) {
						Execution.RunAsync ("/usr/bin/defaults", new [] { "delete", bundleIdentifier, "ApplePersistenceIgnoreState" }, timeout: TimeSpan.FromSeconds (30)).Wait ();
					} else {
						Execution.RunAsync ("/usr/bin/defaults", new [] { "write", bundleIdentifier, "ApplePersistenceIgnoreState", "-bool", "YES" }, timeout: TimeSpan.FromSeconds (30)).Wait ();
					}
				} catch (Exception e) {
					Console.WriteLine ($"Could not delete saved application state '{savedStateDir}': {e.Message}");
				}
			}
		}

		static string? GetBundleIdentifier (string executable)
		{
			// Find the .app bundle directory from the executable path
			var dir = Path.GetDirectoryName (executable);
			while (!string.IsNullOrEmpty (dir) && !dir.EndsWith (".app", StringComparison.OrdinalIgnoreCase))
				dir = Path.GetDirectoryName (dir);

			if (string.IsNullOrEmpty (dir))
				return null;

			// Read the bundle identifier from Info.plist
			var infoPlistPath = Path.Combine (dir, "Contents", "Info.plist");
			if (!File.Exists (infoPlistPath))
				infoPlistPath = Path.Combine (dir, "Info.plist");
			if (!File.Exists (infoPlistPath))
				return null;

			try {
				var infoPlist = PDictionary.OpenFile (infoPlistPath);
				return infoPlist.GetString ("CFBundleIdentifier")?.Value;
			} catch (Exception e) {
				Console.WriteLine ($"Could not read bundle identifier from '{infoPlistPath}': {e.Message}");
				return null;
			}
		}

		public static StringBuilder AssertExecute (string executable, params string [] arguments)
		{
			return AssertExecute (executable, arguments, out _);
		}

		public static StringBuilder AssertExecute (string executable, string [] arguments, out StringBuilder output)
		{
			var rv = ExecutionHelper.Execute (executable, arguments, out output);
			if (rv != 0) {
				Console.WriteLine ($"'{executable} {StringUtils.FormatArguments (arguments)}' exited with exit code {rv}:");
				Console.WriteLine ("\t" + output.ToString ().Replace ("\n", "\n\t").TrimEnd (new char [] { '\n', '\t' }));
			}
			Assert.That (rv, Is.EqualTo (0), $"Unable to execute '{executable} {StringUtils.FormatArguments (arguments)}': exit code {rv}");
			return output;
		}

		protected void ExecuteProjectWithMagicWordAndAssert (string csproj, ApplePlatform platform, string? runtimeIdentifiers = null)
		{
			if (runtimeIdentifiers is null)
				runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);

			var appPath = GetAppPath (csproj, platform, runtimeIdentifiers);
			var appExecutable = GetNativeExecutable (platform, appPath);
			ExecuteWithMagicWordAndAssert (appExecutable);
		}

		protected bool IsRuntimeIdentifierSigned (string runtimeIdentifiers)
		{
			foreach (var rid in runtimeIdentifiers.Split (';', StringSplitOptions.RemoveEmptyEntries)) {
				if (rid.StartsWith ("ios-", StringComparison.OrdinalIgnoreCase))
					return true;
				if (rid.StartsWith ("tvos-", StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

		protected bool TryGetEntitlements (string nativeExecutable, [NotNullWhen (true)] out PDictionary? entitlements)
		{
			var entitlementsPath = Path.Combine (Cache.CreateTemporaryDirectory (), "EntitlementsInBinary.plist");
			var args = new string [] {
				"--display",
				"--entitlements",
				entitlementsPath,
				"--xml",
				nativeExecutable
			};
			var rv = ExecutionHelper.Execute ("codesign", args, out var codesignOutput, TimeSpan.FromSeconds (15));
			Assert.That (rv, Is.EqualTo (0), $"'codesign {string.Join (" ", args)}' failed:\n{codesignOutput}");
			if (File.Exists (entitlementsPath)) {
				entitlements = PDictionary.OpenFile (entitlementsPath);
				return entitlements is not null;
			}
			entitlements = null;
			return false;
		}

		public static void AssertWarningMessages (IList<BuildLogEvent> actualWarnings, params string [] expectedWarningMessages)
		{
			AssertBuildMessages ("warning", actualWarnings, expectedWarningMessages);
		}

		public static void AssertErrorMessages (IList<BuildLogEvent> actualErrors, params string [] expectedErrorMessages)
		{
			AssertBuildMessages ("error", actualErrors, expectedErrorMessages);
		}

		public static void AssertErrorMessages (IList<BuildLogEvent> actualErrors, Func<string, bool> [] matchesExpectedErrorMessage, Func<string> [] rendersExpectedErrorMessage)
		{
			AssertBuildMessages ("error", actualErrors, matchesExpectedErrorMessage, rendersExpectedErrorMessage);
		}

		public static void AssertBuildMessages (string type, IList<BuildLogEvent> actualMessages, params string [] expectedMessages)
		{
			AssertBuildMessages (type, actualMessages,
				expectedMessages.Select (v => new Func<string, bool> ((msg) => msg == Canonicalize (v))).ToArray (),
				expectedMessages.Select (v => new Func<string> (() => v)).ToArray ()
			);
		}

		static string Canonicalize (string? msg)
		{
			if (msg is null)
				return string.Empty;
			return msg.Trim ('\n', '\r', ' ', '\t');
		}

		static string makeSingleLine (string? msg)
		{
			if (msg is null)
				return "";
			return msg.TrimEnd ().Replace ("\n", "\\n").Replace ("\r", "\\r");
		}

		public static void AssertBuildMessages (string type, IList<BuildLogEvent> actualMessages, Func<string, bool> [] matchesExpectedMessage, Func<string> [] rendersExpectedMessage)
		{
			var expectedCount = matchesExpectedMessage.Length;
			if (expectedCount != rendersExpectedMessage.Length)
				throw new InvalidOperationException ($"Mismatched function count");

			if (actualMessages.Count != expectedCount) {
				Assert.Fail ($"Expected {expectedCount} {type}(s), got {actualMessages.Count} {type}(s)\n" +
					$"\tExpected:\n" +
					$"\t\t{string.Join ("\n\t\t", rendersExpectedMessage.Select (v => makeSingleLine (v ())))}\n" +
					$"\tActual:\n" +
					$"\t\t{string.Join ("\n\t\t", actualMessages.Select (v => makeSingleLine (v.Message)))}");
				return;
			}

			var failures = new List<string> ();
			for (var i = 0; i < expectedCount; i++) {
				var actual = Canonicalize (actualMessages [i].Message);
				var isExpected = matchesExpectedMessage [i];
				if (!isExpected (actual)) {
					actual = makeSingleLine (actual);
					var expected = makeSingleLine (Canonicalize (rendersExpectedMessage [i] ()));
					failures.Add ($"\tUnexpected {type} message #{i}:\n\t\tExpected: {expected}\n\t\tActual:   {actual}");
				}
			}
			if (!failures.Any ())
				return;

			Assert.Fail ($"Failure when comparing {type} messages:\n{string.Join ("\n", failures)}\n\tAll {type}s:\n\t\t{string.Join ("\n\t\t", actualMessages.Select (v => v.Message?.TrimEnd ()))}");
		}

		public void AssertThatLinkerExecuted (ExecutionResult result)
		{
			var targets = BinLog.GetAllTargets (result.BinLogPath);
			if (AreAssembliesPreparedAndPostProcessed (targets)) {
				// The assembly preparer and post-processor did the work our custom trimmer steps
				// would otherwise have done, in which case the trimmer might not even run (that's
				// the case when we're not trimming anything).
				return;
			}

			var output = BinLog.PrintToString (result.BinLogPath);
			Assert.That (output, Does.Contain ("Building target \"_RunILLink\" completely."), "Linker did not executed as expected.");
			Assert.That (output, Does.Contain ("LinkerConfiguration:"), "Custom steps did not run as expected.");
		}

		// Our custom trimmer steps aren't executed when the assembly preparer prepares the assemblies
		// before the trimmer runs, and post-processes them afterwards, because then those two passes
		// do all the work instead.
		static bool AreAssembliesPreparedAndPostProcessed (IEnumerable<TargetExecutionResult> targets)
		{
			var prepared = targets.Any (v => v.TargetName == "_ExecutePrepareAssemblies" && !v.Skipped);
			var postProcessed = targets.Any (v => (v.TargetName == "_ExecutePostprocessAssemblies" || v.TargetName == "_ExecutePostprocessAssembliesAfterIlc") && !v.Skipped);
			return prepared && postProcessed;
		}

		public void AssertThatLinkerDidNotExecute (ExecutionResult result)
		{
			var output = BinLog.PrintToString (result.BinLogPath);
			Assert.That (output, Does.Not.Contain ("Building target \"_RunILLink\" completely."), "Linker did not executed as expected.");
			Assert.That (output, Does.Not.Contain ("LinkerConfiguration:"), "Custom steps did not run as expected.");
		}


		public void AssertTargetExecuted (IEnumerable<TargetExecutionResult> executedTargets, string targetName, string message)
		{
			var targets = executedTargets.Where (v => v.TargetName == targetName);
			if (!targets.Any ())
				Assert.Fail ($"The target '{targetName}' was not executed: no corresponding targets found in binlog ({message})");
			if (!targets.Any (v => !v.Skipped))
				Assert.Fail ($"The target '{targetName}' was not executed: the target was found {targets.Count ()} time(s) in the binlog, but they were all skipped ({message})");
		}

		public void AssertTargetNotExecuted (IEnumerable<TargetExecutionResult> executedTargets, string targetName, string message)
		{
			var targets = executedTargets.Where (v => v.TargetName == targetName);
			if (targets.Any (v => !v.Skipped))
				Assert.Fail ($"The target '{targetName}' was unexpectedly executed ({message})");
		}

		static bool? is_in_ci;
		public static bool IsInCI {
			get {
				if (!is_in_ci.HasValue) {
					var in_ci = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("BUILD_REVISION"));
					in_ci |= !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("BUILD_SOURCEVERSION")); // set by Azure DevOps
					is_in_ci = in_ci;
				}
				return is_in_ci.Value;
			}
		}

		static bool? is_pull_request;
		public static bool IsPullRequest {
			get {
				if (!is_pull_request.HasValue) {
					var pr = string.Equals (Environment.GetEnvironmentVariable ("BUILD_REASON"), "PullRequest", StringComparison.Ordinal);
					pr |= string.Equals (Environment.GetEnvironmentVariable ("IS_PR"), "true", StringComparison.OrdinalIgnoreCase);
					is_pull_request = pr;
				}
				return is_pull_request.Value;
			}
		}

		protected void AddNoWarnForPreviewVersions (ApplePlatform platform, IList<string> supportedApiVersion, Dictionary<string, string> properties)
		{
			// If any of the api versions we support are higher than the api version we're built for, we need to ignore any XCODE_*_PREVIEW warnings.
			var osVersion = Version.Parse (Configuration.GetNuGetOsVersion (platform));
			var nowarn = new List<string> ();
			foreach (var apiVersion in supportedApiVersion) {
				var v = apiVersion [(apiVersion.IndexOf ('-') + 1)..];
				var version = Version.Parse (v);
				if (version > osVersion)
					nowarn.Add ($"XCODE_{v.Replace ('.', '_')}_PREVIEW");
			}
			if (nowarn.Count > 0)
				properties ["NoWarn"] = string.Join (";", nowarn);
		}

		public static bool UsesCompressedBindingResourcePackage (ApplePlatform platform, string mode = "auto")
		{
			if (string.Equals (mode, "true", StringComparison.OrdinalIgnoreCase)) {
				return true;
			} else if (string.Equals (mode, "false", StringComparison.OrdinalIgnoreCase)) {
				return false;
			} else if (string.Equals (mode, "auto", StringComparison.OrdinalIgnoreCase)) {
				// we used to compress only if there were symlinks (would happen on macOS and Mac Catalyst),
				// but now we always compress to avoid MAX_PATH issues with iOS on Windows.
				return true;
			} else {
				throw new ArgumentOutOfRangeException (nameof (mode), "Must be 'true', 'false' or 'auto'");
			}

		}
	}
}
