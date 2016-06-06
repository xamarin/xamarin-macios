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

		public static async Task RunAsync (this Process process, string outputFile, bool append)
		{
			Directory.CreateDirectory (Path.GetDirectoryName (outputFile));
			using (var stream = new StreamWriter (outputFile, append))
				await RunAsync (process, stream, stream);
		}

		public static async Task RunAsync (this Process process, StreamWriter StdoutStream, StreamWriter StderrStream)
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
					StdoutStream.WriteLine (e.Data);
				} else {
					stdout_completion.SetResult (true);
				}
			};

			process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
			{
				if (e.Data != null) {
					lock (StderrStream)
						StderrStream.WriteLine (e.Data);
				} else {
					stderr_completion.SetResult (true);
				}
			};

			process.Start ();

			process.BeginErrorReadLine ();
			process.BeginOutputReadLine ();

			new Thread (() =>
			{
				process.WaitForExit ();
				exit_completion.SetResult (true);
			}) {
				IsBackground = true,
			}.Start ();

			await Task.WhenAll (stderr_completion.Task, stdout_completion.Task, exit_completion.Task);
		}
	}
}
