#if !MONOMAC && !NET

using System;
using System.Text;

#nullable enable

namespace Foundation {
	public class MonoTouchException : Exception {
		NSException? native_exc;

		public MonoTouchException () : base ()
		{
			native_exc = new NSException ("default", String.Empty, null);
		}

		public MonoTouchException (NSException exc) : base ()
		{
			native_exc = exc;
		}

		public NSException? NSException {
			get {
				return native_exc;
			}
		}

		public string? Reason {
			get {
				return native_exc?.Reason;
			}
		}

		public string? Name {
			get {
				return native_exc?.Name;
			}
		}

		public override string Message {
			get {
				var sb = new StringBuilder ("Objective-C exception thrown.  Name: ").Append (Name);
				sb.Append (" Reason: ").AppendLine (Reason);
				sb.AppendLine ("Native stack trace:");
				AppendNativeStackTrace (sb);
				return sb.ToString ();
			}
		}

		void AppendNativeStackTrace (StringBuilder sb)
		{
			var callStackSymbols = native_exc?.CallStackSymbols;
			if (callStackSymbols is not null) {
				foreach (var symbol in callStackSymbols)
					sb.Append ('\t').AppendLine (symbol);
			}
		}

		public override string ToString ()
		{
			var msg = base.ToString ();
			if (native_exc is null)
				return msg;

			var sb = new StringBuilder (msg);
			AppendNativeStackTrace (sb);
			return sb.ToString ();
		}
	}
}

#endif // !MONOMAC && !NET
