#nullable enable

using System;
using System.Buffers;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;
using MetalPerformanceShaders;

namespace MetalPerformanceShadersGraph {
	public static partial class MPSGraphMemoryOps_Extensions {
		public static unsafe MPSGraphTensor Constant (this MPSGraph graph, float scalar)
		{
			return graph.Constant ((double) scalar, new [] { 1 }, MPSDataType.Float32);
		}

		public static unsafe MPSGraphTensor Constant (this MPSGraph graph, ReadOnlySpan<float> values, int [] shape)
		{
			var length = 1;
			for (var i = 0; i < shape.Length; i++)
				length *= shape [i];
			if (length != values.Length)
				throw new ArgumentException ($"The number of values ({values.Length}) does not match the shape length ({length}).");
			fixed (float* p = values) {
				using var data = NSData.FromBytesNoCopy ((IntPtr) p, (nuint) (values.Length * 4), freeWhenDone: false);
				return graph.Constant (data, shape, MPSDataType.Float32);
			}
		}

		public static MPSGraphTensor Variable (this MPSGraph graph, float initialValue, int [] shape, string? name = null)
		{
			var length = 1;
			for (var i = 0; i < shape.Length; i++)
				length *= shape [i];
			var pool = ArrayPool<float>.Shared;
			var a = pool.Rent (length);
			Array.Fill (a, initialValue);
			var v = Variable (graph, a, shape, name);
			pool.Return (a);
			return v;
		}

		public static unsafe MPSGraphTensor Variable (this MPSGraph graph, ReadOnlySpan<float> initialValues, int [] shape, string? name = null)
		{
			var length = 1;
			for (var i = 0; i < shape.Length; i++)
				length *= shape [i];
			if (length != initialValues.Length)
				throw new ArgumentException ($"The number of initial values ({initialValues.Length}) does not match the shape length ({length}).");
			fixed (float* p = initialValues) {
				using var data = NSData.FromBytesNoCopy ((IntPtr) p, (nuint) (initialValues.Length * 4), freeWhenDone: false);
				return graph.Variable (data, shape, MPSDataType.Float32, name);
			}
		}
	}
}
