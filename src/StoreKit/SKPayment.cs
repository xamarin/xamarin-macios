// Copyright 2015 Xamarin Inc.

using System;

using StoreKit;
using System.Runtime.Versioning;

namespace StoreKit {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class SKPayment {

#if !XAMCORE_3_0

		[Obsolete ("Use CreateFrom (SKProduct) instead.")]
		public static SKPayment PaymentWithProduct (SKProduct product)
		{
			return CreateFrom (product);
		}

#if !MONOMAC

		[Obsolete ("Use CreateFrom (string) instead.")]
		public static SKPayment PaymentWithProduct (string identifier)
		{
			return CreateFrom (identifier);
		}
#endif

#endif
	}
}
