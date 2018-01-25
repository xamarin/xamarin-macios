#if !MONOMAC

using System;

namespace Foundation {
	public class MonoTouchException : Exception {
		private NSException native_exc;

		public MonoTouchException () : base () {
			native_exc = new NSException ("default", String.Empty, null);
		}

		public MonoTouchException (NSException exc) : base () {
			native_exc = exc;
		}

		public NSException NSException {
			get {
				return native_exc;
			}
		}

		public string Reason {
			get {
				return native_exc.Reason;
			}
		}

		public string Name {
			get {
				return native_exc.Name;
			}
		}

		public override string Message {
			get {
				return string.Format ("Objective-C exception thrown.  Name: {0} Reason: {1}\nNative stack trace:\n{2}", Name, Reason, NativeStackTrace);
			}
		}

		string NativeStackTrace {
			get {
				var symbols = native_exc.CallStackSymbols;
				var msg = string.Empty;
				for (int i = 0; i < symbols.Length; i++) {
					msg += "\t" + symbols [i] + "\n";
				}
				return msg;
			}
		}

		public override string ToString ()
		{
			var msg = base.ToString ();

			if (native_exc != null)
				msg += NativeStackTrace;

			return msg;
		}
	}
}

#endif // !MONOMAC
