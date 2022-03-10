#if !WATCH
using Foundation;
using CoreMedia;
using ObjCRuntime;

namespace AVFoundation {

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (11, 0)]
	[NoWatch]
	[Mac (10, 13)]
	[iOS (11, 0)]
#endif
	public partial class AudioRendererWasFlushedAutomaticallyEventArgs {
		public CMTime AudioRendererFlushTime { 
			get {
				return _AudioRendererFlushTime.CMTimeValue;
			}
		}
	}
}
#endif
