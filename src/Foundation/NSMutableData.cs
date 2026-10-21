//
// NSMutableData.cs:
// Author:
//   Miguel de Icaza

// Copyright 2010, Novell, Inc.
// Copyright 2013-2014 Xamarin Inc (http://www.xamarin.com)

#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace Foundation {
	public partial class NSMutableData : IEnumerable, IEnumerable<byte> {

		public override byte this [nint idx] {
			set {
				if (idx < 0 || (ulong) idx > Length)
					throw new ArgumentException (nameof (idx));
				Marshal.WriteByte (new IntPtr (((long) Bytes) + idx), value);
			}
		}

		/// <summary>Appends a copy of the bytes in a managed array.</summary>
		/// <param name="bytes">The bytes to append.</param>
		/// <exception cref="ArgumentNullException"><paramref name="bytes" /> is <see langword="null" />.</exception>
		public void AppendBytes (byte [] bytes)
		{
			if (bytes is null)
				throw new ArgumentNullException (nameof (bytes));

			AppendBytes (new ReadOnlySpan<byte> (bytes));
		}

		/// <summary>Appends a copy of a range of bytes from a managed array.</summary>
		/// <param name="bytes">The array containing the bytes to append.</param>
		/// <param name="start">The zero-based index of the first byte to append.</param>
		/// <param name="len">The number of bytes to append.</param>
		/// <exception cref="ArgumentNullException"><paramref name="bytes" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="start" /> or <paramref name="len" /> is negative, or the range is outside <paramref name="bytes" />.</exception>
		public void AppendBytes (byte [] bytes, nint start, nint len)
		{
			if (bytes is null)
				throw new ArgumentNullException (nameof (bytes));

			if (start < 0 || start > bytes.Length)
				throw new ArgumentException (nameof (start));
			if (len < 0 || len > bytes.Length - start)
				throw new ArgumentException (nameof (len));

			AppendBytes (new ReadOnlySpan<byte> (bytes, (int) start, (int) len));
		}

		/// <summary>Appends a copy of the bytes in a span.</summary>
		/// <param name="bytes">The bytes to append.</param>
		/// <remarks>The bytes are copied during this call; the span is not retained.</remarks>
		public void AppendBytes (ReadOnlySpan<byte> bytes)
		{
			if (bytes.IsEmpty)
				return;

			unsafe {
				fixed (byte* p = bytes) {
					AppendBytes ((IntPtr) p, (nuint) bytes.Length);
				}
			}
		}

		/// <summary>Replaces a range of the data with a copy of the bytes in a span.</summary>
		/// <param name="range">The range of existing bytes to replace. A zero-length range inserts bytes at its location.</param>
		/// <param name="buffer">The replacement bytes. An empty span removes the requested range.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="range" /> is outside the data.</exception>
		/// <remarks>The data length changes by the difference between the span length and the range length. The bytes are copied during this call; the span is not retained. Do not modify the data concurrently with this call.</remarks>
		public void ReplaceBytes (NSRange range, ReadOnlySpan<byte> buffer)
		{
			var length = Length;
			if (range.Location < 0 || range.Length < 0 || (nuint) range.Location > length || (nuint) range.Length > length - (nuint) range.Location)
				throw new ArgumentOutOfRangeException (nameof (range));

			unsafe {
				fixed (byte* p = buffer) {
					ReplaceBytes (range, (IntPtr) p, (nuint) buffer.Length);
				}
			}
		}

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			IntPtr source = Bytes;
			nuint top = Length;

			for (nuint i = 0; i < top; i++) {
				if (source == Bytes && top == Length)
					yield return Marshal.ReadByte (source, (int) i);
				else
					throw new InvalidOperationException ("The NSMutableData has changed");
			}
		}

		/// <summary>Gets an enumerator for the bytes in the mutable data.</summary>
		/// <returns>An enumerator for the bytes in the mutable data.</returns>
		IEnumerator<byte> IEnumerable<byte>.GetEnumerator ()
		{
			IntPtr source = Bytes;
			nuint top = Length;

			for (nuint i = 0; i < top; i++) {
				if (source == Bytes && top == Length)
					yield return Marshal.ReadByte (source, (int) i);
				else
					throw new InvalidOperationException ("The NSMutableData has changed");
			}
		}
	}
}
