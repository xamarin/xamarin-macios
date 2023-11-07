using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Foundation {
	public partial class NSOutputStream : NSStream {
		const string selWriteMaxLength = "write:maxLength:";

		public nint Write (byte [] buffer)
		{
			if (buffer is null)
				throw new ArgumentNullException ("buffer");

			return Write (buffer, (nuint) buffer.Length);
		}

		// This is done manually because the generator can't handle byte[] as a native pointer (it will try to use NSArray instead).
		public nint Write (byte [] buffer, nuint len)
		{
			return objc_msgSend (Handle, Selector.GetHandle (selWriteMaxLength), buffer, len);
		}

		public unsafe nint Write (byte [] buffer, int offset, nuint len)
		{
			if (offset + (long) len > buffer.Length)
				throw new ArgumentException ();

			fixed (byte* ptr = &buffer [offset])
				return objc_msgSend (Handle, Selector.GetHandle (selWriteMaxLength), (IntPtr) ptr, len);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern nint objc_msgSend (IntPtr handle, IntPtr sel, [In, Out] byte [] buffer, nuint len);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern nint objc_msgSend (IntPtr handle, IntPtr sel, IntPtr buffer, nuint len);
	}
}
