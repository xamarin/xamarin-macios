#if !WATCH
using XamCore.Foundation;
using XamCore.CoreMedia;
using XamCore.ObjCRuntime;

namespace XamCore.AVFoundation {

	[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
	public partial class AudioRendererWasFlushedAutomaticallyEventArgs {
		public CMTime AudioRendererFlushTime { 
			get {
				return _AudioRendererFlushTime.CMTimeValue;
			}
		}
	}
}
#endif