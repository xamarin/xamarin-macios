using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared.Tasks;

namespace Xharness.Jenkins.Reports {

	/// <summary>
	/// To be implemented by those classes that write reports regarding the results of the executed tasks.
	/// </summary>
	interface IReportWriter {
		void Write (IList<ITestTask> tasks, StreamWriter writer);
	}
}
