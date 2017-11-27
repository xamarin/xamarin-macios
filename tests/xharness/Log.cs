using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace xharness
{
	public abstract class Log : TextWriter
	{
		public Logs Logs { get; private set; }
		public string Description;
		public bool Timestamp;

		protected Log (Logs logs)
		{
			Logs = logs;
		}

		protected Log (Logs logs, string description)
		{
			Logs = logs;
			Description = description;
		}

		public abstract string FullPath { get; }

		protected virtual void WriteImpl (string value)
		{
			Write (Encoding.UTF8.GetBytes (value));
		}

		public virtual void Write (byte [] buffer)
		{
			Write (buffer, 0, buffer.Length);
		}

		public virtual void Write (byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		public virtual StreamReader GetReader ()
		{
			throw new NotSupportedException ();
		}

		public override void Write (string value)
		{
			if (Timestamp)
				value = DateTime.Now.ToString ("HH:mm:ss.fffffff") + " " + value;
			WriteImpl (value);
		}

		public override void WriteLine (string value)
		{
			Write (value + "\n");
		}

		public override void WriteLine (string format, params object [] args)
		{
			Write (string.Format (format, args) + "\n");
		}

		public override string ToString ()
		{
			return Description;
		}

		public override void Flush ()
		{
		}

		public override Encoding Encoding {
			get {
				return System.Text.Encoding.UTF8;
			}
		}

		public static Log CreateAggregatedLog (params Log [] logs)
		{
			return new AggregatedLog (logs);
		}

		// Log that will duplicate log output to multiple other logs.
		class AggregatedLog : Log
		{
			Log [] logs;

			public AggregatedLog (params Log [] logs)
				: base (null)
			{
				this.logs = logs;
			}

			public override string FullPath => throw new NotImplementedException ();

			protected override void WriteImpl (string value)
			{
				foreach (var log in logs)
					log.WriteImpl (value);
			}

			public override void Write (byte [] buffer, int offset, int count)
			{
				foreach (var log in logs)
					log.Write (buffer, offset, count);
			}
		}
	}

	public class LogFile : Log
	{
		object lock_obj = new object ();
		public string Path { get; private set; }
		FileStream writer;
		bool disposed;

		public LogFile (Logs logs, string description, string path, bool append = true)
			: base (logs, description)
		{
			Path = path;
			if (!append)
				File.WriteAllText (path, string.Empty);
		}

		public override void Write (byte [] buffer, int offset, int count)
		{
			try {
				// We don't want to open the file every time someone writes to the log, so we keep it as an instance
				// variable until we're disposed. Due to the async nature of how we run tests, writes may still
				// happen after we're disposed, in which case we create a temporary stream we close after writing
				lock (lock_obj) {
					var fs = writer;
					if (fs == null) {
						fs = new FileStream (Path, FileMode.Append, FileAccess.Write, FileShare.Read);
					}
					fs.Write (buffer, offset, count);
					if (disposed) {
						fs.Dispose ();
					} else {
						writer = fs;
					}
				}
			} catch (Exception e) {
				Console.WriteLine ($"Failed to write to the file {Path}: {e.Message}.");
				return;
			}
		}

		public override string FullPath {
			get {
				return Path;
			}
		}

		public override StreamReader GetReader ()
		{
			return new StreamReader (new FileStream (Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			if (writer != null) {
				writer.Dispose ();
				writer = null;
			}

			disposed = true;
		}
	}

	public class Logs : List<Log>, IDisposable
	{
		public string Directory;

		public Logs (string directory)
		{
			Directory = directory;
		}

		// Create a new log backed with a file
		public LogFile Create (string filename, string name)
		{
			return Create (Directory, filename, name);
		}

		LogFile Create (string directory, string filename, string name)
		{
			System.IO.Directory.CreateDirectory (directory);
			var rv = new LogFile (this, name, Path.GetFullPath (Path.Combine (directory, filename)));
			Add (rv);
			return rv;
		}

		// Adds an existing file to this collection of logs.
		// If the file is not inside the log directory, then it's copied there.
		// 'path' must be a full path to the file.
		public LogFile AddFile (string path)
		{
			return AddFile (path, Path.GetFileName (path));
		}

		// Adds an existing file to this collection of logs.
		// If the file is not inside the log directory, then it's copied there.
		// 'path' must be a full path to the file.
		public LogFile AddFile (string path, string name)
		{
			if (!path.StartsWith (Directory, StringComparison.Ordinal)) {
				var newPath = Path.Combine (Directory, Path.GetFileNameWithoutExtension (path) + "-" + Harness.Timestamp + Path.GetExtension (path));
				File.Copy (path, newPath, true);
				path = newPath;
			}

			var log = new LogFile (this, name, path, true);
			Add (log);
			return log;
		}

		// Create an empty file in the log directory and return the full path to the file
		public string CreateFile (string path, string description)
		{
			using (var rv = new LogFile (this, description, Path.Combine (Directory, path), false)) {
				Add (rv);
				return rv.FullPath;
			}
		}

		public void Dispose ()
		{
			foreach (var log in this)
				log.Dispose ();
		}
	}

	// A log that writes to standard output
	public class ConsoleLog : Log
	{
		StringBuilder captured = new StringBuilder ();

		public ConsoleLog ()
			: base (null)
		{
		}

		protected override void WriteImpl (string value)
		{
			captured.Append (value);
			Console.Write (value);
		}

		public override string FullPath {
			get {
				throw new NotSupportedException ();
			}
		}

		public override StreamReader GetReader ()
		{
			var str = new MemoryStream (System.Text.Encoding.UTF8.GetBytes (captured.ToString ()));
			return new StreamReader (str, System.Text.Encoding.UTF8, false);
		}
	}

	// A log that captures data written to a separate file between two moments in time
	// (between StartCapture and StopCapture).
	public class CaptureLog : Log
	{
		public string CapturePath { get; private set; }
		public string Path { get; set; }

		long startPosition;
		long endPosition;
		bool entire_file;

		public CaptureLog (Logs logs, string capture_path, bool entire_file = false)
			: base (logs)
		{
			CapturePath = capture_path;
			this.entire_file = entire_file;
		}

		public void StartCapture ()
		{
			if (entire_file)
				return;

			if (File.Exists (CapturePath))
				startPosition = new FileInfo (CapturePath).Length;
		}

		public void StopCapture ()
		{
			if (!File.Exists (CapturePath)) {
				File.WriteAllText (Path, $"Could not capture the file '{CapturePath}' because it doesn't exist.");
				return;
			}

			if (entire_file) {
				File.Copy (CapturePath, Path, true);
				return;
			}

			endPosition = new FileInfo (CapturePath).Length;

			Capture ();
		}

		void Capture ()
		{
			if (startPosition == 0 || entire_file)
				return;
			
			var currentEndPosition = endPosition;
			if (currentEndPosition == 0)
				currentEndPosition = new FileInfo (CapturePath).Length;
			
			var length = (int) (currentEndPosition - startPosition);
			var currentLength = new FileInfo (CapturePath).Length;
			var capturedLength = 0L;

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

		public override void Flush ()
		{
			base.Flush ();

			Capture ();
		}

		protected override void WriteImpl (string value)
		{
			throw new InvalidOperationException ();
		}

		public override string FullPath {
			get {
				return Path;
			}
		}
	}

	// A log that forwards all written data to a callback
	public class CallbackLog : Log
	{
		public Action<string> OnWrite;

		public CallbackLog (Action<string> onWrite)
			: base (null)
		{
			OnWrite = onWrite;
		}

		public override string FullPath => throw new NotImplementedException ();

		protected override void WriteImpl (string value)
		{
			OnWrite (value);
		}
	}
}

