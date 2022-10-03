//
// BACompar.cs: Compatibility functions
//
// Copyright 2022 Microsoft Inc. All rights reserved.
using System;
using System.Runtime.Versioning;
using Foundation;

#nullable enable


namespace BackgroundAssets {

	public partial class BAAppExtensionInfo {

#if !NET
		[Obsolete ("This property has been removed. It will always return string.Empty.")]
#else
		[UnsupportedOSPlatform ("ios16.1")]
		[UnsupportedOSPlatform ("tvos16.1")]
		[UnsupportedOSPlatform ("maccatalyst16.1")]
		[UnsupportedOSPlatform ("macos13.0")]
#endif
		public virtual string ApplicationIdentifier => string.Empty;

#if !NET
		[Obsolete ("This property has been removed. It will always return false.")]
#else
		[UnsupportedOSPlatform ("ios16.1")]
		[UnsupportedOSPlatform ("tvos16.1")]
		[UnsupportedOSPlatform ("maccatalyst16.1")]
		[UnsupportedOSPlatform ("macos13.0")]
#endif
		bool DownloadSizeRestricted => false;

#if !NET
		[Obsolete ("This property has been removed. It will always return NSDate.Now.")]
#else
		[UnsupportedOSPlatform ("ios16.1")]
		[UnsupportedOSPlatform ("tvos16.1")]
		[UnsupportedOSPlatform ("maccatalyst16.1")]
		[UnsupportedOSPlatform ("macos13.0")]
#endif
		public virtual NSDate LastPeriodicCheckTime => NSDate.Now;

#if !NET
		[Obsolete ("This property has been removed. It will always return NSDate.Now.")]
#else
		[UnsupportedOSPlatform ("ios16.1")]
		[UnsupportedOSPlatform ("tvos16.1")]
		[UnsupportedOSPlatform ("maccatalyst16.1")]
		[UnsupportedOSPlatform ("macos13.0")]
#endif
		public virtual NSDate LastApplicationLaunchTime => NSDate.Now;
	}

	public partial class BADownloadManager {

#if !NET
		[Obsolete ("This method has been removed. It will always execution the handler with false. Use PerformWithExclusiveControl (Action<bool, NSError>) instead.")]
#else
		[UnsupportedOSPlatform ("ios16.1")]
		[UnsupportedOSPlatform ("tvos16.1")]
		[UnsupportedOSPlatform ("maccatalyst16.1")]
		[UnsupportedOSPlatform ("macos13.0")]
#endif
		public virtual void PerformWithExclusiveControl (NSDate date, Action<bool, NSError?> performHandler) => performHandler?.Invoke (false, null);
	}

	public partial class BADownload {

#if !NET
		[Obsolete ("This property has been removed. It will always return null.")]
#else
		[UnsupportedOSPlatform ("ios16.1")]
		[UnsupportedOSPlatform ("tvos16.1")]
		[UnsupportedOSPlatform ("maccatalyst16.1")]
		[UnsupportedOSPlatform ("macos13.0")]
#endif
		public virtual NSError? Error  => null;
	}

}
