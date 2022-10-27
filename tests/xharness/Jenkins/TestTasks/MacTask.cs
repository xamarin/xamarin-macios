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
					return TestProject?.IsDotNetProject == true ? "Mac [dotnet]" : "Mac";
				case TestPlatform.Mac_Modern:
					return "Mac Modern";
				case TestPlatform.Mac_Full:
					return "Mac Full";
				case TestPlatform.Mac_System:
					return "Mac System";
				case TestPlatform.MacCatalyst:
					return "Mac Catalyst [dotnet]";
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
