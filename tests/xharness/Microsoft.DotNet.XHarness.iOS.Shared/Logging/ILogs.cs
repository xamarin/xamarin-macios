using System;
using System.Collections.Generic;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	public interface ILogs : IList<ILog>, IDisposable {
		string Directory { get; set; }

		// Create a new log backed with a file
		ILog Create (string filename, string name, bool? timestamp = null);

		// Adds an existing file to this collection of logs.
		// If the file is not inside the log directory, then it's copied there.
		// 'path' must be a full path to the file.
		ILog AddFile (string path);

		// Adds an existing file to this collection of logs.
		// If the file is not inside the log directory, then it's copied there.
		// 'path' must be a full path to the file.
		ILog AddFile (string path, string name);

		// Create an empty file in the log directory and return the full path to the file
		string CreateFile (string path, string description);
	}
}
