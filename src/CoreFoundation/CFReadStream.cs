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

#nullable enable

using System.ComponentModel;
using CoreFoundation;

using CFIndex = System.IntPtr;

namespace CoreFoundation {
	/// <summary>A <see cref="CoreFoundation.CFStream" /> that reads streams of bytes.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	// CFStream.h
	public class CFReadStream : CFStream {
		[Preserve (Conditional = true)]
		internal CFReadStream (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFErrorRef */ IntPtr CFReadStreamCopyError (/* CFReadStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public override CFException? GetError ()
		{
			var error = CFReadStreamCopyError (Handle);
			if (error == IntPtr.Zero)
				return null;
			return CFException.FromCFError (error);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFReadStreamOpen (/* CFReadStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override bool DoOpen ()
		{
			return CFReadStreamOpen (Handle) != 0;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFReadStreamClose (/* CFReadStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected override void DoClose ()
		{
			CFReadStreamClose (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFStreamStatus -> CFIndex */ nint CFReadStreamGetStatus (/* CFReadStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override CFStreamStatus DoGetStatus ()
		{
			return (CFStreamStatus) (long) CFReadStreamGetStatus (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFReadStreamHasBytesAvailable (/* CFReadStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool HasBytesAvailable ()
		{
			return CFReadStreamHasBytesAvailable (Handle) != 0;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFReadStreamScheduleWithRunLoop (/* CFReadStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

		/// <param name="loop">To be added.</param>
		///         <param name="mode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected override void ScheduleWithRunLoop (CFRunLoop loop, NSString? mode)
		{
			if (loop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (loop));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));
			CFReadStreamScheduleWithRunLoop (Handle, loop.Handle, mode.Handle);
			GC.KeepAlive (loop);
			GC.KeepAlive (mode);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFReadStreamUnscheduleFromRunLoop (/* CFReadStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

		/// <param name="loop">To be added.</param>
		///         <param name="mode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected override void UnscheduleFromRunLoop (CFRunLoop loop, NSString? mode)
		{
			if (loop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (loop));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));
			CFReadStreamUnscheduleFromRunLoop (Handle, loop.Handle, mode.Handle);
			GC.KeepAlive (loop);
			GC.KeepAlive (mode);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe static extern byte CFReadStreamSetClient (/* CFReadStreamRef */ IntPtr stream, /* CFOptionFlags */ nint streamEvents,
			/* CFReadStreamClientCallBack */ delegate* unmanaged<IntPtr, nint, IntPtr, void> clientCB, /* CFStreamClientContext* */ IntPtr clientContext);

#if !XAMCORE_5_0
		[Obsolete ("Use the other overload.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		protected override bool DoSetClient (CFStreamCallback? callback, CFIndex eventTypes,
											 IntPtr context)
		{
			throw new InvalidOperationException ($"Use the other overload.");
		}
#endif // !XAMCORE_5_0

		unsafe protected override byte DoSetClient (delegate* unmanaged<IntPtr, nint, IntPtr, void> callback, CFIndex eventTypes, IntPtr context)
		{
			return CFReadStreamSetClient (Handle, (nint) eventTypes, callback, context);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFIndex */ nint CFReadStreamRead (/* CFReadStreamRef */ IntPtr handle, /* UInt8* */ IntPtr buffer, /* CFIndex */ nint count);

		/// <summary>Reads bytes from the stream into the provided buffer.</summary>
		/// <param name="buffer">The nonempty destination buffer.</param>
		/// <returns>The number of bytes read, zero at the end of the stream, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="buffer" /> is empty.</exception>
		/// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
		public nint Read (byte [] buffer)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			return Read (buffer, 0, buffer.Length);
		}

		/// <summary>Reads bytes from the stream into a range of the provided buffer.</summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="offset">The nonnegative byte offset at which to store data.</param>
		/// <param name="count">The positive maximum number of bytes to read.</param>
		/// <returns>The number of bytes read, zero at the end of the stream, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="offset" /> is negative, <paramref name="count" /> is not positive, or the range exceeds the buffer length.</exception>
		/// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
		public unsafe nint Read (byte [] buffer, int offset, int count)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			GetCheckedHandle ();
			if (offset < 0)
				throw new ArgumentException ();
			if (count < 1)
				throw new ArgumentException ();
			if (offset > buffer.Length || count > buffer.Length - offset)
				throw new ArgumentException ();
			return Read (buffer.AsSpan (offset, count));
		}

		/// <summary>Reads bytes from the stream into the provided span.</summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <returns>The number of bytes read, zero for an empty span or at the end of the stream, or -1 if an error occurs.</returns>
		/// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
		/// <remarks>An empty span does not access the native stream. Otherwise, the buffer is pinned only during the native call.</remarks>
		public unsafe nint Read (Span<byte> buffer)
		{
			var handle = GetCheckedHandle ();
			if (buffer.IsEmpty)
				return 0;
			fixed (byte* ptr = buffer) {
				var result = CFReadStreamRead (handle, (IntPtr) ptr, buffer.Length);
				GC.KeepAlive (this);
				return result;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFTypeRef */ IntPtr CFReadStreamCopyProperty (/* CFReadStreamRef */ IntPtr stream, /* CFStreamRef */ IntPtr propertyName);

		/// <param name="name">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override IntPtr DoGetProperty (NSString name)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			IntPtr result = CFReadStreamCopyProperty (Handle, name.Handle);
			GC.KeepAlive (name);
			return result;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFReadStreamSetProperty (/* CFReadStreamRef */ IntPtr stream, /* CFStreamRef */ IntPtr propertyName, /* CFTypeRef */ IntPtr propertyValue);

		/// <param name="name">To be added.</param>
		///         <param name="value">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override bool DoSetProperty (NSString name, INativeObject? value)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			bool result = CFReadStreamSetProperty (Handle, name.Handle, value.GetHandle ()) != 0;
			GC.KeepAlive (name);
			GC.KeepAlive (value);
			return result;
		}
	}
}
