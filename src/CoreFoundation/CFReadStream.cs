//
// MonoMac.CoreFoundation.CFReadStream
//
// Authors:
//      Martin Baulig (martin.baulig@xamarin.com)
//		Rolf Bjarbe Kvinge (rolf@xamarin.com)
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
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if XAMCORE_2_0
using CFIndex = System.nint;
#endif

namespace CoreFoundation {

	// CFStream.h
	public class CFReadStream : CFStream {
		public CFReadStream (IntPtr handle)
			: base (handle)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFErrorRef */ IntPtr CFReadStreamCopyError (/* CFReadStreamRef */ IntPtr stream);

		public override CFException GetError ()
		{
			var error = CFReadStreamCopyError (Handle);
			if (error == IntPtr.Zero)
				return null;
			return CFException.FromCFError (error);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFReadStreamOpen (/* CFReadStreamRef */ IntPtr stream);

		protected override bool DoOpen ()
		{
			return CFReadStreamOpen (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFReadStreamClose (/* CFReadStreamRef */ IntPtr stream);

		protected override void DoClose ()
		{
			CFReadStreamClose (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFStreamStatus -> CFIndex */ nint CFReadStreamGetStatus (/* CFReadStreamRef */ IntPtr stream);

		protected override CFStreamStatus DoGetStatus ()
		{
			return (CFStreamStatus) (long) CFReadStreamGetStatus (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFReadStreamHasBytesAvailable (/* CFReadStreamRef */ IntPtr stream);

		public bool HasBytesAvailable ()
		{
			return CFReadStreamHasBytesAvailable (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFReadStreamScheduleWithRunLoop (/* CFReadStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

		protected override void ScheduleWithRunLoop (CFRunLoop loop, NSString mode)
		{
			if (loop == null)
				throw new ArgumentNullException ("loop");
			if (mode == null)
				throw new ArgumentNullException ("mode");
			CFReadStreamScheduleWithRunLoop (Handle, loop.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFReadStreamUnscheduleFromRunLoop (/* CFReadStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

		protected override void UnscheduleFromRunLoop (CFRunLoop loop, NSString mode)
		{
			if (loop == null)
				throw new ArgumentNullException ("loop");
			if (mode == null)
				throw new ArgumentNullException ("mode");
			CFReadStreamUnscheduleFromRunLoop (Handle, loop.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CFReadStreamSetClient (/* CFReadStreamRef */ IntPtr stream, /* CFOptionFlags */ nint streamEvents,
			/* CFReadStreamClientCallBack */ CFStreamCallback clientCB, /* CFStreamClientContext* */ IntPtr clientContext);

		protected override bool DoSetClient (CFStreamCallback callback, CFIndex eventTypes,
		                                     IntPtr context)
		{
			return CFReadStreamSetClient (Handle, eventTypes, callback, context);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFIndex */ nint CFReadStreamRead (/* CFReadStreamRef */ IntPtr handle, /* UInt8* */ IntPtr buffer, /* CFIndex */ nint count);

		public nint Read (byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			return Read (buffer, 0, buffer.Length);
		}

		public unsafe nint Read (byte[] buffer, int offset, int count)
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
				return CFReadStreamRead (Handle, ((IntPtr) ptr) + offset, count);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFTypeRef */ IntPtr CFReadStreamCopyProperty (/* CFReadStreamRef */ IntPtr stream, /* CFStreamRef */ IntPtr propertyName);

		protected override IntPtr DoGetProperty (NSString name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			return CFReadStreamCopyProperty (Handle, name.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFReadStreamSetProperty (/* CFReadStreamRef */ IntPtr stream, /* CFStreamRef */ IntPtr propertyName, /* CFTypeRef */ IntPtr propertyValue);

		protected override bool DoSetProperty (NSString name, INativeObject value)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			return CFReadStreamSetProperty (Handle, name.Handle, value == null ? IntPtr.Zero : value.Handle);
		}
	}
}
