#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.Metal;
using XamCore.Foundation;

namespace XamCore.MetalPerformanceShaders {
	public partial class MPSImageHistogram {
		public virtual nuint HistogramSizeForSourceFormat (MTLPixelFormat sourceFormat)
		{
			return GetHistogramSize (sourceFormat);
		}
	}
}
#endif