using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace xharness
{
	public class DeviceLogCapturer
	{
		public Harness Harness;
		public string LogPath;
		public string DeviceName;

		StreamWriter writer;
		Process process;
		CountdownEvent streamEnds;

		public void StartCapture ()
		{
			writer = new StreamWriter (new FileStream (LogPath, FileMode.Create));
			streamEnds = new CountdownEvent (2);

			process = new Process ();
			process.StartInfo.FileName = Harness.MlaunchPath;
			var sb = new StringBuilder ();
			sb.Append ("--logdev ");
			sb.Append ("--sdkroot ").Append (Harness.Quote (Harness.XcodeRoot)).Append (' ');
			AppRunner.AddDeviceName (sb, DeviceName);
			process.StartInfo.Arguments = sb.ToString ();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
				if (e.Data == null) {
					streamEnds.Signal ();
				} else {
					lock (writer) {
						writer.WriteLine (e.Data);
					}
				}
			};
			process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				if (e.Data == null) {
					streamEnds.Signal ();
				} else {
					lock (writer) {
						writer.WriteLine (e.Data);
					}
				}
			};
			Harness.Log (1, "{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
			process.Start ();
			process.BeginOutputReadLine ();
			process.BeginErrorReadLine ();
		}

		public void StopCapture ()
		{
			process.Kill ();
			if (!streamEnds.Wait (TimeSpan.FromSeconds (5))) {
				Harness.Log ("Could not kill 'mtouch --logdev' process in 5 seconds.");
			}
			process.Dispose ();
			writer.Flush ();
			writer.Dispose ();
		}
	}
}

