//
// Copyright 2012 Xamarin Inc
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

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {

	public partial class NSThread {

		public static double Priority {
			get { return _GetPriority (); }
			// ignore the boolean return value
			set { _SetPriority (value); }
		}

		[DllImport ("__Internal")]
		static extern IntPtr xamarin_init_nsthread (IntPtr handle, bool is_direct_binding, IntPtr target, IntPtr selector, IntPtr argument);

		IntPtr InitNSThread (NSObject target, Selector selector, NSObject argument)
		{
			if (target == null)
				throw new ArgumentNullException ("target");	
			if (selector == null)
				throw new ArgumentNullException ("selector");

			return xamarin_init_nsthread (IsDirectBinding ? this.Handle : this.SuperHandle, IsDirectBinding, target.Handle, selector.Handle, argument == null ? IntPtr.Zero : argument.Handle);
		}

		[Export ("initWithTarget:selector:object:")]
		public NSThread (NSObject target, Selector selector, NSObject argument)
			: base ()
		{
			Handle = InitNSThread (target, selector, argument);
		}
	}
}