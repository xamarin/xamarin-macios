using System;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tasks {
	public interface IAcquiredResource : IDisposable
	{
		Resource Resource { get; }
	}
}
