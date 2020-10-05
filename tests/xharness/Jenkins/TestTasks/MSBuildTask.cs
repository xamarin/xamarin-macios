using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	class MSBuildTask : BuildProjectTask
	{
		public ILog BuildLog;

		protected virtual string ToolName => Harness.XIBuildPath;

		protected virtual List<string> ToolArguments {
			get {
				var binlogPath = BuildLog.FullPath.Replace (".txt", ".binlog");

				var args = new List<string> ();
				args.Add ("--");
				args.Add ("/verbosity:diagnostic");
				args.Add ($"/bl:{binlogPath}");
				if (SpecifyPlatform)
					args.Add ($"/p:Platform={ProjectPlatform}");
				if (SpecifyConfiguration)
					args.Add ($"/p:Configuration={ProjectConfiguration}");
				args.Add (ProjectFile);
				return args;
			}
		}

		public MSBuildTask (IProcessManager processManager) : base (processManager)
		{
		}

		protected override async Task ExecuteAsync ()
		{
			using (var resource = await NotifyAndAcquireDesktopResourceAsync ()) {
				BuildLog = Logs.Create ($"build-{Platform}-{Timestamp}.txt", LogType.BuildLog.ToString ());

				await RestoreNugetsAsync (BuildLog, resource, useXIBuild: true);

				using (var xbuild = new Process ()) {
					xbuild.StartInfo.FileName = ToolName;
					xbuild.StartInfo.Arguments = StringUtils.FormatArguments (ToolArguments);
					SetEnvironmentVariables (xbuild);
					xbuild.StartInfo.EnvironmentVariables ["MSBuildExtensionsPath"] = null;
					LogEvent (BuildLog, "Building {0} ({1})", TestName, Mode);
					if (!Harness.DryRun) {
						var timeout = TimeSpan.FromMinutes (60);
						var result = await ProcessManager.RunAsync (xbuild, BuildLog, timeout);
						if (result.TimedOut) {
							ExecutionResult = TestExecutingResult.TimedOut;
							BuildLog.WriteLine ("Build timed out after {0} seconds.", timeout.TotalSeconds);
						} else if (result.Succeeded) {
							ExecutionResult = TestExecutingResult.Succeeded;
						} else {
							ExecutionResult = TestExecutingResult.Failed;
							if (Jenkins.IsMonoMulti3Issue (BuildLog)) { 
								KnownFailure = $"<a href='https://github.com/mono/mono/issues/18560'>Undefined symbol ___multi3 on Release Mode</a>";
							}
						}
					}
					Jenkins.MainLog.WriteLine ("Built {0} ({1})", TestName, Mode);
				}

				BuildLog.Dispose ();
			}
		}

		async Task CleanProjectAsync (ILog log, string project_file, string project_platform, string project_configuration)
		{
			// Don't require the desktop resource here, this shouldn't be that resource sensitive
			using (var xbuild = new Process ()) {
				xbuild.StartInfo.FileName = Harness.XIBuildPath;
				var args = new List<string> ();
				args.Add ("--");
				args.Add ("/verbosity:diagnostic");
				if (project_platform != null)
					args.Add ($"/p:Platform={project_platform}");
				if (project_configuration != null)
					args.Add ($"/p:Configuration={project_configuration}");
				args.Add (project_file);
				args.Add ("/t:Clean");
				xbuild.StartInfo.Arguments = StringUtils.FormatArguments (args);
				SetEnvironmentVariables (xbuild);
				LogEvent (log, "Cleaning {0} ({1}) - {2}", TestName, Mode, project_file);
				var timeout = TimeSpan.FromMinutes (1);
				await ProcessManager.RunAsync (xbuild, log, timeout);
				log.WriteLine ("Clean timed out after {0} seconds.", timeout.TotalSeconds);
				Jenkins.MainLog.WriteLine ("Cleaned {0} ({1})", TestName, Mode);
			}
		}

		public async override Task CleanAsync ()
		{
			var log = Logs.Create ($"clean-{Platform}-{Timestamp}.txt", "Clean log");
			await CleanProjectAsync (log, ProjectFile, SpecifyPlatform ? ProjectPlatform : null, SpecifyConfiguration ? ProjectConfiguration : null);

			// Iterate over all the project references as well.
			var doc = new XmlDocument ();
			doc.LoadWithoutNetworkAccess (ProjectFile);
			foreach (var pr in doc.GetProjectReferences ()) {
				var path = pr.Replace ('\\', '/');
				await CleanProjectAsync (log, path, SpecifyPlatform ? ProjectPlatform : null, SpecifyConfiguration ? ProjectConfiguration : null);
			}
		}
	}
}
