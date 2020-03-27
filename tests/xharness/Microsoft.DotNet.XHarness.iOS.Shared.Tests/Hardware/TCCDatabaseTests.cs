using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Hardware {

	[TestFixture]
	public class TCCDatabaseTests {

		Mock<IProcessManager> processManager;
		TCCDatabase database;
		Mock<ILog> executionLog;
		string simRuntime;
		string dataPath;

		[SetUp]
		public void SetUp ()
		{
			processManager = new Mock<IProcessManager> ();
			database = new TCCDatabase (processManager.Object);
			executionLog = new Mock<ILog> ();
			simRuntime = "com.apple.CoreSimulator.SimRuntime.iOS-12-1";
			dataPath = "/path/to/my/data";
		}

		[TearDown]
		public void TearDown ()
		{
			processManager = null;
			database = null;
		}

		[TestCase ("com.apple.CoreSimulator.SimRuntime.iOS-12-1", 3)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.iOS-10-1", 2)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.iOS-9-1", 2)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.iOS-7-1", 1)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.tvOS-12-3", 3)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.tvOS-8-1", 2)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.watchOS-5-1", 3)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.watchOS-4-1", 2)]
		public void GetTCCFormatTest (string runtime, int expected)
		{
			Assert.AreEqual (expected, database.GetTCCFormat (runtime));
		}

		[Test]
		public void GetTCCFormatUnknownTest ()
		{
			Assert.Throws<NotImplementedException> (() => database.GetTCCFormat ("unknown-sim-runtime"));
		}

		[Test]
		public async Task AgreeToPromptsAsyncNoIdentifiers ()
		{
			// we should write in the log that we did not managed to agree to it
			executionLog.Setup (l => l.WriteLine (It.IsAny<string> ()));
			await database.AgreeToPromptsAsync (simRuntime, dataPath, executionLog.Object);
			executionLog.Verify (l => l.WriteLine ("No bundle identifiers given when requested permission editing."));
		}

		[Test]
		public async Task AgreeToPropmtsAsyncTimeoutsTest ()
		{
			string processName = null;
			// set the process manager to always return a failure so that we do eventually get a timeout
			processManager.Setup (p => p.ExecuteCommandAsync (It.IsAny<string> (), It.IsAny<List<string>> (), It.IsAny<ILog> (), It.IsAny<TimeSpan> (), null, null))
				.Returns<string, IList<string>, ILog, TimeSpan, Dictionary<string, string>, CancellationToken?> ((p, a, l, t, e, c) => {
					processName = p;
					return Task.FromResult (new ProcessExecutionResult { ExitCode = 1, TimedOut = true });
				});
			// try to accept and fail because we always timeout
			await database.AgreeToPromptsAsync (simRuntime, dataPath, executionLog.Object, "my-bundle-id", "your-bundle-id");

			// verify that we did write in the logs and that we did call sqlite3
			Assert.AreEqual ("sqlite3", processName, "sqlite3 process");
			executionLog.Verify (l => l.WriteLine ("Failed to edit TCC.db, the test run might hang due to permission request dialogs"), Times.AtLeastOnce);
		}

		[TestCase ("com.apple.CoreSimulator.SimRuntime.iOS-12-1", 3)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.iOS-10-1", 2)]
		[TestCase ("com.apple.CoreSimulator.SimRuntime.iOS-7-1", 1)]
		public async Task AgreeToPropmtsAsyncSuccessTest (string runtime, int dbVersion)
		{
			string bundleIdentifier = "my-bundle-identifier";
			var services = new string [] {
					"kTCCServiceAddressBook",
					"kTCCServiceCalendar",
					"kTCCServicePhotos",
					"kTCCServiceMediaLibrary",
					"kTCCServiceMicrophone",
					"kTCCServiceUbiquity",
					"kTCCServiceWillow"
				};
			var expectedArgs = new StringBuilder ("\n");
			// assert the sql used depending on the version
			foreach (var id in new [] { bundleIdentifier, bundleIdentifier + ".watchkitapp" }) {
				switch (dbVersion) {
				case 1:
					foreach (var s in services) {
						expectedArgs.AppendFormat ("DELETE FROM access WHERE service = '{0}' AND client = '{1}';\n", s, id);
						expectedArgs.AppendFormat ("INSERT INTO access VALUES('{0}','{1}',0,1,0,NULL);\n", s, id);
					}
					break;
				case 2:
					foreach (var s in services) {
						expectedArgs.AppendFormat ("DELETE FROM access WHERE service = '{0}' AND client = '{1}';\n", s, id);
						expectedArgs.AppendFormat ("INSERT INTO access VALUES('{0}','{1}',0,1,0,NULL,NULL);\n", s, id);
					}
					break;
				case 3:
					foreach (var s in services) {
						expectedArgs.AppendFormat ("INSERT OR REPLACE INTO access VALUES('{0}','{1}',0,1,0,NULL,NULL,NULL,'UNUSED',NULL,NULL,{2});\n", s, id, DateTimeOffset.Now.ToUnixTimeSeconds ());
					}
					break;
				}
			}
			string processName = null;
			IList<string> args = new List<string> ();
			processManager.Setup (p => p.ExecuteCommandAsync (It.IsAny<string> (), It.IsAny<List<string>> (), It.IsAny<ILog> (), It.IsAny<TimeSpan> (), null, null))
				.Returns<string, IList<string>, ILog, TimeSpan, Dictionary<string, string>, CancellationToken?> ((p, a, l, t, e, c) => {
					processName = p;
					args = a;
					return Task.FromResult (new ProcessExecutionResult { ExitCode = 0, TimedOut = false });
				});

			await database.AgreeToPromptsAsync (runtime, dataPath, executionLog.Object, bundleIdentifier);

			Assert.AreEqual ("sqlite3", processName, "sqlite3 process");
			// assert that the sql is present
			Assert.That (args, Has.Member (dataPath));
			Assert.That (args, Has.Member (expectedArgs.ToString ()), "insert sql");
		}
	}
}
