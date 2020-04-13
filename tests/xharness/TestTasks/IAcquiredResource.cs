using System;
namespace Xharness.TestTasks {
	public interface IAcquiredResource : IDisposable
	{
		Resource Resource { get; }
	}
}
