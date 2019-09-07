#if (XAMCORE_2_0 && !MONOMAC) && !XAMCORE_4_0
using System;
using Metal;
using Foundation;

namespace MetalPerformanceShaders {
	public partial class MPSNDArrayDescriptor {

		public unsafe static MPSNDArrayDescriptor Create (MPSDataType dataType, params nuint[] dimensionSizes)
		{
			fixed (nuint* p = dimensionSizes) {
				return Create (dataType, (nuint)dimensionSizes.Length, (IntPtr)p);
			}
		}
		public unsafe void Reshape (params nuint[] dimensionSizes)
		{
			fixed (nuint* p = dimensionSizes) {
				Reshape ((nuint)dimensionSizes.Length, (IntPtr)p);
			}
		}
	}

	public partial class MPSNDArray {

		public void ExportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset)
		{
			ExportData (cmdBuf, buffer, sourceDataType, offset, IntPtr.Zero);
		}
		public unsafe void ExportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset, nint[] rowStrides)
		{
			if (rowStrides == null) {
				ExportData (cmdBuf, buffer, sourceDataType, offset, IntPtr.Zero);
			}
			else {
				fixed (nint* p = rowStrides) {
					ExportData (cmdBuf, buffer, sourceDataType, offset, (IntPtr)p);
				}
			}
		}

		public void ImportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset)
		{
			ImportData (cmdBuf, buffer, sourceDataType, offset, IntPtr.Zero);
		}
		public unsafe void ImportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset, nint[] rowStrides)
		{
			if (rowStrides == null) {
				ImportData (cmdBuf, buffer, sourceDataType, offset, IntPtr.Zero);
			}
			else {
				fixed (nint* p = rowStrides) {
					ImportData (cmdBuf, buffer, sourceDataType, offset, (IntPtr)p);
				}
			}
		}

		public void WriteBytes (IntPtr buffer)
		{
			WriteBytes (buffer, IntPtr.Zero);
		}
		public unsafe void WriteBytes (IntPtr buffer, nint[] strideBytesPerDimension)
		{
			if (strideBytesPerDimension == null) {
				WriteBytes (buffer, IntPtr.Zero);
			}
			else {
				fixed (nint* p = strideBytesPerDimension) {
					WriteBytes (buffer, (IntPtr)p);
				}
			}
		}

		public void ReadBytes (IntPtr buffer)
		{
			ReadBytes (buffer, IntPtr.Zero);
		}
		public unsafe void ReadBytes (IntPtr buffer, nint[] strideBytesPerDimension)
		{
			if (strideBytesPerDimension == null) {
				ReadBytes (buffer, IntPtr.Zero);
			}
			else {
				fixed (nint* p = strideBytesPerDimension) {
					ReadBytes (buffer, (IntPtr)p);
				}
			}
		}
	}
}
#endif
