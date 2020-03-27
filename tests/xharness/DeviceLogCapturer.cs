using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xharness.Execution;
using Xharness.Logging;
using Xharness.Utilities;

namespace Xharness
{
	public interface IDeviceLogCapturerFactory {
		IDeviceLogCapturer Create (ILog mainLog, ILog deviceLog, string deviceName);
	}

	public class DeviceLogCapturerFactory : IDeviceLogCapturerFactory {
		readonly IProcessManager processManager;
		readonly string xcodeRoot;
		readonly string mlaunchPath;

		public DeviceLogCapturerFactory (IProcessManager processManager, string xcodeRoot, string mlaunchPath)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.xcodeRoot = xcodeRoot ?? throw new ArgumentNullException (nameof (xcodeRoot));
			this.mlaunchPath = mlaunchPath ?? throw new ArgumentNullException (nameof (mlaunchPath));
		}

		public IDeviceLogCapturer Create (ILog mainLog, ILog deviceLog, string deviceName)
		{
			return new DeviceLogCapturer (processManager, mainLog, deviceLog, deviceName, xcodeRoot, mlaunchPath);
		}
	}

	public interface IDeviceLogCapturer {
		void StartCapture ();
		void StopCapture ();
	}

	public class DeviceLogCapturer : IDeviceLogCapturer {
		readonly IProcessManager processManager;
		readonly ILog mainLog;
		readonly ILog deviceLog;
		readonly string deviceName;
		readonly string xcodeRoot;
		readonly string mlaunchPath;

		public DeviceLogCapturer (IProcessManager processManager, ILog mainLog, ILog deviceLog, string deviceName, string xcodeRoot, string mlaunchPath)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.mainLog = mainLog ?? throw new ArgumentNullException (nameof (mainLog));
			this.deviceLog = deviceLog ?? throw new ArgumentNullException (nameof (deviceLog));
			this.deviceName = deviceName ?? throw new ArgumentNullException (nameof (deviceName));
			this.xcodeRoot = xcodeRoot ?? throw new ArgumentNullException (nameof (xcodeRoot));
			this.mlaunchPath = mlaunchPath ?? throw new ArgumentNullException (nameof (mlaunchPath));
		}

		Process process;
		CountdownEvent streamEnds;

		public void StartCapture ()
		{
			streamEnds = new CountdownEvent (2);

			var sb = new List<string> {
				"--logdev",
				"--sdkroot",
				xcodeRoot,
				"--devname",
				deviceName
			};

			process = new Process ();
			process.StartInfo.FileName = mlaunchPath;
			process.StartInfo.Arguments = StringUtils.FormatArguments (sb);
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

