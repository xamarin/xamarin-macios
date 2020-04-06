using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {

	public interface IDeviceLoaderFactory {
		IHardwareDeviceLoader CreateLoader ();
	}

	public class DeviceLoaderFactory : IDeviceLoaderFactory {
		readonly IProcessManager processManager;

		public DeviceLoaderFactory (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public IHardwareDeviceLoader CreateLoader () => new HardwareDeviceLoader (processManager);
	}
}
