using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;

namespace Xharness.Jenkins.TestTasks {
	abstract class BuildToolTask : AppleTestTask, IBuildToolTask {
		protected BuildTool buildToolTask;

		public IProcessManager ProcessManager { get; }

		public IFileBackedLog BuildLog {
			get => buildToolTask.BuildLog;
			set => buildToolTask.BuildLog = value;
		}

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

		public List<string> Constants {
			get => buildToolTask.Constants;
		}

		public override TestProject TestProject {
			get => base.TestProject;
			set {
				base.TestProject = value;
				buildToolTask.TestProject = value;
			}
		}

		protected BuildToolTask (Jenkins jenkins, TestProject testProject, IProcessManager processManager) : base (jenkins)
		{
			base.TestProject = testProject;
			ProcessManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			InitializeTool ();
			buildToolTask.TestProject = testProject;
		}

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

		protected virtual void InitializeTool () => buildToolTask = new BuildTool (ProcessManager);
		public virtual Task CleanAsync () => buildToolTask.CleanAsync ();
	}
}
