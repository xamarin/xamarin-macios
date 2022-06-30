#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Mono.Cecil;

using NUnit.Framework;

using Xamarin.Utils;
using Xamarin.Tests;
using Xamarin.MacDev;

namespace Xamarin.Tests {
	[TestFixture]
	public abstract class TestBaseClass {
		protected Dictionary<string, string> verbosity = new Dictionary<string, string> {
			{ "_BundlerVerbosity", "1" },
		};

		protected Dictionary<string, string> GetDefaultProperties (string? runtimeIdentifiers = null)
		{
			var rv = new Dictionary<string, string> (verbosity);
			if (!string.IsNullOrEmpty (runtimeIdentifiers))
				SetRuntimeIdentifiers (rv, runtimeIdentifiers);
			return rv;
		}

		protected void SetRuntimeIdentifiers (Dictionary<string, string> properties, string runtimeIdentifiers)
		{
			var multiRid = runtimeIdentifiers.IndexOf (';') >= 0 ? "RuntimeIdentifiers" : "RuntimeIdentifier";
			properties [multiRid] = runtimeIdentifiers;
		}

		protected string GetProjectPath (string project, string runtimeIdentifiers, ApplePlatform platform, out string appPath, string? subdir = null, string configuration = "Debug", string? netVersion = null)
		{
			return GetProjectPath (project, null, runtimeIdentifiers, platform, out appPath, configuration, netVersion);
		}

		protected string GetProjectPath (string project, string? subdir, string runtimeIdentifiers, ApplePlatform platform, out string appPath, string configuration = "Debug", string? netVersion = null)
		{
			var rv = GetProjectPath (project, subdir, platform);
			if (string.IsNullOrEmpty (runtimeIdentifiers))
				runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);
			var appPathRuntimeIdentifier = runtimeIdentifiers.IndexOf (';') >= 0 ? "" : runtimeIdentifiers;
			appPath = Path.Combine (Path.GetDirectoryName (rv)!, "bin", configuration, platform.ToFramework (netVersion), appPathRuntimeIdentifier, project + ".app");
			return rv;
		}

		protected string GetAppPath (string projectPath, ApplePlatform platform, string runtimeIdentifiers, string configuration = "Debug")
		{
			var appPathRuntimeIdentifier = runtimeIdentifiers.IndexOf (';') >= 0 ? "" : runtimeIdentifiers;
			return Path.Combine (Path.GetDirectoryName (projectPath)!, "bin", configuration, platform.ToFramework (), appPathRuntimeIdentifier, Path.GetFileNameWithoutExtension (projectPath) + ".app");
		}

		protected string GetDefaultRuntimeIdentifier (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				return "iossimulator-x64";
			case ApplePlatform.TVOS:
				return "tvossimulator-x64";
			case ApplePlatform.MacOSX:
				return "osx-x64";
			case ApplePlatform.MacCatalyst:
				return "maccatalyst-x64";
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		protected string GetProjectPath (string project, string? subdir = null, ApplePlatform? platform = null)
		{
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

		protected void Clean (string project_path)
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

		protected static bool CanExecute (ApplePlatform platform, string runtimeIdentifiers)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
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
			case ApplePlatform.WatchOS:
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
			case ApplePlatform.WatchOS:
				return string.Empty;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return Path.Combine ("Contents", "MonoBundle");
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}

		protected string GetInfoPListPath (ApplePlatform platform, string app_directory)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
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

				bool onlyHasEmptyMethods = definition.Assembly.MainModule.Types.All (t => 
					t.Methods.Where (m => m.HasBody).All (m => m.Body.Instructions.Count == 1));
				if (onlyHasEmptyMethods) {
					assembliesWithOnlyEmptyMethods.Add (assembly);
				}
			}

			// Some assemblies, such as Facades, will be completely empty even when not stripped
			Assert.That (assemblies.Length == assembliesWithOnlyEmptyMethods.Count, Is.EqualTo (shouldStrip), $"Unexpected stripping status: of {assemblies.Length} assemblies {assembliesWithOnlyEmptyMethods.Count} were empty.");
		}

		protected void AssertDSymDirectory (string appPath)
		{
			var dSYMDirectory = appPath + ".dSYM";
			Assert.That (dSYMDirectory, Does.Exist, "dsym directory");
		}

		protected string GetNativeExecutable (ApplePlatform platform, string app_directory)
		{
			var executableName = Path.GetFileNameWithoutExtension (app_directory);
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				return Path.Combine (app_directory, executableName);
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return Path.Combine (app_directory, "Contents", "MacOS", executableName);
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}

		protected string GetResourcesDirectory (ApplePlatform platform, string app_directory)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
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

		protected void ExecuteWithMagicWordAndAssert (ApplePlatform platform, string runtimeIdentifiers, string executable)
		{
			if (!CanExecute (platform, runtimeIdentifiers))
				return;

			ExecuteWithMagicWordAndAssert (executable);
		}

		protected void ExecuteWithMagicWordAndAssert (string executable)
		{
			if (!File.Exists (executable))
				throw new FileNotFoundException ($"The executable '{executable}' does not exists.");

			var magicWord = Guid.NewGuid ().ToString ();
			var env = new Dictionary<string, string?> {
				{ "MAGIC_WORD", magicWord },
				{ "DYLD_FALLBACK_LIBRARY_PATH", null }, // VSMac might set this, which may cause tests to crash.
			};

			var output = new StringBuilder ();
			var rv = Execution.RunWithStringBuildersAsync (executable, Array.Empty<string> (), environment: env, standardOutput: output, standardError: output, timeout: TimeSpan.FromSeconds (15)).Result;
			Assert.That (output.ToString (), Does.Contain (magicWord), "Contains magic word");
			Assert.AreEqual (0, rv.ExitCode, "ExitCode");
		}

		public static StringBuilder AssertExecute (string executable, params string[] arguments)
		{
			return AssertExecute (executable, arguments, out _);
		}

		public static StringBuilder AssertExecute (string executable, string[] arguments, out StringBuilder output)
		{
			var rv = ExecutionHelper.Execute (executable, arguments, out output);
			if (rv != 0) {
				Console.WriteLine ($"'{executable} {StringUtils.FormatArguments (arguments)}' exited with exit code {rv}:");
				Console.WriteLine ("\t" + output.ToString ().Replace ("\n", "\n\t").TrimEnd (new char [] { '\n', '\t' }));
			}
			Assert.AreEqual (0, rv, $"Unable to execute '{executable} {StringUtils.FormatArguments (arguments)}': exit code {rv}");
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
	}
}
