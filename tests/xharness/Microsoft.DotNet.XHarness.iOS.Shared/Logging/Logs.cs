using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	public class Logs : List<ILog>, ILogs {
		public string Directory { get; set; }

		public Logs (string directory)
		{
			Directory = directory ?? throw new ArgumentNullException (nameof (directory));
		}

		// Create a new log backed with a file
		public ILog Create (string filename, string name, bool? timestamp = null)
		{
			return Create (Directory, filename, name, timestamp);
		}

		LogFile Create (string directory, string filename, string name, bool? timestamp = null)
		{
			System.IO.Directory.CreateDirectory (directory);
			var rv = new LogFile (name, Path.GetFullPath (Path.Combine (directory, filename)));
			if (timestamp.HasValue)
				rv.Timestamp = timestamp.Value;
			Add (rv);
			return rv;
		}

		// Adds an existing file to this collection of logs.
		// If the file is not inside the log directory, then it's copied there.
		// 'path' must be a full path to the file.
		public ILog AddFile (string path)
		{
			return AddFile (path, Path.GetFileName (path));
		}

		// Adds an existing file to this collection of logs.
		// If the file is not inside the log directory, then it's copied there.
		// 'path' must be a full path to the file.
		public ILog AddFile (string path, string name)
		{
			if (path == null)
				throw new ArgumentNullException (nameof (path));

			if (!path.StartsWith (Directory, StringComparison.Ordinal)) {
				var newPath = Path.Combine (Directory, Path.GetFileNameWithoutExtension (path) + "-" + Helpers.Timestamp + Path.GetExtension (path));
				File.Copy (path, newPath, true);
				path = newPath;
			}

			var log = new LogFile (name, path, true);
			Add (log);
			return log;
		}

		// Create an empty file in the log directory and return the full path to the file
		public string CreateFile (string path, string description)
		{
			if (path == null)
				throw new ArgumentNullException (nameof (path));
			using (var rv = new LogFile (description, Path.Combine (Directory, path), false)) {
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
}
