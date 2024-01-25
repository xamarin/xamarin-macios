#nullable enable

using System;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

namespace MetricKit {

	public partial class MXMetricManager {

		public static CoreFoundation.OSLog MakeLogHandle (NSString category)
		{
			var ptr = _MakeLogHandle (category);
			return new CoreFoundation.OSLog (ptr, owns: true);
		}
	}
}
