using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.Jenkins.TestTasks {
	class DotNetBuildTask : MSBuildTask {

		public DotNetBuildTask (Jenkins jenkins, TestProject testProject, IProcessManager processManager) 
			: base (jenkins, testProject, processManager) { }

		protected override string ToolName => Harness.DOTNET;

		public override void SetEnvironmentVariables (Process process)
		{
			base.SetEnvironmentVariables (process);
			// modify those env vars that we do care about

			process.StartInfo.EnvironmentVariables ["MSBUILD_EXE_PATH"] = null;
			process.StartInfo.EnvironmentVariables ["MSBuildExtensionsPathFallbackPathsOverride"] = null;
			process.StartInfo.EnvironmentVariables ["MSBuildSDKsPath"] = null;
			process.StartInfo.EnvironmentVariables ["TargetFrameworkFallbackSearchPaths"] = null;
			process.StartInfo.EnvironmentVariables ["MSBuildExtensionsPathFallbackPathsOverride"] = null;
		}

		protected override void InitializeTool () =>
			buildToolTask = new Xharness.TestTasks.DotNetBuildTask (
				msbuildPath: ToolName,
				processManager: ProcessManager,
				resourceManager: Jenkins,
				eventLogger: this,
				envManager: this,
				errorKnowledgeBase: Jenkins);

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
