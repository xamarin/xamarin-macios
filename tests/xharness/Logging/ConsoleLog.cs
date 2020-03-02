using System;
using System.IO;
using System.Text;

namespace xharness.Logging {
	// A log that writes to standard output
	public class ConsoleLog : Log {
		StringBuilder captured = new StringBuilder ();

		public ConsoleLog ()
			: base (null)
		{
		}

		public override void WriteImpl (string value)
		{
			captured.Append (value);
			Console.Write (value);
		}

		public override string FullPath {
			get {
				throw new NotSupportedException ();
			}
		}

		public override StreamReader GetReader ()
		{
			var str = new MemoryStream (System.Text.Encoding.UTF8.GetBytes (captured.ToString ()));
			return new StreamReader (str, System.Text.Encoding.UTF8, false);
		}
	}
}