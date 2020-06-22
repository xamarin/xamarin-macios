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

	public partial class NSNetService {

		[Obsolete ("This constructor does not create a valid instance of the type")]
		public NSNetService ()
		{
		}
	}
#endif

#if !XAMCORE_4_0 && !WATCH
	public partial class NSUserActivity {

		[Obsolete ("Use the constructor that allows you to set an activity type.")]
		public NSUserActivity ()
			: this (String.Empty)
		{
		}
	}
#endif
}
