//
// Compat.cs: Stuff we won't provide in Xamarin.iOS.dll or newer XAMCORE_* profiles
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin, Inc.

#if !NET
using System;

namespace Foundation {

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

#if !XAMCORE_3_0
	public partial class NSOperation {

		[Obsolete ("Use 'WaitUntilFinished' method.")]
		public virtual void WaitUntilFinishedNS ()
		{
			WaitUntilFinished ();
		}
	}

	public partial class NSNetService {

		[Obsolete ("This constructor does not create a valid instance of the type")]
		public NSNetService ()
		{
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
}

#endif // !NET
