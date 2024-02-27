//
// Copyright 2010, Novell, Inc.
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

#if MONOMAC && !XAMCORE_3_0

using System;
using System.Runtime.InteropServices;

using Foundation;

namespace ObjCRuntime {
	public class SelectorMarshaler : ICustomMarshaler {
		static SelectorMarshaler marshaler;

		public object MarshalNativeToManaged (IntPtr handle) {
			return new Selector (handle);
		}

		public IntPtr MarshalManagedToNative (object obj) {
			if (obj is null)
				return IntPtr.Zero;
			if (!(obj is Selector))
				throw new MarshalDirectiveException ("This custom marshaler must be used on a Selector derived type.");

			return (obj as Selector).Handle;
		}

		public void CleanUpNativeData (IntPtr handle) {
		}

		public void CleanUpManagedData (object obj) {
		}

		public int GetNativeDataSize () {
			return -1;
		}

		public static ICustomMarshaler GetInstance(string cookie) {
			if(marshaler is null)
				return marshaler = new SelectorMarshaler ();

			return marshaler;
		}
	}
}

#endif // MONOMAC && !XAMCORE_3_0
