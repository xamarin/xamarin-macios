using System;
using System.IO;
using System.Text;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {

	// defines common log types
	public enum LogType {
		XmlLog,
		NUnitResult,
		SystemLog,
		CompanionSystemLog,
		BuildLog,
		TestLog,
		ExtensionTestLog,
		ExecutionLog,
		TrxLog,
		HtmlLog,
	}

	public interface ILog : IDisposable {
		string Description { get; set; }
		bool Timestamp { get; set; }
		string FullPath { get; }
		Encoding Encoding { get; }

		void Write (byte [] buffer, int offset, int count);
		StreamReader GetReader ();
		void Write (string value);
		void WriteLine (string value);
		void WriteLine (StringBuilder value);
		void WriteLine (string format, params object [] args);
		void Flush ();
	}
}
