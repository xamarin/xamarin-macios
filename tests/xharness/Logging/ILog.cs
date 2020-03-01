using System;
using System.IO;
using System.Text;

namespace xharness.Logging {

	// defines common log types
	public enum LogType {
		XML_LOG,
		NUNIT_RESULT,
		SYSTEM_LOG,
		COMPANION_SYSTEM_LOG,
		BUILD_LOG,
		TEST_LOG,
		EXTENSION_TEST_LOG,
		EXECUTION_LOG,
	}

	public static class LogTypeExtensions {

		public static string ToString (this LogType type)
		{
			switch (type) {
			case LogType.XML_LOG:
				return "XML log";
			case LogType.NUNIT_RESULT:
				return "NUnit results";
			case LogType.SYSTEM_LOG:
				return "System log";
			case LogType.COMPANION_SYSTEM_LOG:
				return "System log (companion)";
			case LogType.BUILD_LOG:
				return "Build log";
			case LogType.TEST_LOG:
				return "Test log";
			case LogType.EXTENSION_TEST_LOG:
				return "Extension test log";
			case LogType.EXECUTION_LOG:
				return "Execution log";
			default:
				throw new ArgumentException ($"Unknoe tyme for {nameof (type)}");
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
