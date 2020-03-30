using System;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

using ExceptionLogger = System.Action<int, string>;

namespace Xharness {
	public interface ITestReporterFactory {
		ITestReporter Create (ILog mainLog,
			ILog runLog,
			ILogs logs,
			ICrashSnapshotReporter crashSnapshotReporter,
			ISimpleListener simpleListener,
			IResultParser parser,
			AppBundleInformation appInformation,
			RunMode runMode,
			XmlResultJargon xmlJargon,
			string device,
			TimeSpan timeout,
			double launchTimeout,
			string additionalLogsDirectory = null,
			Action<int, string> exceptionLogger = null);
	}

	public class TestReporterFactory : ITestReporterFactory {
		readonly IProcessManager processManager;

		public TestReporterFactory (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public ITestReporter Create (ILog mainLog,
			ILog runLog,
			ILogs logs,
			ICrashSnapshotReporter crashReporter,
			ISimpleListener simpleListener,
			IResultParser parser,
			AppBundleInformation appInformation,
			RunMode runMode,
			XmlResultJargon xmlJargon,
			string device,
			TimeSpan timeout,
			double launchTimeout,
			string additionalLogsDirectory = null,
			ExceptionLogger exceptionLogger = null)
		{
			return new TestReporter (processManager,
				mainLog,
				runLog,
				logs,
				crashReporter,
				simpleListener,
				parser,
				appInformation,
				runMode,
				xmlJargon,
				device,
				timeout,
				launchTimeout,
				additionalLogsDirectory,
				exceptionLogger);
		}
	}
}

