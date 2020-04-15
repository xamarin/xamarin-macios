using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness.TestTasks {
	public interface IRunXITask<TDevice> : IRunTestTask where TDevice : class, IDevice {

		public AppRunner Runner { get; set; }
		public AppRunner AdditionalRunner { get; set; }
		TestTarget AppRunnerTarget { get; set; }
		public IEnumerable<TDevice> Candidates { get; }
		public TDevice Device { get; set; }
		public TDevice CompanionDevice { get; set; }
	}
}
