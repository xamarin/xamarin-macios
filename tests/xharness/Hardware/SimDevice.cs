using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness.Hardware {

	public class SimDevice : ISimulatorDevice {
		public IProcessManager ProcessManager { get; set; } = new ProcessManager ();
		public ITCCDatabase TCCDatabase { get; set; } = new TCCDatabase ();


		public string UDID { get; set; }
		public string Name { get; set; }
		public string SimRuntime { get; set; }
		public string SimDeviceType { get; set; }
		public string DataPath { get; set; }
		public string LogPath { get; set; }
		public string SystemLog => Path.Combine (LogPath, "system.log");

		public IHarness Harness;

		public bool IsWatchSimulator => SimRuntime.StartsWith ("com.apple.CoreSimulator.SimRuntime.watchOS", StringComparison.Ordinal);

		public string OSVersion {
			get {
				var v = SimRuntime.Substring ("com.apple.CoreSimulator.SimRuntime.".Length);
				var dash = v.IndexOf ('-');
				return v.Substring (0, dash) + " " + v.Substring (dash + 1).Replace ('-', '.');
			}
		}

		public async Task EraseAsync (ILog log)
		{
			// here we don't care if execution fails.
			// erase the simulator (make sure the device isn't running first)
			await Harness.ExecuteXcodeCommandAsync ("simctl", new [] { "shutdown", UDID }, log, TimeSpan.FromMinutes (1));
			await Harness.ExecuteXcodeCommandAsync ("simctl", new [] { "erase", UDID }, log, TimeSpan.FromMinutes (1));

			// boot & shutdown to make sure it actually works
			await Harness.ExecuteXcodeCommandAsync ("simctl", new [] { "boot", UDID }, log, TimeSpan.FromMinutes (1));
			await Harness.ExecuteXcodeCommandAsync ("simctl", new [] { "shutdown", UDID }, log, TimeSpan.FromMinutes (1));
		}

		public async Task ShutdownAsync (ILog log)
		{
			await Harness.ExecuteXcodeCommandAsync ("simctl", new [] { "shutdown", UDID }, log, TimeSpan.FromMinutes (1));
		}

		public static async Task KillEverythingAsync (ILog log, IProcessManager processManager = null)
		{
			if (processManager == null)
				processManager = new ProcessManager ();
			await processManager.ExecuteCommandAsync ("launchctl", new [] { "remove", "com.apple.CoreSimulator.CoreSimulatorService" }, log, TimeSpan.FromSeconds (10));

			var to_kill = new string [] { "iPhone Simulator", "iOS Simulator", "Simulator", "Simulator (Watch)", "com.apple.CoreSimulator.CoreSimulatorService", "ibtoold" };

			var args = new List<string> ();
			args.Add ("-9");
			args.AddRange (to_kill);
			await processManager.ExecuteCommandAsync ("killall", args, log, TimeSpan.FromSeconds (10));

			foreach (var dir in new string [] {
				Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Saved Application State", "com.apple.watchsimulator.savedState"),
				Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Saved Application State", "com.apple.iphonesimulator.savedState"),
			}) {
				try {
					if (Directory.Exists (dir))
						Directory.Delete (dir, true);
				} catch (Exception e) {
					log.WriteLine ("Could not delete the directory '{0}': {1}", dir, e.Message);
				}
			}
		}

		async Task OpenSimulator (ILog log)
		{
			string simulator_app;

			if (IsWatchSimulator && Harness.XcodeVersion.Major < 9) {
				simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "Simulator (Watch).app");
			} else {
				simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "Simulator.app");
				if (!Directory.Exists (simulator_app))
					simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "iOS Simulator.app");
			}

			await ProcessManager.ExecuteCommandAsync ("open", new [] { "-a", simulator_app, "--args", "-CurrentDeviceUDID", UDID }, log, TimeSpan.FromSeconds (15));
		}

		public async Task PrepareSimulatorAsync (ILog log, params string [] bundle_identifiers)
		{
			// Kill all existing processes
			await KillEverythingAsync (log);

			// We shutdown and erase all simulators.
			await EraseAsync (log);

			// Edit the permissions to prevent dialog boxes in the test app
			var TCC_db = Path.Combine (DataPath, "data", "Library", "TCC", "TCC.db");
			if (!File.Exists (TCC_db)) {
				log.WriteLine ("Opening simulator to create TCC.db");
				await OpenSimulator (log);

				var tcc_creation_timeout = 60;
				var watch = new Stopwatch ();
				watch.Start ();
				while (!File.Exists (TCC_db) && watch.Elapsed.TotalSeconds < tcc_creation_timeout) {
					log.WriteLine ("Waiting for simulator to create TCC.db... {0}", (int)(tcc_creation_timeout - watch.Elapsed.TotalSeconds));
					await Task.Delay (TimeSpan.FromSeconds (0.250));
				}
			}

			if (File.Exists (TCC_db)) {
				await TCCDatabase.AgreeToPromptsAsync (SimRuntime, TCC_db, log, bundle_identifiers);
			} else {
				log.WriteLine ("No TCC.db found for the simulator {0} (SimRuntime={1} and SimDeviceType={1})", UDID, SimRuntime, SimDeviceType);
			}

			// Make sure we're in a clean state
			await KillEverythingAsync (log);

			// Make 100% sure we're shutdown
			await ShutdownAsync (log);
		}

	}
}
