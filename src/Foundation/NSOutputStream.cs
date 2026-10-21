
namespace Foundation {
	public partial class NSOutputStream : NSStream {
		const string selWriteMaxLength = "write:maxLength:";

		/// <summary>Writes bytes from the provided buffer to the stream.</summary>
		/// <param name="buffer">The source buffer.</param>
		/// <returns>The number of bytes written, which may be less than the buffer length, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <remarks>An empty buffer is passed to the native write method with a length of zero, which may change the stream's state.</remarks>
		public nint Write (byte [] buffer)
		{
			if (buffer is null)
				throw new ArgumentNullException ("buffer");

			return Write (buffer, (nuint) buffer.Length);
		}

		// This is done manually because the generator can't handle byte[] as a native pointer (it will try to use NSArray instead).
		/// <summary>Writes bytes from the start of the provided buffer to the stream.</summary>
		/// <param name="buffer">The source buffer.</param>
		/// <param name="len">The maximum number of bytes to write, which must not exceed the buffer length.</param>
		/// <returns>The number of bytes written, which may be less than <paramref name="len" />, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="len" /> exceeds the buffer length.</exception>
		/// <remarks>A length of zero is passed to the native write method and may change the stream's state.</remarks>
		public nint Write (byte [] buffer, nuint len)
		{
			if (buffer is null)
				throw new ArgumentNullException (nameof (buffer));
			if (len > (nuint) buffer.Length)
				throw new ArgumentException ();
			var result = objc_msgSend (Handle, Selector.GetHandle (selWriteMaxLength), buffer, len);
			GC.KeepAlive (this);
			return result;
		}

		/// <summary>Writes bytes from a range of the provided buffer to the stream.</summary>
		/// <param name="buffer">The source buffer.</param>
		/// <param name="offset">The byte offset of the first byte to write.</param>
		/// <param name="len">The maximum number of bytes to write.</param>
		/// <returns>The number of bytes written, which may be less than <paramref name="len" />, or -1 if an error occurs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">The requested range extends beyond <paramref name="buffer" />.</exception>
		/// <exception cref="IndexOutOfRangeException"><paramref name="offset" /> is negative, or equals the buffer length and <paramref name="len" /> is zero.</exception>
		/// <remarks>A length of zero is passed to the native write method and may change the stream's state.</remarks>
		public unsafe nint Write (byte [] buffer, int offset, nuint len)
		{
			if (buffer is null)
				throw new ArgumentNullException (nameof (buffer));
			if (offset < 0)
				throw new IndexOutOfRangeException ();
			if (offset > buffer.Length || len > (nuint) (buffer.Length - offset))
				throw new ArgumentException ();

			fixed (byte* ptr = &buffer [offset]) {
				var result = objc_msgSend (Handle, Selector.GetHandle (selWriteMaxLength), (IntPtr) ptr, len);
				GC.KeepAlive (this);
				return result;
			}
		}

		/// <summary>Writes bytes from the provided span to the stream.</summary>
		/// <param name="buffer">The source bytes.</param>
		/// <returns>The number of bytes written, which may be less than the span length, or -1 if an error occurs.</returns>
		/// <remarks>The buffer is pinned only during the call. An empty span is passed to the native write method with a length of zero, which may change the stream's state.</remarks>
		public unsafe nint Write (ReadOnlySpan<byte> buffer)
		{
			byte empty = 0;
			fixed (byte* ptr = buffer) {
				var result = objc_msgSend (Handle, Selector.GetHandle (selWriteMaxLength), (IntPtr) (buffer.IsEmpty ? &empty : ptr), (nuint) buffer.Length);
				GC.KeepAlive (this);
				return result;
			}
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern nint objc_msgSend (IntPtr handle, IntPtr sel, [In, Out] byte [] buffer, nuint len);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern nint objc_msgSend (IntPtr handle, IntPtr sel, IntPtr buffer, nuint len);
	}
}
