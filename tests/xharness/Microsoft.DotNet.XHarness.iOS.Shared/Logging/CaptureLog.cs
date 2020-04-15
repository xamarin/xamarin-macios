using System;
using System.IO;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	
	public interface ICaptureLogFactory {
		ICaptureLog Create (string logDirectory, string systemLogPath, bool entireFile, string description = null);
	}

	public class CaptureLogFactory : ICaptureLogFactory {
		public ICaptureLog Create (string logDirectory, string systemLogPath, bool entireFile, string description = null)
		{
			return new CaptureLog (logDirectory, systemLogPath, entireFile) {
				Description = description
			};
		}
	}

	public interface ICaptureLog : ILog {
		void StartCapture ();
		void StopCapture ();
	}

	// A log that captures data written to a separate file between two moments in time
	// (between StartCapture and StopCapture).
	public class CaptureLog : Log, ICaptureLog {
		readonly bool entireFile;

		long startPosition;
		long endPosition;
		bool started;

		public string CapturePath { get; }
		public override string FullPath { get; }

		public CaptureLog (string path, string capture_path, bool entireFile = false)
			: base ()
		{
			FullPath = path ?? throw new ArgumentNullException (nameof (path));
			CapturePath = capture_path ?? throw new ArgumentNullException (nameof (path));
			this.entireFile = entireFile;
		}

		public void StartCapture ()
		{
			if (entireFile)
				return;

			if (File.Exists (CapturePath))
				startPosition = new FileInfo (CapturePath).Length;
			started = true;
		}

		public void StopCapture ()
		{
			if (!started && !entireFile)
				throw new InvalidOperationException ("StartCapture most be called before StopCature on when the entire file will be captured.");
			if (!File.Exists (CapturePath)) {
				File.WriteAllText (FullPath, $"Could not capture the file '{CapturePath}' because it doesn't exist.");
				return;
			}

			if (entireFile) {
				File.Copy (CapturePath, FullPath, true);
				return;
			}

			endPosition = new FileInfo (CapturePath).Length;

			Capture ();
		}

		void Capture ()
		{
			if (startPosition == 0 || entireFile)
				return;

			if (!File.Exists (CapturePath)) {
				File.WriteAllText (FullPath, $"Could not capture the file '{CapturePath}' because it does not exist.");
				return;
			}

			var currentEndPosition = endPosition;
			if (currentEndPosition == 0)
				currentEndPosition = new FileInfo (CapturePath).Length;

			var length = (int) (currentEndPosition - startPosition);
			var currentLength = new FileInfo (CapturePath).Length;
			var capturedLength = 0L;

			if (length < 0) {
				// The file shrank? lets copy the entire file in this case, which is better than nothing
				File.Copy (CapturePath, FullPath, true);
				return;
			}

			if (File.Exists (FullPath))
				capturedLength = new FileInfo (FullPath).Length;

			// capture 1k more data than when we stopped, since the system log
			// is cached in memory and flushed once in a while (so when the app
			// requests the system log to be captured, it's usually not complete).
			var availableLength = currentLength - startPosition;
			if (availableLength <= capturedLength)
				return; // We've captured before, and nothing new as added since last time.

			// Capture at most 1k more
			availableLength = Math.Min (availableLength, length + 1024);

			using (var reader = new FileStream (CapturePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var writer = new FileStream (FullPath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				var buffer = new byte [4096];
				reader.Position = startPosition;
				while (availableLength > 0) {
					int read = reader.Read (buffer, 0, Math.Min (buffer.Length, length));
					if (read > 0) {
						writer.Write (buffer, 0, read);
						availableLength -= read;
					} else {
						// There's nothing more to read.
						// I can't see how we get here, since we calculate the amount to read based on what's available, but it does happen randomly.
						break;
					}
				}
			}
		}

		public override StreamReader GetReader ()
		{
			return File.Exists (CapturePath)
				? new StreamReader (new FileStream (CapturePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				: null;
		}

		public override void Flush ()
		{
			Capture ();
		}

		protected override void WriteImpl (string value) => throw new InvalidOperationException ();

		public override void Dispose ()
		{
		}
	}
}
