using System;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.Jenkins.TestTasks
{
	public abstract class BuildToolTask : TestTask
	{
		protected readonly IProcessManager ProcessManager;

		public bool SpecifyPlatform = true;
		public bool SpecifyConfiguration = true;

		protected BuildToolTask (IProcessManager processManager)
		{
			ProcessManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public override string Mode {
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
