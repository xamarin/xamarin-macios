using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace xharness
{
	public class ProcessExecutionResult
	{
		public bool TimedOut { get; set; }
		public int ExitCode { get; set; }

		public bool Succeeded { get { return !TimedOut && ExitCode == 0; } }
	}

	public static class ProcessHelper
	{
		public static Task<ProcessExecutionResult> ExecuteCommandAsync (string filename, string args, Log log, TimeSpan timeout, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null)
		{
			return ExecuteCommandAsync (filename, args, log.GetWriter (), timeout, environment_variables, cancellation_token);
		}

		public static async Task<ProcessExecutionResult> ExecuteCommandAsync (string filename, string args, string outputPath, TimeSpan timeout, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null)
		{
			Directory.CreateDirectory (Path.GetDirectoryName (outputPath));
			using (var fs = new FileStream (outputPath, FileMode.Append, FileAccess.Write, FileShare.Read)) {
				using (var stream = new StreamWriter (fs))
					return await ExecuteCommandAsync (filename, args, stream, timeout, environment_variables, cancellation_token);
			}
		}

		public static async Task<ProcessExecutionResult> ExecuteCommandAsync (string filename, string args, TextWriter output, TimeSpan timeout, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null)
		{
			using (var p = new Process ()) {
				p.StartInfo.FileName = filename;
				p.StartInfo.Arguments = args;
				return await p.RunAsync (output, output, timeout, environment_variables, cancellation_token);
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
	}

	public static class Process_Extensions
	{
		public static async Task<ProcessExecutionResult> RunAsync (this Process process, Log log, CancellationToken? cancellation_token = null)
		{
			var stream = log.GetWriter ();
			return await RunAsync (process, stream, stream, cancellation_token: cancellation_token);
		}

		public static async Task<ProcessExecutionResult> RunAsync (this Process process, string outputFile, bool append, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null)
		{
			Directory.CreateDirectory (Path.GetDirectoryName (outputFile));
			using (var fs = new FileStream (outputFile, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read)) {
				using (var stream = new StreamWriter (fs))
					return await RunAsync (process, stream, stream, timeout, environment_variables, cancellation_token);
			}
		}

		public static Task<ProcessExecutionResult> RunAsync (this Process process, Log log, bool append, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null)
		{
			var writer = log.GetWriter ();
			return RunAsync (process, writer, writer, timeout, environment_variables, cancellation_token);
		}

		public static async Task<ProcessExecutionResult> RunAsync (this Process process, TextWriter StdoutStream, TextWriter StderrStream, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null)
		{
			var stdout_completion = new TaskCompletionSource<bool> ();
			var stderr_completion = new TaskCompletionSource<bool> ();
			var exit_completion = new TaskCompletionSource<bool> ();
			var rv = new ProcessExecutionResult ();

			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
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
					stdout_completion.SetResult (true);
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
					stderr_completion.SetResult (true);
				}
			};


			StdoutStream.WriteLine ("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
			process.Start ();

			process.BeginErrorReadLine ();
			process.BeginOutputReadLine ();

			if (cancellation_token.HasValue) {
				cancellation_token.Value.Register (() => {
					if (exit_completion.TrySetCanceled ())
						ProcessHelper.kill (process.Id, 9);
				});
			}

			new Thread (() =>
			{
				if (timeout.HasValue) {
					if (!process.WaitForExit ((int) timeout.Value.TotalMilliseconds)) {
						ProcessHelper.kill (process.Id, 9);
						process.WaitForExit ((int) TimeSpan.FromSeconds (5).TotalMilliseconds); // Wait 5s for the kill to work
						rv.TimedOut = true;
						lock (StderrStream)
							StderrStream.WriteLine ($"Execution timed out after {timeout.Value.TotalSeconds} seconds and the process was killed.");
					}
				} else {
					process.WaitForExit ();
				}
				exit_completion.TrySetResult (true);
			}) {
				IsBackground = true,
			}.Start ();

			await Task.WhenAll (stderr_completion.Task, stdout_completion.Task, exit_completion.Task);

			rv.ExitCode = process.ExitCode;
			return rv;
		}
	}
}
