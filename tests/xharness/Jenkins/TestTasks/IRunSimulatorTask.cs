using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness.Jenkins.TestTasks {
	public interface IRunSimulatorTask : IRunXITask<ISimulatorDevice> {

		Task<IAcquiredResource> AcquireResourceAsync ();
	}
}
