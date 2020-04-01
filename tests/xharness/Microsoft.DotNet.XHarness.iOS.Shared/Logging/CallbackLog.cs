using System;
using System.IO;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	// A log that forwards all written data to a callback
	public class CallbackLog : Log {
		readonly Action<string> onWrite;

		public CallbackLog (Action<string> onWrite)
			: base ("Callback log")
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
			throw new NotSupportedException ();
		}

		protected override void WriteImpl (string value)
		{
			onWrite (value);
		}
	}
}
