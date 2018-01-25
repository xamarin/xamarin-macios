// Copyright 2014 Xamarin Inc.

#if !XAMCORE_2_0

using System;

#if !MONOMAC
using UIKit;
#endif

namespace Foundation {

	// not static in case Apple decide to introduce their own type
	public partial class NSExtension {

		private NSExtension ()
		{
		}

		[Obsolete ("It is not necessary to call this method anymore. It will be removed in a future release.")]
		public static void Initialize ()
		{
#if !MONOMAC
			UIApplication.Initialize ();
#endif
		}
	}
}
#endif
