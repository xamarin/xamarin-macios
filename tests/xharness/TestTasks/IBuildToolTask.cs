using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;

namespace Xharness.TestTasks {

	/// <summary>
	/// Interface to be implemented by those tasks that represent a build execution. 
	/// </summary>
	public interface IBuildToolTask : ITestTask {
		IProcessManager ProcessManager { get; }
		string TestName { get; set; }
		bool SpecifyPlatform { get; set; }
		bool SpecifyConfiguration { get; set; }
		TestProject TestProject { get; set; }
		TestPlatform Platform { get; set; }
		string Mode { get; set; }
		public Task CleanAsync ();
	}
}
