using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;

namespace Xharness.Jenkins.TestTasks {

	public class BuildTool {
		public string TestName { get; set; }
		public IProcessManager ProcessManager { get; }
		public TestPlatform Platform { get; set; }
		public TestProject TestProject { get; set; }
		public IFileBackedLog BuildLog { get; set; }

		public bool SpecifyPlatform { get; set; } = true;
		public bool SpecifyConfiguration { get; set; } = true;
		public List<string> Constants { get; } = new List<string> ();

		public BuildTool (IProcessManager processManager)
		{
			ProcessManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public BuildTool (IProcessManager processManager, TestPlatform platform) : this (processManager)
		{
			Platform = platform;
		}

		public virtual string Mode {
			get { return Platform.ToString (); }
			set { throw new NotSupportedException (); }
		}

		public virtual Task CleanAsync ()
		{
			Console.WriteLine ("Clean is not implemented for {0}", GetType ().Name);
			return Task.CompletedTask;
		}
	}
}
