// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

using Xamarin.Utils;

namespace Xamarin.Tests {
	[TestFixture]
	public class ExecutionTests {
		[SetUp]
		public void SetUp ()
		{
			if (OperatingSystem.IsWindows ())
				Assert.Ignore ("These tests require Unix executables.");
		}

		[Test]
		public async Task SuccessfulExecution ()
		{
			var called = false;
			var execution = new Execution {
				FileName = "/bin/sh",
				Arguments = ["-c", "echo stdout; echo stderr >&2"],
				Timeout = TimeSpan.FromSeconds (30),
				TimeoutCallback = _ => called = true,
			};

			await execution.RunAsync ();

			Assert.That (execution.ExitCode, Is.EqualTo (0), "Exit code");
			Assert.That (execution.TimedOut, Is.False, "Timed out");
			Assert.That (called, Is.False, "Timeout callback");
			Assert.That (execution.Output.StandardOutput.Trim (), Is.EqualTo ("stdout"), "Standard output");
			Assert.That (execution.Output.StandardError.Trim (), Is.EqualTo ("stderr"), "Standard error");
		}

		[TestCase (false)]
		[TestCase (true)]
		public async Task TimeoutCallbackRunsBeforeKill (bool throwInCallback)
		{
			var called = false;
			var runningInCallback = false;
			using var log = new StringWriter ();
			var execution = new Execution {
				FileName = "/bin/sleep",
				Arguments = ["60"],
				Timeout = TimeSpan.FromMilliseconds (100),
				Log = log,
				TimeoutCallback = process => {
					called = true;
					runningInCallback = !process.HasExited;
					if (throwInCallback)
						throw new InvalidOperationException ("Diagnostic failure");
				},
			};

			await execution.RunAsync ();

			Assert.That (called, Is.True, "Timeout callback");
			Assert.That (runningInCallback, Is.True, "Process must still be running when collecting diagnostics");
			Assert.That (execution.TimedOut, Is.True, "Timed out");
			Assert.That (execution.ExitCode, Is.Not.EqualTo (0), "Process must be killed even if diagnostics fail");
			if (throwInCallback)
				Assert.That (log.ToString (), Does.Contain ("Diagnostic failure"), "Diagnostic errors must be logged");
		}

		[Test]
		public async Task TimeoutDiagnostics ()
		{
			if (!OperatingSystem.IsMacOS ())
				Assert.Ignore ("Process sampling requires macOS.");

			using var log = new StringWriter ();
			var execution = new Execution {
				FileName = "/bin/sleep",
				Arguments = ["60"],
				Timeout = TimeSpan.FromMilliseconds (100),
				Log = log,
				TimeoutCallback = process => TestBaseClass.CollectTimeoutDiagnostics (process, log),
			};

			await execution.RunAsync ();

			var diagnostics = log.ToString ();
			Assert.That (execution.TimedOut, Is.True, "Timed out");
			Assert.That (execution.ExitCode, Is.Not.EqualTo (0), "Process must be killed after sampling");
			Assert.That (diagnostics, Does.Contain ("OS architecture:"), "Machine context");
			Assert.That (diagnostics, Does.Contain ("ps exited with code 0 (timed out: False)"), "Process status");
			Assert.That (diagnostics, Does.Contain ("sample exited with code 0 (timed out: False)"), "Sampling");
			Assert.That (diagnostics, Does.Contain ("Call graph:"), "Thread stacks");
		}
	}
}
