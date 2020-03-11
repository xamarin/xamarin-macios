using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xharness.Hardware;

namespace Xharness.Jenkins.TestTasks
{
	class RunDeviceTask : RunXITask<IHardwareDevice>
	{
		AppInstallMonitorLog install_log;
		public override string ProgressMessage {
			get {
				var log = install_log;
				if (log == null)
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

		public RunDeviceTask (MSBuildTask build_task, IEnumerable<IHardwareDevice> candidates)
			: base (build_task, candidates.OrderBy ((v) => v.DebugSpeed))
		{
			switch (build_task.Platform) {
			case TestPlatform.iOS:
			case TestPlatform.iOS_Unified:
			case TestPlatform.iOS_Unified32:
			case TestPlatform.iOS_Unified64:
				AppRunnerTarget = AppRunnerTarget.Device_iOS;
				break;
			case TestPlatform.iOS_TodayExtension64:
				AppRunnerTarget = AppRunnerTarget.Device_iOS;
				break;
			case TestPlatform.tvOS:
				AppRunnerTarget = AppRunnerTarget.Device_tvOS;
				break;
			case TestPlatform.watchOS:
			case TestPlatform.watchOS_32:
			case TestPlatform.watchOS_64_32:
				AppRunnerTarget = AppRunnerTarget.Device_watchOS;
				break;
			default:
				throw new NotImplementedException ();
			}
		}

		protected override async Task RunTestAsync ()
		{
			Jenkins.MainLog.WriteLine ("Running '{0}' on device (candidates: '{1}')", ProjectFile, string.Join ("', '", Candidates.Select ((v) => v.Name).ToArray ()));

			var uninstall_log = Logs.Create ($"uninstall-{Timestamp}.log", "Uninstall log");
			using (var device_resource = await NotifyBlockingWaitAsync (Jenkins.GetDeviceResources (Candidates).AcquireAnyConcurrentAsync ())) {
				try {
					// Set the device we acquired.
					Device = Candidates.First ((d) => d.UDID == device_resource.Resource.Name);
					if (Device.DevicePlatform == DevicePlatform.watchOS)
						CompanionDevice = Jenkins.Devices.FindCompanionDevice (Jenkins.DeviceLoadLog, Device);
					Jenkins.MainLog.WriteLine ("Acquired device '{0}' for '{1}'", Device.Name, ProjectFile);

					runner = new AppRunner {
						Harness = Harness,
						ProjectFile = ProjectFile,
						Target = AppRunnerTarget,
						LogDirectory = LogDirectory,
						MainLog = uninstall_log,
						DeviceName = Device.Name,
						CompanionDeviceName = CompanionDevice?.Name,
						Configuration = ProjectConfiguration,
						TimeoutMultiplier = TimeoutMultiplier,
						Variation = Variation,
						BuildTask = BuildTask,
					};

					// Sometimes devices can't upgrade (depending on what has changed), so make sure to uninstall any existing apps first.
					if (Jenkins.UninstallTestApp) {
						runner.MainLog = uninstall_log;
						var uninstall_result = await runner.UninstallAsync ();
						if (!uninstall_result.Succeeded)
							MainLog.WriteLine ($"Pre-run uninstall failed, exit code: {uninstall_result.ExitCode} (this hopefully won't affect the test result)");
					} else {
						uninstall_log.WriteLine ($"Pre-run uninstall skipped.");
					}

					if (!Failed) {
						// Install the app
						this.install_log = new AppInstallMonitorLog (Logs.Create ($"install-{Timestamp}.log", "Install log"));
						try {
							runner.MainLog = this.install_log;
							var install_result = await runner.InstallAsync (install_log.CancellationToken);
							if (!install_result.Succeeded) {
								FailureMessage = $"Install failed, exit code: {install_result.ExitCode}.";
								ExecutionResult = TestExecutingResult.Failed;
								if (Harness.InCI)
									XmlResultParser.GenerateFailure (Logs, "install", runner.AppName, runner.Variation,
										$"AppInstallation on {runner.DeviceName}", $"Install failed on {runner.DeviceName}, exit code: {install_result.ExitCode}",
										install_log.FullPath, Harness.XmlJargon);
							}
						} finally {
							this.install_log.Dispose ();
							this.install_log = null;
						}
					}

					if (!Failed) {
						// Run the app
						runner.MainLog = Logs.Create ($"run-{Device.UDID}-{Timestamp}.log", "Run log");
						await runner.RunAsync ();

						if (!string.IsNullOrEmpty (runner.FailureMessage))
							FailureMessage = runner.FailureMessage;
						else if (runner.Result != TestExecutingResult.Succeeded)
							FailureMessage = GuessFailureReason (runner.MainLog);

						if (runner.Result == TestExecutingResult.Succeeded && Platform == TestPlatform.iOS_TodayExtension64) {
							// For the today extension, the main app is just a single test.
							// This is because running the today extension will not wake up the device,
							// nor will it close & reopen the today app (but launching the main app
							// will do both of these things, preparing the device for launching the today extension).

							AppRunner todayRunner = new AppRunner {
								Harness = Harness,
								ProjectFile = TestProject.GetTodayExtension ().Path,
								Target = AppRunnerTarget,
								LogDirectory = LogDirectory,
								MainLog = Logs.Create ($"extension-run-{Device.UDID}-{Timestamp}.log", "Extension run log"),
								DeviceName = Device.Name,
								CompanionDeviceName = CompanionDevice?.Name,
								Configuration = ProjectConfiguration,
								Variation = Variation,
								BuildTask = BuildTask,
							};
							additional_runner = todayRunner;
							await todayRunner.RunAsync ();
							foreach (var log in todayRunner.Logs.Where ((v) => !v.Description.StartsWith ("Extension ", StringComparison.Ordinal)))
								log.Description = "Extension " + log.Description [0].ToString ().ToLower () + log.Description.Substring (1);
							ExecutionResult = todayRunner.Result;

							if (!string.IsNullOrEmpty (todayRunner.FailureMessage))
								FailureMessage = todayRunner.FailureMessage;
						} else {
							ExecutionResult = runner.Result;
						}
					}
				} finally {
					// Uninstall again, so that we don't leave junk behind and fill up the device.
					if (Jenkins.UninstallTestApp) {
						runner.MainLog = uninstall_log;
						var uninstall_result = await runner.UninstallAsync ();
						if (!uninstall_result.Succeeded)
							MainLog.WriteLine ($"Post-run uninstall failed, exit code: {uninstall_result.ExitCode} (this won't affect the test result)");
					} else {
						uninstall_log.WriteLine ($"Post-run uninstall skipped.");
					}

					// Also clean up after us locally.
					if (Harness.InCI || Jenkins.CleanSuccessfulTestRuns && Succeeded)
						await BuildTask.CleanAsync ();
				}
			}
		}

		protected override string XIMode {
			get {
				return "device";
			}
		}
	}
}
