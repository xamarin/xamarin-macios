//
// SLComposeViewController.cs: Extensions to the SLComposeViewController class
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012 Xamarin Inc
//

#nullable enable

#if !MONOMAC
using System;
using ObjCRuntime;
using Foundation;
using Accounts;
using UIKit;

namespace Social {

	public partial class SLComposeViewController {
		public static SLComposeViewController FromService (SLServiceKind serviceKind)
		{
			return FromService (serviceKind.GetConstant ()!);
		}

		public static bool IsAvailable (SLServiceKind serviceKind)
		{
			return IsAvailable (serviceKind.GetConstant ()!);
		}
	}
}
#endif
