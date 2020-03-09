using System;
namespace Xharness.Logging {
	// A log that forwards all written data to a callback
	public class CallbackLog : Log {
		readonly Action<string> OnWrite;

		public CallbackLog (Action<string> onWrite)
			: base (null)
		{
			OnWrite = onWrite;
		}

		public override string FullPath => throw new NotSupportedException ();

		public override void WriteImpl (string value)
		{
			OnWrite (value);
		}
	}
}
