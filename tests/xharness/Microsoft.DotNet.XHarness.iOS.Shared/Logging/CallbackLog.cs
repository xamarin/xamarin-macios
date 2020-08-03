using System;
using System.IO;
using System.Text;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	// A log that forwards all written data to a callback
	public class CallbackLog : Log {
		readonly StringBuilder captured = new StringBuilder ();
		readonly Action<string> onWrite;

		public CallbackLog (Action<string> onWrite, string description = "Callback log")
			: base (description)
		{
			this.onWrite = onWrite;
		}

		public override string FullPath => throw new NotSupportedException ();

		public override void Dispose ()
		{
		}

		public override void Flush ()
		{
		}

		public override StreamReader GetReader ()
		{
			lock (captured) {
				var str = new MemoryStream (Encoding.GetBytes (captured.ToString ()));
				return new StreamReader (str, Encoding, false);
			}
		}

		protected override void WriteImpl (string value)
		{
			lock (captured)
				captured.Append (value);
			onWrite (value);
		}
	}
}
