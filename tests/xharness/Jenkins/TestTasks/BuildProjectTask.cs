using System;
using Microsoft.DotNet.XHarness.Common.Execution;

namespace Xharness.Jenkins.TestTasks {
	abstract class BuildProjectTask : BuildToolTask {
		BuildProject BuildProject => buildToolTask as BuildProject;

		public string SolutionPath {
			get => BuildProject.SolutionPath;
			set => BuildProject.SolutionPath = value;
		}

		protected BuildProjectTask (Jenkins jenkins, TestProject testProject, IProcessManager processManager)
			: base (jenkins, testProject, processManager)
		{
		}

		public virtual bool RestoreNugets {
			get => BuildProject.RestoreNugets;
			set => BuildProject.RestoreNugets = value;
		}

		public override bool SupportsParallelExecution => BuildProject.SupportsParallelExecution;

		protected override void InitializeTool ()
			=> buildToolTask = new BuildProject (() => Jenkins.Harness.XIBuildPath, ProcessManager, ResourceManager, this, this);
	}
}
