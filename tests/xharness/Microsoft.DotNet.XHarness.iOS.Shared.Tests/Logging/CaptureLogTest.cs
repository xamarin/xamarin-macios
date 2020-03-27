using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Logging {

	[TestFixture]
	public class CaptureLogTest {

		string filePath;
		string capturePath;
		Mock<ILogs> logs;

		[SetUp]
		public void SetUp ()
		{
			filePath = Path.GetTempFileName ();
			capturePath = Path.GetTempFileName ();
			logs = new Mock<ILogs> ();
			File.Delete (filePath);
			File.Delete (capturePath);
		}

		[TearDown]
		public void TearDown ()
		{
			if (File.Exists (filePath))
				File.Delete (filePath);
		}

		

		[Test]
		public void ConstructorNullFilePath ()
		{
			Assert.Throws<ArgumentNullException> (() => {
				var captureLog = new CaptureLog (null, filePath, false);
			});
		}


		[Test]
		public void CaptureRightOrder ()
		{
			var ignoredLine = "This lines should not be captured";
			var logLines = new [] { "first line", "second line", "thrid line" };
			using (var stream = File.Create (filePath))
			using (var writer = new StreamWriter (stream)) {
				writer.WriteLine (ignoredLine);
			}
			using (var captureLog = new CaptureLog (capturePath, filePath, false)) {
				captureLog.StartCapture ();
				using (var writer = new StreamWriter (filePath)) {
					foreach (var line in logLines)
						writer.WriteLine (line);
				}
				captureLog.StopCapture ();
				// get the stream and assert we do have the correct lines
				using (var captureStream = captureLog.GetReader ()) {
					string line;
					while ((line = captureStream.ReadLine ()) != null) {
						Assert.Contains (line, logLines, "Lines not captured");
					}
				}
			}
		}

		[Test]
		public void CaptureMissingFileTest ()
		{
			using (var captureLog = new CaptureLog (capturePath, filePath, false)) {
				Assert.AreEqual (capturePath, captureLog.FullPath, "capture path");
				captureLog.StartCapture ();
				captureLog.StopCapture ();
			}
			// read the data that was added to the capture path and  ensure that we do have the name of the missing file
			using (var reader = new StreamReader (capturePath)) {
				var line = reader.ReadLine ();
				StringAssert.Contains (filePath, line, "file path missing");
			}
		}

		[Test]
		public void CaptureWrongOrder ()
		{
			Assert.Throws<InvalidOperationException> (() => {
				using (var captureLog = new CaptureLog (capturePath, filePath, false)) {
					captureLog.StopCapture ();
				}
			});
		}

		[Test]
		public void CaptureWrongOrderEntirePath ()
		{
			Assert.DoesNotThrow (() => {
				using (var captureLog = new CaptureLog (capturePath, filePath, true)) {
					captureLog.StopCapture ();
				}
			});
		}
	}
}
