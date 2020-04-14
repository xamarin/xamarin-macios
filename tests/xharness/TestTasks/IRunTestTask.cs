using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.TestTasks {
	public interface IRunTestTask : ITestTask {
		Task RunTestAsync ();
		Task VerifyBuildAsync ();
	}
}
