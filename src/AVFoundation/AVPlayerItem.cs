#if !WATCH

using System;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AVFoundation {
	public partial class AVPlayerItem {

		[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
		public AVVideoApertureMode VideoApertureMode {
			get { return AVVideoApertureModeExtensions.GetValue (_VideoApertureMode); }
			set { _VideoApertureMode = value.GetConstant (); }
		}
	}
}

#endif
