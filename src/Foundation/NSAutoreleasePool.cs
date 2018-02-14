//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
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
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {
	[Register ("NSAutoreleasePool", true)]
	public class NSAutoreleasePool : NSObject
#if !COREBUILD
	, IDisposable
#endif
	{
#if !COREBUILD
		public override IntPtr ClassHandle { get { return Class.GetHandle ("NSAutoreleasePool"); } }

		[Export ("init")]
		public NSAutoreleasePool () : base (NSObjectFlag.Empty)
		{
			IsDirectBinding = GetType () == typeof (NSAutoreleasePool);
			if (IsDirectBinding) {
				Handle = Messaging.IntPtr_objc_msgSend (this.Handle, Selector.GetHandle ("init"));
			} else {
				Handle = Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("init"));
			}

		}

#if XAMCORE_4_0
		protected
#else
		public
#endif
		NSAutoreleasePool (NSObjectFlag t) : base (t) {}

#if XAMCORE_4_0
		protected
#else
		public
#endif
		NSAutoreleasePool (IntPtr handle) : base (handle) {}

#if !XAMCORE_2_0
		protected override void Dispose (bool disposing) {
			base.Dispose (disposing);
		}
#endif
#endif
	}
}
