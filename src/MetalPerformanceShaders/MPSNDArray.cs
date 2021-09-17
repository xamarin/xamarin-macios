using System;
using Metal;
using Foundation;

namespace MetalPerformanceShaders {
	public partial class MPSNDArray {

		public void ExportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset)
		{
			ExportData (cmdBuf, buffer, sourceDataType, offset, IntPtr.Zero);
		}
		public unsafe void ExportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset, nint[] rowStrides)
		{
			fixed (nint* p = rowStrides) {
				ExportData (cmdBuf, buffer, sourceDataType, offset, (IntPtr)p);
			}
		}

		public void ImportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset)
		{
			ImportData (cmdBuf, buffer, sourceDataType, offset, IntPtr.Zero);
		}
		public unsafe void ImportData (IMTLCommandBuffer cmdBuf, IMTLBuffer buffer, MPSDataType sourceDataType, nuint offset, nint[] rowStrides)
		{
			fixed (nint* p = rowStrides) {
				ImportData (cmdBuf, buffer, sourceDataType, offset, (IntPtr)p);
			}
		}

		public void WriteBytes (IntPtr buffer)
		{
			WriteBytes (buffer, IntPtr.Zero);
		}
		public unsafe void WriteBytes (IntPtr buffer, nint[] strideBytesPerDimension)
		{
			fixed (nint* p = strideBytesPerDimension) {
				WriteBytes (buffer, (IntPtr)p);
			}
		}

		public void ReadBytes (IntPtr buffer)
		{
			ReadBytes (buffer, IntPtr.Zero);
		}
		public unsafe void ReadBytes (IntPtr buffer, nint[] strideBytesPerDimension)
		{
			fixed (nint* p = strideBytesPerDimension) {
				ReadBytes (buffer, (IntPtr)p);
			}
		}
	}
}
