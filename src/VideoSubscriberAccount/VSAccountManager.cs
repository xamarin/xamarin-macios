//
// VSAccountManager extensions & syntax sugar
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#nullable enable

#if !MONOMAC && !__MACCATALYST__

using System;
using System.Threading.Tasks;

using Foundation;

namespace VideoSubscriberAccount {
	public partial class VSAccountManager {

		public void CheckAccessStatus (VSAccountManagerAccessOptions accessOptions, Action<VSAccountAccessStatus, NSError> completionHandler)
		{
			if (accessOptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (accessOptions));
			if (completionHandler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (completionHandler));

			CheckAccessStatus (accessOptions.Dictionary, completionHandler);
		}

		public Task<VSAccountAccessStatus> CheckAccessStatusAsync (VSAccountManagerAccessOptions accessOptions)
		{
			if (accessOptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (accessOptions));

			return CheckAccessStatusAsync (accessOptions.Dictionary);
		}
	}
}

#endif
