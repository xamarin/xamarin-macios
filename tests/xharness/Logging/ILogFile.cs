using System;
using System.IO;

namespace xharness.Logging {
	public interface ILogFile : ILog, IDisposable {
		string Path { get; }
	}
}
