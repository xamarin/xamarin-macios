#if (XAMCORE_2_0 && !MONOMAC) && !XAMCORE_4_0
using System;
using Metal;
using Foundation;

namespace MetalPerformanceShaders {
	public partial class MPSMatrixDescriptor {

		[Obsolete ("Please use 'GetRowBytesFromColumns' instead.")]
		public static nuint GetRowBytes (nuint columns, MPSDataType dataType)
		{
			return GetRowBytesFromColumns (columns, dataType);
		}
	}
}
#endif
