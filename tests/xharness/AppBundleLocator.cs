using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.Common.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness {
	public class AppBundleLocator : IAppBundleLocator {
		readonly IProcessManager processManager;
		readonly Func<ILog> getLog;
		readonly string msBuildPath;
		readonly string systemDotnetPath;
		readonly string dotnetPath;

		// Gets either the system .NET or DOTNET variable, depending on any global.json
		// config file found in the specified directory or any containing directories.
		readonly Dictionary<string, string> dotnet_executables = new Dictionary<string, string> ();

		public AppBundleLocator (IProcessManager processManager, Func<ILog> getLog, string msBuildPath, string systemDotnetPath, string dotnetPath)
		{
			this.processManager = processManager;
			this.getLog = getLog;
			this.msBuildPath = msBuildPath;
			this.systemDotnetPath = systemDotnetPath;
			this.dotnetPath = dotnetPath;
		}

		public async Task<string> LocateAppBundle (XmlDocument projectFile, string projectFilePath, TestTarget target, string buildConfiguration)
		{
			string platform = string.Empty;
			if (target != TestTarget.None)
				platform = target.IsSimulator () ? "iPhoneSimulator" : "iPhone";

			if (projectFile.IsDotNetProject ()) {
				var properties = new Dictionary<string, string> {
					{ "Configuration", buildConfiguration },
				};

				if (!string.IsNullOrEmpty (platform))
					properties ["Platform"] = platform;

				return await GetPropertyByMSBuildEvaluationAsync (projectFile, projectFilePath, "OutputPath", "_GenerateBundleName", properties);
			} else {
				return projectFile.GetOutputPath (platform, buildConfiguration).Replace ('\\', Path.DirectorySeparatorChar);
			}
		}

		// Retrieves a property from an MSBuild project file by executing MSBuild and getting MSBuild to print the property.
		// We do this by creating a custom MSBuild file which:
		// * Will import the project file we're inspecting
		// * Has a target that will print a given property
		// and then executing MSBuild on this custom MSBuild file.
		private async Task<string> GetPropertyByMSBuildEvaluationAsync (XmlDocument csproj, string projectPath, string evaluateProperty, string dependsOnTargets = "", Dictionary<string, string> properties = null)
		{
			var xml =
@"<Project DefaultTargets='WriteProperty' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
	<!-- Import the project we want to inspect -->
	<Import Project='$(ProjectFile)' Condition=""'$(ProjectFile)' != ''"" />
	<!-- Target to write out the property we want -->
	<Target Name='WriteProperty' DependsOnTargets='%DEPENDSONTARGETS%'>
		<PropertyGroup>
			<_Properties>$(%PROPERTY%)</_Properties>
		</PropertyGroup>
		<Error Text='The ProjectFile variable must be set.' Condition=""'$(ProjectFile)' == ''"" />
		<Error Text='The OutputFile variable must be set.' Condition=""'$(OutputFile)' == ''"" />
		<WriteLinesToFile File='$(OutputFile)' Lines='$(_Properties)' Overwrite='true' />
	</Target>
</Project>
";

			var dir = Path.GetDirectoryName (projectPath);
			var inspector = Path.Combine (dir, "PropertyInspector.csproj");
			var output = Path.Combine (dir, "PropertyInspector.txt");
			try {
				File.WriteAllText (inspector, xml.Replace ("%PROPERTY%", evaluateProperty).Replace ("%DEPENDSONTARGETS%", dependsOnTargets));
				using (var proc = new Process ()) {
					var isDotNetProject = csproj.IsDotNetProject ();

					proc.StartInfo.FileName = isDotNetProject ? GetDotNetExecutable (projectPath) : msBuildPath;
					var args = new List<string> ();

					if (isDotNetProject)
						args.Add ("build");

					args.Add ("/p:ProjectFile=" + projectPath);
					args.Add ("/p:OutputFile=" + output);

					foreach (var prop in properties)
						args.Add ($"/p:{prop.Key}={prop.Value}");

					args.Add (inspector);

					var env = new Dictionary<string, string> {
						{ "MSBUILD_EXE_PATH", null },
					};

					proc.StartInfo.Arguments = StringUtils.FormatArguments (args);
					proc.StartInfo.WorkingDirectory = dir;

					// Don't evaluate in parallel on multiple threads to avoid overloading the mac.
					var acquired = await evaluate_semaphore.WaitAsync (TimeSpan.FromMinutes (5));
					try {
						var log = getLog () ?? new ConsoleLog ();
						var memoryLog = new MemoryLog ();
						var aggregated = Log.CreateAggregatedLog (memoryLog, log);
						if (!acquired)
							aggregated.WriteLine ("Unable to acquire lock to evaluate MSBuild property in 5 minutes; will try to evaluate anyway.");
						var rv = await processManager.RunAsync (proc, aggregated, environmentVariables: env, timeout: TimeSpan.FromMinutes (5));
						if (!rv.Succeeded) {
							var msg = $"Unable to evaluate the property {evaluateProperty} in {projectPath}, build failed with exit code {rv.ExitCode}. Timed out: {rv.TimedOut}";
							Console.WriteLine (msg + " Output: \n" + memoryLog.ToString ());
							throw new Exception (msg);
						}
					} finally {
						if (acquired)
							evaluate_semaphore.Release ();
					}

					return File.ReadAllText (output).Trim ();
				}
			} finally {
				File.Delete (inspector);
				File.Delete (output);
			}
		}

		SemaphoreSlim evaluate_semaphore = new SemaphoreSlim (1);

		public string GetDotNetExecutable (string directory)
		{
			if (directory is null)
				throw new ArgumentNullException (nameof (directory));

			lock (dotnet_executables) {
				if (dotnet_executables.TryGetValue (directory, out var value))
					return value;
			}

			// Find the first global.json up the directory hierarchy (stopping at the root directory)
			string global_json = null;
			var dir = directory;
			while (dir.Length > 2) {
				global_json = Path.Combine (dir, "global.json");
				if (File.Exists (global_json))
					break;
				dir = Path.GetDirectoryName (dir);
			}
			if (!File.Exists (global_json))
				throw new Exception ($"Could not find any global.json file in {directory} or above");

			// Parse the global.json we found, and figure out if it tells us to use .NET 3.1.100 / 5.X.XXX or not.
			var contents = File.ReadAllBytes (global_json);

			using var reader = JsonReaderWriterFactory.CreateJsonReader (contents, new XmlDictionaryReaderQuotas ());
			var doc = new XmlDocument ();
			doc.Load (reader);

			var version = doc.SelectSingleNode ("/root/sdk").InnerText;
			string executable;

			switch (version [0]) {
			case '3':
			case '5':
				executable = systemDotnetPath;
				break;
			default:
				executable = dotnetPath;
				break;
			}

			getLog ()?.WriteLine ($"Mapped .NET SDK version {version} to {executable} for {directory}");

			lock (dotnet_executables) {
				dotnet_executables [directory] = executable;
			}

			return executable;
		}
	}
}
