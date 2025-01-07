using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.Jenkins.TestTasks {
	abstract class MacTask : RunTestTask {
		public MacTask (Jenkins jenkins, BuildToolTask build_task, IMlaunchProcessManager processManager)
			: base (jenkins, build_task, processManager)
		{
		}

		public override string Mode {
			get {
				switch (Platform) {
				case TestPlatform.Mac:
					return "macOS";
				case TestPlatform.MacCatalyst:
					return "Mac Catalyst";
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
