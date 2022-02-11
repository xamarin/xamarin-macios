#if !MONOMAC && !NET && !__MACCATALYST__
using System;
using Metal;
using Foundation;

namespace MetalPerformanceShaders {
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MPSMatrixDescriptor {

		[Obsolete ("Please use 'GetRowBytesFromColumns' instead.")]
		public static nuint GetRowBytes (nuint columns, MPSDataType dataType)
		{
			return GetRowBytesFromColumns (columns, dataType);
		}
	}
}
#endif
