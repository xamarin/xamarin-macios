//
// Compat.cs: Stuff we won't provide in Xamarin.iOS.dll or newer XAMCORE_* profiles
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin, Inc.


using System;

namespace XamCore.Foundation {

#if !XAMCORE_3_0
	public partial class NSOperation {

		[Obsolete ("Use WaitUntilFinished method")]
		public virtual void WaitUntilFinishedNS ()
		{
			WaitUntilFinished ();
		}
	}
#endif
}
