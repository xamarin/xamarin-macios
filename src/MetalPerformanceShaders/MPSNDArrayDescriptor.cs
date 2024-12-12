#nullable enable

using System;

using Metal;
using Foundation;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSNDArrayDescriptor {

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public void PermuteWithDimensionOrder (nuint [] dimensionOrder)
		{
			if (dimensionOrder is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dimensionOrder));

			if (dimensionOrder.Length != (int) NumberOfDimensions)
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (dimensionOrder), $"Length must be equal to 'NumberOfDimensions'.");

			unsafe {
				fixed (nuint* ptr = dimensionOrder) {
					_PermuteWithDimensionOrder ((IntPtr) ptr);
				}
			}
		}
	}
}
