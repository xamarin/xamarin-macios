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
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.WebKit {

	public partial class WebPolicyDelegate {
		static IntPtr selUse = Selector.GetHandle ("use");
		static IntPtr selDownload = Selector.GetHandle ("download");
		static IntPtr selIgnore = Selector.GetHandle ("ignore");
		
		public static void DecideUse (NSObject decisionToken)
		{
			if (decisionToken == null)
				throw new ArgumentNullException ("token");
			
			XamCore.ObjCRuntime.Messaging.void_objc_msgSend (decisionToken.Handle, selUse);
		}
		
		public static void DecideDownload (NSObject decisionToken)
		{
			if (decisionToken == null)
				throw new ArgumentNullException ("decisionToken");
			
			XamCore.ObjCRuntime.Messaging.void_objc_msgSend (decisionToken.Handle, selDownload);
		}
		
		public static void DecideIgnore (NSObject decisionToken)
		{
			if (decisionToken == null)
				throw new ArgumentNullException ("decisionToken");
			
			XamCore.ObjCRuntime.Messaging.void_objc_msgSend (decisionToken.Handle, selIgnore);
		}
		
	}
}
