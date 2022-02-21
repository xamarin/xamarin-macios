using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xharness.Jenkins.TestTasks;

#nullable enable
namespace Xharness.Jenkins.Reports {

	/// <summary>
	/// Knows how to write markdown reports of the executed tasks.
	/// </summary>
	class MarkdownReportWriter : IReportWriter {

		void WriteFailedTestsDetails (IEnumerable<ITestTask> failedTests, StreamWriter writer)
		{
			writer.WriteLine ("## Failed tests");
			writer.WriteLine ();

			foreach (var group in failedTests.GroupBy ((v) => v.TestName)) {
				if (group is IEnumerable<ITestTask> enumerableGroup) {
					foreach (var test in enumerableGroup) {
						writer.Write ($" * {group.Key}");
						if (!string.IsNullOrEmpty (test.Mode))
							writer.Write ($"/{test.Mode}");
						if (!string.IsNullOrEmpty (test.Variation))
							writer.Write ($"/{test.Variation}");
						writer.Write ($": {test.ExecutionResult}");
						if (!string.IsNullOrEmpty (test.FailureMessage))
							writer.Write ($" ({test.FailureMessage})");
						if (test.KnownFailure is not null)
							writer.Write ($" Known issue: [{test.KnownFailure.HumanMessage}]({test.KnownFailure.IssueLink})");
						writer.WriteLine ();
					}
					continue;
				}
			}

		}

		public void Write (IList<ITestTask> allTasks, StreamWriter writer)
		{
			var failedTests = allTasks.Where ((v) => v.Failed);
			var deviceNotFound = allTasks.Where ((v) => v.DeviceNotFound);
			var unfinishedTests = allTasks.Where ((v) => !v.Finished);
			var passedTests = allTasks.Where ((v) => v.Succeeded);
			var runningTests = allTasks.Where ((v) => v.Running && !v.Waiting);
			var buildingTests = allTasks.Where ((v) => v.Building && !v.Waiting);
			var runningQueuedTests = allTasks.Where ((v) => v.Running && v.Waiting);
			var buildingQueuedTests = allTasks.Where ((v) => v.Building && v.Waiting);

			if (unfinishedTests.Any () || failedTests.Any () || deviceNotFound.Any ()) {
				// Don't print when all tests succeed (cleaner)
				writer.WriteLine ("# Test results");
				writer.WriteLine ();
			}
			var details = failedTests.Any ();
			if (details) {
				writer.WriteLine ("<details>");
				writer.Write ("<summary>");
			}
			if (allTasks.Count == 0) {
				writer.Write ($"Loading tests...");
			} else if (unfinishedTests.Any ()) {
				var list = new List<string> ();
				var grouped = allTasks.GroupBy ((v) => v.ExecutionResult).OrderBy ((v) => (int) v.Key);
				foreach (var @group in grouped)
					list.Add ($"{@group.Key.ToString ()}: {@group.Count ()}");
				writer.Write ($"# Test run in progress: ");
				writer.Write (string.Join (", ", list));
			} else if (failedTests.Any ()) {
				writer.Write ($"{failedTests.Count ()} tests failed, ");
				if (deviceNotFound.Any ())
					writer.Write ($"{deviceNotFound.Count ()} tests' device not found, ");
				writer.Write ($"{passedTests.Count ()} tests passed.");
			} else if (deviceNotFound.Any ()) {
				writer.Write ($"{deviceNotFound.Count ()} tests' device not found, {passedTests.Count ()} tests passed.");
			} else if (passedTests.Any ()) {
				writer.Write ($"# :tada: All {passedTests.Count ()} tests passed :tada:");
			} else {
				writer.Write ($"# No tests selected.");
			}
			if (details)
				writer.Write ("</summary>");
			writer.WriteLine ();
			writer.WriteLine ();

			if (failedTests.Any ()) {
				WriteFailedTestsDetails (failedTests, writer);
			}

			if (details)
				writer.WriteLine ("</details>");
			writer.Flush ();
		}
	}
}
