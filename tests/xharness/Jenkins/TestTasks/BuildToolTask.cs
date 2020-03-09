using System;
using System.Threading.Tasks;
using Xharness.Execution;

namespace Xharness.Jenkins.TestTasks
{
	abstract class BuildToolTask : TestTask
	{
		public bool SpecifyPlatform = true;
		public bool SpecifyConfiguration = true;
		public IProcessManager ProcessManager { get; set; } = new ProcessManager ();

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
