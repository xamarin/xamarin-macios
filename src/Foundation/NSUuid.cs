//
// NSUuid.cs: support code for the NSUUID binding
//
// Authors:
//    MIguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2013 Xamarin Inc
//

namespace Foundation {
	partial class NSUuid {

		/// <summary>Create a new <see cref="NSUuid" /> from the specified bytes.</summary>
		/// <param name="bytes">The array of bytes to initialize the new <see cref="NSUuid" /> with. Must have at least 16 elements.</param>
		/// <exception cref="ArgumentNullException"><paramref name="bytes" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="bytes" /> contains fewer than 16 bytes.</exception>
		/// <remarks>The first 16 bytes are copied during construction. Any remaining bytes are ignored.</remarks>
		public NSUuid (byte [] bytes) : this (bytes is null ? throw new ArgumentNullException (nameof (bytes)) : new ReadOnlySpan<byte> (bytes))
		{
		}

		/// <summary>Creates a UUID by copying the first 16 bytes of a span.</summary>
		/// <param name="bytes">The UUID bytes. Must contain at least 16 bytes.</param>
		/// <exception cref="ArgumentException"><paramref name="bytes" /> contains fewer than 16 bytes.</exception>
		/// <remarks>The bytes are copied during construction; the span is not retained. Any bytes after the first 16 are ignored.</remarks>
		public NSUuid (ReadOnlySpan<byte> bytes) : base (NSObjectFlag.Empty)
		{
			if (bytes.Length < 16)
				throw new ArgumentException ("length must be at least 16 bytes");

			unsafe {
				fixed (byte* p = bytes) {
					IntPtr ptr = (IntPtr) p;

					if (IsDirectBinding) {
						InitializeHandle (Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("initWithUUIDBytes:"), ptr), "initWithUUIDBytes:");
					} else {
						var __objc_super__ = new global::ObjCRuntime.ObjCSuper (this);
						InitializeHandle (Messaging.IntPtr_objc_msgSendSuper_IntPtr (&__objc_super__, Selector.GetHandle ("initWithUUIDBytes:"), ptr), "initWithUUIDBytes:");
						GC.KeepAlive (this);
					}
				}
			}
		}

		/// <summary>Gets the UUID as a 16-byte array.</summary>
		/// <returns>A 16-byte array containing the UUID value.</returns>
		public byte [] GetBytes ()
		{
			byte [] ret = new byte [16];

			GetBytes (ret.AsSpan ());

			return ret;
		}

		/// <summary>Copies the 16 UUID bytes into a destination span.</summary>
		/// <param name="buffer">The destination, which must contain at least 16 bytes.</param>
		/// <exception cref="ArgumentException"><paramref name="buffer" /> contains fewer than 16 bytes.</exception>
		/// <remarks>The copy completes during this call. Any bytes after the first 16 are unchanged.</remarks>
		public void GetBytes (Span<byte> buffer)
		{
			if (buffer.Length < 16)
				throw new ArgumentException ("The destination must contain at least 16 bytes.", nameof (buffer));

			unsafe {
				fixed (byte* buf = buffer) {
					GetUuidBytes ((IntPtr) buf);
				}
			}
		}
	}
}
