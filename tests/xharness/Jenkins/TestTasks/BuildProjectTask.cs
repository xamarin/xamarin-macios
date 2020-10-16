using System;
using Microsoft.DotNet.XHarness.Common.Execution;
using Xharness.TestTasks;

namespace Xharness.Jenkins.TestTasks {
	abstract class BuildProjectTask : BuildToolTask
	{
		BuildProject BuildProject => buildToolTask as BuildProject; 

		public string SolutionPath {
			get => BuildProject.SolutionPath;
			set => BuildProject.SolutionPath = value;
		}

		protected BuildProjectTask (Jenkins jenkins, TestProject testProject, IProcessManager processManager) : base (jenkins, processManager)
			=> TestProject = testProject ?? throw new ArgumentNullException (nameof (testProject));

		public virtual bool RestoreNugets => BuildProject.RestoreNugets;

		public override bool SupportsParallelExecution => BuildProject.SupportsParallelExecution;

		protected override void InitializeTool () 
			=> buildToolTask = new BuildProject (() => Jenkins.Harness.XIBuildPath, ProcessManager, ResourceManager, this, this);
	}
}
