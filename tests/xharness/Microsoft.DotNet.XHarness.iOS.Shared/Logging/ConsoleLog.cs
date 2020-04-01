using System;
using System.IO;
using System.Text;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	// A log that writes to standard output
	public class ConsoleLog : Log {
		readonly StringBuilder captured = new StringBuilder ();

		protected override void WriteImpl (string value)
		{
			captured.Append (value);
			Console.Write (value);
		}

		public override string FullPath => throw new NotSupportedException ();

		public override StreamReader GetReader ()
		{
			var str = new MemoryStream (Encoding.GetBytes (captured.ToString ()));
			return new StreamReader (str, Encoding, false);
		}

		public override void Flush ()
		{
		}

		public override void Dispose ()
		{
		}
	}
}
