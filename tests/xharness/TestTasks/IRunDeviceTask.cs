using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;

namespace Xharness.TestTasks {
	public interface IRunDeviceTask : IRunXITask<IHardwareDevice> {

		ITunnelBore TunnelBore { get; }
	}
}
