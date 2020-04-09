using System;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Xharness.TestTasks;

namespace Xharness.Jenkins.TestTasks {
	abstract class BuildProjectTask : BuildToolTask
	{

		public string SolutionPath {
			get => buildProjectTask.SolutionPath;
			set => buildProjectTask.SolutionPath = value;
		}

		Xharness.TestTasks.BuildProjectTask buildProjectTask;

		public override TestProject TestProject { 
			get => base.TestProject;
			set {
				base.TestProject = value;
				buildProjectTask.TestProject = value;
			}
		}


		protected BuildProjectTask (Jenkins jenkins, TestProject testProject, IProcessManager processManager) : base (jenkins, processManager)
		{
			buildProjectTask = new Xharness.TestTasks.BuildProjectTask (processManager, Jenkins, this, this);
			TestProject = testProject ?? throw new ArgumentNullException (nameof (testProject));
		}

		public virtual bool RestoreNugets => buildProjectTask.RestoreNugets;

		public override bool SupportsParallelExecution => buildProjectTask.SupportsParallelExecution;

		// This method must be called with the desktop resource acquired
		// (which is why it takes an IAcquiredResources as a parameter without using it in the function itself).
		protected async Task RestoreNugetsAsync (ILog log, IAcquiredResource resource, bool useXIBuild = false) => 	
			ExecutionResult = await buildProjectTask.RestoreNugetsAsync (log, resource, useXIBuild);
	}
}
