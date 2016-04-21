//
// ProcessUtils.cs
//
// Author:
//       Michael Hutchinson <mhutch@xamarin.com>
//       Jérémie Laval <jeremie.laval@xamarin.com>
//
// Copyright (c) 2012 Xamarin, Inc.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.MacDev.Tasks
{
	public static class ProcessUtils
	{
		public static Task<int> StartProcess (ProcessStartInfo psi, TextWriter stdout, TextWriter stderr, CancellationToken cancellationToken = default (CancellationToken))
		{
			var tcs = new TaskCompletionSource<int> ();
			if (cancellationToken.CanBeCanceled && cancellationToken.IsCancellationRequested) {
				tcs.TrySetCanceled ();
				return tcs.Task;
			}

			psi.UseShellExecute = false;
			psi.RedirectStandardOutput = stdout != null;
			psi.RedirectStandardError = stderr != null;

			var p = Process.Start (psi);

			if (cancellationToken.CanBeCanceled) {
				cancellationToken.Register (() => {
					try {
						if (!p.HasExited)
							p.Kill ();
					} catch (InvalidOperationException ex) {
						if (ex.Message.IndexOf ("already exited", StringComparison.Ordinal) < 0)
							throw;
					}
				});
			}

			bool outputDone = false;
			bool errorDone = false;
			bool exitDone = false;

			p.EnableRaisingEvents = true;
			if (psi.RedirectStandardOutput) {
				bool stdOutInitialized = false;
				p.OutputDataReceived += (sender, e) => {
					try {
						if (e.Data == null) {
							outputDone = true;
							if (exitDone && errorDone)
								tcs.TrySetResult (p.ExitCode);
							return;
						}

						if (stdOutInitialized)
							stdout.WriteLine ();
						stdout.Write (e.Data);
						stdOutInitialized = true;
					} catch (Exception ex) {
						tcs.TrySetException (ex);
					}
				};
				p.BeginOutputReadLine ();
			} else {
				outputDone = true;
			}

			if (psi.RedirectStandardError) {
				bool stdErrInitialized = false;
				p.ErrorDataReceived += (sender, e) => {
					try {
						if (e.Data == null) {
							errorDone = true;
							if (exitDone && outputDone)
								tcs.TrySetResult (p.ExitCode);
							return;
						}

						if (stdErrInitialized)
							stderr.WriteLine ();
						stderr.Write (e.Data);
						stdErrInitialized = true;
					} catch (Exception ex) {
						tcs.TrySetException (ex);
					}
				};
				p.BeginErrorReadLine ();
			} else {
				errorDone = true;
			}

			p.Exited += (sender, e) => {
				exitDone = true;
				if (errorDone && outputDone)
					tcs.TrySetResult (p.ExitCode);
			};

			return tcs.Task;
		}
	}
}
