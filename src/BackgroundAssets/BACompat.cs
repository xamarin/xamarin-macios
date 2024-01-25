//
// BACompar.cs: Compatibility functions
//
// Copyright 2022 Microsoft Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;

#nullable enable


namespace BackgroundAssets {

	public partial class BAAppExtensionInfo {

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property has been removed. It will always return string.Empty.")]
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public virtual string ApplicationIdentifier => string.Empty;

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property has been removed. It will always return string.Empty.")]
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif
		bool DownloadSizeRestricted => false;

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property has been removed. It will always return string.Empty.")]
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public virtual NSDate LastPeriodicCheckTime => NSDate.Now;

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property has been removed. It will always return string.Empty.")]
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public virtual NSDate LastApplicationLaunchTime => NSDate.Now;
	}

	public partial class BADownloadManager {

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property has been removed. It will always return string.Empty.")]
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public virtual void PerformWithExclusiveControl (NSDate date, Action<bool, NSError?> performHandler) => performHandler?.Invoke (false, null);
	}

	public partial class BADownload {

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property has been removed. It will always return string.Empty.")]
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public virtual NSError? Error => null;
	}

}
