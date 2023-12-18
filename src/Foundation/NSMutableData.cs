//
// NSMutableData.cs:
// Author:
//   Miguel de Icaza

// Copyright 2010, Novell, Inc.
// Copyright 2013-2014 Xamarin Inc (http://www.xamarin.com)

#nullable enable

using System;
using System.Runtime.InteropServices;
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

		public void AppendBytes (byte [] bytes)
		{
			if (bytes is null)
				throw new ArgumentNullException (nameof (bytes));

			unsafe {
				fixed (byte* p = bytes) {
					AppendBytes ((IntPtr) p, (nuint) bytes.Length);
				}
			}
		}

		public void AppendBytes (byte [] bytes, nint start, nint len)
		{
			if (bytes is null)
				throw new ArgumentNullException (nameof (bytes));

			if (start < 0 || start > bytes.Length)
				throw new ArgumentException (nameof (start));
			if (start + len > bytes.Length)
				throw new ArgumentException (nameof (len));

			unsafe {
				fixed (byte* p = &bytes [start]) {
					AppendBytes ((IntPtr) p, (nuint) len);
				}
			}
		}

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
