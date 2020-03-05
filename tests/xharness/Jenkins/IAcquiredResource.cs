using System;

namespace xharness.Jenkins
{
    interface IAcquiredResource : IDisposable
	{
		Resource Resource { get; }
	}
}
