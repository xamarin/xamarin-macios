using System;
using System.IO;
using System.Text;

namespace Xharness.Logging {
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
			var str = new MemoryStream (Encoding.GetBytes (captured.ToString ()));
			return new StreamReader (str, Encoding, false);
		}
	}
}
