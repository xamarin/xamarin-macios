using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests {
	[TestFixture]
	public class CrashReportSnapshotTests {

		string tempXcodeRoot;
		string symbolicatePath;

		Mock<IProcessManager> processManager;
		Mock<ILog> log;
		Mock<ILogs> logs;

		[SetUp]
		public void SetUp ()
		{
			processManager = new Mock<IProcessManager> ();
			log = new Mock<ILog> ();
			logs = new Mock<ILogs> ();

			tempXcodeRoot = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
			symbolicatePath = Path.Combine (tempXcodeRoot, "Contents", "SharedFrameworks", "DTDeviceKitBase.framework", "Versions", "A", "Resources");
			
			processManager.SetupGet (x => x.XcodeRoot).Returns (tempXcodeRoot);
			processManager.SetupGet (x => x.MlaunchPath).Returns ("/var/bin/mlaunch");

			// Create fake place for device logs
			Directory.CreateDirectory (tempXcodeRoot);

			// Create fake symbolicate binary
			Directory.CreateDirectory (symbolicatePath);
			File.WriteAllText (Path.Combine (symbolicatePath, "symbolicatecrash"), "");
		}

		[TearDown]
		public void TearDown ()
		{
			Directory.Delete (tempXcodeRoot, true);
		}

		[Test]
		public async Task DeviceCaptureTest ()
		{
			var tempFilePath = Path.GetTempFileName ();

			const string deviceName = "Sample-iPhone";
			const string crashLogPath = "/path/to/crash.log";
			const string symbolicateLogPath = "/path/to/" + deviceName + ".symbolicated.log";

			var crashReport = Mock.Of<ILog> (x => x.FullPath == crashLogPath);
			var symbolicateReport = Mock.Of<ILog> (x => x.FullPath == symbolicateLogPath);

			// Crash report is added
			logs.Setup (x => x.Create (deviceName, "Crash report: " + deviceName, It.IsAny<bool> ()))
				.Returns (crashReport);

			// Symbolicate report is added
			logs.Setup (x => x.Create ("crash.symbolicated.log", "Symbolicated crash report: crash.log", It.IsAny<bool> ()))
				.Returns (symbolicateReport);

			processManager.SetReturnsDefault (Task.FromResult (new ProcessExecutionResult () { ExitCode = 0 }));

			// Act
			var snapshotReport = new CrashSnapshotReporter (processManager.Object,
				log.Object,
				logs.Object,
				true,
				deviceName,
				() => tempFilePath);

			File.WriteAllLines (tempFilePath, new [] { "crash 1", "crash 2" });

			await snapshotReport.StartCaptureAsync ();

			File.WriteAllLines (tempFilePath, new [] { "Sample-iPhone" });

			await snapshotReport.EndCaptureAsync (TimeSpan.FromSeconds (10));

			// Verify
			logs.VerifyAll ();

			// List of crash reports is retrieved
			processManager.Verify (
				x => x.ExecuteCommandAsync (
					It.Is<MlaunchArguments> (args => args.AsCommandLine () ==
						StringUtils.FormatArguments (
							$"--list-crash-reports={tempFilePath}") + " " +
							$"--devname {StringUtils.FormatArguments (deviceName)}"),
					log.Object,
					TimeSpan.FromMinutes (1),
					null,
					null),
				Times.Exactly (2));

			// Device crash log is downloaded
			processManager.Verify (
				x => x.ExecuteCommandAsync (
					It.Is<MlaunchArguments> (args => args.AsCommandLine () ==
						 StringUtils.FormatArguments (
							 $"--download-crash-report={deviceName}") + " " +
							 StringUtils.FormatArguments ($"--download-crash-report-to={crashLogPath}") + " " +
							 $"--devname {StringUtils.FormatArguments (deviceName)}"),
					log.Object,
					TimeSpan.FromMinutes (1),
					null,
					null),
				Times.Once);

			// Symbolicate is ran
			processManager.Verify (
				x => x.ExecuteCommandAsync (
					Path.Combine (symbolicatePath, "symbolicatecrash"),
					It.Is<IList<string>> (args => args.First () == crashLogPath),
					symbolicateReport,
					TimeSpan.FromMinutes (1),
					It.IsAny<Dictionary<string, string>> (),
					null),
				Times.Once);
		}
	}
}
