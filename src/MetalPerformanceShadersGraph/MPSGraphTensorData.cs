#nullable enable

using System;
using System.Buffers;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;
using MetalPerformanceShaders;

namespace MetalPerformanceShadersGraph {
	public partial class MPSGraphTensorData {
		public static MPSGraphTensorData Create (IMTLDevice device, ReadOnlySpan<float> values, params int [] shape)
		{
			var ndarray = MPSNDArray.Create (device, values, shape);
			return new MPSGraphTensorData (ndarray);
		}

		public static MPSGraphTensorData Create (params MPSImage [] imageBatch)
		{
			if (imageBatch is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (imageBatch));
			return new MPSGraphTensorData (NSArray<MPSImage>.FromNSObjects (imageBatch)!);
		}

		public void Read (Span<float> values)
		{
			using var ndarray = this.MPSNDArray;
			ndarray.Read (values);
		}
	}
}
