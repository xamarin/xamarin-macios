// Copyright 2015 Xamarin Inc.

#nullable enable

using System;

using StoreKit;

namespace StoreKit {

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
