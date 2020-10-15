using System.Threading.Tasks;
using Xharness.Jenkins.TestTasks;

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
