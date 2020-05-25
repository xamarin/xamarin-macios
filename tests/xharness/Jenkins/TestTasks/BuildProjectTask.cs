using System;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Tasks;
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
			=> buildToolTask = new BuildProject (Jenkins.Harness.XIBuildPath, ProcessManager, ResourceManager, this, this);

		// This method must be called with the desktop resource acquired
		// (which is why it takes an IAcquiredResources as a parameter without using it in the function itself).
		protected async Task RestoreNugetsAsync (ILog log, IAcquiredResource resource, bool useXIBuild = false) => 	
			ExecutionResult = await BuildProject.RestoreNugetsAsync (log, resource, useXIBuild);
	}
}
