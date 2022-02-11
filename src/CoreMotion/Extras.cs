//
// CoreMotion's extra methods
//
using Foundation;
using System;
using System.Runtime.Versioning;

namespace CoreMotion {

#if NET
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class CMAccelerometerData {
		public override string ToString ()
		{
			return String.Format ("t={0} {1}", Acceleration.ToString (), Timestamp);
		}
	}
}
