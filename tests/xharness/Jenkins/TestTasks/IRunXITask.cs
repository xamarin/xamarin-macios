using System.Collections.Generic;

using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness.Jenkins.TestTasks {
	public interface IRunXITask<TDevice> : IRunTestTask where TDevice : class, IDevice {

		AppRunner Runner { get; set; }
		AppRunner AdditionalRunner { get; set; }
		TestTarget AppRunnerTarget { get; set; }
		IEnumerable<TDevice> Candidates { get; }
		TDevice Device { get; set; }
		TDevice CompanionDevice { get; set; }
	}
}
