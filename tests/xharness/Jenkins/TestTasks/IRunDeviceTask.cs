using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;

namespace Xharness.Jenkins.TestTasks {
	public interface IRunDeviceTask : IRunXITask<IHardwareDevice> {

		ITunnelBore TunnelBore { get; }
	}
}
