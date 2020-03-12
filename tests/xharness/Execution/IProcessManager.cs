using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Logging;

namespace Xharness.Execution {

	public class ProcessExecutionResult {
		public bool TimedOut { get; set; }
		public int ExitCode { get; set; }

		public bool Succeeded => !TimedOut && ExitCode == 0;
	}

	// interface that helps to manage the different processes in the app.
	public interface IProcessManager {
		Task<ProcessExecutionResult> ExecuteCommandAsync (string filename, IList<string> args, ILog log, TimeSpan timeout, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null);
		Task<ProcessExecutionResult> RunAsync (Process process, ILog log, CancellationToken? cancellationToken = null, bool? diagnostics = null);
		Task<ProcessExecutionResult> RunAsync (Process process, ILog log, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null);
		Task<ProcessExecutionResult> RunAsync (Process process, MlaunchArguments args, ILog log, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null);
		Task<ProcessExecutionResult> RunAsync (Process process, ILog log, ILog stdoutLog, ILog stderrLog, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null);
		Task<ProcessExecutionResult> RunAsync (Process process, ILog log, TextWriter StdoutStream, TextWriter StderrStream, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null, CancellationToken? cancellation_token = null, bool? diagnostics = null);
		Task<bool> WaitForExitAsync (Process process, TimeSpan? timeout = null);
		Task KillTreeAsync (Process process, ILog log, bool? diagnostics = true);
		Task KillTreeAsync (int pid, ILog log, bool? diagnostics = true);
		void GetChildrenPS (ILog log, List<int> list, int pid);
	}
}
