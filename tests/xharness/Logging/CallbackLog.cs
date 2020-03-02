using System;
namespace xharness.Logging {
	// A log that forwards all written data to a callback
	public class CallbackLog : Log {
		public Action<string> OnWrite;

		public CallbackLog (Action<string> onWrite)
			: base (null)
		{
			OnWrite = onWrite;
		}

		public override string FullPath => throw new NotImplementedException ();

		public override void WriteImpl (string value)
		{
			OnWrite (value);
		}
	}
}
