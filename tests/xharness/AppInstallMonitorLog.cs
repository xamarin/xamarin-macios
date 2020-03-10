using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Xharness.Logging;

namespace Xharness {
	// Monitor the output from 'mlaunch --installdev' and cancel the installation if there's no output for 1 minute.
	class AppInstallMonitorLog : Log {
		public override string FullPath => copy_to.FullPath;

		readonly ILog copy_to;
		readonly CancellationTokenSource cancellation_source;
		
		public bool CopyingApp;
		public bool CopyingWatchApp;
		public TimeSpan AppCopyDuration;
		public TimeSpan WatchAppCopyDuration;
		public Stopwatch AppCopyStart = new Stopwatch ();
		public Stopwatch WatchAppCopyStart = new Stopwatch ();
		public int AppPercentComplete;
		public int WatchAppPercentComplete;
		public long AppBytes;
		public long WatchAppBytes;
		public long AppTotalBytes;
		public long WatchAppTotalBytes;

		public CancellationToken CancellationToken {
			get {
				return cancellation_source.Token;
			}
		}

		public AppInstallMonitorLog (ILog copy_to)
				: base (copy_to.Logs, $"Watch transfer log for {copy_to.Description}")
		{
			this.copy_to = copy_to;
			cancellation_source = new CancellationTokenSource ();
			cancellation_source.Token.Register (() => {
				copy_to.WriteLine ("App installation cancelled: it timed out after no output for 1 minute.");
			});
		}

		public override Encoding Encoding => copy_to.Encoding;
		public override void Flush ()
		{
			copy_to.Flush ();
		}

		public override StreamReader GetReader ()
		{
			return copy_to.GetReader ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			copy_to.Dispose ();
			cancellation_source.Dispose ();
		}

		void ResetTimer ()
		{
			cancellation_source.CancelAfter (TimeSpan.FromMinutes (1));
		}

		public override void WriteLine (string value)
		{
			var v = value.Trim ();
			if (v.StartsWith ("Installing application bundle", StringComparison.Ordinal)) {
				if (!CopyingApp) {
					CopyingApp = true;
					AppCopyStart.Start ();
				} else if (!CopyingWatchApp) {
					CopyingApp = false;
					CopyingWatchApp = true;
					AppCopyStart.Stop ();
					WatchAppCopyStart.Start ();
				}
			} else if (v.StartsWith ("PercentComplete: ", StringComparison.Ordinal) && int.TryParse (v.Substring ("PercentComplete: ".Length).Trim (), out var percent)) {
				if (CopyingApp)
					AppPercentComplete = percent;
				else if (CopyingWatchApp)
					WatchAppPercentComplete = percent;
			} else if (v.StartsWith ("NumBytes: ", StringComparison.Ordinal) && int.TryParse (v.Substring ("NumBytes: ".Length).Trim (), out var num_bytes)) {
				if (CopyingApp) {
					AppBytes = num_bytes;
					AppCopyDuration = AppCopyStart.Elapsed;
				} else if (CopyingWatchApp) {
					WatchAppBytes = num_bytes;
					WatchAppCopyDuration = WatchAppCopyStart.Elapsed;
				}
			} else if (v.StartsWith ("TotalBytes: ", StringComparison.Ordinal) && int.TryParse (v.Substring ("TotalBytes: ".Length).Trim (), out var total_bytes)) {
				if (CopyingApp)
					AppTotalBytes = total_bytes;
				else if (CopyingWatchApp)
					WatchAppTotalBytes = total_bytes;
			}

			ResetTimer ();

			copy_to.WriteLine (value);
		}

		public override void Write (byte [] buffer, int offset, int count)
		{
			copy_to.Write (buffer, offset, count);
		}
	}
}
