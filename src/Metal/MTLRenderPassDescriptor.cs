#if XAMCORE_2_0 && !COREBUILD
using System;

namespace Metal {
	public partial class MTLRenderPassDescriptor {
		public unsafe void SetSamplePositions (MTLSamplePosition [] positions)
		{
			fixed (void* handle = positions)
				SetSamplePositions ((IntPtr)handle, (nuint)(positions?.Length ?? 0));
		}

		public unsafe nuint GetSamplePositions (MTLSamplePosition [] positions)
		{
			fixed (void* handle = positions) {
				nuint count = GetSamplePositions ((IntPtr)handle, (nuint)(positions?.Length ?? 0));
				return count;
			}
		}
	}
}
#endif