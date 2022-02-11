#if !COREBUILD
using System;
using System.Runtime.Versioning;

#nullable enable

namespace Metal {
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
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
