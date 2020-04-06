using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.Jenkins.TestTasks
{
	public abstract class BuildToolTask : AppleTestTask
	{
		readonly Xharness.TestTasks.BuildToolTask buildToolTask;

		public IProcessManager ProcessManager => buildToolTask.ProcessManager;

		public override string TestName {
			get => base.TestName;
			set {
				base.TestName = value;
				buildToolTask.TestName = value; 
			}
		}

		public bool SpecifyPlatform { 
			get => buildToolTask.SpecifyPlatform;
			set => buildToolTask.SpecifyPlatform = value;
		}

		public bool SpecifyConfiguration {
			get => buildToolTask.SpecifyConfiguration;
			set => buildToolTask.SpecifyConfiguration = value;
		}

		public override TestProject TestProject {
			get => base.TestProject;
			set {
				base.TestProject = value;
				buildToolTask.TestProject = value;
			}
		}

		protected BuildToolTask (Jenkins jenkins, IProcessManager processManager) : base (jenkins)
			=> buildToolTask = new Xharness.TestTasks.BuildToolTask (processManager);

		public override TestPlatform Platform { 
			get => base.Platform;
			set {
				base.Platform = value;
				buildToolTask.Platform = value;
			}
		}

		public override string Mode {
			get => buildToolTask.Mode;
			set => buildToolTask.Mode = value;
		}

		public virtual Task CleanAsync () => buildToolTask.CleanAsync ();
	}
}
