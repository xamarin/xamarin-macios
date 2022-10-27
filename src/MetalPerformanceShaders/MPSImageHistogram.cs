#if !MONOMAC && !NET
using System;
using Metal;
using Foundation;

namespace MetalPerformanceShaders {
	public partial class MPSImageHistogram {
		[Obsolete ("Please use 'GetHistogramSize' instead.")]
		public virtual nuint HistogramSizeForSourceFormat (MTLPixelFormat sourceFormat)
		{
			return GetHistogramSize (sourceFormat);
		}
	}
}
#endif
