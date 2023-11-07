using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

#nullable enable
namespace Xharness.Jenkins {

	/// <summary>
	/// Represent a command that will be executed periodically.
	/// </summary>
	class PeriodicCommand {

		readonly string command;
		readonly string? arguments;
		readonly TimeSpan interval;
		readonly IProcessManager processManager;
		readonly ILog log;

		public PeriodicCommand (string command, IProcessManager processManager, TimeSpan interval, ILogs logs, string? arguments = null)
		{
			if (logs is null)
				throw new ArgumentNullException (nameof (logs));

			this.log = logs.Create ("PeriodicCommand.log", "Periodic command log");
			this.command = command ?? throw new ArgumentNullException (nameof (command));
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.interval = interval;
			this.arguments = arguments;
		}

		async Task ExecuteInternal (CancellationToken? cancellationToken = null)
		{
			log.WriteLine ($"Starting periodic task with interval {interval.TotalMinutes} minutes.");
			while (true) {
				var watch = Stopwatch.StartNew ();
				using (var process = new Process ()) {
					process.StartInfo.FileName = command;
					process.StartInfo.Arguments = arguments;
					ProcessExecutionResult? rv = cancellationToken.HasValue
						? await processManager.RunAsync (process, log, timeout: interval, cancellationToken: cancellationToken)
						: await processManager.RunAsync (process, log, timeout: interval);
					if (rv is not null && !rv.Succeeded)
						log.WriteLine ($"Periodic command failed with exit code {rv.ExitCode} (Timed out: {rv.TimedOut})");
				}
				var ticksLeft = watch.ElapsedTicks - interval.Ticks;
				if (ticksLeft < 0)
					ticksLeft = interval.Ticks;
				var wait = TimeSpan.FromTicks (ticksLeft);
				await Task.Delay (wait);
			}
		}

		public Task Execute (CancellationToken? cancellationToken = null)
			=> cancellationToken is not null
				? Task.Run (async () => await ExecuteInternal (cancellationToken.Value), cancellationToken.Value)
				: Task.Run (async () => await ExecuteInternal ());
	}
}
