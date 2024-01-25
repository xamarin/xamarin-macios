using System.Collections.Generic;
using System.IO;

using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins.Reports {

	/// <summary>
	/// To be implemented by those classes that write reports regarding the results of the executed tasks.
	/// </summary>
	interface IReportWriter {
		void Write (IList<ITestTask> tasks, StreamWriter writer);
	}
}
