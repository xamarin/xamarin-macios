using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.Serialization.Json;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;

using ExceptionLogger = System.Action<int, string>;

namespace Microsoft.DotNet.XHarness.iOS.Shared {

	// main class that gets the result of an executed test application, parses the results and provides information
	// about the success or failure of the execution.
	public class TestReporter : ITestReporter {

		const string timeoutMessage = "Test run timed out after {0} minute(s).";
		const string completionMessage = "Test run completed";
		const string failureMessage = "Test run failed";

		readonly ISimpleListener listener;
		readonly ILog mainLog;
		readonly ILogs crashLogs;
		readonly ILog runLog;
		readonly ILogs logs;
		readonly ICrashSnapshotReporter crashReporter;
		readonly IResultParser resultParser;
		readonly AppBundleInformation appInfo;
		readonly RunMode runMode;
		readonly XmlResultJargon xmlJargon;
		readonly IProcessManager processManager;
		readonly string deviceName;

		readonly TimeSpan timeout;
		readonly Stopwatch timeoutWatch;

		/// <summary>
		/// Additional logs that will be sent with the report in case of a failure.
		/// Used by the Xamarin.Xharness project to add BuildTask logs.
		/// </summary>
		readonly string additionalLogsDirectory;

		/// <summary>
		/// Callback needed for the Xamarin.Xharness project that does extra logging in case of a crash.
		/// </summary>
		readonly ExceptionLogger exceptionLogger;

		bool waitedForExit = true;
		bool launchFailure;
		bool isSimulatorTest;
		bool timedout;

		public ILog CallbackLog { get; private set; }

		public bool? Success { get; private set; }
		public CancellationToken CancellationToken => cancellationTokenSource.Token;

		public bool ResultsUseXml => xmlJargon != XmlResultJargon.Missing;

		readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource ();

		public TestReporter (IProcessManager processManager,
			ILog mainLog,
			ILog runLog,
			ILogs logs,
			ICrashSnapshotReporter crashReporter,
			ISimpleListener simpleListener,
			IResultParser parser,
			AppBundleInformation appInformation,
			RunMode runMode,
			XmlResultJargon xmlJargon,
			string device,
			TimeSpan timeout,
			string additionalLogsDirectory = null,
			ExceptionLogger exceptionLogger = null)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.deviceName = device; // can be null on simulators 
			this.listener = simpleListener ?? throw new ArgumentNullException (nameof (simpleListener));
			this.mainLog = mainLog ?? throw new ArgumentNullException (nameof (mainLog));
			this.runLog = runLog ?? throw new ArgumentNullException (nameof (runLog));
			this.logs = logs ?? throw new ArgumentNullException (nameof (logs));
			this.crashReporter = crashReporter ?? throw new ArgumentNullException (nameof (crashReporter));
			this.crashLogs = new Logs (logs.Directory);
			this.resultParser = parser ?? throw new ArgumentNullException (nameof (parser));
			this.appInfo = appInformation ?? throw new ArgumentNullException (nameof (appInformation));
			this.runMode = runMode;
			this.xmlJargon = xmlJargon;
			this.timeout = timeout;
			this.additionalLogsDirectory = additionalLogsDirectory;
			this.exceptionLogger = exceptionLogger;
			this.timeoutWatch  = Stopwatch.StartNew ();

			CallbackLog = new CallbackLog ((line) => {
				// MT1111: Application launched successfully, but it's not possible to wait for the app to exit as requested because it's not possible to detect app termination when launching using gdbserver
				waitedForExit &= line?.Contains ("MT1111: ") != true;
				if (line?.Contains ("error MT1007") == true)
					launchFailure = true;
			});
		}

		// parse the run log and decide if we managed to start the process or not
		async Task<(int pid, bool launchFailure)> GetPidFromRunLog () {
			(int pid, bool launchFailure) pidData = (-1, true);
			using var reader = runLog.GetReader (); // diposed at the end of the method, which is what we want
			if (reader.Peek () == -1) {
				// empty file! we definetly had a launch error in this case
				pidData.launchFailure = true;
			} else {
				while (!reader.EndOfStream) {
					var line = await reader.ReadLineAsync ();
					if (line.StartsWith ("Application launched. PID = ", StringComparison.Ordinal)) {
						var pidstr = line.Substring ("Application launched. PID = ".Length);
						if (!int.TryParse (pidstr, out pidData.pid))
							mainLog.WriteLine ("Could not parse pid: {0}", pidstr);
					} else if (line.Contains ("Xamarin.Hosting: Launched ") && line.Contains (" with pid ")) {
						var pidstr = line.Substring (line.LastIndexOf (' '));
						if (!int.TryParse (pidstr, out pidData.pid))
							mainLog.WriteLine ("Could not parse pid: {0}", pidstr);
					} else if (line.Contains ("error MT1008")) {
						pidData.launchFailure = true;
					}
				}
			}
			return pidData;
		}

		// parse the main log to get the pid 
		async Task<int> GetPidFromMainLog ()
		{
			int pid = -1;
			using var log_reader = mainLog.GetReader (); // dispose when we leave the method, which is what we want
			string line;
			while ((line = await log_reader.ReadLineAsync ()) != null) {
				const string str = "was launched with pid '";
				var idx = line.IndexOf (str, StringComparison.Ordinal);
				if (idx > 0) {
					idx += str.Length;
					var next_idx = line.IndexOf ('\'', idx);
					if (next_idx > idx)
						int.TryParse (line.Substring (idx, next_idx - idx), out pid);
				}
				if (pid != -1)
					return pid;
			}
			return pid;
		}

		// return the reason for a crash found in a log
		void GetCrashReason (int pid, ILog crashLog, out string crashReason)
		{
			crashReason = null;
			using var crashReader = crashLog.GetReader (); // dispose when we leave the method
			var text = crashReader.ReadToEnd ();

			var reader = JsonReaderWriterFactory.CreateJsonReader (Encoding.UTF8.GetBytes (text), new XmlDictionaryReaderQuotas ());
			var doc = new XmlDocument ();
			doc.Load (reader);
			foreach (XmlNode node in doc.SelectNodes ($"/root/processes/item[pid = '" + pid + "']")) {
				Console.WriteLine (node?.InnerXml);
				Console.WriteLine (node?.SelectSingleNode ("reason")?.InnerText);
				crashReason = node?.SelectSingleNode ("reason")?.InnerText;
			}
		}

		// return if the tcp connection with the device failed
		async Task<bool> TcpConnectionFailed ()
		{
			using var reader = new StreamReader (mainLog.FullPath);
			string line;
			while ((line = await reader.ReadLineAsync ()) != null) {
				if (line.Contains ("Couldn't establish a TCP connection with any of the hostnames")) {
					return true;
				}
			}
			return false;
		}

		// kill any process 
		Task KillAppProcess (int pid, CancellationTokenSource cancellationSource) { 
			var launchTimedout = cancellationSource.IsCancellationRequested;
			var timeoutType = launchTimedout ? "Launch" : "Completion";
			mainLog.WriteLine ($"{timeoutType} timed out after {timeoutWatch.Elapsed.TotalSeconds} seconds");
			return processManager.KillTreeAsync (pid, mainLog, true);
		}

		async Task CollectResult (Task<ProcessExecutionResult> processExecution)
		{ 
			// wait for the execution of the process, once that is done, perform all the parsing operations and
			// leave a clean API to be used by AppRunner, hidding all the diff details
			var result = await processExecution;
			if (!waitedForExit && !result.TimedOut) {
				// mlaunch couldn't wait for exit for some reason. Let's assume the app exits when the test listener completes.
				mainLog.WriteLine ("Waiting for listener to complete, since mlaunch won't tell.");
				if (!await listener.CompletionTask.TimeoutAfter (timeout - timeoutWatch.Elapsed)) {
					result.TimedOut = true;
				}
			}

			if (result.TimedOut) {
				timedout = true;
				Success = false;
				mainLog.WriteLine (timeoutMessage, timeout.TotalMinutes);
			} else if (result.Succeeded) {
				mainLog.WriteLine (completionMessage);
				Success = true;
			} else {
				mainLog.WriteLine (failureMessage);
				Success = false;
			}
		}

		public void LaunchCallback (Task<bool> launchResult)
		{
			if (launchResult.IsFaulted) {
				mainLog.WriteLine ("Test launch failed: {0}", launchResult.Exception);
			} else if (launchResult.IsCanceled) {
				mainLog.WriteLine ("Test launch was cancelled.");
			} else if (launchResult.Result) {
				mainLog.WriteLine ("Test run started");
			} else {
				cancellationTokenSource.Cancel ();
				mainLog.WriteLine ("Test launch timed out after {0} minute(s).", timeoutWatch.Elapsed.TotalMinutes);
				timedout = true;
			}
		}

		public async Task CollectSimulatorResult (Task<ProcessExecutionResult> processExecution)
		{
			isSimulatorTest = true;
			await CollectResult (processExecution);

			if (!Success.Value) {
				var (pid, launchFailure) = await GetPidFromRunLog ();
				this.launchFailure = launchFailure;
				if (pid > 0) {
					await KillAppProcess (pid, cancellationTokenSource);
				} else {
					mainLog.WriteLine ("Could not find pid in mtouch output.");
				}
			}
		}

		public async Task CollectDeviceResult (Task<ProcessExecutionResult> processExecution)
		{
			isSimulatorTest = false;
			await CollectResult (processExecution);
		}

		async Task<(string ResultLine, bool Failed)> GetResultLine (string logPath)
		{
			string resultLine = null;
			bool failed = false;
			using var reader = new StreamReader (logPath);
			string line = null;
			while ((line = await reader.ReadLineAsync ()) != null) {
				if (line.Contains ("Tests run:")) {
					Console.WriteLine (line);
					resultLine = line;
					break;
				} else if (line.Contains ("[FAIL]")) {
					Console.WriteLine (line);
					failed = true;
				}
			}
			return (ResultLine: resultLine, Failed: failed);
		}

		async Task<(string resultLine, bool failed, bool crashed)> ParseResultFile (AppBundleInformation appInfo, string test_log_path, bool timed_out)
		{
			(string resultLine, bool failed, bool crashed) parseResult = (null, false, false);
			if (!File.Exists (test_log_path)) {
				parseResult.crashed = true; // if we do not have a log file, the test crashes
				return parseResult;
			}
			// parsing the result is different if we are in jenkins or not.
			// When in Jenkins, Touch.Unit produces an xml file instead of a console log (so that we can get better test reporting).
			// However, for our own reporting, we still want the console-based log. This log is embedded inside the xml produced
			// by Touch.Unit, so we need to extract it and write it to disk. We also need to re-save the xml output, since Touch.Unit
			// wraps the NUnit xml output with additional information, which we need to unwrap so that Jenkins understands it.
			// 
			// On the other hand, the nunit and xunit do not have that data and have to be parsed.
			// 
			// This if statement has a small trick, we found out that internet sharing in some of the bots (VSTS) does not work, in
			// that case, we cannot do a TCP connection to xharness to get the log, this is a problem since if we did not get the xml
			// from the TCP connection, we are going to fail when trying to read it and not parse it. Therefore, we are not only
			// going to check if we are in CI, but also if the listener_log is valid.
			var path = Path.ChangeExtension (test_log_path, "xml");
			resultParser.CleanXml (test_log_path, path);

			if (ResultsUseXml && resultParser.IsValidXml (path, out var xmlType)) {
				try {
					var newFilename = resultParser.GetXmlFilePath (path, xmlType);

					// at this point, we have the test results, but we want to be able to have attachments in vsts, so if the format is
					// the right one (NUnitV3) add the nodes. ATM only TouchUnit uses V3.
					var testRunName = $"{appInfo.AppName} {appInfo.Variation}";
					if (xmlType == XmlResultJargon.NUnitV3) {
						var logFiles = new List<string> ();
						// add our logs AND the logs of the previous task, which is the build task
						logFiles.AddRange (Directory.GetFiles (crashLogs.Directory));
						if (additionalLogsDirectory != null) // when using the run command, we do not have a build task, ergo, there are no logs to add.
							logFiles.AddRange (Directory.GetFiles (additionalLogsDirectory));
						// add the attachments and write in the new filename
						// add a final prefix to the file name to make sure that the VSTS test uploaded just pick
						// the final version, else we will upload tests more than once
						newFilename = XmlResultParser.GetVSTSFilename (newFilename);
						resultParser.UpdateMissingData (path, newFilename, testRunName, logFiles);
					} else {
						// rename the path to the correct value
						File.Move (path, newFilename);
					}
					path = newFilename;

					// write the human readable results in a tmp file, which we later use to step on the logs
					var tmpFile = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
					(parseResult.resultLine, parseResult.failed) = resultParser.GenerateHumanReadableResults (path, tmpFile, xmlType);
					File.Copy (tmpFile, test_log_path, true);
					File.Delete (tmpFile);

					// we do not longer need the tmp file
					logs.AddFile (path, LogType.XmlLog.ToString ());
					return parseResult;

				} catch (Exception e) {
					mainLog.WriteLine ("Could not parse xml result file: {0}", e);
					// print file for better debugging
					mainLog.WriteLine ("File data is:");
					mainLog.WriteLine (new string ('#', 10));
					using (var stream = new StreamReader (path)) {
						string line;
						while ((line = await stream.ReadLineAsync ()) != null) {
							mainLog.WriteLine (line);
						}
					}
					mainLog.WriteLine (new string ('#', 10));
					mainLog.WriteLine ("End of xml results.");
					if (timed_out) {
						WrenchLog.WriteLine ($"AddSummary: <b><i>{runMode} timed out</i></b><br/>");
						return parseResult;
					} else {
						WrenchLog.WriteLine ($"AddSummary: <b><i>{runMode} crashed</i></b><br/>");
						mainLog.WriteLine ("Test run crashed");
						parseResult.crashed = true;
						return parseResult;
					}
				}

			}
			// delete not needed copy
			File.Delete (path);

			// not the most efficient way but this just happens when we run
			// the tests locally and we usually do not run all tests, we are
			// more interested to be efficent on the bots
			(parseResult.resultLine, parseResult.failed) = await GetResultLine (test_log_path);
			return parseResult;
		}

		async Task<(bool Succeeded, bool Crashed)> TestsSucceeded (AppBundleInformation appInfo, string test_log_path, bool timed_out)
		{
			var (resultLine, failed, crashed) = await ParseResultFile (appInfo, test_log_path, timed_out);
			// read the parsed logs in a human readable way
			if (resultLine != null) {
				var tests_run = resultLine.Replace ("Tests run: ", "");
				if (failed) {
					WrenchLog.WriteLine ("AddSummary: <b>{0} failed: {1}</b><br/>", runMode, tests_run);
					mainLog.WriteLine ("Test run failed");
					return (false, crashed);
				} else {
					WrenchLog.WriteLine ("AddSummary: {0} succeeded: {1}<br/>", runMode, tests_run);
					mainLog.WriteLine ("Test run succeeded");
					return (true, crashed);
				}
			} else if (timed_out) {
				WrenchLog.WriteLine ("AddSummary: <b><i>{0} timed out</i></b><br/>", runMode);
				return (false, false);
			} else {
				WrenchLog.WriteLine ("AddSummary: <b><i>{0} crashed</i></b><br/>", runMode);
				mainLog.WriteLine ("Test run crashed");
				return (false, true);
			}
		}

		// generate all the xml failures that will help the integration with the CI and return the failure reason
		async Task GenerateXmlFailures (string failureMessage, bool crashed, string crashReason)
		{
			using var mainLogReader = mainLog.GetReader ();
			if (!ResultsUseXml) // nothing to do
				return;
			if (!string.IsNullOrEmpty (crashReason)) {
				resultParser.GenerateFailure (
					logs,
					"crash",
					appInfo.AppName,
					appInfo.Variation,
					$"App Crash {appInfo.AppName} {appInfo.Variation}",
					$"App crashed: {failureMessage}",
					mainLogReader,
					xmlJargon);
			} else if (launchFailure) {
				resultParser.GenerateFailure (
					logs,
					"launch",
					appInfo.AppName,
					appInfo.Variation,
					$"App Launch {appInfo.AppName} {appInfo.Variation} on {deviceName}",
					$"{failureMessage} on {deviceName}",
					mainLogReader,
					xmlJargon);
			} else if (!isSimulatorTest && crashed && string.IsNullOrEmpty (crashReason)) {
				// this happens more that what we would like on devices, the main reason most of the time is that we have had netwoking problems and the
				// tcp connection could not be stablished. We are going to report it as an error since we have not parsed the logs, evne when the app might have
				// not crashed. We need to check the main_log to see if we do have an tcp issue or not
				if (await TcpConnectionFailed ()) {
					resultParser.GenerateFailure (
						logs,
						"tcp-connection",
						appInfo.AppName,
						appInfo.Variation,
						$"TcpConnection on {deviceName}",
						$"Device {deviceName} could not reach the host over tcp.",
						mainLogReader,
						xmlJargon);
				}
			} else if (timedout) {
				resultParser.GenerateFailure (
					logs,
					"timeout",
					appInfo.AppName,
					appInfo.Variation,
					$"App Timeout {appInfo.AppName} {appInfo.Variation} on bot {deviceName}",
					$"{appInfo.AppName} {appInfo.Variation} Test run timed out after {timeout.TotalMinutes} minute(s) on bot {deviceName}.",
					mainLogReader,
					xmlJargon);
			}
		}

		public async Task<(TestExecutingResult ExecutingResult, string FailureMessage)> ParseResult ()
		{
			var result = (ExecutingResult: TestExecutingResult.Finished, FailureMessage: (string) null);
			var crashed = false;
			if (File.Exists (listener.TestLog.FullPath)) {
				WrenchLog.WriteLine ("AddFile: {0}", listener.TestLog.FullPath);
				(Success, crashed) = await TestsSucceeded (appInfo, listener.TestLog.FullPath, timedout);
			} else if (timedout) {
				WrenchLog.WriteLine ("AddSummary: <b><i>{0} never launched</i></b><br/>", runMode);
				mainLog.WriteLine ("Test run never launched");
				Success = false;
			} else if (launchFailure) {
 				WrenchLog.WriteLine ("AddSummary: <b><i>{0} failed to launch</i></b><br/>", runMode);
 				mainLog.WriteLine ("Test run failed to launch");
 				Success = false;
			} else {
				WrenchLog.WriteLine ("AddSummary: <b><i>{0} crashed at startup (no log)</i></b><br/>", runMode);
				mainLog.WriteLine ("Test run crashed before it started (no log file produced)");
				crashed = true;
				Success = false;
			}
				
			if (!Success.HasValue)
				Success = false;

			var crashLogWaitTime = 0;
			if (!Success.Value)
				crashLogWaitTime = 5;
			if (crashed)
				crashLogWaitTime = 30;

			await crashReporter.EndCaptureAsync (TimeSpan.FromSeconds (crashLogWaitTime));

			if (timedout) {
				result.ExecutingResult = TestExecutingResult.TimedOut;
			} else if (crashed) {
				result.ExecutingResult = TestExecutingResult.Crashed;
			} else if (Success.Value) {
				result.ExecutingResult = TestExecutingResult.Succeeded;
			} else {
				result.ExecutingResult = TestExecutingResult.Failed;
			}

			// Check crash reports to see if any of them explains why the test run crashed.
			if (!Success.Value) {
				int pid = -1;
				string crashReason = null;
				foreach (var crashLog in crashLogs) {
					try {
						logs.Add (crashLog);

						if (pid == -1) {
							// Find the pid
							pid = await GetPidFromMainLog ();
						}

						GetCrashReason (pid, crashLog, out crashReason);
						if (crashReason != null) 
							break;
					} catch (Exception e) {
						var message = string.Format ("Failed to process crash report '{1}': {0}", e.Message, crashLog.Description);
						mainLog.WriteLine (message);
						exceptionLogger?.Invoke (2, message);
					}
				}
				if (!string.IsNullOrEmpty (crashReason)) {
					if (crashReason == "per-process-limit") {
						result.FailureMessage = "Killed due to using too much memory (per-process-limit).";
					} else {
						result.FailureMessage = $"Killed by the OS ({crashReason})";
					}
				} else if (launchFailure) {
					// same as with a crash
					result.FailureMessage = $"Launch failure";
				} 
				await GenerateXmlFailures (result.FailureMessage, crashed, crashReason);
			}
			return result;
		}

	}
}
