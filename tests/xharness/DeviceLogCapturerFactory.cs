using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness {
	public interface IDeviceLogCapturerFactory {
		IDeviceLogCapturer Create (ILog mainLog, ILog deviceLog, string deviceName);
	}

	public class DeviceLogCapturerFactory : IDeviceLogCapturerFactory {
		readonly IProcessManager processManager;

		public DeviceLogCapturerFactory (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public IDeviceLogCapturer Create (ILog mainLog, ILog deviceLog, string deviceName)
		{
			return new DeviceLogCapturer (processManager, mainLog, deviceLog, deviceName);
		}
	}
}

