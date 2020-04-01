using System;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness {
	public interface ICrashSnapshotReporterFactory {
		ICrashSnapshotReporter Create (ILog log, ILogs logs, bool isDevice, string deviceName);
	}

	public class CrashSnapshotReporterFactory : ICrashSnapshotReporterFactory {
		readonly IProcessManager processManager;

		public CrashSnapshotReporterFactory (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public ICrashSnapshotReporter Create (ILog log, ILogs logs, bool isDevice, string deviceName) =>
			new CrashSnapshotReporter (processManager, log, logs, isDevice, deviceName);
	}
}
