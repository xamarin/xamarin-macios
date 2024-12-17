using System;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace AVFoundation {
	public partial class AVPlayerItem {

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public AVVideoApertureMode VideoApertureMode {
			get { return AVVideoApertureModeExtensions.GetValue (_VideoApertureMode); }
			set {
				var val = value.GetConstant ();
				if (val is not null)
					_VideoApertureMode = val;
			}
		}
	}
}
