using System;
using System.IO;

namespace Xharness.Logging {
	public interface ILogFile : ILog, IDisposable {
		string Path { get; }
	}
}
