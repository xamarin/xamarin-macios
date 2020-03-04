using System;
using System.IO;

namespace Xharness.Logging {
	public class LogFile : Log, ILogFile {
		object lock_obj = new object ();
		public string Path { get; private set; }
		FileStream writer;
		bool disposed;

		public LogFile (ILogs logs, string description, string path, bool append = true)
			: base (logs, description)
		{
			Path = path ?? throw new ArgumentNullException (nameof (path));
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
				Console.Error.WriteLine ($"Failed to write to the file {Path}: {e}.");
				return;
			}
		}

		public override void Flush ()
		{
			base.Flush ();

			if (writer != null && !disposed)
				writer.Flush ();
		}

		public override string FullPath => Path;

		public override StreamReader GetReader ()
		{
			return new StreamReader (new FileStream (Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			lock (lock_obj) {
				writer?.Dispose ();
				writer = null;
			}

			disposed = true;
		}

	}
}
