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
//
using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

	public partial class NSKeyedArchiver {

		public static void GlobalSetClassName (string name, Class kls)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			if (kls is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (kls));

			var ptr = CFString.CreateNative (name);
			ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_IntPtr (class_ptr, Selector.GetHandle ("setClassName:forClass:"), ptr, kls.Handle);
			CFString.ReleaseNative (ptr);
		}

		public static string GlobalGetClassName (Class kls)
		{
			if (kls is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (kls));

			return CFString.FromHandle (ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (class_ptr, Selector.GetHandle ("classNameForClass:"), kls.Handle));
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
