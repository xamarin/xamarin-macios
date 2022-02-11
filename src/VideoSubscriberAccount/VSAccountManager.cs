//
// VSAccountManager extensions & syntax sugar
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !MONOMAC && !__MACCATALYST__

using System;
using System.Threading.Tasks;
using Foundation;
using System.Runtime.Versioning;

namespace VideoSubscriberAccount {
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.14")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class VSAccountManager {

		public void CheckAccessStatus (VSAccountManagerAccessOptions accessOptions, Action<VSAccountAccessStatus, NSError> completionHandler)
		{
			if (accessOptions == null)
				throw new ArgumentNullException (nameof (accessOptions));
			if (completionHandler == null)
				throw new ArgumentNullException (nameof (completionHandler));

			CheckAccessStatus (accessOptions.Dictionary, completionHandler);
		}

		public Task<VSAccountAccessStatus> CheckAccessStatusAsync (VSAccountManagerAccessOptions accessOptions)
		{
			if (accessOptions == null)
				throw new ArgumentNullException (nameof (accessOptions));

			return CheckAccessStatusAsync (accessOptions.Dictionary);
		}
	}
}

#endif
