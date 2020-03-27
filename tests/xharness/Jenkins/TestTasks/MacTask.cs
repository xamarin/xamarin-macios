using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.Jenkins.TestTasks
{
	abstract class MacTask : RunTestTask
	{
		public MacTask (BuildToolTask build_task, IProcessManager processManager)
			: base (build_task, processManager)
		{
		}

		public override string Mode {
			get {
				switch (Platform) {
				case TestPlatform.Mac:
					return "Mac";
				case TestPlatform.Mac_Modern:
					return "Mac Modern";
				case TestPlatform.Mac_Full:
					return "Mac Full";
				case TestPlatform.Mac_System:
					return "Mac System";
				default:
					throw new NotImplementedException (Platform.ToString ());
				}
			}
			set {
				throw new NotSupportedException ();
			}
		}
	}
}
