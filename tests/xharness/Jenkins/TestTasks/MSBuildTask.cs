using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins.TestTasks {
	class MSBuildTask : BuildProjectTask {
		protected virtual string ToolName {
			get {
				if (TestProject.IsDotNetProject)
					return Jenkins.Harness.GetDotNetExecutable (Path.GetDirectoryName (ProjectFile));
				return Jenkins.Harness.XIBuildPath;
			}
		}

		public override void SetEnvironmentVariables (Process process)
		{
			base.SetEnvironmentVariables (process);
			// modify those env vars that we do care about

			if (TestProject.IsDotNetProject) {
				process.StartInfo.EnvironmentVariables.Remove ("MSBUILD_EXE_PATH");
				process.StartInfo.EnvironmentVariables.Remove ("MSBuildExtensionsPathFallbackPathsOverride");
				process.StartInfo.EnvironmentVariables.Remove ("MSBuildSDKsPath");
				process.StartInfo.EnvironmentVariables.Remove ("TargetFrameworkFallbackSearchPaths");
				process.StartInfo.EnvironmentVariables.Remove ("MSBuildExtensionsPathFallbackPathsOverride");
			}
		}

		protected virtual List<string> ToolArguments =>
				MSBuild.GetToolArguments (ProjectPlatform, ProjectConfiguration, ProjectFile, BuildLog);

		MSBuild MSBuild => buildToolTask as MSBuild;

		public MSBuildTask (Jenkins jenkins, TestProject testProject, IProcessManager processManager)
			: base (jenkins, testProject, processManager) { }

		protected override void InitializeTool ()
		{
			if (TestProject.IsDotNetProject) {
				buildToolTask = new DotNetBuild (
					msbuildPath: () => ToolName,
					processManager: ProcessManager,
					resourceManager: ResourceManager,
					eventLogger: this,
					envManager: this,
					errorKnowledgeBase: Jenkins.ErrorKnowledgeBase);
			} else {
				buildToolTask = new MSBuild (
					msbuildPath: () => ToolName,
					processManager: ProcessManager,
					resourceManager: ResourceManager,
					eventLogger: this,
					envManager: this,
					errorKnowledgeBase: Jenkins.ErrorKnowledgeBase);
			}
		}

		protected override async Task ExecuteAsync ()
		{
			using var resource = await NotifyAndAcquireDesktopResourceAsync ();
			BuildLog = Logs.Create ($"build-{Platform}-{Timestamp}.txt", LogType.BuildLog.ToString ());
			(ExecutionResult, KnownFailure) = await MSBuild.ExecuteAsync (
				projectPlatform: ProjectPlatform,
				projectConfiguration: ProjectConfiguration,
				projectFile: ProjectFile,
				resource: resource,
				dryRun: Jenkins.Harness.DryRun,
				buildLog: BuildLog,
				mainLog: Jenkins.MainLog);

			BuildLog.Dispose ();
		}

		public override Task CleanAsync () =>
			MSBuild.CleanAsync (
				projectPlatform: ProjectPlatform,
				projectConfiguration: ProjectConfiguration,
				projectFile: ProjectFile,
				cleanLog: Logs.Create ($"clean-{Platform}-{Timestamp}.txt", "Clean log"),
				mainLog: Jenkins.MainLog);

		public static void SetDotNetEnvironmentVariables (Dictionary<string, string> environment)
		{
			environment ["MSBUILD_EXE_PATH"] = null;
			environment ["MSBuildExtensionsPathFallbackPathsOverride"] = null;
			environment ["MSBuildSDKsPath"] = null;
			environment ["TargetFrameworkFallbackSearchPaths"] = null;
			environment ["MSBuildExtensionsPathFallbackPathsOverride"] = null;
		}
	}
}
