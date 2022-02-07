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
// Copyright 2011, 2012 Xamarin Inc
using System;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace Foundation {

	public partial class NSKeyedUnarchiver {

		public static void GlobalSetClass (Class kls, string codedName)
		{
			if (codedName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (codedName));
			if (kls is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (kls));

			var ptr = CFString.CreateNative (codedName);
			ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_IntPtr (class_ptr, Selector.GetHandle ("setClass:forClassName:"), kls.Handle, ptr);
			CFString.ReleaseNative (ptr);
		}

		public static Class GlobalGetClass (string codedName)
		{
			if (codedName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (codedName));

			var ptr = CFString.CreateNative (codedName);
			var result = new Class (ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (class_ptr, Selector.GetHandle ("classForClassName:"), ptr));
			CFString.ReleaseNative (ptr);
			return result;
		}

#if !NET
		public bool RequiresSecureCoding {
			get {
				return GetRequiresSecureCoding ();
			}
			set {
				SetRequiresSecureCoding (value);
			}
		}
#endif
	}
}
