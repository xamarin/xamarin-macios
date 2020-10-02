#if __IOS__
using System;
using System.ComponentModel;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Introduced (PlatformName.iOS, 8,2, PlatformArchitecture.All)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public enum WKErrorCode : long {
		None = 0,
		UnknownError = 1,
		RequestReplyNotCalledError = 2,
		InvalidArgumentError = 3,
		MediaPlayerError = 4,
		DownloadError = 5,
		RecordingFailedError = 6,
	}

	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	static public class WKErrorCodeExtensions {
		public static NSString GetDomain (this WKErrorCode self)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}
	}
}
#endif // __IOS__
