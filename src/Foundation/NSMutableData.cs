//
// NSMutableData.cs:
// Author:
//   Miguel de Icaza

// Copyright 2010, Novell, Inc.
// Copyright 2013-2014 Xamarin Inc (http://www.xamarin.com)

using System;
using ObjCRuntime;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

namespace Foundation {
	public partial class NSMutableData : IEnumerable, IEnumerable<byte> {

		public override byte this [nint idx] {
			set {
				if (idx < 0 || (ulong) idx > Length)
					throw new ArgumentException ("idx");
				Marshal.WriteByte (new IntPtr (Bytes.ToInt64 () + idx), value);
			}
		}

		public void AppendBytes (byte [] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException ("bytes");
			
			unsafe {
				fixed (byte *p = &bytes[0]){
					AppendBytes ((IntPtr) p, (nuint) bytes.Length);
				}
			}
		}

		public void AppendBytes (byte [] bytes, nint start, nint len)
		{
			if (bytes == null)
				throw new ArgumentNullException ("bytes");

			if (start < 0 || start > bytes.Length)
				throw new ArgumentException ("start");
			if (start+len > bytes.Length)
				throw new ArgumentException ("len");
			
			unsafe {
				fixed (byte *p = &bytes[start]){
					AppendBytes ((IntPtr) p, (nuint) len);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			IntPtr source = Bytes;
			nuint top = Length;

			for (nuint i = 0; i < top; i++){
				if (source == Bytes && top == Length)
					yield return Marshal.ReadByte (source, (int)i);
				else
					throw new InvalidOperationException ("The NSMutableData has changed");
			}
		}

		IEnumerator<byte> IEnumerable<byte>.GetEnumerator ()
		{
			IntPtr source = Bytes;
			nuint top = Length;

			for (nuint i = 0; i < top; i++){
				if (source == Bytes && top == Length)
					yield return Marshal.ReadByte (source, (int)i);
				else
					throw new InvalidOperationException ("The NSMutableData has changed");
			}
		}
#if !XAMCORE_2_0
		// note: duplicate selector were registered for the method and the Length property setter
		[Obsolete ("Use the 'Length' property setter.")]
		public virtual void SetLength (nuint len)
		{
			Length = len;
		}
#endif
	}
}
