using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.Common.Execution;

using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	class MakeTestTaskEnumerable : IEnumerable<MakeTask> {

		readonly Jenkins jenkins;
		readonly IProcessManager processManager;

		public MakeTestTaskEnumerable (Jenkins jenkins, IProcessManager processManager)
		{
			this.jenkins = jenkins ?? throw new ArgumentNullException (nameof (jenkins));
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public IEnumerator<MakeTask> GetEnumerator ()
		{
			var run_mmp = new MakeTask (jenkins: jenkins, processManager: processManager) {
				Platform = TestPlatform.Mac,
				TestName = "MMP Regression Tests",
				Target = "all", // -j" + Environment.ProcessorCount,
				WorkingDirectory = Path.Combine (HarnessConfiguration.RootDirectory, "mmp-regression"),
				Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Mmp) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac),
				Timeout = TimeSpan.FromMinutes (30),
				SupportsParallelExecution = false, // Already doing parallel execution by running "make -jX"
			};
			run_mmp.CompletedTask = new Task (() => {
				foreach (var log in Directory.GetFiles (Path.GetFullPath (run_mmp.WorkingDirectory), "*.log", SearchOption.AllDirectories))
					run_mmp.Logs.AddFile (log, log.Substring (run_mmp.WorkingDirectory.Length + 1));
			});
			run_mmp.Environment.Add ("BUILD_REVISION", "jenkins"); // This will print "@MonkeyWrench: AddFile: <log path>" lines, which we can use to get the log filenames.
			yield return run_mmp;

			var runMacBindingProject = new MakeTask (jenkins: jenkins, processManager: processManager) {
				Platform = TestPlatform.Mac,
				TestName = "Mac Binding Projects",
				Target = "all",
				WorkingDirectory = Path.Combine (HarnessConfiguration.RootDirectory, "mac-binding-project"),
				Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.MacBindingProject) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) || !jenkins.TestSelection.IsEnabled (PlatformLabel.LegacyXamarin),
				Timeout = TimeSpan.FromMinutes (15),
			};
			yield return runMacBindingProject;
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}
}
