using System.Collections.Generic;
using System.IO;

using Microsoft.DotNet.XHarness.iOS.Shared;

using Moq;

using NUnit.Framework;

using Xharness.Jenkins.Reports;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Tests.Jenkins {
	[TestFixture]
	public class MarkdownReportWriterTests {

		string path;
		MarkdownReportWriter reportWriter;

		[SetUp]
		public void SetUp ()
		{
			path = Path.GetTempFileName ();
			File.Delete (path);
			reportWriter = new MarkdownReportWriter ();
		}

		[TearDown]
		public void TearDown ()
		{
			File.Delete (path);
		}

		[Test]
		public void AllSuccessfulTest ()
		{
			int count = 10;
			List<ITestTask> tasks = new List<ITestTask> ();
			for (var i = 0; i < count; i++) {
				var success = new Mock<ITestTask> ();
				success.Setup (t => t.Finished).Returns (true);
				success.Setup (t => t.Succeeded).Returns (true);
				success.Setup (t => t.TestName).Returns ($"Success with id {i}");
				tasks.Add (success.Object);
			}

			for (var i = 0; i < count; i++) {
				var ignored = new Mock<ITestTask> ();
				ignored.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Ignored);
				ignored.Setup (t => t.Finished).Returns (true);
				tasks.Add (ignored.Object);
			}

			using (var writer = new StreamWriter (path)) {
				reportWriter.Write (tasks, writer);
			}

			using (var reader = new StreamReader (path)) {
				var report = reader.ReadToEnd ();
				Assert.NotNull (report);
				Assert.AreEqual ($"# :tada: All {count} tests passed :tada:\n\n", report);
			}
		}

		[Test]
		public void MixResultsFailAndSuccessTest ()
		{
			int count = 10;
			List<ITestTask> tasks = new List<ITestTask> ();
			for (var i = 0; i < count / 2; i++) {
				var success = new Mock<ITestTask> ();
				success.Setup (t => t.Finished).Returns (true);
				success.Setup (t => t.Succeeded).Returns (true);
				success.Setup (t => t.TestName).Returns ($"Success with id {i}");
				tasks.Add (success.Object);
			}

			for (var i = 0; i < count / 2; i++) {
				var failure = new Mock<ITestTask> ();
				failure.Setup (t => t.Finished).Returns (true);
				failure.Setup (t => t.Succeeded).Returns (false);
				failure.Setup (t => t.Failed).Returns (true);
				failure.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Failed);
				failure.Setup (t => t.TestName).Returns ($"Failure with id {i}");
				tasks.Add (failure.Object);
			}

			for (var i = 0; i < count; i++) {
				var ignored = new Mock<ITestTask> ();
				ignored.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Ignored);
				ignored.Setup (t => t.Finished).Returns (true);
				tasks.Add (ignored.Object);
			}

			using (var writer = new StreamWriter (path)) {
				reportWriter.Write (tasks, writer);
			}

			string summaryLine = null;
			int failedTestsLineCount = 0;

			using (var reader = new StreamReader (path)) {
				string line = null;
				while ((line = reader.ReadLine ()) is not null) {
					if (line.StartsWith ("<summary"))
						summaryLine = line;
					if (line.Contains ("Failure with id "))
						failedTestsLineCount++;
				}
			}
			Assert.NotNull (summaryLine, "summary line null");
			Assert.AreEqual ($"<summary>{count / 2} tests failed, {count / 2} tests passed.</summary>", summaryLine, "summary value");
			Assert.AreEqual (count / 2, failedTestsLineCount, "Error count");
		}

		[Test]
		public void MissingDeviceTest ()
		{
			int count = 10;
			List<ITestTask> tasks = new List<ITestTask> ();
			for (var i = 0; i < count; i++) {
				var success = new Mock<ITestTask> ();
				success.Setup (t => t.DeviceNotFound).Returns (true);
				success.Setup (t => t.Finished).Returns (true);
				success.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.DeviceNotFound);
				success.Setup (t => t.TestName).Returns ($"Failed with id {i}");
				tasks.Add (success.Object);
			}

			using (var writer = new StreamWriter (path)) {
				reportWriter.Write (tasks, writer);
			}

			using (var reader = new StreamReader (path)) {
				var report = reader.ReadToEnd ();
				Assert.NotNull (report);
				Assert.AreEqual ($"# Test results\n\n{count} tests' device not found, 0 tests passed.\n\n", report);
			}
		}

		[Test]
		public void NotTestsTest ()
		{
			int count = 10;
			List<ITestTask> tasks = new List<ITestTask> ();
			for (var i = 0; i < count; i++) {
				var ignored = new Mock<ITestTask> ();
				ignored.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Ignored);
				ignored.Setup (t => t.Finished).Returns (true);
				tasks.Add (ignored.Object);
			}

			using (var writer = new StreamWriter (path)) {
				reportWriter.Write (tasks, writer);
			}

			using (var reader = new StreamReader (path)) {
				var report = reader.ReadToEnd ();
				Assert.NotNull (report);
				Assert.AreEqual ("# No tests selected.\n\n", report);
			}
		}

		[Test]
		public void KnownIssueTest ()
		{
			int count = 10;
			List<ITestTask> tasks = new List<ITestTask> ();

			for (var i = 0; i < count; i++) {
				var failure = new Mock<ITestTask> ();
				var knownIssue = new KnownIssue (humanMessage: "Testing known issues", issueLink: "http://github.com");
				failure.Setup (t => t.Finished).Returns (true);
				failure.Setup (t => t.Succeeded).Returns (false);
				failure.Setup (t => t.Failed).Returns (true);
				failure.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Failed);
				failure.Setup (t => t.TestName).Returns ($"Failure with id {i}");
				failure.Setup (t => t.KnownFailure).Returns (knownIssue);
				tasks.Add (failure.Object);
			}


			using (var writer = new StreamWriter (path)) {
				reportWriter.Write (tasks, writer);
			}

			int failureCount = 0;
			using (var reader = new StreamReader (path)) {
				string line = null;
				while ((line = reader.ReadLine ()) is not null) {
					if (line.Contains ("Failure")) {
						Assert.AreEqual ($" * Failure with id {failureCount}: Failed Known issue: [Testing known issues](http://github.com)", line, line);
						failureCount++;
					}
				}
			}
		}
	}
}
