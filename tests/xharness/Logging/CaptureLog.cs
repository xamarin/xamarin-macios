using System;
using System.IO;

namespace Xharness.Logging {

	// A log that captures data written to a separate file between two moments in time
	// (between StartCapture and StopCapture).
	public class CaptureLog : Log
	{
		public readonly string CapturePath;
		public readonly string Path;

		long startPosition;
		long endPosition;
		bool entireFile;
		bool started;

		public CaptureLog (ILogs logs, string path, string capture_path, bool entire_file = false)
			: base (logs)
		{

			Path = path ?? throw new ArgumentNullException (nameof (path));
			CapturePath = capture_path ?? throw new ArgumentNullException (nameof (path));
			this.entireFile = entire_file;
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
				File.WriteAllText (Path, $"Could not capture the file '{CapturePath}' because it doesn't exist.");
				return;
			}

			if (entireFile) {
				File.Copy (CapturePath, Path, true);
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
				File.WriteAllText (Path, $"Could not capture the file '{CapturePath}' because it does not exist.");
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
				File.Copy (CapturePath, Path, true);
				return;
			}

			if (File.Exists (Path))
				capturedLength = new FileInfo (Path).Length;

			// capture 1k more data than when we stopped, since the system log
			// is cached in memory and flushed once in a while (so when the app
			// requests the system log to be captured, it's usually not complete).
			var availableLength = currentLength - startPosition;
			if (availableLength <= capturedLength)
				return; // We've captured before, and nothing new as added since last time.

			// Capture at most 1k more
			availableLength = Math.Min (availableLength, length + 1024);

			using (var reader = new FileStream (CapturePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				using (var writer = new FileStream (Path, FileMode.Create, FileAccess.Write, FileShare.Read)) {
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
		}

		public override StreamReader GetReader ()
		{
			if (File.Exists (CapturePath)) {
				return new StreamReader (new FileStream (CapturePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			} else {
				return new StreamReader (new MemoryStream ());
			}
		}

		public override void Flush ()
		{
			base.Flush ();

			Capture ();
		}

		public override void WriteImpl (string value)
		{
			throw new InvalidOperationException ();
		}

		public override string FullPath {
			get {
				return Path;
			}
		}
	}
}
