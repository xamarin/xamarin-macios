using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Xharness.Execution;
using Xharness.Execution.Mlaunch;
using Xharness.Logging;
using Xharness.Utilities;

namespace Xharness.Tests {
	[TestFixture]
	public class CrashSnapshotReporterTests {
		readonly string mlaunchPath = "./mlaunch";
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

			tempXcodeRoot = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString());
			symbolicatePath = Path.Combine (tempXcodeRoot, "Contents", "SharedFrameworks", "DTDeviceKitBase.framework", "Versions", "A", "Resources");

			// Create fake place for device logs
			Directory.CreateDirectory (tempXcodeRoot);

			// Create fake symbolicate binary
			Directory.CreateDirectory (symbolicatePath);
			File.Create (Path.Combine (symbolicatePath, "symbolicatecrash"));
		}

		[TearDown]
		public void TearDown () {
			Directory.Delete (tempXcodeRoot, true);
		}

		[Test]
		public async Task DeviceCaptureTest ()
		{
			var tempFilePath = Path.GetTempFileName ();

			const string deviceName = "Sample-iPhone";
			const string crashLogPath = "/path/to/crash.log";
			const string symbolicateLogPath = "/path/to/" + deviceName+ ".symbolicated.log";
			
			var crashReport = Mock.Of<ILogFile> (x => x.Path == crashLogPath);
			var symbolicateReport = Mock.Of<ILogFile> (x => x.Path == symbolicateLogPath);

			// Crash report is added
			logs.Setup (x => x.Create (deviceName, "Crash report: " + deviceName, It.IsAny<bool> ()))
				.Returns (crashReport);
			
			// Symbolicate report is added
			logs.Setup (x => x.Create ("crash.symbolicated.log", "Symbolicated crash report: crash.log", It.IsAny<bool> ()))
				.Returns (symbolicateReport);

			// List of crash reports is retrieved
			processManager
				.Setup (x => x.ExecuteCommandAsync (
					mlaunchPath,
					It.Is<MlaunchArguments> (args => args.AsCommandLine () ==
						$"--list-crash-reports {StringUtils.FormatArguments( tempFilePath)} " +
						$"--sdkroot {StringUtils.FormatArguments (tempXcodeRoot)} " +
						$"--devname {StringUtils.FormatArguments (deviceName)}"),
					log.Object,
					TimeSpan.FromMinutes (1),
					null,
					null))
				.ReturnsAsync (new ProcessExecutionResult () { ExitCode = 0 });
			
			// Device crash log is downloaded
			processManager
				.Setup (x => x.ExecuteCommandAsync (
					mlaunchPath,
					It.Is<MlaunchArguments> (args => args.AsCommandLine () ==
						 $"--download-crash-report {StringUtils.FormatArguments (deviceName)} " +
						 $"--download-crash-report-to {StringUtils.FormatArguments (crashLogPath)} " +
						 $"--sdkroot {StringUtils.FormatArguments (tempXcodeRoot)} " +
						 $"--devname {StringUtils.FormatArguments (deviceName)}"),
					log.Object,
					TimeSpan.FromMinutes (1),
					null,
					null))
				.ReturnsAsync (new ProcessExecutionResult () { ExitCode = 0 });

			// Symbolicate is ran
			processManager
				.Setup (x => x.ExecuteCommandAsync (
					Path.Combine (symbolicatePath, "symbolicatecrash"),
					It.Is<IList<string>> (args => args.First () == crashLogPath),
					symbolicateReport,
					TimeSpan.FromMinutes (1),
					It.IsAny <Dictionary<string, string>>(),
					null))
				.ReturnsAsync (new ProcessExecutionResult () { ExitCode = 0 });

			// Act
			var snapshotReport = new CrashSnapshotReporter (processManager.Object,
				log.Object,
				logs.Object,
				tempXcodeRoot,
				mlaunchPath,
				true,
				deviceName,
				() => tempFilePath);

			File.WriteAllLines (tempFilePath, new [] { "crash 1", "crash 2" });

			await snapshotReport.StartCaptureAsync ();
			
			File.WriteAllLines (tempFilePath, new [] { "Sample-iPhone" });

			await snapshotReport.EndCaptureAsync (TimeSpan.FromSeconds (10));

			// Verify all calls above
			processManager.VerifyAll ();
			logs.VerifyAll ();
		}
	}
}
