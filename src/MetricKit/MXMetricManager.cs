#if IOS
using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace MetricKit {

	public partial class MXMetricManager {

		public static OSLog MakeLogHandle (NSString category)
		{
			var ptr = _MakeLogHandle (category);
			return new OSLog (ptr, owns: true);
		}
	}
}
#endif
