using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.Common.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
#nullable enable

namespace Xharness.Jenkins.TestTasks {
	public class BuildProject : BuildTool {
		public IResourceManager ResourceManager { get; set; }
		public IEnvManager EnvironmentManager { get; set; }
		public IEventLogger EventLogger { get; set; }
		readonly Func<string> msbuildPath;
		bool? restoreNuGets;

		public string? SolutionPath { get; set; }

		public BuildProject (Func<string> msbuildPath, IProcessManager processManager, IResourceManager resourceManager, IEventLogger eventLogger, IEnvManager envManager) : base (processManager)
		{
			this.msbuildPath = msbuildPath ?? throw new ArgumentNullException (nameof (msbuildPath));
			ResourceManager = resourceManager ?? throw new ArgumentNullException (nameof (resourceManager));
			EventLogger = eventLogger ?? throw new ArgumentNullException (nameof (eventLogger));
			EnvironmentManager = envManager ?? throw new ArgumentNullException (nameof (envManager));
		}

		public bool RestoreNugets {
			get {
				if (restoreNuGets.HasValue)
					return restoreNuGets.Value;
				if (TestProject.IsDotNetProject)
					return false;
				if (Platform == TestPlatform.MacCatalyst)
					return false;
				return TestProject.RestoreNugetsInProject || !string.IsNullOrEmpty (SolutionPath);
			}
			set {
				restoreNuGets = value;
			}
		}

		public bool SupportsParallelExecution {
			get {
				return Platform.ToString ().StartsWith ("Mac", StringComparison.Ordinal);
			}
		}

		async Task<TestExecutingResult> RestoreNugetsAsync (string projectPath, ILog log)
		{
			using var _ = await ResourceManager.NugetResource.AcquireExclusiveAsync ();
			if (!File.Exists (projectPath))
				throw new FileNotFoundException ("Could not find the solution whose nugets to restore.", projectPath);

			using (var nuget = new Process ()) {
				nuget.StartInfo.FileName = msbuildPath ();
				var args = new List<string> ();
				if (TestProject.IsDotNetProject) {
					args.Add ("restore");
					args.Add ("/verbosity:detailed");
				} else {
					args.Add ("-t");
					args.Add ("--");
					args.Add ("/Library/Frameworks/Mono.framework/Versions/Current/Commands/nuget");
					args.Add ("restore");
					args.Add ("-Verbosity");
					args.Add ("detailed");
				}
				args.Add (projectPath);
				nuget.StartInfo.Arguments = StringUtils.FormatArguments (args);
				EnvironmentManager.SetEnvironmentVariables (nuget);
				EventLogger.LogEvent (log, "Restoring nugets for {0} ({1}) on path {2}", TestName, Mode, projectPath);

				var timeout = TimeSpan.FromMinutes (15);
				var result = await ProcessManager.RunAsync (nuget, log, timeout);
				if (result.TimedOut) {
					log.WriteLine ("Nuget restore timed out after {0} seconds.", timeout.TotalSeconds);
					return TestExecutingResult.TimedOut;
				}

				if (!result.Succeeded) {
					return TestExecutingResult.Failed;
				}
			}

			EventLogger.LogEvent (log, "Restoring nugets completed for {0} ({1}) on path {2}", TestName, Mode, projectPath);
			return TestExecutingResult.Succeeded;
		}

		static IEnumerable<string> GetNestedReferenceProjects (string csproj)
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
				if (!Path.IsPathRooted (fixPath))
					fixPath = Path.Combine (Path.GetDirectoryName (csproj)!, fixPath);
				result.AddRange (GetNestedReferenceProjects (fixPath));
			}
			return result;
		}

		// This method must be called with the desktop resource acquired
		// (which is why it takes an IAcquiredResources as a parameter without using it in the function itself).
		public async Task<TestExecutingResult> RestoreNugetsAsync (ILog log, IAcquiredResource resource)
		{
			if (!RestoreNugets)
				return TestExecutingResult.Ignored;

			if (!File.Exists (SolutionPath ?? TestProject.Path))
				throw new FileNotFoundException ("Could not find the solution whose nugets to restore.", SolutionPath ?? TestProject.Path);

			// might happen that the project does contain reference projects with nugets, grab the reference projects and ensure
			// that they have the nugets restored (usually, watch os test projects
			if (SolutionPath is null) {
				var references = GetNestedReferenceProjects (TestProject.Path);
				foreach (var referenceProject in references) {
					var execResult = await RestoreNugetsAsync (referenceProject, log); // do the replace in case we use win paths
					if ((execResult & TestExecutingResult.TimedOut) == TestExecutingResult.TimedOut) {
						return execResult;
					}
				}
			}

			// restore for the main project/solution]
			return await RestoreNugetsAsync (SolutionPath ?? TestProject.Path, log);
		}
	}
}
