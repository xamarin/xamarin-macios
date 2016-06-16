using System;
using System.Collections.Generic;
using System.IO;

namespace xharness
{
	public class LogFile
	{
		public string Description;
		public string Path;

		protected virtual void Write (string value)
		{
			lock (this) {
				using (var str = new FileStream (Path, FileMode.Append, FileAccess.Write, FileShare.Read)) {
					using (var writer = new StreamWriter (str))
						writer.Write (value);
				}
			}
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
	}

	public class LogFiles : List<LogFile>
	{
		public LogFile Create (string directory, string filename, string name, bool overwrite = true)
		{
			var rv = new LogFile ()
			{
				Path = Path.GetFullPath (Path.Combine (directory, filename)),
				Description = name,
			};
			Add (rv);

			if (File.Exists (rv.Path)) {
				if (overwrite)
					File.Delete (rv.Path);
			} else {
				Directory.CreateDirectory (directory);
			}

			return rv;
		}
	}

	public class CaptureLog : LogFile
	{
		public string CapturePath { get; private set; }

		long startLength;

		public CaptureLog (string capture_path)
		{
			CapturePath = capture_path;
		}

		public void StartCapture ()
		{
			if (File.Exists (CapturePath))
				startLength = new FileInfo (CapturePath).Length;
		}

		public void StopCapture ()
		{
			var endLength = new FileInfo (CapturePath).Length;
			var length = (int) (endLength - startLength);

			using (var reader = new FileStream (CapturePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				using (var writer = new FileStream (Path, FileMode.Create, FileAccess.Write, FileShare.Read)) {
					var buffer = new byte [4096];
					reader.Position = startLength;
					while (length > 0) {
						int read = reader.Read (buffer, 0, Math.Min (buffer.Length, length));
						if (read > 0) {
							writer.Write (buffer, 0, read);
							length -= read;
						}
					}
				}
			}
		}

		protected override void Write (string value)
		{
			throw new InvalidOperationException ();
		}
	}
}

