#if (XAMCORE_2_0 && !MONOMAC) && !XAMCORE_4_0
using System;
using XamCore.Metal;
using XamCore.Foundation;

namespace XamCore.MetalPerformanceShaders {
	public partial class MPSImageHistogram {
		[Obsolete ("Please use 'GetHistogramSize' instead.")]
		public virtual nuint HistogramSizeForSourceFormat (MTLPixelFormat sourceFormat)
		{
			return GetHistogramSize (sourceFormat);
		}
	}
}
#endif