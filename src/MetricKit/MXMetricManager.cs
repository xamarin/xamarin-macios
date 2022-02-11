using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace MetricKit {

#if NET
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
#endif
	public partial class MXMetricManager {

		public static CoreFoundation.OSLog MakeLogHandle (NSString category)
		{
			var ptr = _MakeLogHandle (category);
			return new CoreFoundation.OSLog (ptr, owns: true);
		}
	}
}
