//
// Compat.cs: Stuff we won't provide in Xamarin.iOS.dll or newer XAMCORE_* profiles
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin, Inc.

using System;
using System.ComponentModel;

using ObjCRuntime;

namespace Foundation {

#if !NET
#if MONOMAC
	public partial class NSError {

		// removed in Xcode 11 GM
		[Obsolete ("This API has been removed.")]
		public static NSError GetFileProviderErrorForOutOfDateItem (FileProvider.INSFileProviderItem updatedVersion)
		{
			return null;
		}
	}
#endif

#if !WATCH
	public partial class NSUserActivity {

		[Obsolete ("Use the constructor that allows you to set an activity type.")]
		public NSUserActivity ()
			: this (String.Empty)
		{
		}
	}
#endif
#endif // !NET

#if !XAMCORE_5_0
#if __IOS__ && !__MACCATALYST__
	public partial class NSUrlConnection {
		// Extension from iOS5, NewsstandKit
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("The NewsstandKit framework has been removed from iOS.")]
		public virtual global::NewsstandKit.NKAssetDownload NewsstandAssetDownload {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

	}
#endif // __IOS__
#endif // !XAMCORE_5_0
}

