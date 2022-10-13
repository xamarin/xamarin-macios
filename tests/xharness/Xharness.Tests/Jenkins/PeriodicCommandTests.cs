using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Moq;
using NUnit.Framework;
using Xharness.Jenkins;

namespace Xharness.Tests.Jenkins {

	[TestFixture]
	public class PeriodicCommandTests {

		Mock<IProcessManager> processManager;
		Mock<ILogs> logs;
		Mock<IFileBackedLog> log;
		TimeSpan interval;
		string command;
		string arguments;

		[SetUp]
		public void SetUp ()
		{
			processManager = new Mock<IProcessManager> (MockBehavior.Strict);
			logs = new Mock<ILogs> ();
			log = new Mock<IFileBackedLog> ();

			// common setup for the mocks
			logs.Setup (l => l.Create (It.Is<string> (s => true), It.Is<string> (s => true), null)).Returns (log.Object);
			interval = TimeSpan.FromMilliseconds (100);
			command = "test";
			arguments = "periodic";
		}

		// we do not test the options without the cancellation task because we want to be nice people when running
		// the tests and do not leave a thread doing nothing 
		[Test]
		public async Task TestExecuteNoArgs ()
		{
			var periodicCommand = new PeriodicCommand (command, processManager.Object, interval, logs.Object);
			var executionTcs = new TaskCompletionSource<bool> ();
			var threadCs = new CancellationTokenSource ();

			processManager.Setup (pm => pm.RunAsync (
				It.Is<Process> (p => p.StartInfo.FileName == command && p.StartInfo.Arguments == string.Empty),
				It.IsAny<ILog> (),
				interval,
				null,
				It.IsAny<CancellationToken> (),
				null)).Callback<Process, ILog, TimeSpan?, Dictionary<string, string>, CancellationToken?, bool?> (
					(p, l, i, env, t, d) => {
						executionTcs.TrySetResult (true);
					}).ReturnsAsync (new ProcessExecutionResult { ExitCode = 0, TimedOut = false }).Verifiable ();

			var task = periodicCommand.Execute (threadCs.Token);
			await executionTcs.Task; // wait for the callback in the mock, which is in another thread to set the source
			processManager.VerifyAll ();
			processManager.VerifyNoOtherCalls ();
			threadCs.Cancel (); // clean
		}

		[Test]
		public async Task TestExecuteArgs ()
		{
			// all similar logic to the above one, but with arguments
			var periodicCommand = new PeriodicCommand (command, processManager.Object, interval, logs.Object, arguments: arguments);
			var executionTcs = new TaskCompletionSource<bool> ();
			var threadCs = new CancellationTokenSource ();

			processManager.Setup (pm => pm.RunAsync (
				It.Is<Process> (p => p.StartInfo.FileName == command && p.StartInfo.Arguments == arguments),
				It.IsAny<ILog> (),
				interval,
				null,
				It.IsAny<CancellationToken> (),
				null)).Callback<Process, ILog, TimeSpan?, Dictionary<string, string>, CancellationToken?, bool?> (
					(p, l, i, env, t, d) => {
						executionTcs.TrySetResult (true);
					}).ReturnsAsync (new ProcessExecutionResult { ExitCode = 0, TimedOut = false }).Verifiable ();

			var task = periodicCommand.Execute (threadCs.Token);
			await executionTcs.Task; // wait for the callback in the mock, which is in another thread to set the source
			processManager.VerifyAll ();
			processManager.VerifyNoOtherCalls ();
			threadCs.Cancel (); // clean
		}

	}
}
