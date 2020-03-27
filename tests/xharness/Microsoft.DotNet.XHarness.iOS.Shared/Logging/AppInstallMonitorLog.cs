using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	// Monitor the output from 'mlaunch --installdev' and cancel the installation if there's no output for 1 minute.
	public class AppInstallMonitorLog : Log {

		readonly ILog copy_to;
		readonly CancellationTokenSource cancellationSource;

		public override string FullPath => copy_to.FullPath;

		public CancellationToken CancellationToken => cancellationSource.Token;

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

		public AppInstallMonitorLog (ILog copy_to)
				: base ($"Watch transfer log for {copy_to.Description}")
		{
			this.copy_to = copy_to;
			cancellationSource = new CancellationTokenSource ();
			cancellationSource.Token.Register (() => {
				copy_to.WriteLine ("App installation cancelled: it timed out after no output for 1 minute.");
			});
		}

		public override Encoding Encoding => copy_to.Encoding;

		public override void Flush ()
		{
			copy_to.Flush ();
		}

		public override StreamReader GetReader () => copy_to.GetReader ();

		public override void Dispose ()
		{
			copy_to.Dispose ();
			cancellationSource.Dispose ();
		}

		void ResetTimer ()
		{
			cancellationSource.CancelAfter (TimeSpan.FromMinutes (1));
		}

		protected override void WriteImpl (string value)
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
