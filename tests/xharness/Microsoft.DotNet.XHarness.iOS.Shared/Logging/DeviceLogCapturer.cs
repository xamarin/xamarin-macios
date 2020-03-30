using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	public interface IDeviceLogCapturer {
		void StartCapture ();
		void StopCapture ();
	}

	public class DeviceLogCapturer : IDeviceLogCapturer {
		readonly IProcessManager processManager;
		readonly ILog mainLog;
		readonly ILog deviceLog;
		readonly string deviceName;

		public DeviceLogCapturer (IProcessManager processManager, ILog mainLog, ILog deviceLog, string deviceName)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.mainLog = mainLog ?? throw new ArgumentNullException (nameof (mainLog));
			this.deviceLog = deviceLog ?? throw new ArgumentNullException (nameof (deviceLog));
			this.deviceName = deviceName ?? throw new ArgumentNullException (nameof (deviceName));
		}

		Process process;
		CountdownEvent streamEnds;

		public void StartCapture ()
		{
			streamEnds = new CountdownEvent (2);

			var args = new List<string> {
				"--logdev",
				"--sdkroot",
				processManager.XcodeRoot,
				"--devname",
				deviceName
			};

			process = new Process ();
			process.StartInfo.FileName = processManager.MlaunchPath;
			process.StartInfo.Arguments = StringUtils.FormatArguments (args);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardInput = true;
			process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
				if (e.Data == null) {
					streamEnds.Signal ();
				} else {
					lock (deviceLog) {
						deviceLog.WriteLine (e.Data);
					}
				}
			};
			process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				if (e.Data == null) {
					streamEnds.Signal ();
				} else {
					lock (deviceLog) {
						deviceLog.WriteLine (e.Data);
					}
				}
			};
			deviceLog.WriteLine ("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
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

			processManager.KillTreeAsync (process, mainLog, diagnostics: false).Wait ();
			process.Dispose ();
		}
	}
}

