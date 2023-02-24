#nullable enable

using System;
using Metal;
using Foundation;

namespace MetalPerformanceShaders {
	public partial class MPSNDArray {

		public static MPSNDArray Create (IMTLDevice device, ReadOnlySpan<float> values, params int [] shape)
		{
			var ushape = new nuint [shape.Length];
			for (var i = 0; i < shape.Length; i++) {
				ushape [i] = (nuint) shape [i];
			}
			var desc = MPSNDArrayDescriptor.Create (MPSDataType.Float32, ushape);
			var ndarray = new MPSNDArray (device, desc);
			ndarray.Write (values);
			return ndarray;
		}

		public void ExportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset)
		{
			ExportData (cmdBuf, buffer, sourceDataType, offset, IntPtr.Zero);
		}
		public unsafe void ExportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset, nint [] rowStrides)
		{
			fixed (nint* p = rowStrides) {
				ExportData (cmdBuf, buffer, sourceDataType, offset, (IntPtr) p);
			}
		}

		public void ImportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset)
		{
			ImportData (cmdBuf, buffer, sourceDataType, offset, IntPtr.Zero);
		}
		public unsafe void ImportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset, nint [] rowStrides)
		{
			fixed (nint* p = rowStrides) {
				ImportData (cmdBuf, buffer, sourceDataType, offset, (IntPtr) p);
			}
		}

		public void WriteBytes (IntPtr buffer)
		{
			WriteBytes (buffer, IntPtr.Zero);
		}
		public unsafe void WriteBytes (IntPtr buffer, nint [] strideBytesPerDimension)
		{
			fixed (nint* p = strideBytesPerDimension) {
				WriteBytes (buffer, (IntPtr) p);
			}
		}
		public unsafe void Write (ReadOnlySpan<float> values)
		{
			if (DataType != MPSDataType.Float32)
				throw new InvalidOperationException ($"Attempted to write array data of type {DataType} to span of Float32s.");
			nuint length = 1;
			var ndims = NumberOfDimensions;
			for (nuint i = 0; i < ndims; i++) {
				length *= GetLength (i);
			}
			if (length != (nuint) values.Length)
				throw new ArgumentException ($"The number of values ({values.Length}) does not match the shape length ({length}).");
			fixed (float* p = values) {
				WriteBytes ((IntPtr) p, strideBytesPerDimension: IntPtr.Zero);
			}
		}

		public void ReadBytes (IntPtr buffer)
		{
			ReadBytes (buffer, IntPtr.Zero);
		}
		public unsafe void ReadBytes (IntPtr buffer, nint [] strideBytesPerDimension)
		{
			fixed (nint* p = strideBytesPerDimension) {
				ReadBytes (buffer, (IntPtr) p);
			}
		}
		public unsafe void Read (Span<float> values)
		{
			if (DataType != MPSDataType.Float32)
				throw new InvalidOperationException ($"Attempted to read array data of type {DataType} to span of Float32s.");
			nuint length = 1;
			var ndims = NumberOfDimensions;
			for (nuint i = 0; i < ndims; i++) {
				length *= GetLength (i);
			}
			if (length != (nuint) values.Length)
				throw new ArgumentException ($"The number of values ({values.Length}) does not match the shape length ({length}).");
			fixed (float* p = values) {
				ReadBytes ((IntPtr) p, strideBytesPerDimension: IntPtr.Zero);
			}
		}
	}
}
