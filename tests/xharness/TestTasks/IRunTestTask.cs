using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

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
