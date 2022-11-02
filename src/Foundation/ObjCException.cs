//
// Copyright 2013, Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if MONOMAC || NET

using System;
using System.Text;

using Foundation;

#nullable enable

#if NET
namespace ObjCRuntime {
#else
namespace Foundation {
#endif
	public class ObjCException : Exception {
		NSException native_exc;

		public ObjCException () : base ()
		{
			native_exc = new NSException ("default", String.Empty, null);
		}

		public ObjCException (NSException exc) : base ()
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
				AppendNativeStackTrace (sb);
				return sb.ToString ();
			}
		}

		void AppendNativeStackTrace (StringBuilder sb)
		{
			if (native_exc?.CallStackSymbols?.Length > 0) {
				sb.AppendLine ("Native stack trace:");
				foreach (var symbol in native_exc.CallStackSymbols)
					sb.Append ('\t').AppendLine (symbol);
			}
		}

		public override string ToString ()
		{
			var msg = base.ToString ();
			if (native_exc is null)
				return msg;

			var sb = new StringBuilder (msg);
			sb.AppendLine ();
			AppendNativeStackTrace (sb);
			return sb.ToString ();
		}
	}
}

#endif // MONOMAC || NET
