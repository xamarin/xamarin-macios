using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Logging;

namespace Xharness
{
	public class CrashReportSnapshot
	{
		public Harness Harness { get; set; }
		public ILog Log { get; set; }
		public ILogs Logs { get; set; }
		public string LogDirectory { get; set; }
		public bool Device { get; set; }
		public string DeviceName { get; set; }

		public HashSet<string> InitialSet { get; private set; }
		public IEnumerable<string> Reports { get; private set; }

		public async Task StartCaptureAsync ()
		{
			InitialSet = await Harness.CreateCrashReportsSnapshotAsync (Log, !Device, DeviceName);
		}

		public async Task EndCaptureAsync (TimeSpan timeout)
		{
			// Check for crash reports
			var crash_report_search_done = false;
			var crash_report_search_timeout = timeout.TotalSeconds;
			var watch = new Stopwatch ();
			watch.Start ();
			do {
				var end_crashes = await Harness.CreateCrashReportsSnapshotAsync (Log, !Device, DeviceName);
				end_crashes.ExceptWith (InitialSet);
				Reports = end_crashes;
				if (end_crashes.Count > 0) {
					Log.WriteLine ("Found {0} new crash report(s)", end_crashes.Count);
					List<ILogFile> crash_reports;
					if (!Device) {
						crash_reports = new List<ILogFile> (end_crashes.Count);
						foreach (var path in end_crashes) {
							Logs.AddFile (path, $"Crash report: {Path.GetFileName (path)}");
						}
					} else {
						// Download crash reports from the device. We put them in the project directory so that they're automatically deleted on wrench
						// (if we put them in /tmp, they'd never be deleted).
						var downloaded_crash_reports = new List<ILogFile> ();
						foreach (var file in end_crashes) {
							var name = Path.GetFileName (file);
							var crash_report_target = Logs.Create (name, $"Crash report: {name}", timestamp: false);
							var sb = new List<string> ();
							sb.Add ($"--download-crash-report={file}");
							sb.Add ($"--download-crash-report-to={crash_report_target.Path}");
							sb.Add ("--sdkroot");
							sb.Add (Harness.XcodeRoot);
							if (!string.IsNullOrEmpty (DeviceName)) {
								sb.Add ("--devname");
								sb.Add (DeviceName);
							}
							var result = await Harness.ProcessManager.ExecuteCommandAsync (Harness.MlaunchPath, sb, Log, TimeSpan.FromMinutes (1));
							if (result.Succeeded) {
								Log.WriteLine ("Downloaded crash report {0} to {1}", file, crash_report_target.Path);
								crash_report_target = await Harness.SymbolicateCrashReportAsync (Logs, Log, crash_report_target);
								downloaded_crash_reports.Add (crash_report_target);
							} else {
								Log.WriteLine ("Could not download crash report {0}", file);
							}
						}
						crash_reports = downloaded_crash_reports;
					}
					foreach (var cp in crash_reports) {
						Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", cp.Path);
						Log.WriteLine ("    {0}", cp.Path);
					}
					crash_report_search_done = true;
				} else {
					if (watch.Elapsed.TotalSeconds > crash_report_search_timeout) {
						crash_report_search_done = true;
					} else {
						Log.WriteLine ("No crash reports, waiting a second to see if the crash report service just didn't complete in time ({0})", (int) (crash_report_search_timeout - watch.Elapsed.TotalSeconds));
						Thread.Sleep (TimeSpan.FromSeconds (1));
					}
				}
			} while (!crash_report_search_done);
		}
	}
}
