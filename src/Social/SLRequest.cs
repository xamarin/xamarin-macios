//
// SLRequest.cs: extensions to the SLRequest class
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2013 Xamarin Inc
//

#nullable enable

using System;
using ObjCRuntime;
using Foundation;
using Accounts;

namespace Social {

	public partial class SLRequest {
		internal static NSString KindToType (SLServiceKind kind)
		{
			switch (kind) {
			case SLServiceKind.Facebook:
				return SLServiceType.Facebook;
			case SLServiceKind.Twitter:
				return SLServiceType.Twitter;
			case SLServiceKind.SinaWeibo:
				return SLServiceType.SinaWeibo;
			case SLServiceKind.TencentWeibo:
				return SLServiceType.TencentWeibo;
#if MONOMAC
			case SLServiceKind.LinkedIn:
				return SLServiceType.LinkedIn;
#endif
			default:
				throw new ArgumentOutOfRangeException (nameof (kind));
			}
		}

		public static SLRequest Create (SLServiceKind serviceKind, SLRequestMethod method, NSUrl url, NSDictionary parameters)
		{
			return Create (KindToType (serviceKind), method, url, parameters);
		}
	}
}
