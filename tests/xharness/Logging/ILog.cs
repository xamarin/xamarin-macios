using System;
using System.IO;
using System.Text;

namespace Xharness.Logging {

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
	}

	public static class LogTypeExtensions {

		public static string ToString (this LogType type)
		{
			switch (type) {
			case LogType.XmlLog:
				return "XML log";
			case LogType.NUnitResult:
				return "NUnit results";
			case LogType.SystemLog:
				return "System log";
			case LogType.CompanionSystemLog:
				return "System log (companion)";
			case LogType.BuildLog:
				return "Build log";
			case LogType.TestLog:
				return "Test log";
			case LogType.ExtensionTestLog:
				return "Extension test log";
			case LogType.ExecutionLog:
				return "Execution log";
			default:
				throw new ArgumentException ($"Unknown type for {nameof (type)}");
			}
		}
	}
	public interface ILog : IDisposable {
		ILogs Logs { get; }
		string Description { get; set; }
		bool Timestamp { get; set; }
		string FullPath { get; }
		Encoding Encoding { get; }
		void Write (byte [] buffer);
		void Write (byte [] buffer, int offset, int count);
		StreamReader GetReader ();
		void Write (char value);
		void Write (string value);
		void WriteImpl (string value);
		void WriteLine (string value);
		void WriteLine (StringBuilder value);
		void WriteLine (string format, params object [] args);
		void Flush ();
	}
}
