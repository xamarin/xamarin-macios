using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Hardware {

	[TestFixture]
	public class SimulatorsTest {

		string mlaunchPath;
		string sdkPath;
		Mock<ILog> executionLog;
		Mock<IProcessManager> processManager;
		SimulatorLoader simulators;

		[SetUp]
		public void SetUp ()
		{
			mlaunchPath = "/usr/bin/mlaunch"; // any will be ok, is mocked
			sdkPath = "/Applications/Xcode.app";
			executionLog = new Mock<ILog> ();
			processManager = new Mock<IProcessManager> ();
			simulators = new SimulatorLoader (processManager.Object);
		}

		[TearDown]
		public void TearDown ()
		{
			executionLog = null;
			processManager = null;
			simulators = null;
		}

		[TestCase (false)] // no timeout
		[TestCase (true)] // timeout
		public void LoadAsyncProcessErrorTest (bool timeout)
		{
			string processPath = null;
			MlaunchArguments passedArguments = null;

			// moq It.Is is not working as nicelly as we would like it, we capture data and use asserts
			processManager.Setup (p => p.RunAsync (It.IsAny<Process> (), It.IsAny<MlaunchArguments> (), It.IsAny<ILog> (), It.IsAny<TimeSpan?> (), It.IsAny<Dictionary<string, string>> (), It.IsAny<CancellationToken?> (), It.IsAny<bool?> ()))
				.Returns<Process, MlaunchArguments, ILog, TimeSpan?, Dictionary<string, string>, CancellationToken?, bool?> ((p, args, log, t, env, token, d) => {
					// we are going set the used args to validate them later, will always return an error from this method
					processPath = p.StartInfo.FileName;
					passedArguments = args;
					if (!timeout)
						return Task.FromResult (new ProcessExecutionResult { ExitCode = 1, TimedOut = false });
					else
						return Task.FromResult (new ProcessExecutionResult { ExitCode = 0, TimedOut = true });
				});

			Assert.ThrowsAsync<Exception> (async () => {
				await simulators.LoadDevices (executionLog.Object);
			});

			// validate the execution of mlaunch
			MlaunchArgument listSimArg = passedArguments.Where (a => a is ListSimulatorsArgument).FirstOrDefault ();
			Assert.IsNotNull (listSimArg, "list devices arg missing");

			MlaunchArgument outputFormatArg = passedArguments.Where (a => a is XmlOutputFormatArgument).FirstOrDefault ();
			Assert.IsNotNull (outputFormatArg, "output format arg missing");
		}

		void CopySampleData (string tempPath)
		{
			var name = GetType ().Assembly.GetManifestResourceNames ().Where (a => a.EndsWith ("simulators.xml", StringComparison.Ordinal)).FirstOrDefault ();
			using (var outputStream = new StreamWriter (tempPath))
			using (var sampleStream = new StreamReader (GetType ().Assembly.GetManifestResourceStream (name))) {
				string line;
				while ((line = sampleStream.ReadLine ()) != null)
					outputStream.WriteLine (line);
			}
		}

		[Test]
		public async Task LoadAsyncProcessSuccess ()
		{
			string processPath = null;
			MlaunchArguments passedArguments = null;

			// moq It.Is is not working as nicelly as we would like it, we capture data and use asserts
			processManager.Setup (p => p.RunAsync (It.IsAny<Process> (), It.IsAny<MlaunchArguments> (), It.IsAny<ILog> (), It.IsAny<TimeSpan?> (), It.IsAny<Dictionary<string, string>> (), It.IsAny<CancellationToken?> (), It.IsAny<bool?> ()))
				.Returns<Process, MlaunchArguments, ILog, TimeSpan?, Dictionary<string, string>, CancellationToken?, bool?> ((p, args, log, t, env, token, d) => {
					processPath = p.StartInfo.FileName;
					passedArguments = args;

					// we get the temp file that was passed as the args, and write our sample xml, which will be parsed to get the devices :)
					var tempPath = args.Where (a => a is ListSimulatorsArgument).First ().AsCommandLineArgument ();
					tempPath = tempPath.Substring (tempPath.IndexOf ('=') + 1).Replace ("\"", string.Empty);

					CopySampleData (tempPath);
					return Task.FromResult (new ProcessExecutionResult { ExitCode = 0, TimedOut = false });
				});

			await simulators.LoadDevices (executionLog.Object);

			MlaunchArgument listSimArg = passedArguments.Where (a => a is ListSimulatorsArgument).FirstOrDefault ();
			Assert.IsNotNull (listSimArg, "list devices arg missing");

			MlaunchArgument outputFormatArg = passedArguments.Where (a => a is XmlOutputFormatArgument).FirstOrDefault ();
			Assert.IsNotNull (outputFormatArg, "output format arg missing");

			Assert.AreEqual (75, simulators.AvailableDevices.Count ());
		}

		[TestCase (TestTarget.Simulator_iOS64, 1)]
		[TestCase (TestTarget.Simulator_iOS32, 1)]
		[TestCase (TestTarget.Simulator_tvOS, 1)]
		[TestCase (TestTarget.Simulator_watchOS, 2)]
		public async Task FindAsyncDoNotCreateTest (TestTarget target, int expected)
		{
			string processPath = null;
			MlaunchArguments passedArguments = null;

			processManager
				.Setup (h => h.ExecuteXcodeCommandAsync ("simctl", It.Is<string []> (args => args [0] == "create"), executionLog.Object, TimeSpan.FromMinutes (1)))
				.ReturnsAsync (new ProcessExecutionResult () { ExitCode = 0 });

			// moq It.Is is not working as nicelly as we would like it, we capture data and use asserts
			processManager
				.Setup (p => p.RunAsync (It.IsAny<Process> (), It.IsAny<MlaunchArguments> (), It.IsAny<ILog> (), It.IsAny<TimeSpan?> (), It.IsAny<Dictionary<string, string>> (), It.IsAny<CancellationToken?> (), It.IsAny<bool?> ()))
				.Returns<Process, MlaunchArguments, ILog, TimeSpan?, Dictionary<string, string>, CancellationToken?, bool?> ((p, args, log, t, env, token, d) => {
					processPath = p.StartInfo.FileName;
					passedArguments = args;

					// we get the temp file that was passed as the args, and write our sample xml, which will be parsed to get the devices :)
					var tempPath = args.Where (a => a is ListSimulatorsArgument).First ().AsCommandLineArgument ();
					tempPath = tempPath.Substring (tempPath.IndexOf ('=') + 1).Replace ("\"", string.Empty);

					CopySampleData (tempPath);
					return Task.FromResult (new ProcessExecutionResult { ExitCode = 0, TimedOut = false });
				});

			await simulators.LoadDevices (executionLog.Object);
			var sims = await simulators.FindSimulators (target, executionLog.Object, false, false);

			Assert.AreEqual (expected, sims.Count (), $"{target} simulators count");
		}

		[Test]
		public async Task FindAndCacheSimulatorsWithFailingMlaunchTest ()
		{
			// Moq.SetupSequence doesn't allow custom callbacks so we need to count ourselves
			var calls = 0;

			processManager
				.Setup (p => p.RunAsync (It.IsAny<Process>(), It.Is<MlaunchArguments> (args => args.Any (a => a is ListSimulatorsArgument)), It.IsAny<ILog> (), It.IsAny<TimeSpan> (), It.IsAny<Dictionary<string, string>> (), It.IsAny<CancellationToken?> (), It.IsAny<bool?> ()))
				.Returns<Process, MlaunchArguments, ILog, TimeSpan, Dictionary<string, string>, CancellationToken?, bool?> ((process, args, log, t, env, token, diagnostics) => {
					calls++;

					if (calls == 1) {
						// Mlaunch can sometimes time out and we are testing that a subsequent Load will trigger it again
						return Task.FromResult (new ProcessExecutionResult { ExitCode = 137, TimedOut = true });
					}

					// We get the temp file that was passed as the args, and write our sample xml, which will be parsed to get the devices
					var tempPath = args.Where (a => a is ListSimulatorsArgument).First ().AsCommandLineArgument ();
					tempPath = tempPath.Substring (tempPath.IndexOf ('=') + 1).Replace ("\"", string.Empty);

					CopySampleData (tempPath);
					return Task.FromResult (new ProcessExecutionResult { ExitCode = 0, TimedOut = false });
				});

			Assert.ThrowsAsync<Exception> (async () => await simulators.LoadDevices (executionLog.Object));

			Assert.IsEmpty (simulators.AvailableDevices);
			Assert.AreEqual (1, calls);
			await simulators.LoadDevices (executionLog.Object);
			Assert.AreEqual (2, calls);
			Assert.IsNotEmpty (simulators.AvailableDevices);
			await simulators.LoadDevices (executionLog.Object);
			Assert.AreEqual (2, calls);
			await simulators.LoadDevices (executionLog.Object);
			Assert.AreEqual (2, calls);
		}
	}
}
