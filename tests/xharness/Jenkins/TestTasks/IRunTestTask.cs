using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.Jenkins.TestTasks {
	public interface IRunTestTask : ITestTask {
		IHarness Harness { get; }
		double TimeoutMultiplier { get; }
		IMlaunchProcessManager ProcessManager { get; }
		IBuildToolTask BuildTask { get; }
		Task RunTestAsync ();
		Task VerifyBuildAsync ();
	}
}
