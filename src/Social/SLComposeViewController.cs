//
// SLComposeViewController.cs: Extensions to the SLComposeViewController class
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012 Xamarin Inc
//
#if !MONOMAC
using System;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.Foundation;
using XamCore.Accounts;
using XamCore.UIKit;

namespace XamCore.Social {

	public partial class SLComposeViewController {
		public static SLComposeViewController FromService (SLServiceKind serviceKind)
		{
			return FromService (SLRequest.KindToType (serviceKind));
		}

		public static bool IsAvailable (SLServiceKind serviceKind)
		{
			return IsAvailable (SLRequest.KindToType (serviceKind));
		}
	}
}
#endif
