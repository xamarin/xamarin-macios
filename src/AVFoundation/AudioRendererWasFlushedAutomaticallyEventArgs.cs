#if !WATCH
using Foundation;
using CoreMedia;
using ObjCRuntime;

#nullable enable

namespace AVFoundation {

#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
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
