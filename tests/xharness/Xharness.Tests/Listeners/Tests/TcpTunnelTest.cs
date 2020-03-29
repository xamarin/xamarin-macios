using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Xharness.Execution;
using Xharness.Execution.Mlaunch;
using Xharness.Listeners;
using Xharness.Logging;

namespace Xharness.Tests.Listeners.Tests {
	[TestFixture]
	public class TcpTunnelTest {

		TcpTunnel tunnel;
		Mock<ITunnelListener> tunnelLister;
		Mock<IProcessManager> processManager;
		Mock<ILog> mainLog;
		readonly string device = "iPhone";
		readonly int port = 1234;

		[SetUp]
		public void SetUp ()
		{
			tunnelLister = new Mock<ITunnelListener> ();
			processManager = new Mock<IProcessManager> ();
			mainLog = new Mock<ILog> ();
			tunnel = new TcpTunnel (processManager.Object);
		}

		[TearDown]
		public void TearDown ()
		{
			tunnelLister = null;
			processManager = null;
		}

		[Test]
		public void ConstructorTest () =>
			Assert.Throws<ArgumentNullException> (() => new TcpTunnel (null));

		[Test]
		public void OpenNullArgumentsTest ()
		{
			Assert.Throws<ArgumentNullException> (() => tunnel.Open (null, tunnelLister.Object, TimeSpan.FromMinutes (2), mainLog.Object), "null device");
			Assert.Throws<ArgumentNullException> (() => tunnel.Open (device, null, TimeSpan.FromMinutes (2), mainLog.Object), "null listener");
			Assert.Throws<ArgumentNullException> (() => tunnel.Open (device, tunnelLister.Object, TimeSpan.FromMinutes (2), null), "null log");
		}

		[Test]
		public async Task OpenTest ()
		{
			var tunnelCompletionSoruce = new TaskCompletionSource<bool> ();
			var processExecutionCompletionSource = new TaskCompletionSource<ProcessExecutionResult> ();
			CallbackLog callbackLog = null;
			Func<ILog, bool> logCb = (l) => {
				// capture the log, return true
				callbackLog = l as CallbackLog;
				return callbackLog != null;
			};

			Func<MlaunchArguments, bool> argsCb = (args) => {
				bool correctTcpArg = false;
				bool correctDeviceArg = false;
				// validate we do have all the corret args
				foreach (var a in args) { 
					if (a is TcpTunnelArgument tcpTunnelArgument) {
						var arg = tcpTunnelArgument.AsCommandLineArgument ();
						correctTcpArg = arg.Contains ($"{port}:{port}");
					}
					if (a is DeviceNameArgument deviceNameArgument) {
						var arg = deviceNameArgument.AsCommandLineArgument ();
						correctDeviceArg = arg.Contains (device);
					}
				}
				return correctTcpArg && correctDeviceArg;
			};

			tunnelLister.Setup (t => t.Port).Returns (port);
			tunnelLister.Setup (t => t.TunnelHoleThrough).Returns (tunnelCompletionSoruce);
			// assert that we call the process manager to execute mlaunch with the correct parameters, fake a successful write
			// in the mainlogs that will start the tunnel and set the task to be completed and the tcp listener to be readu.
			processManager.Setup (p => p.ExecuteCommandAsync (
				It.Is<MlaunchArguments> (a => argsCb (a)),
				It.Is<ILog> (l => logCb (l)), 
				It.IsAny<TimeSpan> (), 
				It.IsAny<Dictionary<string, string>>(),
				It.IsAny<CancellationToken> ())).Returns (processExecutionCompletionSource.Task);

			// call open, this will ensure that we are calling the process manager correctly, we did capture the log
			// so we write on it to assert that the tunnel thinks it was correctly started and that the listener knows it
			// too
			tunnel.Open (device, tunnelLister.Object, TimeSpan.FromMinutes (2), mainLog.Object);

			// lie to the tcp object via the callback log
			Assert.NotNull (callbackLog, "callbackLog");
			callbackLog.WriteLine ("Tcp tunnel started on device");
			Assert.IsTrue (await tunnel.Started, "tunnel started");
			Assert.IsTrue (await tunnelCompletionSoruce.Task, "listener started");
		}

		[Test]
		public void CloseNotStartedTest () =>
			Assert.ThrowsAsync<InvalidOperationException> (() => tunnel.Close ());

	}
}
