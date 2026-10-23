// Copyright 2011-2014 Xamarin Inc
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
using CoreFoundation;

namespace Foundation {
	public partial class NSInputStream : NSStream {
		const string selReadMaxLength = "read:maxLength:";

		CFStreamEventType flags;
		IntPtr callback;
		CFStreamClientContext context;

		// This is done manually because the generator can't handle byte[] as a native pointer (it will try to use NSArray instead).
		/// <summary>Reads data from the stream into the provided buffer.</summary>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="len">The maximum number of bytes to read, which must not exceed the length of <paramref name="buffer" />.</param>
		/// <returns>The number of bytes read, zero at the end of the stream, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="len" /> exceeds the buffer length.</exception>
		public nint Read (byte [] buffer, nuint len)
		{
			if (buffer is null)
				throw new ArgumentNullException (nameof (buffer));
			if (len > (nuint) buffer.Length)
				throw new ArgumentException ("The requested length exceeds the buffer length.", nameof (len));
			var result = objc_msgSend (Handle, Selector.GetHandle (selReadMaxLength), buffer, len);
			GC.KeepAlive (this);
			return result;
		}

		/// <summary>Reads data from the stream into the provided buffer starting at the given offset.</summary>
		/// <param name="buffer">The buffer where data should be stored.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin storing data.</param>
		/// <param name="len">The maximum number of bytes to read.</param>
		/// <returns>The number of bytes read, zero at the end of the stream, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">The requested range extends beyond <paramref name="buffer" />.</exception>
		/// <exception cref="IndexOutOfRangeException"><paramref name="offset" /> is negative, or equals the buffer length and <paramref name="len" /> is zero.</exception>
		public unsafe nint Read (byte [] buffer, int offset, nuint len)
		{
			if (buffer is null)
				throw new ArgumentNullException (nameof (buffer));
			if (offset < 0)
				throw new IndexOutOfRangeException ();
			if (offset > buffer.Length || len > (nuint) (buffer.Length - offset))
				throw new ArgumentException ();

			fixed (byte* ptr = &buffer [offset])
				return Read ((IntPtr) ptr, len);
		}

		/// <summary>Reads data from the stream into the provided span.</summary>
		/// <param name="buffer">The destination buffer, whose length is the maximum number of bytes to read.</param>
		/// <returns>The number of bytes read, zero at the end of the stream, or -1 if an error occurs.</returns>
		/// <remarks>The buffer is pinned only during the call. An empty span is passed to the native read method with a length of zero. Overrides of <see cref="Read(IntPtr, nuint)" /> are honored.</remarks>
		public unsafe nint Read (Span<byte> buffer)
		{
			byte empty = 0;
			fixed (byte* ptr = buffer)
				return Read ((IntPtr) (buffer.IsEmpty ? &empty : ptr), (nuint) buffer.Length);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern nint objc_msgSend (IntPtr handle, IntPtr sel, [In, Out] byte [] buffer, nuint len);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern nint objc_msgSend (IntPtr handle, IntPtr sel, IntPtr buffer, nuint len);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern unsafe nint objc_msgSendSuper (global::ObjCRuntime.ObjCSuper* handle, IntPtr sel, IntPtr buffer, nuint len);

		/// <summary>Reads data from the stream into an unmanaged buffer.</summary>
		/// <param name="buffer">A nonzero pointer to writable storage for at least <paramref name="len" /> bytes, valid for the duration of the call.</param>
		/// <param name="len">The maximum number of bytes to read.</param>
		/// <returns>The number of bytes read, zero at the end of the stream, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is zero, even if <paramref name="len" /> is zero.</exception>
		[Export ("read:maxLength:")]
		public virtual unsafe nint Read (IntPtr buffer, nuint len)
		{
			if (buffer == IntPtr.Zero)
				throw new ArgumentNullException ("buffer");

			if (IsDirectBinding) {
				var result = objc_msgSend (this.Handle, Selector.GetHandle (selReadMaxLength), buffer, len);
				GC.KeepAlive (this);
				return result;
			} else {
				var __objc_super__ = new global::ObjCRuntime.ObjCSuper (this);
				var __result__ = objc_msgSendSuper (&__objc_super__, Selector.GetHandle (selReadMaxLength), buffer, len);
				GC.KeepAlive (this);
				return __result__;
			}
		}

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			context.Release ();
			context.Info = IntPtr.Zero;

			base.Dispose (disposing);
		}

		// Private API, so no documentation.
		/// <summary>Adds a client for the stream. This method is not supposed to be called by managed code, it will be called by consumers of the stream. When overriding it make sure to call the base implementation.</summary>
		/// <param name="inFlags">Flags.</param>
		/// <param name="inCallback">The callbacks to call when events occur.</param>
		/// <param name="inContextPtr">User-defined data for the callback.</param>
		/// <returns><see langword="true" /> if the client was added successfully; otherwise, <see langword="false" />.</returns>
		[Export ("_setCFClientFlags:callback:context:")]
		protected virtual bool SetCFClientFlags (CFStreamEventType inFlags, IntPtr inCallback, IntPtr inContextPtr)
		{
			CFStreamClientContext inContext;

			if (inContextPtr == IntPtr.Zero)
				return false;

			inContext = Marshal.PtrToStructure<CFStreamClientContext> (inContextPtr)!;
			if (inContext.Version != 0)
				return false;

			context.Release ();
			context = inContext;
			context.Retain ();

			flags = inFlags;
			callback = inCallback;

			return true;
		}

		[Export ("getBuffer:length:")]
		protected unsafe virtual bool GetBuffer (out IntPtr buffer, out nuint len)
		{
			buffer = IntPtr.Zero;
			len = 0;
			return false;
		}

		/// <summary>Notifies consumers of events in the stream.</summary>
		/// <param name="eventType">The events to notify.</param>
		public void Notify (CFStreamEventType eventType)
		{
			if ((flags & eventType) == 0)
				return;

			context.Invoke (callback, Handle, eventType);
		}
	}
}
