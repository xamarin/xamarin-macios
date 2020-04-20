using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Tasks;

namespace Xharness.TestTasks {
	public interface IRunSimulatorTask : IRunXITask<ISimulatorDevice> {

		Task<IAcquiredResource> AcquireResourceAsync ();
	}
}
