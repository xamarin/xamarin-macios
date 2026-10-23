// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Utils;

if (!OperatingSystem.IsMacOS ()) {
	Console.Error.WriteLine ("This script only supports macOS hosts.");
	return 1;
}

Options options;
try {
	options = Options.Parse (args);
} catch (Exception ex) {
	Console.Error.WriteLine (ex.Message);
	PrintUsage ();
	return 1;
}

if (options.ShowHelp) {
	PrintUsage ();
	return 0;
}

var runner = new AudioUnitExtensionTestRunner (options);
return await runner.RunAsync ();

static void PrintUsage ()
{
	Console.WriteLine ("Usage:");
	Console.WriteLine ("  run-audio-unit-extension-tests --platform <platform> --rid <rid> --config <config> --app <app path> --extension <appex path> --executable <host executable> --log-file <log file> [--results-file <results file>] --timeout-seconds <seconds> [--simulator-udid <udid>] [--test-filter <test name>] [--lsregister <path>]");
}

sealed class AudioUnitExtensionTestRunner {
	const string DefaultBundleIdentifier = "com.xamarin.monotouch-test.AudioUnitExtension";
	const string MacCatalystBundleIdentifier = "com.xamarin.monotouch-test.AudioUnitExtension.MacCatalyst";
	const string TopShelfBundleIdentifier = "com.xamarin.monotouch-test.TopShelfExtension";
	const string DesktopContainerBundleIdentifier = "com.xamarin.monotouch-test.audiounit.containerapp";
	const string MobileContainerBundleIdentifier = "com.xamarin.monotouch-test";

	// Predicate used to capture the system log for diagnostic purposes only. The
	// actual test results are streamed back over a TCP connection (see below).
	const string LogPredicate = "process == \"monotouchtest\" OR process == \"ContainerApp\" OR eventMessage CONTAINS[c] \"monotouch-test-app-extension\"";

	const string EndMarker = "<!-- the end -->";

	static readonly Regex TestResultsTagRegex = new ("<test-results\\b[^>]*>", RegexOptions.Compiled);

	readonly Options options;
	readonly object logLock = new ();

	string BundleIdentifier => options.Platform switch {
		"MacCatalyst" => MacCatalystBundleIdentifier,
		"tvOS" => TopShelfBundleIdentifier,
		_ => DefaultBundleIdentifier,
	};
	string DefaultsDomain => options.SimulatorUdid is null
		? Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Containers", BundleIdentifier, "Data", "Library", "Preferences", BundleIdentifier + ".plist")
		: BundleIdentifier;
	string ContainerBundleIdentifier => options.SimulatorUdid is null ? DesktopContainerBundleIdentifier : MobileContainerBundleIdentifier;

	string ResultsFilePath {
		get {
			if (!string.IsNullOrEmpty (options.ResultsFilePath))
				return options.ResultsFilePath;
			var directory = Path.GetDirectoryName (options.LogFilePath)!;
			var name = Path.GetFileNameWithoutExtension (options.LogFilePath);
			return Path.Combine (directory, name + ".nunit-results.xml");
		}
	}

	public AudioUnitExtensionTestRunner (Options options)
	{
		this.options = options;
	}

	public async Task<int> RunAsync ()
	{
		Directory.CreateDirectory (Path.GetDirectoryName (options.LogFilePath)!);
		File.WriteAllText (options.LogFilePath, "");

		Log ($"Platform: {options.Platform}");
		Log ($"RID: {options.Rid}");
		Log ($"Config: {options.Config}");
		Log ($"App: {options.AppPath}");
		Log ($"Extension: {options.ExtensionPath}");
		Log ($"Log file: {options.LogFilePath}");
		if (!string.IsNullOrEmpty (options.TestFilter))
			Log ($"Test filter: {options.TestFilter}");
		if (!string.IsNullOrEmpty (options.SimulatorUdid))
			Log ($"Simulator UDID: {options.SimulatorUdid}");
		Log ("");

		var exitCode = 0;
		var logStart = DateTime.Now;

		// Listen on a free localhost port. The extension connects back to this
		// port and streams the NUnit XML result (see Touch.Client's TouchOptions
		// / TouchRunner, which read the network configuration from NSUserDefaults).
		var listener = new TcpListener (IPAddress.Loopback, 0);
		listener.Start ();
		var port = ((IPEndPoint) listener.LocalEndpoint).Port;
		Log ($"Listening for test results on 127.0.0.1:{port}.");

		using var hostCts = new CancellationTokenSource ();
		Task? hostTask = null;
		string? stagedAppDirectory = null;
		var registeredAppPath = options.AppPath;
		var registeredExtensionPath = options.ExtensionPath;
		var executablePath = options.ExecutablePath;

		try {
			if (options.SimulatorUdid is null) {
				stagedAppDirectory = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Applications", "dotnet-macios-tests", Guid.NewGuid ().ToString ("N"));
				registeredAppPath = Path.Combine (stagedAppDirectory, Path.GetFileName (options.AppPath));
				registeredExtensionPath = Path.Combine (registeredAppPath, Path.GetRelativePath (options.AppPath, options.ExtensionPath));
				executablePath = Path.Combine (registeredAppPath, Path.GetRelativePath (options.AppPath, options.ExecutablePath));
				Directory.CreateDirectory (stagedAppDirectory);
				await RunToolAsync ("ditto", options.AppPath, registeredAppPath);
				Log ("");
				await RunToolAsync (options.LsRegisterPath, "-f", registeredAppPath);
				Log ("");
				await RunToolAsync ("pluginkit", "-a", registeredExtensionPath);
				Log ("");
			} else {
				await RunToolAsync ("xcrun", "simctl", "install", options.SimulatorUdid, options.AppPath);
				Log ("");
			}

			await ConfigureDefaultsAsync (port);

			hostTask = StartHost (registeredAppPath, hostCts.Token);

			var (result, timedOut) = await ReceiveResultsAsync (listener);

			if (timedOut) {
				Log ($"Timed out waiting for the extension test results after {options.Timeout.TotalMinutes:0} minutes.");
				exitCode = 1;
			} else {
				exitCode = Math.Max (exitCode, ProcessResults (result));
			}
		} catch (Exception ex) {
			Log (ex.ToString ());
			exitCode = 1;
		} finally {
			listener.Stop ();

			// Stop the container host so its process (and the extension) can exit.
			if (options.SimulatorUdid is null)
				TerminateHostProcess (executablePath);
			hostCts.Cancel ();
			if (hostTask is not null) {
				try {
					await hostTask;
				} catch {
				}
			}

			await CleanupDefaultsAsync ();
			if (options.SimulatorUdid is not null)
				await RunBestEffortAsync ("xcrun", "simctl", "uninstall", options.SimulatorUdid, ContainerBundleIdentifier);
			if (stagedAppDirectory is not null) {
				await RunBestEffortAsync ("pluginkit", "-r", registeredExtensionPath);
				await RunBestEffortAsync (options.LsRegisterPath, "-u", registeredAppPath);
				Directory.Delete (stagedAppDirectory, true);
			}

			var logEnd = DateTime.Now;
			Log ("");
			Log ("System log (diagnostics):");
			await CaptureSystemLogAsync (logStart, logEnd);
		}

		return exitCode;
	}

	async Task ConfigureDefaultsAsync (int port)
	{
		await RunDefaultsToolAsync (false, "write", DefaultsDomain, "network.enabled", "-bool", "YES");
		await RunDefaultsToolAsync (false, "write", DefaultsDomain, "network.host.name", "-string", "127.0.0.1");
		await RunDefaultsToolAsync (false, "write", DefaultsDomain, "network.host.port", "-int", port.ToString (CultureInfo.InvariantCulture));
		await RunDefaultsToolAsync (false, "write", DefaultsDomain, "network.transport", "-string", "TCP");
		await RunDefaultsToolAsync (false, "write", DefaultsDomain, "execution.usetcptunnel", "-bool", "NO");
		await RunDefaultsToolAsync (false, "write", DefaultsDomain, "xml.enabled", "-bool", "YES");

		if (string.IsNullOrEmpty (options.TestFilter)) {
			await RunDefaultsToolAsync (true, "delete", DefaultsDomain, "test.name");
		} else {
			await RunDefaultsToolAsync (false, "write", DefaultsDomain, "test.name", "-string", options.TestFilter);
		}
		Log ("");
	}

	async Task CleanupDefaultsAsync ()
	{
		foreach (var key in new [] { "network.enabled", "network.host.name", "network.host.port", "network.transport", "execution.usetcptunnel", "xml.enabled", "test.name" })
			await RunDefaultsToolAsync (true, "delete", DefaultsDomain, key);
	}

	async Task StartHost (string appPath, CancellationToken cancellationToken)
	{
		var environment = new Dictionary<string, string?> {
			[options.SimulatorUdid is null ? "RUN_EXTENSION_TESTS" : "SIMCTL_CHILD_RUN_EXTENSION_TESTS"] = "1",
		};
		if (options.SimulatorUdid is not null) {
			var arguments = new List<string> { "simctl", "launch", "--console-pty", "--terminate-running-process", options.SimulatorUdid, ContainerBundleIdentifier };
			Log ($"Executing: SIMCTL_CHILD_RUN_EXTENSION_TESTS=1 xcrun {StringUtils.FormatArguments (arguments)}");
			await Execution.RunWithCallbacksAsync (
				"xcrun",
				arguments,
				environment: environment,
				standardOutput: Log,
				standardError: Log,
				cancellationToken: cancellationToken);
			return;
		}

		if (options.SimulatorUdid is null) {
			var arguments = new List<string> { "-W", "-n", "--env", "RUN_EXTENSION_TESTS=1", appPath };
			Log ($"Executing: open {StringUtils.FormatArguments (arguments)}");
			await Execution.RunWithCallbacksAsync (
				"open",
				arguments,
				standardOutput: Log,
				standardError: Log,
				cancellationToken: cancellationToken);
			return;
		}
	}

	void TerminateHostProcess (string executablePath)
	{
		foreach (var process in System.Diagnostics.Process.GetProcessesByName (Path.GetFileName (executablePath))) {
			using (process) {
				try {
					if (process.MainModule?.FileName == executablePath) {
						Log ($"Terminating host process {process.Id}: {executablePath}");
						process.Kill ();
						process.WaitForExit (TimeSpan.FromSeconds (10));
					}
				} catch (Exception ex) {
					Log ($"Could not terminate host process {process.Id}: {ex.Message}");
				}
			}
		}
	}

	Task RunDefaultsToolAsync (bool bestEffort, params string [] arguments)
	{
		if (options.SimulatorUdid is null)
			return bestEffort ? RunBestEffortAsync ("defaults", arguments) : RunToolAsync ("defaults", arguments);

		var simulatorArguments = new List<string> { "simctl", "spawn", options.SimulatorUdid, "defaults" };
		simulatorArguments.AddRange (arguments);
		return bestEffort ? RunBestEffortAsync ("xcrun", simulatorArguments.ToArray ()) : RunToolAsync ("xcrun", simulatorArguments.ToArray ());
	}

	async Task<(string Result, bool TimedOut)> ReceiveResultsAsync (TcpListener listener)
	{
		using var timeoutCts = new CancellationTokenSource (options.Timeout);

		TcpClient client;
		try {
			client = await listener.AcceptTcpClientAsync (timeoutCts.Token);
		} catch (OperationCanceledException) {
			Log ("The extension never connected to report test results.");
			return ("", true);
		}

		Log ("The extension connected; reading test results.");

		var payload = new StringBuilder ();
		var timedOut = false;
		var gotEnd = false;

		using (client)
		using (var stream = client.GetStream ())
		using (var reader = new StreamReader (stream, Encoding.UTF8)) {
			while (true) {
				string? line;
				try {
					line = await reader.ReadLineAsync (timeoutCts.Token);
				} catch (OperationCanceledException) {
					timedOut = true;
					break;
				}

				if (line is null)
					break;

				payload.AppendLine (line);
				AppendToLogFile (line);

				if (line.Contains (EndMarker, StringComparison.Ordinal)) {
					gotEnd = true;
					break;
				}
			}
		}

		if (gotEnd)
			Log ("Received the end-of-results marker.");
		else if (!timedOut)
			Log ("The extension disconnected before sending the end-of-results marker.");

		return (payload.ToString (), timedOut);
	}

	int ProcessResults (string payload)
	{
		if (string.IsNullOrWhiteSpace (payload)) {
			Log ("Did not receive any test results from the extension.");
			return 1;
		}

		// Persist the NUnit XML result (everything up to and including
		// </test-results>) for consumption by CI.
		var endTag = "</test-results>";
		var endIndex = payload.IndexOf (endTag, StringComparison.Ordinal);
		var xml = endIndex >= 0 ? payload.Substring (0, endIndex + endTag.Length) : payload;
		File.WriteAllText (ResultsFilePath, xml);
		Log ($"Wrote NUnit results to: {ResultsFilePath}");

		var tagMatch = TestResultsTagRegex.Match (payload);
		if (!tagMatch.Success) {
			Log ("Did not find an NUnit <test-results> element in the test output.");
			return 1;
		}

		var tag = tagMatch.Value;
		var total = GetAttribute (tag, "total");
		var errors = GetAttribute (tag, "errors");
		var failures = GetAttribute (tag, "failures");
		var notRun = GetAttribute (tag, "not-run");
		var inconclusive = GetAttribute (tag, "inconclusive");
		var ignored = GetAttribute (tag, "ignored");

		Log ($"Tests run: {total} Failures: {failures} Errors: {errors} Not-run: {notRun} Inconclusive: {inconclusive} Ignored: {ignored}");

		if (total <= 0) {
			Log ("The extension did not execute any tests.");
			return 1;
		}

		if (failures > 0 || errors > 0) {
			Log ($"❌ Extension test run failed ({failures} failures, {errors} errors).");
			return 1;
		}

		Log ("✅ Extension test run succeeded");
		return 0;
	}

	static int GetAttribute (string tag, string name)
	{
		var match = Regex.Match (tag, name + "=\"(\\d+)\"");
		return match.Success ? int.Parse (match.Groups [1].Value, CultureInfo.InvariantCulture) : -1;
	}

	async Task CaptureSystemLogAsync (DateTime start, DateTime end)
	{
		Log ($"Executing: log show --style compact --predicate {LogPredicate} --start {FormatTimestamp (start)} --end {FormatTimestamp (end)}");
		var execution = await Execution.RunWithCallbacksAsync (
			"log",
			new List<string> { "show", "--style", "compact", "--predicate", LogPredicate, "--start", FormatTimestamp (start), "--end", FormatTimestamp (end) },
			standardOutput: AppendToLogFile,
			standardError: AppendToLogFile);
		if (execution.ExitCode != 0)
			Log ($"'log show' exited with code {execution.ExitCode}.");
	}

	async Task RunToolAsync (string fileName, params string [] arguments)
	{
		Log ($"Executing: {StringUtils.FormatArguments (Prepend (fileName, arguments))}");
		var execution = await Execution.RunWithCallbacksAsync (fileName, arguments, standardOutput: AppendToLogFile, standardError: AppendToLogFile);
		if (execution.ExitCode != 0)
			throw new InvalidOperationException ($"'{fileName}' exited with code {execution.ExitCode}.");
	}

	async Task RunBestEffortAsync (string fileName, params string [] arguments)
	{
		await Execution.RunWithCallbacksAsync (fileName, arguments, standardOutput: AppendToLogFile, standardError: AppendToLogFile);
	}

	static IList<string> Prepend (string fileName, string [] arguments)
	{
		var list = new List<string> (arguments.Length + 1) { fileName };
		list.AddRange (arguments);
		return list;
	}

	void Log (string line)
	{
		lock (logLock) {
			File.AppendAllText (options.LogFilePath, line + Environment.NewLine);
			Console.WriteLine (line);
		}
	}

	void AppendToLogFile (string line)
	{
		lock (logLock) {
			File.AppendAllText (options.LogFilePath, line + Environment.NewLine);
		}
	}

	static string FormatTimestamp (DateTime timestamp)
		=> timestamp.ToString ("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}

sealed class Options {
	public bool ShowHelp { get; private init; }
	public string Platform { get; private init; } = "";
	public string Rid { get; private init; } = "";
	public string Config { get; private init; } = "";
	public string AppPath { get; private init; } = "";
	public string ExtensionPath { get; private init; } = "";
	public string ExecutablePath { get; private init; } = "";
	public string LogFilePath { get; private init; } = "";
	public string? ResultsFilePath { get; private init; }
	public string? SimulatorUdid { get; private init; }
	public string LsRegisterPath { get; private init; } = "/System/Library/Frameworks/CoreServices.framework/Frameworks/LaunchServices.framework/Support/lsregister";
	public string? TestFilter { get; private init; }
	public TimeSpan Timeout { get; private init; }

	public static Options Parse (string [] args)
	{
		var parsed = new Dictionary<string, string> (StringComparer.Ordinal);

		for (var i = 0; i < args.Length; i++) {
			var argument = args [i];
			if (argument is "--help" or "-h")
				return new Options { ShowHelp = true };
			if (!argument.StartsWith ("--", StringComparison.Ordinal))
				throw new ArgumentException ($"Unknown argument: {argument}");
			if (i + 1 >= args.Length)
				throw new ArgumentException ($"Missing value for argument: {argument}");
			parsed [argument] = args [++i];
		}

		var timeoutSeconds = int.Parse (GetRequired (parsed, "--timeout-seconds"), CultureInfo.InvariantCulture);
		if (timeoutSeconds <= 0)
			throw new ArgumentOutOfRangeException (nameof (args), "The timeout must be a positive number of seconds.");

		return new Options {
			Platform = GetRequired (parsed, "--platform"),
			Rid = GetRequired (parsed, "--rid"),
			Config = GetRequired (parsed, "--config"),
			AppPath = Path.GetFullPath (GetRequired (parsed, "--app")),
			ExtensionPath = Path.GetFullPath (GetRequired (parsed, "--extension")),
			ExecutablePath = Path.GetFullPath (GetRequired (parsed, "--executable")),
			LogFilePath = Path.GetFullPath (GetRequired (parsed, "--log-file")),
			ResultsFilePath = GetOptional (parsed, "--results-file") is string resultsFilePath ? Path.GetFullPath (resultsFilePath) : null,
			SimulatorUdid = GetOptional (parsed, "--simulator-udid"),
			LsRegisterPath = GetOptional (parsed, "--lsregister") ?? "/System/Library/Frameworks/CoreServices.framework/Frameworks/LaunchServices.framework/Support/lsregister",
			TestFilter = GetOptional (parsed, "--test-filter"),
			Timeout = TimeSpan.FromSeconds (timeoutSeconds),
		};
	}

	static string GetRequired (Dictionary<string, string> parsed, string key)
	{
		if (!parsed.TryGetValue (key, out var value) || string.IsNullOrEmpty (value))
			throw new ArgumentException ($"Missing required argument: {key}");
		return value;
	}

	static string? GetOptional (Dictionary<string, string> parsed, string key)
	{
		parsed.TryGetValue (key, out var value);
		return string.IsNullOrEmpty (value) ? null : value;
	}
}
