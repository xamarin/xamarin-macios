using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared {
	public interface ICrashSnapshotReporter {
		Task EndCaptureAsync (TimeSpan timeout);
		Task StartCaptureAsync ();
	}

	public class CrashSnapshotReporter : ICrashSnapshotReporter {
		readonly IProcessManager processManager;
		readonly ILog log;
		readonly ILogs logs;
		readonly bool isDevice;
		readonly string deviceName;
		readonly Func<string> tempFileProvider;
		readonly string symbolicateCrashPath;

		HashSet<string> initialCrashes;

		public CrashSnapshotReporter (IProcessManager processManager,
			ILog log,
			ILogs logs,
			bool isDevice,
			string deviceName,
			Func<string> tempFileProvider = null)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.log = log ?? throw new ArgumentNullException (nameof (log));
			this.logs = logs ?? throw new ArgumentNullException (nameof (logs));
			this.isDevice = isDevice;
			this.deviceName = deviceName;
			this.tempFileProvider = tempFileProvider ?? Path.GetTempFileName;

			symbolicateCrashPath = Path.Combine (processManager.XcodeRoot, "Contents", "SharedFrameworks", "DTDeviceKitBase.framework", "Versions", "A", "Resources", "symbolicatecrash");
			if (!File.Exists (symbolicateCrashPath))
				symbolicateCrashPath = Path.Combine (processManager.XcodeRoot, "Contents", "SharedFrameworks", "DVTFoundation.framework", "Versions", "A", "Resources", "symbolicatecrash");
			if (!File.Exists (symbolicateCrashPath))
				symbolicateCrashPath = null;
		}

		public async Task StartCaptureAsync ()
		{
			initialCrashes = await CreateCrashReportsSnapshotAsync ();
		}

		public async Task EndCaptureAsync (TimeSpan timeout)
		{
			// Check for crash reports
			var stopwatch = Stopwatch.StartNew ();

			do {
				var newCrashFiles = await CreateCrashReportsSnapshotAsync ();
				newCrashFiles.ExceptWith (initialCrashes);

				if (newCrashFiles.Count == 0) {
					if (stopwatch.Elapsed.TotalSeconds > timeout.TotalSeconds) {
						break;
					} else {
						log.WriteLine (
							"No crash reports, waiting a second to see if the crash report service just didn't complete in time ({0})",
							(int) (timeout.TotalSeconds - stopwatch.Elapsed.TotalSeconds));

						Thread.Sleep (TimeSpan.FromSeconds (1));
					}

					continue;
				}

				log.WriteLine ("Found {0} new crash report(s)", newCrashFiles.Count);

				IEnumerable<ILog> crashReports;
				if (!isDevice) {
					crashReports = new List<ILog> (newCrashFiles.Count);
					foreach (var path in newCrashFiles) {
						logs.AddFile (path, $"Crash report: {Path.GetFileName (path)}");
					}
				} else {
					// Download crash reports from the device. We put them in the project directory so that they're automatically deleted on wrench
					// (if we put them in /tmp, they'd never be deleted).
					crashReports = newCrashFiles
						.Select (async crash => await ProcessCrash (crash))
						.Select (t => t.Result)
						.Where (c => c != null);
				}

				foreach (var cp in crashReports) {
					WrenchLog.WriteLine ("AddFile: {0}", cp.FullPath);
					log.WriteLine ("    {0}", cp.FullPath);
				}

				break;

			} while (true);
		}

		async Task<ILog> ProcessCrash (string crashFile)
		{
			var name = Path.GetFileName (crashFile);
			var crashReportFile = logs.Create (name, $"Crash report: {name}", timestamp: false);
			var args = new MlaunchArguments (
				new DownloadCrashReportArgument (crashFile),
				new DownloadCrashReportToArgument (crashReportFile.FullPath));

			if (!string.IsNullOrEmpty (deviceName)) args.Add (new DeviceNameArgument (deviceName));

			var result = await processManager.ExecuteCommandAsync (args, log, TimeSpan.FromMinutes (1));

			if (result.Succeeded) {
				log.WriteLine ("Downloaded crash report {0} to {1}", crashFile, crashReportFile.FullPath);
				return await GetSymbolicateCrashReportAsync (crashReportFile);
			} else {
				log.WriteLine ("Could not download crash report {0}", crashFile);
				return null;
			}
		}

		async Task<ILog> GetSymbolicateCrashReportAsync (ILog report)
		{
			if (symbolicateCrashPath == null) {
				log.WriteLine ("Can't symbolicate {0} because the symbolicatecrash script {1} does not exist", report.FullPath, symbolicateCrashPath);
				return report;
			}

			var name = Path.GetFileName (report.FullPath);
			var symbolicated = logs.Create (Path.ChangeExtension (name, ".symbolicated.log"), $"Symbolicated crash report: {name}", timestamp: false);
			var environment = new Dictionary<string, string> { { "DEVELOPER_DIR", Path.Combine (processManager.XcodeRoot, "Contents", "Developer") } };
			var result = await processManager.ExecuteCommandAsync (symbolicateCrashPath, new [] { report.FullPath }, symbolicated, TimeSpan.FromMinutes (1), environment);
			if (result.Succeeded) {
				log.WriteLine ("Symbolicated {0} successfully.", report.FullPath);
				return symbolicated;
			} else {
				log.WriteLine ("Failed to symbolicate {0}.", report.FullPath);
				return report;
			}
		}

		async Task<HashSet<string>> CreateCrashReportsSnapshotAsync ()
		{
			var crashes = new HashSet<string> ();

			if (!isDevice) {
				var dir = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "Library", "Logs", "DiagnosticReports");
				if (Directory.Exists (dir))
					crashes.UnionWith (Directory.EnumerateFiles (dir));
			} else {
				var tempFile = tempFileProvider ();
				try {
					var args = new MlaunchArguments (new ListCrashReportsArgument (tempFile));

					if (!string.IsNullOrEmpty (deviceName)) args.Add (new DeviceNameArgument (deviceName));

					var result = await processManager.ExecuteCommandAsync (args, log, TimeSpan.FromMinutes (1));
					if (result.Succeeded)
						crashes.UnionWith (File.ReadAllLines (tempFile));
				} finally {
					File.Delete (tempFile);
				}
			}

			return crashes;
		}
	}
}
