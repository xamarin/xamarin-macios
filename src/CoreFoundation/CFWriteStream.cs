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

using System.ComponentModel;
using CoreFoundation;

using CFIndex = System.IntPtr;

namespace CoreFoundation {
	/// <summary>A <see cref="CoreFoundation.CFStream" /> that writes streams of bytes.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CFWriteStream : CFStream {
		[Preserve (Conditional = true)]
		internal CFWriteStream (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFErrorRef */ IntPtr CFWriteStreamCopyError (/* CFWriteStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public override CFException? GetError ()
		{
			var error = CFWriteStreamCopyError (Handle);
			if (error == IntPtr.Zero)
				return null;
			return CFException.FromCFError (error);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFWriteStreamOpen (/* CFWriteStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override bool DoOpen ()
		{
			return CFWriteStreamOpen (Handle) != 0;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamClose (/* CFWriteStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected override void DoClose ()
		{
			CFWriteStreamClose (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFStreamStatus */ nint CFWriteStreamGetStatus (/* CFWriteStreamRef */ IntPtr stream);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override CFStreamStatus DoGetStatus ()
		{
			return (CFStreamStatus) (long) CFWriteStreamGetStatus (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFWriteStreamCanAcceptBytes (/* CFWriteStreamRef */ IntPtr handle);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool CanAcceptBytes ()
		{
			return CFWriteStreamCanAcceptBytes (Handle) != 0;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern nint CFWriteStreamWrite (IntPtr handle, IntPtr buffer, nint count);

		/// <summary>Writes bytes from the provided buffer to the stream.</summary>
		/// <param name="buffer">The nonempty source buffer.</param>
		/// <returns>The number of bytes written, which may be less than the buffer length, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="buffer" /> is empty.</exception>
		/// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
		public int Write (byte [] buffer)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			return Write (buffer, 0, buffer.Length);
		}

		/// <summary>Writes bytes from a range of the provided buffer to the stream.</summary>
		/// <param name="buffer">The source buffer.</param>
		/// <param name="offset">The nonnegative byte offset of the first byte to write.</param>
		/// <param name="count">The positive maximum number of bytes to write.</param>
		/// <returns>The number of bytes written, which may be less than <paramref name="count" />, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="offset" /> is negative, <paramref name="count" /> is not positive, or the range exceeds the buffer length.</exception>
		/// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
		public unsafe int Write (byte [] buffer, nint offset, nint count)
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
			return Write (buffer.AsSpan ((int) offset, (int) count));
		}

		/// <summary>Writes bytes from the provided span to the stream.</summary>
		/// <param name="buffer">The source bytes.</param>
		/// <returns>The number of bytes written, which may be less than the span length, zero for an empty span, or -1 if an error occurs.</returns>
		/// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
		/// <remarks>An empty span does not access the native stream. Otherwise, the buffer is pinned only during the native call.</remarks>
		public unsafe int Write (ReadOnlySpan<byte> buffer)
		{
			var handle = GetCheckedHandle ();
			if (buffer.IsEmpty)
				return 0;
			fixed (byte* ptr = buffer) {
				var result = (int) CFWriteStreamWrite (handle, (IntPtr) ptr, buffer.Length);
				GC.KeepAlive (this);
				return result;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe static extern /* Boolean */ byte CFWriteStreamSetClient (/* CFWriteStreamRef */ IntPtr stream, /* CFOptionFlags */ nint streamEvents,
			/* CFWriteStreamClientCallBack */ delegate* unmanaged<IntPtr, nint, IntPtr, void> clientCB, /* CFStreamClientContext* */ IntPtr clientContext);

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
			return CFWriteStreamSetClient (Handle, (nint) eventTypes, callback, context);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamScheduleWithRunLoop (/* CFWriteStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

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
			CFWriteStreamScheduleWithRunLoop (Handle, loop.Handle, mode.Handle);
			GC.KeepAlive (loop);
			GC.KeepAlive (mode);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFWriteStreamUnscheduleFromRunLoop (/* CFWriteStreamRef */ IntPtr stream, /* CFRunLoopRef */ IntPtr runLoop, /* CFStringRef */ IntPtr runLoopMode);

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
			CFWriteStreamUnscheduleFromRunLoop (Handle, loop.Handle, mode.Handle);
			GC.KeepAlive (loop);
			GC.KeepAlive (mode);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFTypeRef */ IntPtr CFWriteStreamCopyProperty (/* CFWriteStreamRef */ IntPtr stream, /* CFStringRef */ IntPtr propertyName);

		/// <param name="name">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override IntPtr DoGetProperty (NSString name)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			IntPtr result = CFWriteStreamCopyProperty (Handle, name.Handle);
			GC.KeepAlive (name);
			return result;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFWriteStreamSetProperty (/* CFWriteStreamRef */ IntPtr stream, /* CFStringRef */ IntPtr propertyName, /* CFTypeRef */ IntPtr value);

		/// <param name="name">To be added.</param>
		///         <param name="value">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override bool DoSetProperty (NSString name, INativeObject? value)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			bool result = CFWriteStreamSetProperty (Handle, name.Handle, value.GetHandle ()) != 0;
			GC.KeepAlive (name);
			GC.KeepAlive (value);
			return result;
		}
	}
}
