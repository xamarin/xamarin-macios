// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Xml.Linq;
using Microsoft.DotNet.XHarness.Common;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.XmlResults;
using Xharness.Jenkins.TestTasks;

namespace Xamarin.Tests {
	[TestFixture]
	public class InstallLogReaderTests {
		const string FailureMessage = "Install failed on Test device, exit code: 42";

		[TestCase (true)]
		[TestCase (false)]
		public void MissingLogGeneratesFailure (bool directoryExists)
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var installDirectory = Path.Combine (directory, "install");
			if (directoryExists)
				Directory.CreateDirectory (installDirectory);

			using var logs = new Logs (directory);
			using var mainLog = new MemoryLog ();
			using var installLog = new AppInstallMonitorLog (new LogFile ("Install log", Path.Combine (installDirectory, "install.log")));
			using var reader = InstallLogReader.Open (installLog, mainLog);
			var report = GenerateFailure (logs, reader);

			Assert.That (mainLog.ToString (), Does.Contain (installLog.FullPath).And.Contain ("is unavailable"));
			Assert.That (report.ToString (), Does.Contain (installLog.FullPath).And.Contain ("is unavailable"));
			Assert.That (File.Exists (installLog.FullPath), Is.False, "Reporting must not create a replacement install log.");
		}

		[Test]
		public void ExistingLogIsFlushedBeforeGeneratingFailure ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			using var logs = new Logs (directory);
			using var mainLog = new MemoryLog ();
			using var installLog = new AppInstallMonitorLog (logs.Create ("install.log", "Install log"));
			const string diagnostic = "The device rejected the installation.";
			installLog.WriteLine (diagnostic);

			using var reader = InstallLogReader.Open (installLog, mainLog);
			var report = GenerateFailure (logs, reader);

			Assert.That (report.ToString (), Does.Contain (diagnostic));
			Assert.That (mainLog.ToString (), Is.Empty);
		}

		[Test]
		public void EmptyLogGeneratesFailure ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			using var logs = new Logs (directory);
			using var mainLog = new MemoryLog ();
			using var installLog = new AppInstallMonitorLog (logs.Create ("install.log", "Install log"));
			File.WriteAllText (installLog.FullPath, "");

			using var reader = InstallLogReader.Open (installLog, mainLog);
			GenerateFailure (logs, reader);

			Assert.That (mainLog.ToString (), Is.Empty);
		}

		[Test]
		public void OtherReadErrorsAreNotSuppressed ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			using var mainLog = new MemoryLog ();
			using var installLog = new AppInstallMonitorLog (new LogFile ("Install log", directory));

			Assert.That (() => InstallLogReader.Open (installLog, mainLog), Throws.TypeOf<UnauthorizedAccessException> ());
			Assert.That (mainLog.ToString (), Is.Empty);
		}

		static XDocument GenerateFailure (Logs logs, TextReader reader)
		{
			new XmlResultParser ().GenerateFailure (
				logs, "install", "Test app", null, "AppInstallation on Test device",
				FailureMessage, reader, XmlResultJargon.NUnitV3);

			var report = XDocument.Load (logs.Single (log => log.Description == "XmlLog").FullPath);
			var testCase = report.Descendants ("test-case").Single ();
			Assert.That (testCase.Attribute ("result")?.Value, Is.EqualTo ("Failed"));
			Assert.That (testCase.Element ("failure")?.Element ("message")?.Value, Does.Contain (FailureMessage));
			return report;
		}
	}
}
