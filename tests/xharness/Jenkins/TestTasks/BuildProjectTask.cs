using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	abstract class BuildProjectTask : BuildToolTask
	{
		public string SolutionPath;

		protected BuildProjectTask (IProcessManager processManager) : base (processManager)
		{
		}

		public bool RestoreNugets {
			get {
				return TestProject.RestoreNugetsInProject || !string.IsNullOrEmpty (SolutionPath);
			}
		}

		public override bool SupportsParallelExecution {
			get {
				return Platform.ToString ().StartsWith ("Mac", StringComparison.Ordinal);
			}
		}

		async Task<TestExecutingResult> RestoreNugetsAsync (string projectPath, ILog log, bool useXIBuild = false)
		{
			using (var resource = await Jenkins.NugetResource.AcquireExclusiveAsync ()) {
				// we do not want to use xibuild on solutions, we will have some failures with Mac Full
				var isSolution = projectPath.EndsWith (".sln", StringComparison.Ordinal);
				if (!File.Exists (projectPath))
					throw new FileNotFoundException ("Could not find the solution whose nugets to restore.", projectPath);

				using (var nuget = new Process ()) {
					nuget.StartInfo.FileName = useXIBuild && !isSolution ? Harness.XIBuildPath :
						"/Library/Frameworks/Mono.framework/Versions/Current/Commands/nuget";
					var args = new List<string> ();
					args.Add ((useXIBuild && !isSolution ? "/" : "") + "restore"); // diff param depending on the tool
					args.Add (projectPath);
					if (useXIBuild && !isSolution)
						args.Add ("/verbosity:detailed");
					else {
						args.Add ("-verbosity");
						args.Add ("detailed");
					}
					nuget.StartInfo.Arguments = StringUtils.FormatArguments (args);
					SetEnvironmentVariables (nuget);
					LogEvent (log, "Restoring nugets for {0} ({1}) on path {2}", TestName, Mode, projectPath);

					var timeout = TimeSpan.FromMinutes (15);
					var result = await ProcessManager.RunAsync (nuget, log, timeout);
					if (result.TimedOut) {
						log.WriteLine ("Nuget restore timed out after {0} seconds.", timeout.TotalSeconds);
						return TestExecutingResult.TimedOut;
					} else if (!result.Succeeded) {
						return TestExecutingResult.Failed;
					}
				}

				LogEvent (log, "Restoring nugets completed for {0} ({1}) on path {2}", TestName, Mode, projectPath);
				return TestExecutingResult.Succeeded;
			}
		}

		List<string> GetNestedReferenceProjects (string csproj)
		{
			if (!File.Exists (csproj))
				throw new FileNotFoundException ("Could not find the project whose reference projects needed to be found.", csproj);
			var result = new List<string> ();
			var doc = new XmlDocument ();
			doc.Load (csproj.Replace ("\\", "/"));
			foreach (var referenceProject in doc.GetProjectReferences ()) {
				var fixPath = referenceProject.Replace ("\\", "/"); // do the replace in case we use win paths
				result.Add (fixPath);
				// get all possible references
				result.AddRange (GetNestedReferenceProjects (fixPath));
			}
			return result;
		}

		// This method must be called with the desktop resource acquired
		// (which is why it takes an IAcquiredResources as a parameter without using it in the function itself).
		protected async Task RestoreNugetsAsync (ILog log, IAcquiredResource resource, bool useXIBuild = false)
		{
			if (!RestoreNugets)
				return;

			if (!File.Exists (SolutionPath ?? TestProject.Path))
				throw new FileNotFoundException ("Could not find the solution whose nugets to restore.", SolutionPath ?? TestProject.Path);

			// might happen that the project does contain reference projects with nugets, grab the reference projects and ensure
			// thast they have the nugets restored (usually, watch os test projects
			if (SolutionPath == null) {
				var references = GetNestedReferenceProjects (TestProject.Path);
				foreach (var referenceProject in references) {
					var execResult = await RestoreNugetsAsync (referenceProject, log, useXIBuild); // do the replace in case we use win paths
					if (execResult == TestExecutingResult.TimedOut) {
						ExecutionResult = execResult;
						return;
					}
				}
			}

			// restore for the main project/solution]
			ExecutionResult = await RestoreNugetsAsync (SolutionPath ?? TestProject.Path, log, useXIBuild);
		}
	}
}
