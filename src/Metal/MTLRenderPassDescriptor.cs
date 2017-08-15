using System;

namespace XamCore.Metal {
	public partial class MTLRenderPassDescriptor {
		public unsafe void SetSamplePositions (MTLSamplePosition [] positions)
		{
			unsafe
			{
				fixed (void* handle = positions)
					SetSamplePositions ((IntPtr)handle, positions?.Length ?? 0);
			}
		}

		public unsafe nuint GetSamplePositions (MTLSamplePosition [] positions)
		{
			unsafe {
				fixed (void* handle = positions) {
					nuint count = GetSamplePositions ((IntPtr)handle, positions?.Length ?? 0);
					return count;
				}
			}
		}
	}
}
