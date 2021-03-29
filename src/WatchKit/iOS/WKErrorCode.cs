#if __IOS__
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
#endif
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

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
#endif
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
