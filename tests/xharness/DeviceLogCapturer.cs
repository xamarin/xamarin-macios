using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xharness.Logging;
using Xharness.Utilities;

namespace Xharness
{
	public class DeviceLogCapturer
	{
		public Harness Harness;
		public ILog Log;
		public string DeviceName;

		Process process;
		CountdownEvent streamEnds;

		public void StartCapture ()
		{
			streamEnds = new CountdownEvent (2);

			process = new Process ();
			process.StartInfo.FileName = Harness.MlaunchPath;
			var sb = new List<string> ();
			sb.Add ("--logdev");
			sb.Add ("--sdkroot");
			sb.Add (Harness.XcodeRoot);
			AppRunner.AddDeviceName (sb, DeviceName);
			process.StartInfo.Arguments = StringUtils.FormatArguments (sb);
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

			Harness.ProcessManager.KillTreeAsync (process, Harness.HarnessLog, diagnostics: false).Wait ();
			process.Dispose ();
		}
	}
}

