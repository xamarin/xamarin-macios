using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Tasks;

namespace Xharness.TestTasks {
	public interface IRunTestTask : ITestTask {
		IHarness Harness { get; }
		double TimeoutMultiplier { get; }
		IProcessManager ProcessManager { get; }
		IBuildToolTask BuildTask { get; }
		Task RunTestAsync ();
		Task VerifyBuildAsync ();
	}
}
