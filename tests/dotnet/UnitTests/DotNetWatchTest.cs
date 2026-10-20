// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Utils;

namespace Xamarin.Tests {
	[TestFixture]
	public class DotNetWatchTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		[TestCase (ApplePlatform.MacOSX, true)]
		public void DotNetWatch (ApplePlatform platform, bool enableSandbox)
		{
			DotNetWatchImpl (platform, enableSandbox, usePhysicalDevice: false);
		}

		// This test is opt-in: set the DEVICE environment variable to the name of a connected device.
		[Test]
		[TestCase (ApplePlatform.iOS)]
		public void DotNetWatchDeviceUsb (ApplePlatform platform)
		{
			var deviceName = Environment.GetEnvironmentVariable ("DEVICE");
			if (string.IsNullOrEmpty (deviceName))
				Assert.Inconclusive ("Set the DEVICE environment variable to a connected device name to run this test.");

			DotNetWatchImpl (platform, enableSandbox: false, usePhysicalDevice: true, connectionMode: "usb");
		}

		// This test is opt-in: set the DEVICE environment variable to the name of a connected device.
		// The device must also be reachable from the mac over the network (e.g. on the same wifi), since
		// the app connects back to mlaunch over the network for hot reload in this scenario.
		[Test]
		[TestCase (ApplePlatform.iOS)]
		public void DotNetWatchDeviceWifi (ApplePlatform platform)
		{
			var deviceName = Environment.GetEnvironmentVariable ("DEVICE");
			if (string.IsNullOrEmpty (deviceName))
				Assert.Inconclusive ("Set the DEVICE environment variable to a connected device name to run this test.");

			DotNetWatchImpl (platform, enableSandbox: false, usePhysicalDevice: true, connectionMode: "wifi");
		}

		void DotNetWatchImpl (ApplePlatform platform, bool enableSandbox, bool usePhysicalDevice, string connectionMode = "usb")
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var projectPath = GetProjectPath ("HotReloadTestApp", platform: platform);
			var projectDirectory = Path.GetDirectoryName (projectPath)!;

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var additionalFile = Path.Combine (tmpdir, "AdditionalFile.cs");

			// Debug logging is annoying here, because the test runner captures stdout/stderr, so it won't be visible until the test fails,
			// which can take a while because when things go wrong here it will most likely result in timeouts.
			// So instead we log to a separate file (debug.log in the test's temporary directory), which can be viewed as the test is running.
			//
			// In addition, extensive logging can be opted into by setting the DOTNET_WATCH_TEST_LOG environment variable to any non-empty value.
			// When enabled, everything we log is *also* written to:
			// * The current terminal. The test runner captures Console output (so Console.WriteLine won't show up until the test ends, which
			//   is exactly when things have already gone wrong), so we locate the terminal device ourselves and write directly to it.
			// * A well-known file (/tmp/DotNetWatchTestOutput.txt), which can be tailed while the test is running.
			// The path to the well-known file is printed to the terminal at the very beginning of the test.
			var debugLogPath = Path.Combine (tmpdir, "debug.log");
			var debugLogStream = new FileStream (debugLogPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			var debugLogWriters = new List<TextWriter> {
				new StreamWriter (debugLogStream) { AutoFlush = true },
			};

			var verboseLogging = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("DOTNET_WATCH_TEST_LOG"));
			string? verboseLogPath = null;
			if (verboseLogging) {
				verboseLogPath = "/tmp/DotNetWatchTestOutput.txt";
				var verboseLogStream = new FileStream (verboseLogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				debugLogWriters.Add (new StreamWriter (verboseLogStream) { AutoFlush = true });
				var terminalWriter = TryOpenTerminal ();
				if (terminalWriter is not null)
					debugLogWriters.Add (terminalWriter);
			}

			var debugLog = new TeeTextWriter (debugLogWriters.ToArray ());
			void Log (string message)
			{
				debugLog.WriteLine ($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
			}

			if (verboseLogging)
				Log ($"=== DotNetWatch ({platform}, enableSandbox: {enableSandbox}): logging to {verboseLogPath} and {debugLogPath} ===");

			Log ($"Starting DotNetWatch test for {platform} (enableSandbox: {enableSandbox}).");
			Log ($"Project path: {projectPath}");
			Log ($"Project directory: {projectDirectory}");
			Log ($"Temporary directory: {tmpdir}");

			Log ("Cleaning the project...");
			Clean (projectPath);
			Log ("Cleaned the project.");

			var firstContent = """
					namespace HotReloadTestApp;
					public partial class Program {
						static partial void ChangeVariable ()
						{
							Variable = "Variable will change...";
						}
					}
					""";

			var secondContent = """
					namespace HotReloadTestApp;
					public partial class Program {
						static partial void ChangeVariable ()
						{
							Variable = "Variable has changed";
							ContinueLooping = false;
						}
					}
					""";

			Log ($"Writing initial content to {additionalFile}...");
			File.WriteAllText (additionalFile, firstContent);
			Log ($"Wrote initial content to {additionalFile}.");

			var output = new List<string> ();
			var appStarted = new TaskCompletionSource<bool> ();
			var waitingForChanges = new TaskCompletionSource<bool> ();
			var variableChanged = new TaskCompletionSource<bool> ();
			var cts = new CancellationTokenSource ();
			var appOutput = new List<string> ();

			var outputProcessor = new Action<string> (line => {
				if (line.Contains ("Variable has not changed")) {
					if (appStarted.TrySetResult (true))
						Log ("Got 'Variable has not changed' => the app has started.");
				}
				if (line.Contains ("Variable has changed")) {
					if (variableChanged.TrySetResult (true))
						Log ("Got 'Variable has changed' => the variable changed after the hot reload.");
				}
				if (line.Contains ("Waiting for changes")) {
					if (waitingForChanges.TrySetResult (true))
						Log ("Got 'Waiting for changes' => 'dotnet watch' is waiting for changes.");
				}
				if (line.Contains ("Build FAILED.")) {
					if (waitingForChanges.TrySetResult (false))
						Log ("Got 'Build FAILED' => the build failed.");
				}
			});

			// I'm not sure what 'dotnet watch' does with the terminal, but Console.WriteLine from the test app doesn't seem to
			// reliably be captured here, so instead we have the test app write its output to a file, and we poll that file and
			// process new lines as they are written.
			// However, for mobile platforms, test app stdout is captured correctly, so we process both the output from the file
			// and stdout we capture from 'dotnet watch' the same way, to make sure we don't miss any output.
			var logPath = Path.Combine (tmpdir, "output.log");
			if (enableSandbox) {
				// When the sandbox is enabled, the app can't write to our temp directory.
				// Put the log file in the app's sandbox container, which is accessible to both the app and the test runner.
				var containerDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Containers", "com.xamarin.hotreloadtestapp", "Data", "tmp");
				Directory.CreateDirectory (containerDir);
				logPath = Path.Combine (containerDir, "output.log");
			}
			Log ($"The app will write its output to: {logPath}");
			// Delete any stale log file from a previous run. The sandboxed log file lives in a
			// well-known location in the app's container (shared between all sandboxed runs), and
			// the app opens it without truncating, so stale lines from a previous run could
			// otherwise satisfy the output checks and make the test pass without validating anything.
			if (File.Exists (logPath))
				File.Delete (logPath);
			var pollThread = new Thread ((v) => {
				Log ($"Output polling thread started. Polling '{logPath}' for app output.");
				var reportedLines = 0;
				var reportedFileExists = false;
				for (var i = 0; i < 120; i++) {
					if (File.Exists (logPath)) {
						if (!reportedFileExists) {
							Log ($"The app log file '{logPath}' appeared after {i} second(s).");
							reportedFileExists = true;
						}
						var lines = File.ReadAllLines (logPath);
						Array.ForEach (lines, outputProcessor);
						lock (appOutput) {
							appOutput.Clear ();
							appOutput.AddRange (lines);
						}
						if (lines.Length != reportedLines) {
							Log ($"The app log file now has {lines.Length} line(s) (was {reportedLines}); new line(s):");
							for (var l = reportedLines; l < lines.Length; l++)
								Log ($"[app] {lines [l]}");
							reportedLines = lines.Length;
						}
					} else if (!reportedFileExists && (i % 10) == 0) {
						Log ($"Still waiting for the app log file '{logPath}' to appear ({i} second(s) elapsed)...");
					}
					Thread.Sleep (TimeSpan.FromSeconds (1));
				}
				Log ("Output polling thread finished.");
			}) {
				IsBackground = true,
				Name = "Output Polling Thread",
			};
			pollThread.Start ();

			Action<string> outputCallback = (line) => {
				Log ($"[dotnet watch] {line}");
				lock (output) {
					output.Add (line);
					outputProcessor (line);
				}
			};

			var args = new List<string> {
				"watch",
				"--non-interactive",
				"--disable-build-servers",
			};

			var deviceName = "";
			if (usePhysicalDevice) {
				deviceName = Environment.GetEnvironmentVariable ("DEVICE") ?? "";
				Assert.That (deviceName, Is.Not.Empty, "The DEVICE environment variable must be set to a connected device name to run this test.");
				debugLog.WriteLine ($"Using physical device: {deviceName}");
				args.Add ($"--device={deviceName}");
			} else if (platform == ApplePlatform.iOS || platform == ApplePlatform.TVOS) {
				var runtimeIdentifier = GetDefaultRuntimeIdentifier (platform);
				Log ($"Computing a device to use for {platform} (runtime identifier: {runtimeIdentifier})...");
				var device = GetDeviceAsync (projectDirectory, runtimeIdentifier).GetAwaiter ().GetResult ();
				Log ($"Using device: {device}");
				args.Add ($"--device={device}");
			}

			var env = new Dictionary<string, string?> {
				{ "AdditionalFile", additionalFile },
				{ "UseMonoRuntime", "false" },
				{ "RunWithOpen", "false" }, // this makes it so that the watched process is a subprocess, which means that ctrl-c in the terminal will kill everything. It also means that it'll get killed if something times out in the test.
				{ "EnableSandbox", enableSandbox ? "true" : "false" },
			};

			if (usePhysicalDevice) {
				// On a physical device, the app can't write to the Mac filesystem, so don't set the log file path.
				// The app's stdout will be captured by mlaunch and forwarded to dotnet watch.
				// Also set the runtime identifier explicitly to target the physical device.
				env ["RuntimeIdentifier"] = $"{platform.AsString ().ToLowerInvariant ()}-arm64";
				env ["Device"] = deviceName;
				// How the device connects back to the mac for hot reload (usb or wifi).
				env ["HotReloadConnectionMode"] = connectionMode;
			} else {
				// On simulators and desktop, the app can write to the Mac filesystem.
				env ["HOTRELOAD_TEST_APP_LOGFILE"] = logPath;
			}

			Log ("Starting 'dotnet watch' with:");
			Log ($"    Command: {DotNet.Executable} {string.Join (" ", StringUtils.QuoteForProcess (args) ?? [])}");
			Log ($"    Working directory: {projectDirectory}");
			foreach (var kvp in env)
				Log ($"    Environment variable: {kvp.Key}={kvp.Value}");

			var watchTask = Execution.RunWithCallbacksAsync (
				DotNet.Executable,
				args,
				environment: env,
				standardOutput: outputCallback,
				standardError: outputCallback,
				workingDirectory: projectDirectory,
				timeout: TimeSpan.FromMinutes (10),
				cancellationToken: cts.Token,
				log: debugLog,
				closeStandardInput: true
			);
			Log ("Started 'dotnet watch'.");

			try {
				// Wait for the app to start and show initial output
				Log ("Waiting for app start...");
				if (!Task.WhenAny (appStarted.Task, waitingForChanges.Task, watchTask).Wait (TimeSpan.FromMinutes (2))) {
					Log ("Timed out waiting for the app to start.");
					Assert.Fail ($"Timed out waiting for the app to start. Output:\n{string.Join ("\n", output)}\nDebug output:\n{string.Join ("\n", File.ReadAllLines (debugLogPath))}");
				}
				if (watchTask.IsCompleted) {
					Log ("FAIL: 'dotnet watch' finished prematurely.");
					Assert.Fail ($"'dotnet watch' finished prematurely. Output:\n{string.Join ("\n", output)}\nDebug output:\n{string.Join ("\n", File.ReadAllLines (debugLogPath))}");
				}
				if (!appStarted.Task.IsCompleted) {
					if (waitingForChanges.Task.IsCompleted && !waitingForChanges.Task.Result) {
						Log ("The build failed before the app could start.");
						Assert.Fail ($"Build failed before the app could start. Output:\n{string.Join ("\n", output)}\nDebug output:\n{string.Join ("\n", File.ReadAllLines (debugLogPath))}");
					}
					if (!appStarted.Task.Wait (TimeSpan.FromMinutes (1))) {
						Log ("Timed out waiting for the app to start.");
						Assert.Fail ($"Timed out waiting for the app to start. Output:\n{string.Join ("\n", output)}\nDebug output:\n{string.Join ("\n", File.ReadAllLines (debugLogPath))}");
					}
				}
				Log ("App started!");

				Log ("Waiting for 'dotnet watch' to be waiting for changes...");
				if (!waitingForChanges.Task.Wait (TimeSpan.FromMinutes (1))) {
					Log ("Timed out waiting for 'dotnet watch' to be waiting for changes.");
					Assert.Fail ($"Timed out waiting for the 'dotnet watch' to be waiting for changes. Output:\n{string.Join ("\n", output)}\nDebug output:\n{string.Join ("\n", File.ReadAllLines (debugLogPath))}");
				}
				if (!waitingForChanges.Task.Result) {
					Log ("The build failed.");
					Assert.Fail ($"Build failed. Output:\n{string.Join ("\n", output)}\nDebug output:\n{string.Join ("\n", File.ReadAllLines (debugLogPath))}");
				}
				Log ("Waiting for changes!");

				// Write AdditionalFile.cs to trigger a rebuild via dotnet watch
				Log ($"Writing updated content to {additionalFile} to trigger a hot reload...");
				File.WriteAllText (additionalFile, secondContent);
				Log ($"Wrote updated content to {additionalFile}.");

				// Wait for dotnet watch to pick up the change and the app to show the updated output
				Log ("Waiting for app restart...");
				if (!variableChanged.Task.Wait (TimeSpan.FromMinutes (1))) {
					Log ("Timed out waiting for the variable to change.");
					Assert.Fail ($"Timed out waiting for the variable to change. Output:\n{string.Join ("\n", output)}\nDebug output:\n{string.Join ("\n", File.ReadAllLines (debugLogPath))}");
				}
				Log ("App restarted!");
			} finally {
				// Always cancel the watch process, even if the test failed
				Log ("Terminating the watch process...");
				cts.Cancel ();

				try {
					Log ("Waiting for exit...");
					if (!watchTask.Wait (TimeSpan.FromSeconds (30)))
						Log ("Watch process did not exit within 30 seconds.");
					else
						Log ("Waited for exit");
				} catch (Exception ex) {
					// Expected - the process was cancelled
					Log ($"Exception while waiting for exit (may be expected due to cancellation): {ex.Message}");
				}

				Log ("DotNetWatch test finished.");
				// Don't dispose 'debugLog' here: output can still arrive after the test has finished, because things
				// are happening on other threads (the polling thread and the 'dotnet watch' output callbacks). Just
				// leave it for the GC to collect whenever it can.
			}
		}

		// Pick any device for the specified project, and compatible with the specified runtime identifier (if provided).
		// We just need any device to test that dotnet watch can detect it and deploy to it.
		static async Task<string> GetDeviceAsync (string projectDirectory, string? runtimeIdentifier = null)
		{
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var outputFile = Path.Combine (tmpdir, "AvailableDevices.json");
			var args = new List<string> {
				"build",
				"-t:ComputeAvailableDevices",
				"-getItem:Devices",
				$"-getResultOutputFile:{outputFile}",
			};

			if (!string.IsNullOrEmpty (runtimeIdentifier))
				args.Add ($"-p:RuntimeIdentifier={runtimeIdentifier}");

			var rv = await Execution.RunWithCallbacksAsync (
				DotNet.Executable,
				args,
				workingDirectory: projectDirectory,
				timeout: TimeSpan.FromMinutes (1),
				log: Console.Out
			);
			Assert.That (rv.ExitCode, Is.EqualTo (0), "Failed to compute available devices");

			var output = File.ReadAllText (outputFile);
			var doc = JsonDocument.Parse (output);
			// The devices are ordered, so that:
			// * We get the same device each time, to make tests more reliable.
			// * We get the most recent OS version available, to make sure we're testing on a recent OS version.
			// * We get iPhones before iPads (by sorting by device type identifier), just because they take up less of the screen during a test run.
			var devices = doc.RootElement.GetProperty ("Items").GetProperty ("Devices").EnumerateArray ().Select (e => {
				var identity = e.GetProperty ("Identity").GetString ()!;
				var osVersion = Version.Parse (e.GetProperty ("OSVersion").GetString ()!);
				var deviceTypeIdentifier = e.GetProperty ("DeviceTypeIdentifier").GetString ()!;
				return (Identity: identity, OsVersion: osVersion, DeviceTypeIdentifier: deviceTypeIdentifier);
			}).OrderBy (d => d.OsVersion).ThenByDescending (d => d.DeviceTypeIdentifier).ThenBy (d => d.Identity).ToList ();
			if (!devices.Any ())
				Assert.Inconclusive ("No devices found. Output:\n" + output);
			return devices.First ().Identity;
		}

		[DllImport ("libc", EntryPoint = "ttyname")]
		static extern IntPtr ttyname (int filedescriptor);

		// Returns the path to the terminal (tty) of the current process, similar to the "tty" command, or null if there's no terminal.
		static string? GetCurrentTerminal ()
		{
			// Check the file descriptors we write to (stdout = 1, stderr = 2) for a terminal.
			foreach (var fd in new [] { 1, 2 }) {
				var ptr = ttyname (fd);
				if (ptr != IntPtr.Zero) {
					var name = Marshal.PtrToStringAnsi (ptr);
					if (!string.IsNullOrEmpty (name))
						return name;
				}
			}
			return null;
		}

		// Opens the current terminal for writing, or returns null if there's no terminal available.
		static TextWriter? TryOpenTerminal ()
		{
			// First try the specific terminal device (e.g. /dev/ttys003). If that fails - the test runner may have
			// redirected all the standard file descriptors - fall back to /dev/tty, which always refers to the
			// controlling terminal of the process (if any).
			foreach (var path in new [] { GetCurrentTerminal (), "/dev/tty" }) {
				if (string.IsNullOrEmpty (path))
					continue;
				try {
					var stream = new FileStream (path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
					return new StreamWriter (stream) { AutoFlush = true };
				} catch {
					// No accessible terminal at this path, try the next one.
				}
			}
			return null;
		}

		// A TextWriter that forwards everything written to it to a set of other TextWriters.
		// All writes are synchronized, because 'debugLog' is written to from multiple threads (the test thread, the
		// output polling thread, and the 'dotnet watch' output callbacks), as well as passed to
		// Execution.RunWithCallbacksAsync (which writes to it without any locking on our part).
		sealed class TeeTextWriter : TextWriter {
			readonly TextWriter [] writers;
			readonly object lockObj = new object ();

			public TeeTextWriter (params TextWriter [] writers)
			{
				this.writers = writers;
			}

			public override Encoding Encoding => Encoding.UTF8;

			public override void Write (char value)
			{
				lock (lockObj) {
					foreach (var writer in writers)
						writer.Write (value);
				}
			}

			public override void Write (string? value)
			{
				lock (lockObj) {
					foreach (var writer in writers)
						writer.Write (value);
				}
			}

			public override void WriteLine (string? value)
			{
				lock (lockObj) {
					foreach (var writer in writers)
						writer.WriteLine (value);
				}
			}

			public override void Flush ()
			{
				lock (lockObj) {
					foreach (var writer in writers)
						writer.Flush ();
				}
			}
		}
	}
}
