using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Execution {

	[TestFixture]
	public class ProcessManagerTests {

		// not very portable, but we want to make sure that the class does the right thing
		// maybe in the futue we could create a dummy process that will help us
		string logPath;
		string stdoutLogPath;
		string stderrLogPath;
		string stdoutMessage;
		string stderrMessage;
		ILog executionLog;
		ILog stdoutLog;
		ILog stderrLog;
		string dummyProcess;
		Process testProcess;
		ProcessManager manager;
		Dictionary<string, string> environmentVariables = new Dictionary<string, string> ();

		[SetUp]
		public void SetUp ()
		{
			logPath = Path.GetTempFileName ();
			stderrLogPath = Path.GetTempFileName ();
			stdoutLogPath = Path.GetTempFileName ();
			stdoutMessage = "Hola mundo!!!";
			stderrMessage = "Adios mundo cruel";
			executionLog = new LogFile ("my execution log", logPath);
			stdoutLog = new LogFile ("my stdout log", stdoutLogPath);
			stderrLog = new LogFile ("my stderr log", stderrLogPath);
			dummyProcess = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "DummyTestProcess.exe");
			manager = new ProcessManager ("/path/to/xcode", "/path/to/mlaunch", (v) => "/path/to/dotnet", "/path/to/msbuild");
			testProcess = new Process ();
			testProcess.StartInfo.FileName = "mono";
		}

		[TearDown]
		public void TearDown ()
		{
			executionLog?.Dispose ();
			executionLog = null;
			stdoutLog?.Dispose ();
			stdoutLog = null;
			stderrLog?.Dispose ();
			stderrLog = null;
			testProcess?.Dispose ();
			testProcess = null;
			manager = null;

			if (File.Exists (logPath))
				File.Delete (logPath);
			if (File.Exists (stderrLogPath))
				File.Delete (stderrLogPath);
			if (File.Exists (stdoutLogPath))
				File.Delete (stdoutLogPath);
		}

		void AssertStdoutAndStderr ()
		{
			bool stdoutFound = false;
			bool stderrFound = false;

			executionLog?.Dispose ();

			using (var reader = new StreamReader (logPath)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line.Contains (stdoutMessage))
						stdoutFound = true;
					if (line.Contains (stderrMessage))
						stderrFound = true;
				}
			}
			Assert.IsTrue (stdoutFound, "stdout was not captured");
			Assert.IsTrue (stderrFound, "stderr was not captured");
		}

		[TestCase (0, 1, true, false, Description = "Success")] // 0, short timeout, success, no timeout
		[TestCase (1, 1, false, false, Description = "Failure")] // 1, short timeout, failure, no timeout
		[TestCase (0, 60, false, true, Description = "Timeout")] // 0, long timeout, failure, timeout
		public async Task ExecuteCommandAsyncTest (int resultCode, int timeoutCount, bool success, bool timeout)
		{
			var args = new List<string> {
				dummyProcess,
				$"--exit-code={resultCode}",
				$"--timeout={timeoutCount}",
				$"--stdout=\"{stdoutMessage}\"",
				$"--stderr=\"{stderrMessage}\""
			};
			var result = await manager.ExecuteCommandAsync ("mono", args, executionLog, new TimeSpan (0, 0, 5));
			if (!timeout)
				Assert.AreEqual (resultCode, result.ExitCode, "exit code");
			Assert.AreEqual (success, result.Succeeded, "success");
			Assert.AreEqual (timeout, result.TimedOut, "timeout");
			AssertStdoutAndStderr ();
		}

		[TestCase (0, 1, true, false, Description = "Success")] // 0, short timeout, success, no timeout
		[TestCase (1, 1, false, false, Description = "Failure")] // 1, short timeout, failure, no timeout
		public async Task RunAsyncProcessNoArgsTest (int resultCode, int timeoutCount, bool success, bool timeout)
		{
			var source = new CancellationTokenSource ();
			testProcess.StartInfo.Arguments = $"{dummyProcess} --exit-code={resultCode} --timeout={timeoutCount} --stdout=\"{stdoutMessage}\" --stderr=\"{stderrMessage}\"";
			var result = await manager.RunAsync (testProcess, executionLog, executionLog, executionLog, cancellationToken: source.Token);
			if (!timeout)
				Assert.AreEqual (resultCode, result.ExitCode, "exit code");
			Assert.AreEqual (success, result.Succeeded, "success");
			Assert.AreEqual (timeout, result.TimedOut, "timeout");
			AssertStdoutAndStderr ();
		}

		[TestCase (0, 1, true, false, Description = "Success")] // 0, short timeout, success, no timeout
		[TestCase (1, 1, false, false, Description = "Failure")] // 1, short timeout, failure, no timeout
		[TestCase (0, 60, false, true, Description = "Timeout")] // 0, long timeout, failure, timeout
		public async Task RunAsycnProcessAppendSuccessTest (int resultCode, int timeoutCounter, bool success, bool timeout)
		{
			// write some trash in the test log so that we ensure that we did append
			string oldMessage = "Hello hello!";
			using (var writer = new StreamWriter (logPath)) {
				writer.WriteLine (oldMessage);
			}
			testProcess.StartInfo.Arguments = $"{dummyProcess} --exit-code={resultCode} --timeout={timeoutCounter} --stdout=\"{stdoutMessage}\" --stderr=\"{stderrMessage}\"";
			var result = await manager.RunAsync (testProcess, executionLog, new TimeSpan (0, 0, 5));
			if (!timeout)
				Assert.AreEqual (resultCode, result.ExitCode, "exit code");
			Assert.AreEqual (timeout, result.TimedOut, "timeout");
			Assert.AreEqual (success, result.Succeeded, "success");
			AssertStdoutAndStderr ();
		}

		[TestCase (0, 1, true, false, Description = "Success")] // 0, short timeout, success, no timeout
		[TestCase (1, 1, false, false, Description = "Failure")] // 1, short timeout, failure, no timeout
		[TestCase (0, 60, false, true, Description = "Timeout")] // 0, long timeout, failure, timeout
		public async Task RunAsyncProcessTextWritersTest (int resultCode, int timeoutCounter, bool success, bool timeout)
		{
			//Task<ProcessExecutionResult> RunAsync (Process process, ILog log, ILog stdoutLog, ILog stderrLog, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null);
			var source = new CancellationTokenSource ();
			testProcess.StartInfo.Arguments = $"{dummyProcess} --exit-code={resultCode} --timeout={timeoutCounter} --stdout=\"{stdoutMessage}\" --stderr=\"{stderrMessage}\"";
			var result = await manager.RunAsync (testProcess, executionLog, stdoutLog, stderrLog, new TimeSpan (0, 0, 5), environmentVariables, source.Token);
			if (!timeout)
				Assert.AreEqual (resultCode, result.ExitCode, "exit code");
			Assert.AreEqual (timeout, result.TimedOut, "timeout");
			Assert.AreEqual (success, result.Succeeded, "success");
			// assert the diff logs have the correct line
			bool stdoutFound = false;
			bool stderrFound = false;

			stdoutLog?.Dispose ();
			stderrLog?.Dispose ();

			using (var reader = new StreamReader (stdoutLogPath)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line.Contains (stdoutMessage))
						stdoutFound = true;
				}
			}
			Assert.IsTrue (stdoutFound, "stdout was not captured");

			using (var reader = new StreamReader (stderrLogPath)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line.Contains (stderrMessage))
						stderrFound = true;
				}
			}
			Assert.IsTrue (stderrFound, "stderr was not captured");
		}

	}
}
