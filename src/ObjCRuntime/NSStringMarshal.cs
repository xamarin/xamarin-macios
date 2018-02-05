// Copyright 2011, 2012 Xamarin Inc
using System.Runtime.InteropServices;
using System;
using Foundation;

#if !XAMCORE_2_0
namespace ObjCRuntime {

	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct NSStringStruct {
		public IntPtr ClassPtr;
		public int Flags;
		public char *UnicodePtr;
		public int Length;

#if !COREBUILD
		// The class pointer that we picked at runtime
		public readonly static IntPtr ReferencePtr;
		
		static NSStringStruct ()
		{
			using (var k = new NSString (""))
				ReferencePtr = Marshal.ReadIntPtr (k.Handle);
		}
#endif // !COREBUILD
	}
}
#endif
