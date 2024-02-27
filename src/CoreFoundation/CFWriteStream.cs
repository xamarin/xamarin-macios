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

#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

#if NET
using CFIndex = System.IntPtr;
#else
using CFIndex = System.nint;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFWriteStream : CFStream {
		[Preserve (Conditional = true)]
		internal CFWriteStream (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFErrorRef */ IntPtr CFWriteStreamCopyError (/* CFWriteStreamRef */ IntPtr stream);

		public override CFException? GetError ()
		{
			var error = CFWriteStreamCopyError (Handle);
			if (error == IntPtr.Zero)
				return null;
			return CFException.FromCFError (error);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFWriteStreamOpen (/* CFWriteStreamRef */ IntPtr stream);

		protected override bool DoOpen ()
		{
			return CFWriteStreamOpen (Handle) != 0;
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
		extern static /* Boolean */ byte CFWriteStreamCanAcceptBytes (/* CFWriteStreamRef */ IntPtr handle);

		public bool CanAcceptBytes ()
		{
			return CFWriteStreamCanAcceptBytes (Handle) != 0;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern nint CFWriteStreamWrite (IntPtr handle, IntPtr buffer, nint count);

		public int Write (byte [] buffer)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			return Write (buffer, 0, buffer.Length);
		}

		public unsafe int Write (byte [] buffer, nint offset, nint count)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			GetCheckedHandle ();
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
#if NET8_0_OR_GREATER
		unsafe static extern /* Boolean */ byte CFWriteStreamSetClient (/* CFWriteStreamRef */ IntPtr stream, /* CFOptionFlags */ nint streamEvents,
			/* CFWriteStreamClientCallBack */ delegate* unmanaged<IntPtr, nint, IntPtr, void> clientCB, /* CFStreamClientContext* */ IntPtr clientContext);
#else
		[return: MarshalAs (UnmanagedType.I1)]
		static extern /* Boolean */ bool CFWriteStreamSetClient (/* CFWriteStreamRef */ IntPtr stream, /* CFOptionFlags */ nint streamEvents,
			/* CFWriteStreamClientCallBack */ CFStreamCallback? clientCB, /* CFStreamClientContext* */ IntPtr clientContext);
#endif

#if !XAMCORE_5_0
#if NET8_0_OR_GREATER
		[Obsolete ("Use the other overload.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
#endif
		protected override bool DoSetClient (CFStreamCallback? callback, CFIndex eventTypes,
											 IntPtr context)
		{
#if NET8_0_OR_GREATER
			throw new InvalidOperationException ($"Use the other overload.");
#else
			return CFWriteStreamSetClient (Handle, (nint) eventTypes, callback, context);
#endif
		}
#endif // !XAMCORE_5_0

#if NET8_0_OR_GREATER
		unsafe protected override byte DoSetClient (delegate* unmanaged<IntPtr, nint, IntPtr, void> callback, CFIndex eventTypes, IntPtr context)
		{
			return CFWriteStreamSetClient (Handle, (nint) eventTypes, callback, context);
		}
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamScheduleWithRunLoop (/* CFWriteStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

		protected override void ScheduleWithRunLoop (CFRunLoop loop, NSString? mode)
		{
			if (loop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (loop));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));
			CFWriteStreamScheduleWithRunLoop (Handle, loop.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamUnscheduleFromRunLoop (/* CFWriteStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

		protected override void UnscheduleFromRunLoop (CFRunLoop loop, NSString? mode)
		{
			if (loop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (loop));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));
			CFWriteStreamUnscheduleFromRunLoop (Handle, loop.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFTypeRef */ IntPtr CFWriteStreamCopyProperty (/* CFWriteStreamRef */ IntPtr stream, /* CFStringRef */ IntPtr propertyName);

		protected override IntPtr DoGetProperty (NSString name)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			return CFWriteStreamCopyProperty (Handle, name.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFWriteStreamSetProperty (/* CFWriteStreamRef */ IntPtr stream, /* CFStringRef */ IntPtr propertyName, /* CFTypeRef */ IntPtr value);

		protected override bool DoSetProperty (NSString name, INativeObject? value)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			return CFWriteStreamSetProperty (Handle, name.Handle, value.GetHandle ()) != 0;
		}
	}
}
