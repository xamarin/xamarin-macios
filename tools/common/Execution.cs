/*
 * Copyright 2014 Xamarin Inc. All rights reserved.
 * Copyright 2019, 2020 Microsoft Corp. All rights reserved.
 *
 * Authors:
 *   Rolf Bjarne Kvinge <rolf@xamarin.com>
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Xamarin.Utils {
	public class ExecutionOutput {
		public bool Complete { get; internal set; }

		List<(bool IsError, string Line)> lines = new ();

		void VerifyComplete ()
		{
			if (!Complete)
				throw new InvalidOperationException ("Cannot read output before execution is complete.");
		}

		void VerifyNotComplete ()
		{
			if (Complete)
				throw new InvalidOperationException ("Cannot write output after execution is complete.");
		}

		public IEnumerable<string> StandardOutputLines {
			get {
				VerifyComplete ();
				foreach (var (isError, line) in lines) {
					if (!isError)
						yield return line;
				}
			}
		}

		public IEnumerable<string> StandardErrorLines {
			get {
				VerifyComplete ();
				foreach (var (isError, line) in lines) {
					if (isError)
						yield return line;
				}
			}
		}

		public IList<string> MergedOutputLines {
			get {
				VerifyComplete ();
				return lines.Select (line => line.Line).ToList ();
			}
		}

		public string StandardOutput {
			get {
				VerifyComplete ();
				var sb = new StringBuilder ();
				foreach (var line in StandardOutputLines) {
					sb.AppendLine (line);
				}
				return sb.ToString ();
			}
		}

		public string StandardError {
			get {
				VerifyComplete ();
				var sb = new StringBuilder ();
				foreach (var line in StandardErrorLines) {
					sb.AppendLine (line);
				}
				return sb.ToString ();
			}
		}

		public string MergedOutput {
			get {
				VerifyComplete ();
				var sb = new StringBuilder ();
				foreach (var (isError, line) in lines) {
					sb.AppendLine (line);
				}
				return sb.ToString ();
			}
		}

		public void WriteStandardOutput (string line)
		{
			VerifyNotComplete ();
			lock (lines)
				lines.Add ((false, line));
		}

		public void WriteStandardError (string line)
		{
			VerifyNotComplete ();
			lock (lines)
				lines.Add ((true, line));
		}
	}

	public class Execution {
		public string? FileName;
		public IList<string>? Arguments;
		public IDictionary<string, string?>? Environment;
		public string? WorkingDirectory;
		public TimeSpan? Timeout;
		public CancellationToken? CancellationToken;
		public bool CloseStandardInput;

		public TextWriter? Log;

		public int ExitCode { get; private set; }
		public bool TimedOut { get; private set; }

		public Action<string>? StandardOutputLineCallback;
		public Action<string>? StandardErrorLineCallback;
		public Action<Process>? TimeoutCallback;

		public ExecutionOutput Output { get; private set; } = new ExecutionOutput ();

		public TimeSpan Duration { get; private set; }

		static Thread StartOutputThread (TaskCompletionSource<Execution> tcs, object lockobj, StreamReader reader, Action<string> writer, string thread_name)
		{
			var thread = new Thread (() => {
				try {
					while (reader.ReadLine () is string line) {
						lock (lockobj)
							writer (line);
					}
				} catch (Exception e) {
					tcs.TrySetException (e);
				} finally {
					// The Process instance doesn't dispose these streams, which means we need to do it,
					// otherwise we can run out of file descriptors while waiting for the GC to kick in.
					// Ref: https://bugzilla.xamarin.com/show_bug.cgi?id=43462
					reader.Dispose ();
				}
			}) {
				IsBackground = true,
				Name = thread_name,
			};
			thread.Start ();
			return thread;
		}

		static void KillProcess (Process p)
		{
#if NET
			p.Kill (true);
#else
			p.Kill ();
#endif
		}

		public Task<Execution> RunAsync ()
		{
			var tcs = new TaskCompletionSource<Execution> ();
			var lockobj = new object ();

			try {
				var p = new Process ();
				p.StartInfo.FileName = FileName;
				p.StartInfo.Arguments = Arguments is not null ? StringUtils.FormatArguments (Arguments) : "";
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardInput = CloseStandardInput;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;
				if (!string.IsNullOrEmpty (WorkingDirectory))
					p.StartInfo.WorkingDirectory = WorkingDirectory;

				// mtouch/mmp writes UTF8 data outside of the ASCII range, so we need to make sure
				// we read it in the same format. This also means we can't use the events to get
				// stdout/stderr, because mono's Process class parses those using Encoding.Default.
				p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
				p.StartInfo.StandardErrorEncoding = Encoding.UTF8;

				if (Environment is not null) {
					foreach (var kvp in Environment) {
						if (kvp.Value is null) {
							p.StartInfo.EnvironmentVariables.Remove (kvp.Key);
						} else {
							p.StartInfo.EnvironmentVariables [kvp.Key] = kvp.Value;
						}
					}
				}

				StandardErrorLineCallback ??= Output.WriteStandardError;
				StandardOutputLineCallback ??= Output.WriteStandardOutput;

				var thread = new Thread (() => {
					try {
						if (Log is not null) {
							if (!string.IsNullOrEmpty (p.StartInfo.WorkingDirectory))
								Log.Write ($"cd {StringUtils.Quote (p.StartInfo.WorkingDirectory!)} && ");
							Log.WriteLine ("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
						}

						var stopwatch = Stopwatch.StartNew ();
						p.Start ();
						if (CloseStandardInput)
							p.StandardInput.Close ();
						var pid = p.Id;

						var stdoutThread = StartOutputThread (tcs, lockobj, p.StandardOutput, StandardOutputLineCallback, $"StandardOutput reader for {p.StartInfo.FileName} (PID: {pid})");
						var stderrThread = StartOutputThread (tcs, lockobj, p.StandardError, StandardErrorLineCallback, $"StandardError reader for {p.StartInfo.FileName} (PID: {pid})");

						CancellationToken?.Register (() => {
							// Don't call tcs.TrySetCanceled, that won't return an Execution result to the caller.
							try {
								Log?.WriteLine ($"Command '{p.StartInfo.FileName} {p.StartInfo.Arguments}' (pid: {pid}) was cancelled, and will be killed.");
								KillProcess (p);
							} catch (Exception ex) {
								// The process could be disposed already. Just ignore any exceptions here.
								Log?.WriteLine ($"Failed to cancel and kill PID {pid}: {ex.Message}");
							}
						});

						if (Timeout.HasValue) {
							if (!p.WaitForExit ((int) Timeout.Value.TotalMilliseconds)) {
								Log?.WriteLine ($"Command '{p.StartInfo.FileName} {p.StartInfo.Arguments}' (pid: {pid}) didn't finish in {Timeout.Value.TotalMilliseconds} ms, and will be killed.");
								TimedOut = true;
								try {
									TimeoutCallback?.Invoke (p);
								} catch (Exception ex) {
									(Log ?? Console.Error).WriteLine ($"Timeout callback failed for PID {pid}: {ex}");
								}
								try {
									KillProcess (p);
								} catch (Exception ex) {
									// According to the documentation, there can be exceptions here we can't prepare for, so just ignore them.
									Log?.WriteLine ($"Failed to kill PID {pid}: {ex.Message}");
								}
							}
						}
						// Always call this WaitForExit overload to be make sure the stdout/stderr buffers have been flushed,
						// even if we've called the WaitForExit (int) overload
						p.WaitForExit ();
						ExitCode = p.ExitCode;
						Duration = stopwatch.Elapsed;

						stdoutThread.Join (TimeSpan.FromSeconds (1));
						stderrThread.Join (TimeSpan.FromSeconds (1));
						Output.Complete = true;
						tcs.TrySetResult (this);
					} catch (Exception e) {
						tcs.TrySetException (e);
					} finally {
						p.Dispose ();
					}
				}) {
					IsBackground = true,
					Name = $"Thread waiting for {p.StartInfo.FileName} to finish",
				};
				thread.Start ();
			} catch (Exception e) {
				tcs.TrySetException (e);
			}

			return tcs.Task;
		}

		public static Task<Execution> RunWithCallbacksAsync (string filename, IList<string> arguments, Dictionary<string, string?>? environment = null, Action<string>? standardOutput = null, Action<string>? standardError = null, TextWriter? log = null, string? workingDirectory = null, TimeSpan? timeout = null, CancellationToken? cancellationToken = null, bool closeStandardInput = false)
		{
			return new Execution {
				FileName = filename,
				Arguments = arguments,
				Environment = environment,
				StandardOutputLineCallback = standardOutput,
				StandardErrorLineCallback = standardError,
				WorkingDirectory = workingDirectory,
				CancellationToken = cancellationToken,
				Timeout = timeout,
				Log = log,
				CloseStandardInput = closeStandardInput,
			}.RunAsync ();
		}

		public static Task<Execution> RunWithTextWritersAsync (string filename, IList<string> arguments, Dictionary<string, string?>? environment = null, TextWriter? standardOutput = null, TextWriter? standardError = null, TextWriter? log = null, string? workingDirectory = null, TimeSpan? timeout = null, CancellationToken? cancellationToken = null)
		{
			return new Execution {
				FileName = filename,
				Arguments = arguments,
				Environment = environment,
				StandardOutputLineCallback = standardOutput is null ? null : standardOutput.WriteLine,
				StandardErrorLineCallback = standardError is null ? null : standardError.WriteLine,
				WorkingDirectory = workingDirectory,
				CancellationToken = cancellationToken,
				Timeout = timeout,
				Log = log,
			}.RunAsync ();
		}

		public static Task<Execution> RunAsync (string filename, IList<string> arguments, Dictionary<string, string?>? environment = null, TextWriter? log = null, string? workingDirectory = null, TimeSpan? timeout = null, CancellationToken? cancellationToken = null)
		{
			return new Execution {
				FileName = filename,
				Arguments = arguments,
				Environment = environment,
				WorkingDirectory = workingDirectory,
				CancellationToken = cancellationToken,
				Timeout = timeout,
				Log = log,
			}.RunAsync ();
		}

		[Obsolete ("Use 'RunAsync' instead.")]
		public static async Task<Execution> RunWithStringBuildersAsync (string filename, IList<string> arguments, Dictionary<string, string?>? environment = null, StringBuilder? standardOutput = null, StringBuilder? standardError = null, TextWriter? log = null, string? workingDirectory = null, TimeSpan? timeout = null, CancellationToken? cancellationToken = null)
		{
			var rv = await RunAsync (filename, arguments, environment, log, workingDirectory, timeout, cancellationToken);
			if (standardOutput is not null) {
				if (standardError == standardOutput) {
					standardOutput.Append (rv.Output.MergedOutput);
				} else {
					standardOutput.Append (rv.Output.StandardOutput);
					if (standardError is not null) {
						standardError.Append (rv.Output.StandardError);
					}
				}
			} else if (standardError is not null) {
				standardError.Append (rv.Output.StandardError);
			}
			return rv;
		}
	}
}
