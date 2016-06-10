using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace xharness
{
	public static class Process_Extensions
	{
		public static async Task RunAsync (this Process process, LogFile log)
		{
			using (var stream = new StreamWriter (log.Path, false))
				await RunAsync (process, stream, stream);
		}

		public static async Task RunAsync (this Process process, string outputFile, bool append, TimeSpan? timeout = null)
		{
			Directory.CreateDirectory (Path.GetDirectoryName (outputFile));
			using (var fs = new FileStream (outputFile, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read)) {
				using (var stream = new StreamWriter (fs))
					await RunAsync (process, stream, stream, timeout);
			}
		}

		public static async Task RunAsync (this Process process, StreamWriter StdoutStream, StreamWriter StderrStream, TimeSpan? timeout = null)
		{
			var stdout_completion = new TaskCompletionSource<bool> ();
			var stderr_completion = new TaskCompletionSource<bool> ();
			var exit_completion = new TaskCompletionSource<bool> ();

			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;

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

			process.Start ();

			process.BeginErrorReadLine ();
			process.BeginOutputReadLine ();

			new Thread (() =>
			{
				if (timeout.HasValue) {
					if (!process.WaitForExit ((int) timeout.Value.TotalMilliseconds)) {
						process.Kill ();
						process.WaitForExit ((int) 5); // Wait 5s for the kill to work, just
						exit_completion.SetException (new TimeoutException { Timeout = timeout.Value });
					} else {
						exit_completion.SetResult (true);
					}
				} else {
					process.WaitForExit ();
					exit_completion.SetResult (true);
				}
			}) {
				IsBackground = true,
			}.Start ();

			await Task.WhenAll (stderr_completion.Task, stdout_completion.Task, exit_completion.Task);
		}
	}

	class TimeoutException : Exception
	{
		public TimeSpan Timeout;
	}
}
