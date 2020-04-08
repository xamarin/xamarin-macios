using System;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.TestTasks {

	public class BuildToolTask
	{
		public IProcessManager ProcessManager { get; }
		public TestPlatform Platform { get; set; }

		public bool SpecifyPlatform { get; set; } = true;
		public bool SpecifyConfiguration { get; set; } = true;

		public BuildToolTask (IProcessManager processManager)
		{ 
			ProcessManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public BuildToolTask (IProcessManager processManager, TestPlatform platform) : this (processManager)
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
