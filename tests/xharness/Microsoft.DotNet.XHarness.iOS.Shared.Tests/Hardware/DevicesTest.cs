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
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Hardware {

	[TestFixture]
	public class DevicesTest {

		Devices devices;
		string mlaunchPath;
		string sdkPath;
		Mock<IProcessManager> processManager;
		Mock<ILog> executionLog;

		[SetUp]
		public void SetUp ()
		{
			processManager = new Mock<IProcessManager> ();
			devices = new Devices (processManager.Object);
			executionLog = new Mock<ILog> ();
			mlaunchPath = "/usr/bin/mlaunch"; // any will be ok, is mocked
			sdkPath = "/Applications/Xcode.app";
		}

		[TearDown]
		public void TearDown ()
		{
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
				await devices.LoadAsync (executionLog.Object);
			});

			MlaunchArgument sdkRootArg = passedArguments.Where (a => a is SdkRootArgument).FirstOrDefault ();
			Assert.IsNotNull (sdkRootArg, "sdk arg missing");
			AssertArgumentValue (sdkRootArg, $"--sdkroot {sdkPath}", "sdk arg wrong");

			MlaunchArgument listDevArg = passedArguments.Where (a => a is ListDevicesArgument).FirstOrDefault ();
			Assert.IsNotNull (listDevArg, "list devices arg missing");

			MlaunchArgument outputFormatArg = passedArguments.Where (a => a is XmlOutputFormatArgument).FirstOrDefault ();
			Assert.IsNotNull (outputFormatArg, "output format arg missing");
		}

		[TestCase (true)]
		[TestCase (false)]
		public async Task LoadAsyncProcessSuccess (bool extraData)
		{
			string processPath = null;
			MlaunchArguments passedArguments = null;

			// moq It.Is is not working as nicelly as we would like it, we capture data and use asserts
			processManager.Setup (p => p.RunAsync (It.IsAny<Process> (), It.IsAny<MlaunchArguments> (), It.IsAny<ILog> (), It.IsAny<TimeSpan?> (), It.IsAny<Dictionary<string, string>> (), It.IsAny<CancellationToken?> (), It.IsAny<bool?> ()))
				.Returns<Process, MlaunchArguments, ILog, TimeSpan?, Dictionary<string, string>, CancellationToken?, bool?> ((p, args, log, t, env, token, d) => {
					processPath = p.StartInfo.FileName;
					passedArguments = args;

					// we get the temp file that was passed as the args, and write our sample xml, which will be parsed to get the devices :)
					var tempPath = args.Where (a => a is ListDevicesArgument).First ().AsCommandLineArgument ();
					tempPath = tempPath.Substring (tempPath.IndexOf('=') + 1).Replace ("\"", string.Empty);

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

			MlaunchArgument sdkRootArg = passedArguments.Where (a => a is SdkRootArgument).FirstOrDefault ();
			Assert.IsNotNull (sdkRootArg, "sdk arg missing");
			AssertArgumentValue (sdkRootArg, $"--sdkroot {sdkPath}", "sdk arg wrong");

			MlaunchArgument listDevArg = passedArguments.Where (a => a is ListDevicesArgument).FirstOrDefault ();
			Assert.IsNotNull (listDevArg, "list devices arg missing");

			MlaunchArgument outputFormatArg = passedArguments.Where (a => a is XmlOutputFormatArgument).FirstOrDefault ();
			Assert.IsNotNull (outputFormatArg, "output format arg missing");

			if (extraData) {
				MlaunchArgument listExtraDataArg = passedArguments.Where (a => a is ListExtraDataArgument).FirstOrDefault ();
				Assert.IsNotNull (listExtraDataArg, "list extra data arg missing");
			}

			Assert.AreEqual (2, devices.Connected64BitIOS.Count ());
			Assert.AreEqual (1, devices.Connected32BitIOS.Count ());
			Assert.AreEqual (0, devices.ConnectedTV.Count ());
		}

		private void AssertArgumentValue (MlaunchArgument arg, string expected, string message = null)
		{
			var value = arg.AsCommandLineArgument ().Split (new char [] { '=' }, 2).LastOrDefault ();
			Assert.AreEqual (expected, value, message);
		}
	}
}
