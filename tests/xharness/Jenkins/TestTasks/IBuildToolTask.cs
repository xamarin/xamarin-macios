using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;

namespace Xharness.Jenkins.TestTasks {

	/// <summary>
	/// Interface to be implemented by those tasks that represent a build execution. 
	/// </summary>
	public interface IBuildToolTask : ITestTask {
		IFileBackedLog BuildLog { get; }
		bool SpecifyPlatform { get; set; }
		bool SpecifyConfiguration { get; set; }

		IProcessManager ProcessManager { get; }
		TestProject TestProject { get; set; }

		Task CleanAsync ();
	}
}
