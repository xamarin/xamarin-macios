using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;

namespace Xharness.Jenkins.TestTasks {
	class RunDeviceTask : RunXITask<IHardwareDevice>, IRunDeviceTask {
		public ITunnelBore TunnelBore { get; private set; }

		RunDevice runDevice;
		public override string ProgressMessage {
			get {
				var log = runDevice.InstallLog;
				if (log is null)
					return base.ProgressMessage;

				var percent_complete = log.CopyingApp ? log.AppPercentComplete : log.WatchAppPercentComplete;
				var bytes = log.CopyingApp ? log.AppBytes : log.WatchAppBytes;
				var total_bytes = log.CopyingApp ? log.AppTotalBytes : log.WatchAppTotalBytes;
				var elapsed = log.CopyingApp ? log.AppCopyDuration : log.WatchAppCopyDuration;
				var speed_bps = elapsed.Ticks == 0 ? -1 : bytes / elapsed.TotalSeconds;
				var estimated_left = TimeSpan.FromSeconds ((total_bytes - bytes) / speed_bps);
				var transfer_percent = 100 * (double) bytes / total_bytes;
				var str = log.CopyingApp ? "App" : "Watch App";
				var rv = $"{str} installation: {percent_complete}% done.\n" +
					$"\tApp size: {total_bytes:N0} bytes ({total_bytes / 1024.0 / 1024.0:N2} MB)\n" +
					$"\tTransferred: {bytes:N0} bytes ({bytes / 1024.0 / 1024.0:N2} MB)\n" +
					$"\tTransferred in {elapsed.TotalSeconds:#.#}s ({elapsed})\n" +
					$"\tTransfer speed: {speed_bps:N0} B/s ({speed_bps / 1024.0 / 1024.0:N} MB/s, {60 * speed_bps / 1024.0 / 1024.0:N2} MB/m)\n" +
					$"\tEstimated time left: {estimated_left.TotalSeconds:#.#}s ({estimated_left})";
				return rv;
			}
		}

		public RunDeviceTask (Jenkins jenkins, IHardwareDeviceLoader devices, MSBuildTask buildTask, IMlaunchProcessManager processManager, ITunnelBore tunnelBore, IErrorKnowledgeBase errorKnowledgeBase, bool useTcpTunnel, IEnumerable<IHardwareDevice> candidates)
			: base (jenkins, buildTask, processManager, candidates.OrderBy ((v) => v.DebugSpeed))
		{
			TunnelBore = tunnelBore;
			runDevice = new RunDevice (
				testTask: this,
				devices: devices,
				resourceManager: ResourceManager,
				mainLog: Jenkins.MainLog,
				deviceLoadLog: Jenkins.DeviceLoadLog,
				errorKnowledgeBase: errorKnowledgeBase,
				defaultLogDirectory: Jenkins.Harness.LogDirectory,
				uninstallTestApp: Jenkins.UninstallTestApp,
				cleanSuccessfulTestRuns: Jenkins.CleanSuccessfulTestRuns,
				generateXmlFailures: Jenkins.Harness.InCI,
				inCI: Jenkins.Harness.InCI,
				useTcpTunnel: useTcpTunnel,
				xmlResultJargon: Jenkins.Harness.XmlJargon
			);
		}

		public override Task RunTestAsync () => runDevice.RunTestAsync ();

		protected override string XIMode {
			get {
				return "device";
			}
		}
	}
}
