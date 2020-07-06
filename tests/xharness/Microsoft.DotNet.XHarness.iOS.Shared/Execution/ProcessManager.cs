using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Execution {
	public class ProcessManager : IProcessManager {
		static readonly Lazy<string> autoDetectedXcodeRoot = new Lazy<string>(DetectXcodePath, LazyThreadSafetyMode.PublicationOnly);

		readonly string xcodeRoot;
		public string XcodeRoot => xcodeRoot ?? autoDetectedXcodeRoot.Value;
		public string MlaunchPath { get; }
		public GetDotNetExecutableDelegate GetDotNetExecutable { get; private set; }
		public string MSBuildPath { get; private set; }

		Version xcode_version;
		public Version XcodeVersion {
			get {
				if (xcode_version == null) {
					var doc = new XmlDocument ();
					doc.Load (Path.Combine (XcodeRoot, "Contents", "version.plist"));
					xcode_version = Version.Parse (doc.SelectSingleNode ("//key[text() = 'CFBundleShortVersionString']/following-sibling::string").InnerText);
				}
				return xcode_version;
			}
		}

		public ProcessManager (string xcodeRoot = null, string mlaunchPath = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch", GetDotNetExecutableDelegate dotNetPath = null, string msBuildPath = null)
		{
			this.xcodeRoot = xcodeRoot;
			MlaunchPath = mlaunchPath;
			GetDotNetExecutable = dotNetPath;
			MSBuildPath = msBuildPath;
		}

		public async Task<ProcessExecutionResult> ExecuteCommandAsync (string filename,
			IList<string> args,
			ILog log,
			TimeSpan timeout,
			Dictionary<string, string> environmentVariables = null,
			CancellationToken? cancellationToken = null)
		{
			using var p = new Process ();
			p.StartInfo.FileName = filename ?? throw new ArgumentNullException (nameof (filename));
			p.StartInfo.Arguments = StringUtils.FormatArguments (args);
			return await RunAsync (p, log, timeout, environmentVariables, cancellationToken);
		}

		public async Task<ProcessExecutionResult> ExecuteCommandAsync (MlaunchArguments args,
			ILog log,
			TimeSpan timeout,
			Dictionary<string, string> environmentVariables = null,
			CancellationToken? cancellationToken = null)
		{
			using var p = new Process ();
			return await RunAsync (p, args, log, timeout, environmentVariables, cancellationToken);
		}

		public async Task<ProcessExecutionResult> ExecuteXcodeCommandAsync (string executable, IList<string> args, ILog log, TimeSpan timeout)
		{
			using var p = new Process ();
			p.StartInfo.FileName = Path.Combine (XcodeRoot, "Contents", "Developer", "usr", "bin", executable);
			p.StartInfo.Arguments = StringUtils.FormatArguments (args);
			return await RunAsync (p, log, timeout, diagnostics: false);
		}

		[DllImport ("/usr/lib/libc.dylib")]
		internal static extern int kill (int pid, int sig);

		public Task<ProcessExecutionResult> RunAsync (Process process,
			ILog log,
			TimeSpan? timeout = null,
			Dictionary<string, string> environment_variables = null,
			CancellationToken? cancellationToken = null,
			bool? diagnostics = null)
		{
			return RunAsync (process, log, log, log, timeout, environment_variables, cancellationToken, diagnostics);
		}

		public Task<ProcessExecutionResult> RunAsync (Process process,
			MlaunchArguments args,
			ILog log,
			TimeSpan? timeout = null,
			Dictionary<string, string> environmentVariables = null,
			CancellationToken? cancellationToken = null,
			bool? diagnostics = null)
		{
			if (!args.Any (a => a is SdkRootArgument))
				args.Prepend (new SdkRootArgument (XcodeRoot));

			process.StartInfo.FileName = MlaunchPath;
			process.StartInfo.Arguments = args.AsCommandLine ();

			return RunAsync (process, log, timeout, environmentVariables, cancellationToken, diagnostics);
		}

		public Task<ProcessExecutionResult> RunAsync (Process process,
			ILog log,
			ILog stdout,
			ILog stderr,
			TimeSpan? timeout = null,
			Dictionary<string, string> environmentVariables = null,
			CancellationToken? cancellationToken = null,
			bool? diagnostics = null)
		{
			return RunAsyncInternal (process, log, stdout, stderr, timeout, environmentVariables, cancellationToken, diagnostics);
		}

		public Task KillTreeAsync (Process process, ILog log, bool? diagnostics = true)
		{
			return KillTreeAsyncInternal (process.Id, log, diagnostics);
		}

		public Task KillTreeAsync (int pid, ILog log, bool? diagnostics = true)
		{
			return KillTreeAsyncInternal (pid, log, diagnostics);
		}

		static async Task<ProcessExecutionResult> RunAsyncInternal (Process process,
			ILog log,
			ILog stdout,
			ILog stderr,
			TimeSpan? timeout = null,
			Dictionary<string, string> environmentVariables = null,
			CancellationToken? cancellationToken = null,
			bool? diagnostics = null)
		{
			var stdout_completion = new TaskCompletionSource<bool> ();
			var stderr_completion = new TaskCompletionSource<bool> ();
			var rv = new ProcessExecutionResult ();

			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
			// Make cute emojiis show up as cute emojiis in the output instead of ugly text symbols!
			process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
			process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
			process.StartInfo.UseShellExecute = false;

			if (environmentVariables != null) {
				foreach (var kvp in environmentVariables) {
					if (kvp.Value == null) {
						process.StartInfo.EnvironmentVariables.Remove (kvp.Key);
					} else {
						process.StartInfo.EnvironmentVariables [kvp.Key] = kvp.Value;
					}
				}
			}

			process.OutputDataReceived += (sender, e) => {
				if (e.Data != null) {
					lock (stdout) {
						stdout.WriteLine (e.Data);
						stdout.Flush ();
					}
				} else {
					stdout_completion.TrySetResult (true);
				}
			};

			process.ErrorDataReceived += (sender, e) => {
				if (e.Data != null) {
					lock (stderr) {
						stderr.WriteLine (e.Data);
						stderr.Flush ();
					}
				} else {
					stderr_completion.TrySetResult (true);
				}
			};

			var sb = new StringBuilder ();
			if (process.StartInfo.EnvironmentVariables != null) {
				var currentEnvironment = Environment.GetEnvironmentVariables ().Cast<System.Collections.DictionaryEntry> ().ToDictionary ((v) => (string) v.Key, (v) => (string) v.Value, StringComparer.Ordinal);
				var processEnvironment = process.StartInfo.EnvironmentVariables.Cast<System.Collections.DictionaryEntry> ().ToDictionary ((v) => (string) v.Key, (v) => (string) v.Value, StringComparer.Ordinal);
				var allKeys = currentEnvironment.Keys.Union (processEnvironment.Keys).Distinct ();
				foreach (var key in allKeys) {
					string a = null, b = null;
					currentEnvironment.TryGetValue (key, out a);
					processEnvironment.TryGetValue (key, out b);
					if (a != b)
						sb.Append ($"{key}={StringUtils.Quote (b)} ");
				}
			}
			sb.Append ($"{StringUtils.Quote (process.StartInfo.FileName)} {process.StartInfo.Arguments}");
			log.WriteLine (sb);

			process.Start ();
			var pid = process.Id;

			process.BeginErrorReadLine ();
			process.BeginOutputReadLine ();

			cancellationToken?.Register (() => {
				var hasExited = false;
				try {
					hasExited = process.HasExited;
				} catch {
					// Process.HasExited can sometimes throw exceptions, so
					// just ignore those and to be safe treat it as the
					// process didn't exit (the safe option being to not leave
					// processes behind).
				}
				if (!hasExited) {
					stderr.WriteLine ($"Execution of {pid} was cancelled.");
					kill (pid, 9);
				}
			});

			if (timeout.HasValue) {
				if (!await WaitForExitAsync (process, timeout.Value)) {
					await KillTreeAsyncInternal (process.Id, log, diagnostics ?? true);
					rv.TimedOut = true;
					lock (stderr)
						log.WriteLine ($"{pid} Execution timed out after {timeout.Value.TotalSeconds} seconds and the process was killed.");
				}
			}
			await WaitForExitAsync (process);
			Task.WaitAll (new Task [] { stderr_completion.Task, stdout_completion.Task }, TimeSpan.FromSeconds (1));

			try {
				rv.ExitCode = process.ExitCode;
			} catch (Exception e) {
				rv.ExitCode = 12345678;
				log.WriteLine ($"Failed to get ExitCode: {e}");
			}
			return rv;
		}

		static async Task KillTreeAsyncInternal (int pid, ILog log, bool? diagnostics = true)
		{
			var pids = GetChildrenPS (log, pid);

			if (diagnostics == true) {
				log.WriteLine ($"Pids to kill: {string.Join (", ", pids.Select ((v) => v.ToString ()).ToArray ())}");
				using (var ps = new Process ()) {
					log.WriteLine ("Writing process list:");
					ps.StartInfo.FileName = "ps";
					ps.StartInfo.Arguments = "-A -o pid,ruser,ppid,pgid,%cpu=%CPU,%mem=%MEM,flags=FLAGS,lstart,rss,vsz,tty,state,time,command";
					await RunAsyncInternal (ps, log, log, log, TimeSpan.FromSeconds (5), diagnostics: false);
				}

				foreach (var diagnose_pid in pids) {
					var template = Path.GetTempFileName ();
					try {
						var commands = new StringBuilder ();
						using (var dbg = new Process ()) {
							commands.AppendLine ($"process attach --pid {diagnose_pid}");
							commands.AppendLine ("thread list");
							commands.AppendLine ("thread backtrace all");
							commands.AppendLine ("detach");
							commands.AppendLine ("quit");
							dbg.StartInfo.FileName = "/usr/bin/lldb";
							dbg.StartInfo.Arguments = StringUtils.FormatArguments ("--source", template);
							File.WriteAllText (template, commands.ToString ());

							log.WriteLine ($"Printing backtrace for pid={pid}");
							await RunAsyncInternal (dbg, log, log, log, TimeSpan.FromSeconds (30), diagnostics: false);
						}
					} finally {
						try {
							File.Delete (template);
						} catch {
							// Don't care
						}
					}
				}
			}

			// Send SIGABRT since that produces a crash report
			// lldb may fail to attach to system processes, but crash reports will still be produced with potentially helpful stack traces.
			for (int i = 0; i < pids.Count; i++)
				kill (pids [i], 6);

			// send kill -9 anyway as a last resort
			for (int i = 0; i < pids.Count; i++)
				kill (pids [i], 9);
		}

		static async Task<bool> WaitForExitAsync (Process process, TimeSpan? timeout = null)
		{
			if (process.HasExited)
				return true;

			var tcs = new TaskCompletionSource<bool> ();

			void ProcessExited (object sender, EventArgs ea)
			{
				process.Exited -= ProcessExited;
				tcs.TrySetResult (true);
			}

			process.Exited += ProcessExited;
			process.EnableRaisingEvents = true;

			// Check if process exited again, in case it exited after we checked
			// the last time, but before we attached the event handler.
			if (process.HasExited) {
				process.Exited -= ProcessExited;
				tcs.TrySetResult (true);
				return true;
			}

			if (timeout.HasValue) {
				return await tcs.Task.TimeoutAfter (timeout.Value);
			} else {
				await tcs.Task;
				return true;
			}
		}

		static List<int> GetChildrenPS (ILog log, int pid)
		{
			var list = new List<int> ();

			using (Process ps = new Process ()) {
				ps.StartInfo.FileName = "ps";
				ps.StartInfo.Arguments = "-eo ppid,pid";
				ps.StartInfo.UseShellExecute = false;
				ps.StartInfo.RedirectStandardOutput = true;
				ps.Start ();

				string stdout = ps.StandardOutput.ReadToEnd ();

				if (!ps.WaitForExit (1000)) {
					log.WriteLine ("ps didn't finish in a reasonable amount of time (1 second).");
					return list;
				}

				if (ps.ExitCode != 0)
					return list;

				stdout = stdout.Trim ();

				if (string.IsNullOrEmpty (stdout))
					return list;

				var dict = new Dictionary<int, List<int>> ();
				foreach (string line in stdout.Split (new char [] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)) {
					var l = line.Trim ();
					var space = l.IndexOf (' ');
					if (space <= 0)
						continue;

					var parent = l.Substring (0, space);
					var process = l.Substring (space + 1);

					if (int.TryParse (parent, out var parent_id) && int.TryParse (process, out var process_id)) {
						if (!dict.TryGetValue (parent_id, out var children))
							dict [parent_id] = children = new List<int> ();

						children.Add (process_id);
					}
				}

				var queue = new Queue<int> ();
				queue.Enqueue (pid);

				do {
					var parent_id = queue.Dequeue ();
					list.Add (parent_id);
					if (dict.TryGetValue (parent_id, out var children)) {
						foreach (var child in children)
							queue.Enqueue (child);
					}
				} while (queue.Count > 0);
			}

			return list;
		}

		static string DetectXcodePath ()
		{
			using var process = new Process ();
			process.StartInfo.FileName = "xcode-select";
			process.StartInfo.Arguments = "-p";

			var log = new MemoryLog ();
			var stdout = new MemoryLog () { Timestamp = false };
			var stderr = new ConsoleLog ();
			var timeout = TimeSpan.FromSeconds (30);

			var result = RunAsyncInternal (process, log, stdout, stderr, timeout).GetAwaiter ().GetResult ();

			if (!result.Succeeded)
				throw new Exception ("Failed to detect Xcode path from xcode-select!");

			// Something like /Applications/Xcode114.app/Contents/Developers
			var xcodeRoot = stdout.ToString ().Trim ();

			if (string.IsNullOrEmpty (xcodeRoot))
				throw new Exception ("Failed to detect Xcode path from xcode-select!");

			// We need /Applications/Xcode114.app only
			return Path.GetDirectoryName(Path.GetDirectoryName(xcodeRoot));
		}
	}
}
