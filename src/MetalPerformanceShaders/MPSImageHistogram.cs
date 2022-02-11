#if !MONOMAC && !NET
using System;
using Metal;
using Foundation;

namespace MetalPerformanceShaders {
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class MPSImageHistogram {
		[Obsolete ("Please use 'GetHistogramSize' instead.")]
		public virtual nuint HistogramSizeForSourceFormat (MTLPixelFormat sourceFormat)
		{
			return GetHistogramSize (sourceFormat);
		}
	}
}
#endif
