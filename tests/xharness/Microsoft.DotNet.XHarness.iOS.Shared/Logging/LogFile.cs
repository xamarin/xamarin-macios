using System;
using System.IO;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	public class LogFile : Log, ILog {
		readonly object lock_obj = new object ();

		public override string FullPath { get; }

		FileStream writer;
		bool disposed;

		public LogFile (string description, string path, bool append = true)
			: base (description)
		{
			FullPath = path ?? throw new ArgumentNullException (nameof (path));
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
					var fs = writer ?? new FileStream (FullPath, FileMode.Append, FileAccess.Write, FileShare.Read);

					fs.Write (buffer, offset, count);

					if (disposed) {
						fs.Dispose ();
					} else {
						writer = fs;
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine ($"Failed to write to the file {FullPath}: {e}.");
				return;
			}
		}

		public override void Flush ()
		{
			if (!disposed)
				writer?.Flush ();
		}

		protected override void WriteImpl (string value)
		{
			var bytes = Encoding.GetBytes (value);
			Write (bytes, 0, bytes.Length);
		}

		public override StreamReader GetReader ()
		{
			return new StreamReader (new FileStream (FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}

		public override void Dispose ()
		{
			lock (lock_obj) {
				writer?.Dispose ();
				writer = null;
			}

			disposed = true;
		}
	}
}
