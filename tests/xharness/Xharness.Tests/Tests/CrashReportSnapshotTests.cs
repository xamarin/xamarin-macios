using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness.Tests {
	[TestFixture]
	public class CrashReportSnapshotTests {
		const string xcodeRoot = "/path/to/xcode";
		const string mlaunchPath = "/path/to/mlaunch";
		
		Mock<IProcessManager> processManager;
		Mock<ILog> log;
		Mock<ILogs> logs;

		[SetUp]
		public void SetUp ()
		{
			processManager = new Mock<IProcessManager> ();
			log = new Mock<ILog> ();
			logs = new Mock<ILogs> ();
		}

		[Test]
		public async Task DeviceCaptureTest ()
		{
			var snapshotReport = new CrashReportSnapshot (processManager.Object,
				log.Object,
				logs.Object,
				xcodeRoot,
				mlaunchPath,
				true,
				"Sample iPhone");

			await snapshotReport.StartCaptureAsync ();

			// mlaunch "--list-crash-reports=C:\\Users\\prvysoky\\AppData\\Local\\Temp\\tmp43C3.tmp", "--sdkroot", "/path/to/xcode", "--devname", "Sample iPhone"

			await snapshotReport.EndCaptureAsync (TimeSpan.FromSeconds (10));
		}
	}
}
