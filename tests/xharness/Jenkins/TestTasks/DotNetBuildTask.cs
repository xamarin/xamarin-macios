using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.Jenkins.TestTasks {
	class DotNetBuildTask : MSBuildTask {

		public DotNetBuildTask (IProcessManager processManager) : base (processManager)
		{
			SetDotNetEnvironmentVariables (Environment);
		}

		protected override string ToolName {
			get { return Harness.DOTNET; }
		}

		protected override List<string> ToolArguments {
			get {
				var args = base.ToolArguments;
				// 'dotnet build' takes almost the same arguments as 'msbuild', so just massage a little bit.
				args.Remove ("--");
				args.Insert (0, "build");
				return args;
			}
		}


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
