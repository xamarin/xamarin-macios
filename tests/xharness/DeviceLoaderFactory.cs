using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness {

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
