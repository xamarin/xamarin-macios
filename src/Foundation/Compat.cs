//
// Compat.cs: Stuff we won't provide in Xamarin.iOS.dll or newer XAMCORE_* profiles
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin, Inc.


using System;

namespace Foundation {

#if !XAMCORE_3_0
	public partial class NSOperation {

		[Obsolete ("Use 'WaitUntilFinished' method.")]
		public virtual void WaitUntilFinishedNS ()
		{
			WaitUntilFinished ();
		}
	}
#endif

#if !XAMCORE_4_0 && (XAMCORE_2_0 || !MONOMAC) && !WATCH
	public partial class NSUserActivity {

		[Obsolete ("Use the constructor that allows you to set an activity type.")]
		public NSUserActivity ()
#if XAMCORE_2_0
			: this (String.Empty)
#else
			: this (NSString.Empty)
#endif
		{
		}
	}
#endif
}
