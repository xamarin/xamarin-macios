using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Xamarin.Utils;

namespace xharness
{
	public class DeviceLogCapturer
	{
		public Harness Harness;
		public Log Log;
		public string DeviceName;

		Process process;
		CountdownEvent streamEnds;

		public void StartCapture ()
		{
			streamEnds = new CountdownEvent (2);

			process = new Process ();
			process.StartInfo.FileName = Harness.MlaunchPath;
			var sb = new StringBuilder ();
			sb.Append ("--logdev ");
			sb.Append ("--sdkroot ").Append (StringUtils.Quote (Harness.XcodeRoot)).Append (' ');
			AppRunner.AddDeviceName (sb, DeviceName);
			process.StartInfo.Arguments = sb.ToString ();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardInput = true;
			process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
				if (e.Data == null) {
					streamEnds.Signal ();
				} else {
					lock (Log) {
						Log.WriteLine (e.Data);
					}
				}
			};
			process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				if (e.Data == null) {
					streamEnds.Signal ();
				} else {
					lock (Log) {
						Log.WriteLine (e.Data);
					}
				}
			};
			Log.WriteLine ("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
			process.Start ();
			process.BeginOutputReadLine ();
			process.BeginErrorReadLine ();
		}

		public void StopCapture ()
		{
			if (process.HasExited)
				return;

			process.StandardInput.WriteLine ();
			if (process.WaitForExit ((int) TimeSpan.FromSeconds (5).TotalMilliseconds))
				return;

			process.KillTreeAsync (Harness.HarnessLog, diagnostics: false).Wait ();
			process.Dispose ();
		}
	}
}

