//
// MonoMac.CoreFoundation.CFWriteStream
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//		Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2012, 2014 Xamarin Inc. (http://www.xamarin.com) All rights reserved.
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
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if XAMCORE_2_0
using CFIndex = System.nint;
#endif

namespace CoreFoundation {

	public class CFWriteStream : CFStream {
		internal CFWriteStream (IntPtr handle)
			: base (handle)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFErrorRef */ IntPtr CFWriteStreamCopyError (/* CFWriteStreamRef */ IntPtr stream);

		public override CFException GetError ()
		{
			var error = CFWriteStreamCopyError (Handle);
			if (error == IntPtr.Zero)
				return null;
			return CFException.FromCFError (error);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFWriteStreamOpen (/* CFWriteStreamRef */ IntPtr stream);

		protected override bool DoOpen ()
		{
			return CFWriteStreamOpen (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamClose (/* CFWriteStreamRef */ IntPtr stream);

		protected override void DoClose ()
		{
			CFWriteStreamClose (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFStreamStatus */ nint CFWriteStreamGetStatus (/* CFWriteStreamRef */ IntPtr stream);

		protected override CFStreamStatus DoGetStatus ()
		{
			return (CFStreamStatus) (long) CFWriteStreamGetStatus (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFWriteStreamCanAcceptBytes (/* CFWriteStreamRef */ IntPtr handle);

		public bool CanAcceptBytes ()
		{
			return CFWriteStreamCanAcceptBytes (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern nint CFWriteStreamWrite (IntPtr handle, IntPtr buffer, nint count);

		public int Write (byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			return Write (buffer, 0, buffer.Length);
		}

		public unsafe int Write (byte[] buffer, nint offset, nint count)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			CheckHandle ();
			if (offset < 0)
				throw new ArgumentException ();
			if (count < 1)
				throw new ArgumentException ();
			if (offset + count > buffer.Length)
				throw new ArgumentException ();
			fixed (byte* ptr = buffer)
				return (int) CFWriteStreamWrite (Handle, ((IntPtr) ptr) + (int) offset, count);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern /* Boolean */ bool CFWriteStreamSetClient (/* CFWriteStreamRef */ IntPtr stream, /* CFOptionFlags */ nint streamEvents,
			/* CFWriteStreamClientCallBack */ CFStreamCallback clientCB, /* CFStreamClientContext* */ IntPtr clientContext);

		protected override bool DoSetClient (CFStreamCallback callback, CFIndex eventTypes,
		                                     IntPtr context)
		{
			return CFWriteStreamSetClient (Handle, eventTypes, callback, context);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamScheduleWithRunLoop (/* CFWriteStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

		protected override void ScheduleWithRunLoop (CFRunLoop loop, NSString mode)
		{
			if (loop == null)
				throw new ArgumentNullException ("loop");
			if (mode == null)
				throw new ArgumentNullException ("mode");
			CFWriteStreamScheduleWithRunLoop (Handle, loop.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamUnscheduleFromRunLoop (/* CFWriteStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

		protected override void UnscheduleFromRunLoop (CFRunLoop loop, NSString mode)
		{
			if (loop == null)
				throw new ArgumentNullException ("loop");
			if (mode == null)
				throw new ArgumentNullException ("mode");
			CFWriteStreamUnscheduleFromRunLoop (Handle, loop.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFTypeRef */ IntPtr CFWriteStreamSetProperty (/* CFWriteStreamRef */ IntPtr stream, /* CFStringRef */ IntPtr propertyName);

		protected override IntPtr DoGetProperty (NSString name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			return CFWriteStreamSetProperty (Handle, name.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFWriteStreamSetProperty (/* CFWriteStreamRef */ IntPtr stream, /* CFStringRef */ IntPtr propertyName, /* CFTypeRef */ IntPtr value);

		protected override bool DoSetProperty (NSString name, INativeObject value)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			return CFWriteStreamSetProperty (Handle, name.Handle, value == null ? IntPtr.Zero : value.Handle);
		}
	}
}

