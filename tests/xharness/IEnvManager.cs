using System.Diagnostics;

namespace Xharness {
	/// <summary>
	/// Knows how to handle the different enviroment variables to be used with the processes executed by xharness and
	/// the different tests tasks.
	/// </summary>
	public interface IEnvManager {
		void SetEnvironmentVariables (Process process);
	}
}
