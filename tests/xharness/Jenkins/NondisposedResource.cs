namespace xharness.Jenkins
{
    partial class RunSimulatorTask
    {
        class NondisposedResource : IAcquiredResource
		{
			public IAcquiredResource Wrapped;

			public Resource Resource
			{
				get
				{
					return Wrapped.Resource;
				}
			}

			public void Dispose()
			{
				// Nope, no disposing here.
			}
		}
	}
}
