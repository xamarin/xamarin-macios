using System;

namespace Xharness.Jenkins.TestTasks {
	public interface IAcquiredResource : IDisposable {
		Resource Resource { get; }
	}
}
