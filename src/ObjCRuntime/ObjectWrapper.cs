using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime {
	[StructLayout(LayoutKind.Explicit)]
	internal struct ObjectWrapper {
		[FieldOffset(0)] object obj;
		[FieldOffset(0)] IntPtr handle;

		internal static IntPtr Convert (object obj) {
			if (obj == null)
				return IntPtr.Zero;

			ObjectWrapper wrapper = new ObjectWrapper ();

			wrapper.obj = obj;

			return wrapper.handle;
		}
		
		internal static object Convert (IntPtr ptr) 
		{
			ObjectWrapper wrapper = new ObjectWrapper ();
			
			wrapper.handle = ptr;
				
			return wrapper.obj;
		}
	}
}
