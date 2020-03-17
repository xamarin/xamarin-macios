using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness
{
	public interface ICrashReportSnapshotFactory {
		ICrashReportSnapshot Create (ILog log, ILogs logs, bool isDevice, string deviceName);
	}

	public class CrashReportSnapshotFactory : ICrashReportSnapshotFactory {
		readonly IProcessManager processManager;
		readonly string xcodeRoot;
		readonly string mlaunchPath;

		public CrashReportSnapshotFactory (IProcessManager processManager, string xcodeRoot, string mlaunchPath)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.xcodeRoot = xcodeRoot ?? throw new ArgumentNullException (nameof (xcodeRoot));
			this.mlaunchPath = mlaunchPath ?? throw new ArgumentNullException (nameof (mlaunchPath));
		}

		public ICrashReportSnapshot Create (ILog log, ILogs logs, bool isDevice, string deviceName) =>
			new CrashReportSnapshot (processManager, log, logs, xcodeRoot, mlaunchPath, isDevice, deviceName);
	}

	public interface ICrashReportSnapshot {
		Task EndCaptureAsync (TimeSpan timeout);
		Task StartCaptureAsync ();
	}

	public class CrashReportSnapshot : ICrashReportSnapshot {
		readonly IProcessManager processManager;
		readonly ILog log;
		readonly ILogs logs;
		readonly string xcodeRoot;
		readonly string mlaunchPath;
		readonly bool isDevice;
		readonly string deviceName;

		HashSet<string> initialSet;

		public CrashReportSnapshot (IProcessManager processManager,
								    ILog log,
								    ILogs logs,
								    string xcodeRoot,
								    string mlaunchPath,
								    bool isDevice,
								    string deviceName)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.log = log ?? throw new ArgumentNullException (nameof (log));
			this.logs = logs ?? throw new ArgumentNullException (nameof (logs));
			this.xcodeRoot = xcodeRoot ?? throw new ArgumentNullException (nameof (xcodeRoot));
			this.mlaunchPath = mlaunchPath ?? throw new ArgumentNullException (nameof (mlaunchPath));
			this.isDevice = isDevice;
			this.deviceName = deviceName;
		}

		public async Task StartCaptureAsync ()
		{
			initialSet = await CreateCrashReportsSnapshotAsync ();
		}

		public async Task EndCaptureAsync (TimeSpan timeout)
		{
			// Check for crash reports
			var crash_report_search_done = false;
			var crash_report_search_timeout = timeout.TotalSeconds;
			var watch = new Stopwatch ();
			watch.Start ();
			do {
				var end_crashes = await CreateCrashReportsSnapshotAsync ();
				end_crashes.ExceptWith (initialSet);

				if (end_crashes.Count == 0) {
					if (watch.Elapsed.TotalSeconds > crash_report_search_timeout) {
						crash_report_search_done = true;
					} else {
						log.WriteLine ("No crash reports, waiting a second to see if the crash report service just didn't complete in time ({0})", (int) (crash_report_search_timeout - watch.Elapsed.TotalSeconds));
						Thread.Sleep (TimeSpan.FromSeconds (1));
					}

					continue;
				}

				log.WriteLine ("Found {0} new crash report(s)", end_crashes.Count);
				List<ILogFile> crash_reports;
				if (!isDevice) {
					crash_reports = new List<ILogFile> (end_crashes.Count);
					foreach (var path in end_crashes) {
						logs.AddFile (path, $"Crash report: {Path.GetFileName (path)}");
					}
				} else {
					// Download crash reports from the device. We put them in the project directory so that they're automatically deleted on wrench
					// (if we put them in /tmp, they'd never be deleted).
					var downloaded_crash_reports = new List<ILogFile> ();
					foreach (var file in end_crashes) {
						var name = Path.GetFileName (file);
						var crash_report_target = logs.Create (name, $"Crash report: {name}", timestamp: false);
						var sb = new List<string> ();
						sb.Add ($"--download-crash-report={file}");
						sb.Add ($"--download-crash-report-to={crash_report_target.Path}");
						sb.Add ("--sdkroot");
						sb.Add (xcodeRoot);
						if (!string.IsNullOrEmpty (deviceName)) {
							sb.Add ("--devname");
							sb.Add (deviceName);
						}
						var result = await processManager.ExecuteCommandAsync (mlaunchPath, sb, log, TimeSpan.FromMinutes (1));
						if (result.Succeeded) {
							log.WriteLine ("Downloaded crash report {0} to {1}", file, crash_report_target.Path);
							crash_report_target = await SymbolicateCrashReportAsync (crash_report_target);
							downloaded_crash_reports.Add (crash_report_target);
						} else {
							log.WriteLine ("Could not download crash report {0}", file);
						}
					}
					crash_reports = downloaded_crash_reports;
				}

				foreach (var cp in crash_reports) {
					WrenchLog.WriteLine ("AddFile: {0}", cp.Path);
					log.WriteLine ("    {0}", cp.Path);
				}
				crash_report_search_done = true;
			} while (!crash_report_search_done);
		}

		async Task<ILogFile> SymbolicateCrashReportAsync (ILogFile report)
		{
			var symbolicatecrash = Path.Combine (xcodeRoot, "Contents/SharedFrameworks/DTDeviceKitBase.framework/Versions/A/Resources/symbolicatecrash");
			if (!File.Exists (symbolicatecrash))
				symbolicatecrash = Path.Combine (xcodeRoot, "Contents/SharedFrameworks/DVTFoundation.framework/Versions/A/Resources/symbolicatecrash");

			if (!File.Exists (symbolicatecrash)) {
				log.WriteLine ("Can't symbolicate {0} because the symbolicatecrash script {1} does not exist", report.Path, symbolicatecrash);
				return report;
			}

			var name = Path.GetFileName (report.Path);
			var symbolicated = logs.Create (Path.ChangeExtension (name, ".symbolicated.log"), $"Symbolicated crash report: {name}", timestamp: false);
			var environment = new Dictionary<string, string> { { "DEVELOPER_DIR", Path.Combine (xcodeRoot, "Contents", "Developer") } };
			var rv = await processManager.ExecuteCommandAsync (symbolicatecrash, new [] { report.Path }, symbolicated, TimeSpan.FromMinutes (1), environment);
			if (rv.Succeeded) {
				log.WriteLine ("Symbolicated {0} successfully.", report.Path);
				return symbolicated;
			} else {
				log.WriteLine ("Failed to symbolicate {0}.", report.Path);
				return report;
			}
		}

		async Task<HashSet<string>> CreateCrashReportsSnapshotAsync ()
		{
			var rv = new HashSet<string> ();

			if (!isDevice) {
				var dir = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "Library", "Logs", "DiagnosticReports");
				if (Directory.Exists (dir))
					rv.UnionWith (Directory.EnumerateFiles (dir));
			} else {
				var tmp = Path.GetTempFileName ();
				try {
					var sb = new List<string> ();
					sb.Add ($"--list-crash-reports={tmp}");
					sb.Add ("--sdkroot");
					sb.Add (xcodeRoot);
					if (!string.IsNullOrEmpty (deviceName)) {
						sb.Add ("--devname");
						sb.Add (deviceName);
					}
					var result = await processManager.ExecuteCommandAsync (mlaunchPath, sb, log, TimeSpan.FromMinutes (1));
					if (result.Succeeded)
						rv.UnionWith (File.ReadAllLines (tmp));
				} finally {
					File.Delete (tmp);
				}
			}

			return rv;
		}
	}
}
