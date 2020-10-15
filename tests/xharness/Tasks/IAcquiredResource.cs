using System;

namespace Xharness.Tasks {
	public interface IAcquiredResource : IDisposable {
		Resource Resource { get; }
	}
}
