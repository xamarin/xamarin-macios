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
			if (buffers == null)
				throw new ArgumentNullException (nameof (buffers));
			if (offsets == null)
				throw new ArgumentNullException (nameof (offsets));

			unsafe {
				// cannot used fixed or we get:
				// Cannot take the address of, get the size of, or declare a pointer to a managed type ('IMTLBuffer')
				var buffersHandle = GCHandle.Alloc (buffers, GCHandleType.Pinned);
				IntPtr buffersPtr = buffersHandle.AddrOfPinnedObject ();
				fixed (void* offsetsPtr = offsets) { // can use fixed
					try {
						table.SetBuffers (buffersPtr, (IntPtr) offsetsPtr, range);
					} finally {
						buffersHandle.Free ();
					}
				}
			}
		}
	}
}
#endif
