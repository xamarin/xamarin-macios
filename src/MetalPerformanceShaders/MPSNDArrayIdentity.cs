#nullable enable

using System;
using Metal;
using Foundation;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSNDArrayIdentity {
		public MPSNDArray? Reshape (IMTLCommandBuffer? commandBuffer, MPSNDArray sourceArray, nuint [] dimensionSizes, MPSNDArray? destinationArray)
		{
			MPSNDArray? rv;
			unsafe {
				fixed (nuint* dimensionsPtr = dimensionSizes) {
					rv = _Reshape (commandBuffer, sourceArray, (nuint) dimensionSizes.Length, (IntPtr) dimensionsPtr, destinationArray);
				}
			}
			return rv;
		}

		public MPSNDArray? Reshape (IMTLComputeCommandEncoder? encoder, IMTLCommandBuffer? commandBuffer, MPSNDArray sourceArray, nuint [] dimensionSizes, MPSNDArray? destinationArray)
		{
			MPSNDArray? rv;
			unsafe {
				fixed (nuint* dimensionsPtr = dimensionSizes) {
					rv = _Reshape (encoder, commandBuffer, sourceArray, (nuint) dimensionSizes.Length, (IntPtr) dimensionsPtr, destinationArray);
				}
			}
			return rv;
		}
	}
}
