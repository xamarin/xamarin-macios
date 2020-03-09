using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Xharness.Hardware;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness.Tests.Hardware.Tests {
	
	[TestFixture]
	public class DevicesTest {

		Devices devices;
		string mlaunchPath;
		string sdkPath;
		Mock<IHarness> harness;
		Mock<IProcessManager> processManager;
		Mock<ILog> executionLog;

		[SetUp]
		public void SetUp ()
		{
			harness = new Mock<IHarness> ();
			processManager = new Mock<IProcessManager> ();
			devices = new Devices {
				Harness = harness.Object,
				ProcessManager = processManager.Object,
			};
			executionLog = new Mock<ILog> ();
			mlaunchPath = "/usr/bin/mlaunch"; // any will be ok, is mocked
			sdkPath = "/Applications/Xcode.app";
			
		}

		[TearDown]
		public void TearDown ()
		{
			harness = null;
			processManager = null;
			executionLog = null;
			devices = null;
		}

		[TestCase (false)] // no timeout
		[TestCase (true)] // timeoout
		public void LoadAsyncProcessErrorTest (bool timeout)
		{
			string processPath = null;
			MlaunchArguments passedArguments = null;
			// set the expectations of the mocks to get an error when
			// executing the process
			harness.Setup (h => h.MlaunchPath).Returns (mlaunchPath);
			harness.Setup (h => h.XcodeRoot).Returns (sdkPath);
			// moq It.Is is not working as nicelly as we would like it, we capture data and use asserts
			processManager.Setup (p => p.RunAsync (It.IsAny<Process> (), It.IsAny<MlaunchArguments> (), It.IsAny<ILog> (), It.IsAny<TimeSpan?> (), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken?> (), It.IsAny<bool?> ()))
				.Returns<Process, MlaunchArguments, ILog, TimeSpan?, Dictionary< string, string>, CancellationToken?, bool?> ((p, args, log, t, env, token, d) => {
					// we are going set the used args to validate them later, will always return an error from this method
					processPath = p.StartInfo.FileName;
					passedArguments = args;
					if (!timeout)
						return Task.FromResult (new ProcessExecutionResult { ExitCode = 1, TimedOut = false });
					else
						return Task.FromResult (new ProcessExecutionResult { ExitCode = 0, TimedOut = true });
			});
			Assert.ThrowsAsync<Exception> (async () => {
				await devices.LoadAsync (executionLog.Object);
			});

			// validate the execution of mlaunch
			Assert.AreEqual (mlaunchPath, processPath, "process path");
			Assert.AreEqual (1, passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.SdkRoot).Count (), "sdk arg missing");
			var sdkArg = passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.SdkRoot).First ();
			Assert.AreEqual (sdkArg.value, sdkPath, "sdk path");
			Assert.AreEqual (1, passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.ListDev).Count (), "sdk path missing.");
			var listDevArg = passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.ListDev).First ();
			Assert.IsNotNull (listDevArg.value, "litdev file");
			Assert.AreEqual (1, passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.OutputFormat).Count (), "output format missing.");
			var outputFormat = passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.OutputFormat).First ();
			Assert.AreEqual (outputFormat.value, "xml"); // format
		}

		[TestCase (true)]
		[TestCase (false)]
		public async Task LoadAsyncProcessSuccess (bool extraData)
		{
			string processPath = null;
			MlaunchArguments passedArguments = null;
			// set the expectations of the mocks to get an error when
			// executing the process
			harness.Setup (h => h.MlaunchPath).Returns (mlaunchPath);
			harness.Setup (h => h.XcodeRoot).Returns (sdkPath);
			// moq It.Is is not working as nicelly as we would like it, we capture data and use asserts
			processManager.Setup (p => p.RunAsync (It.IsAny<Process> (), It.IsAny<MlaunchArguments> (), It.IsAny<ILog> (), It.IsAny<TimeSpan?> (), It.IsAny<Dictionary<string, string>> (), It.IsAny<CancellationToken?> (), It.IsAny<bool?> ()))
				.Returns<Process, MlaunchArguments, ILog, TimeSpan?, Dictionary<string, string>, CancellationToken?, bool?> ((p, args, log, t, env, token, d) => {
					processPath = p.StartInfo.FileName;
					passedArguments = args;

					// we get the temp file that was passed as the args, and write our sample xml, which will be parsed to get the devices :)
					var (type, value) = args.GetArguments ().Where (a => a.type == MlaunchArgumentType.ListDev).First ();
					var tempPath = value;
					var name = GetType ().Assembly.GetManifestResourceNames ().Where (a => a.EndsWith ("devices.xml", StringComparison.Ordinal)).FirstOrDefault ();
					using (var outputStream = new StreamWriter (tempPath))
					using (var sampleStream = new StreamReader (GetType ().Assembly.GetManifestResourceStream (name))) {
						string line;
						while ((line = sampleStream.ReadLine ()) != null)
							outputStream.WriteLine (line);
					}
					return Task.FromResult (new ProcessExecutionResult { ExitCode = 0, TimedOut = false });
				});
			await devices.LoadAsync (executionLog.Object, extraData);
			// assert the devices that are expected from the sample xml
			// validate the execution of mlaunch
			Assert.AreEqual (mlaunchPath, processPath, "process path");
			Assert.AreEqual (1, passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.SdkRoot).Count (), "sdk arg missing");
			var sdkArg = passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.SdkRoot).First ();
			Assert.AreEqual (sdkArg.value, sdkPath, "sdk path");
			Assert.AreEqual (1, passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.ListDev).Count (), "sdk path missing.");
			var listDevArg = passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.ListDev).First ();
			Assert.IsNotNull (listDevArg.value, "litdev file");
			Assert.AreEqual (1, passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.OutputFormat).Count (), "output format missing.");
			var outputFormat = passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.OutputFormat).First ();
			Assert.AreEqual (outputFormat.value, "xml"); // format
			if (extraData) {
				var extraDataArg = passedArguments.GetArguments ().Where (a => a.type == MlaunchArgumentType.ListExtraData).First ();
				Assert.IsNull (extraDataArg.value, "extra data takes no values");
			}
			Assert.AreEqual (2, devices.Connected64BitIOS.Count ());
			Assert.AreEqual (1, devices.Connected32BitIOS.Count());
			Assert.AreEqual (0, devices.ConnectedTV.Count ());
		}

	}
}
