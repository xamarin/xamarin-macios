using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace xharness
{
	public abstract class Log : IDisposable
	{
		public string Description;
		public bool Timestamp;

		protected Log ()
		{
		}

		protected Log (string description)
		{
			Description = description;
		}

		public abstract string FullPath { get; }

		protected abstract void WriteImpl (string value);

		public virtual StreamReader GetReader ()
		{
			throw new NotSupportedException ();
		}

		public virtual TextWriter GetWriter ()
		{
			throw new NotSupportedException ();
		}

		public void Write (string value)
		{
			if (Timestamp)
				value = DateTime.Now.ToString ("HH:mm:ss.fffffff") + " " + value;
			WriteImpl (value);
		}

		public void WriteLine (string value)
		{
			Write (value + "\n");
		}

		public void WriteLine (string format, params object [] args)
		{
			Write (string.Format (format, args) + "\n");
		}

		public override string ToString ()
		{
			return Description;
		}

		public virtual void Flush ()
		{
		}

#region IDisposable Support
		protected virtual void Dispose (bool disposing)
		{
		}

		public void Dispose ()
		{
			Dispose (true);
		}
#endregion
	}

	public class LogFile : Log
	{
		public string Path;
		StreamWriter writer;

		public LogFile (string description, string path)
			: base (description)
		{
			Path = path;
		}

		protected override void WriteImpl (string value)
		{
			lock (this) {
				using (var str = new FileStream (Path, FileMode.Append, FileAccess.Write, FileShare.Read)) {
					using (var writer = new StreamWriter (str)) {
						writer.Write (value);
						writer.Flush ();
					}
				}
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

		public override TextWriter GetWriter ()
		{
			return writer ?? (writer = new StreamWriter (new FileStream (Path, FileMode.Open, FileAccess.Write, FileShare.Read)));
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			if (writer != null) {
				writer.Dispose ();
				writer = null;
			}
		}
	}

	public class LogStream : Log
	{
		string path;
		FileStream fs;
		StreamWriter writer;

		public FileStream FileStream {
			get {
				return fs;
			}
		}

		public override StreamReader GetReader ()
		{
			return new StreamReader (new FileStream (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}

		public override TextWriter GetWriter ()
		{
			return writer ?? (writer = new StreamWriter (fs));
		}

		public LogStream (string description, string path)
			: base (description)
		{
			this.path = path;

			fs = new FileStream (path, FileMode.Create, FileAccess.Write, FileShare.Read);
		}

		protected override void WriteImpl (string value)
		{
			var w = GetWriter ();
			w.Write (value);
			w.Flush ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			if (writer != null) {
				writer.Dispose ();
				writer = null;
			}

			if (fs != null) {
				fs.Dispose ();
				fs = null;
			}
		}

		public override string FullPath {
			get {
				return path;
			}
		}
	}

	public class Logs : List<Log>
	{
		public LogStream CreateStream (string directory, string filename, string name)
		{
			Directory.CreateDirectory (directory);
			var rv = new LogStream (name, Path.GetFullPath (Path.Combine (directory, filename)));
			Add (rv);
			return rv;
		}

		public LogFile CreateFile (string description, string path)
		{
			var rv = new LogFile (description, path);
			Add (rv);
			return rv;
		}
	}

	public class ConsoleLog : Log
	{
		protected override void WriteImpl (string value)
		{
			Console.Write (value);
		}

		public override string FullPath {
			get {
				throw new NotSupportedException ();
			}
		}

		public override TextWriter GetWriter ()
		{
			return Console.Out;
		}
	}

	public class CaptureLog : Log
	{
		public string CapturePath { get; private set; }
		public string Path { get; set; }

		long startPosition;
		long endPosition;

		public CaptureLog (string capture_path)
		{
			CapturePath = capture_path;
		}

		public void StartCapture ()
		{
			if (File.Exists (CapturePath))
				startPosition = new FileInfo (CapturePath).Length;
		}

		public void StopCapture ()
		{
			endPosition = new FileInfo (CapturePath).Length;

			Capture ();
		}

		void Capture ()
		{
			if (startPosition == 0)
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
}

