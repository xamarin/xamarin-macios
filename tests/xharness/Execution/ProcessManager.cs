using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Logging;
using Xharness.Utilities;

namespace Xharness.Execution {
	public class ProcessManager : IProcessManager {
		public ProcessManager ()
		{
		}

		public async Task<ProcessExecutionResult> ExecuteCommandAsync (string filename, IList<string> args, ILog log, TimeSpan timeout, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null)
		{
			using (var p = new Process ()) {
				p.StartInfo.FileName = filename;
				p.StartInfo.Arguments = StringUtils.FormatArguments (args);
				return await RunAsync (p, log, timeout, environment_variables, cancellation_token);
			}
		}

		[DllImport ("/usr/lib/libc.dylib")]
		internal static extern int kill (int pid, int sig);

		public static Task<bool> PollForExitAsync (int pid, TimeSpan timeout)
		{
			var rv = new TaskCompletionSource<bool> ();
			var watch = new Stopwatch ();
			watch.Start ();
			Task.Run (async () => {
				while (watch.ElapsedMilliseconds < timeout.TotalMilliseconds) {
					if (kill (pid, 0) != 0) {
						// pid is not valid anymore, program exited
						rv.SetResult (true);
						return;
					}
					await Task.Delay (TimeSpan.FromMilliseconds (100));
				}

				rv.SetResult (false);
			});
			return rv.Task;
		}

		public async Task<ProcessExecutionResult> RunAsync (Process process, ILog log, CancellationToken? cancellation_token = null, bool? diagnostics = null)
		{
			return await RunAsync (process, log, log, log, cancellation_token: cancellation_token, diagnostics: diagnostics);
		}

		public Task<ProcessExecutionResult> RunAsync (Process process, ILog log, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null)
		{
			return RunAsync (process, log, log, log, timeout, environment_variables, cancellation_token, diagnostics);
		}

		public Task<ProcessExecutionResult> RunAsync (Process process, MlaunchArguments args, ILog log, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null)
		{
			process.StartInfo.Arguments = args.AsCommandLine ();
			return RunAsync (process, log, timeout, environment_variables, cancellation_token, diagnostics);
		}

		public Task<ProcessExecutionResult> RunAsync (Process process, ILog log, ILog stdoutLog, ILog stderrLog, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null)
		{
			if (stdoutLog is TextWriter StdoutStream && stderrLog is TextWriter StderrStream) {
				return RunAsync (process, log, StdoutStream, StderrStream, timeout, environment_variables, cancellation_token, diagnostics);
			} else {
				throw new ArgumentException ("Could not cast ILog to TextWriter.");
			}
		}

		public async Task<ProcessExecutionResult> RunAsync (Process process, ILog log, TextWriter StdoutStream, TextWriter StderrStream, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null)
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

			if (environment_variables != null) {
				foreach (var kvp in environment_variables)
					process.StartInfo.EnvironmentVariables [kvp.Key] = kvp.Value;
			}

			process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
			{
				if (e.Data != null) {
					lock (StdoutStream) {
						StdoutStream.WriteLine (e.Data);
						StdoutStream.Flush ();
					}
				} else {
					stdout_completion.TrySetResult (true);
				}
			};

			process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
			{
				if (e.Data != null) {
					lock (StderrStream) {
						StderrStream.WriteLine (e.Data);
						StderrStream.Flush ();
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

			cancellation_token?.Register (() => {
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
					StderrStream.WriteLine ($"Execution of {pid} was cancelled.");
					kill (pid, 9);
				}
			});

			if (timeout.HasValue) {
				if (!await WaitForExitAsync (process, timeout.Value)) {
					await KillTreeAsync (process, log, diagnostics ?? true);
					rv.TimedOut = true;
					lock (StderrStream)
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

		public async Task<bool> WaitForExitAsync (Process process, TimeSpan? timeout = null)
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

		public Task KillTreeAsync (Process process, ILog log, bool? diagnostics = true)
		{
			return KillTreeAsync (process.Id, log, diagnostics);
		}

		public async Task KillTreeAsync (int pid, ILog log, bool? diagnostics = true)
		{
			var pids = new List<int> ();
			GetChildrenPS (log, pids, pid);
			if (diagnostics == true) {
				log.WriteLine ($"Pids to kill: {string.Join (", ", pids.Select ((v) => v.ToString ()).ToArray ())}");
				using (var ps = new Process ()) {
					log.WriteLine ("Writing process list:");
					ps.StartInfo.FileName = "ps";
					ps.StartInfo.Arguments = "-A -o pid,ruser,ppid,pgid,%cpu=%CPU,%mem=%MEM,flags=FLAGS,lstart,rss,vsz,tty,state,time,command";
					await RunAsync (ps, log, TimeSpan.FromSeconds (5), diagnostics: false);
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
							await RunAsync (dbg, log, TimeSpan.FromSeconds (30), diagnostics: false);
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

		public void GetChildrenPS (ILog log, List<int> list, int pid)
		{
			string stdout;

			using (Process ps = new Process ()) {
				ps.StartInfo.FileName = "ps";
				ps.StartInfo.Arguments = "-eo ppid,pid";
				ps.StartInfo.UseShellExecute = false;
				ps.StartInfo.RedirectStandardOutput = true;
				ps.Start ();
				stdout = ps.StandardOutput.ReadToEnd ();

				if (!ps.WaitForExit (1000)) {
					log.WriteLine ("ps didn't finish in a reasonable amount of time (1 second).");
					return;
				}

				if (ps.ExitCode != 0)
					return;

				stdout = stdout.Trim ();

				if (string.IsNullOrEmpty (stdout))
					return;

				var dict = new Dictionary<int, List<int>> ();
				foreach (string line in stdout.Split (new char [] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)) {
					var l = line.Trim ();
					var space = l.IndexOf (' ');
					if (space <= 0)
						continue;
				
					var parent = l.Substring (0, space);
					var process = l.Substring (space + 1);
					int parent_id, process_id;

					if (int.TryParse (parent, out parent_id) && int.TryParse (process, out process_id)) {
						List<int> children;
						if (!dict.TryGetValue (parent_id, out children))
							dict [parent_id] = children = new List<int> ();
						children.Add (process_id);
					}
				}

				var queue = new Queue<int> ();
				queue.Enqueue (pid);

				do {
					List<int> children;
					var parent_id = queue.Dequeue ();
					list.Add (parent_id);
					if (dict.TryGetValue (parent_id, out children)) {
						foreach (var child in children)
							queue.Enqueue (child);
					}
				} while (queue.Count > 0);
			}
		}
	}
}
