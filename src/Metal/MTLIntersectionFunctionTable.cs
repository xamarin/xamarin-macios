#if !TVOS
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Metal {

	// add some extension methods to make the API of the protocol nicer
	public static class MTLIntersectionFunctionTableExtensions {

		[Mac (11,0), iOS (14,0), NoTV]
		public static void SetBuffers (this IMTLIntersectionFunctionTable table, IMTLBuffer[] buffers, nuint[] offsets, NSRange range)
		{
			// pin address, pass it as a IntPtr which is what the protocol expects
			var offsetsHandle = GCHandle.Alloc (offsets, GCHandleType.Pinned);
			IntPtr offsetsPtr = offsetsHandle.AddrOfPinnedObject ();
			try {
				table.SetBuffers (buffers, offsetsPtr, range);
			} finally {
				offsetsHandle.Free ();
			}
		}
	}
}
#endif
