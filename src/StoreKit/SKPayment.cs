// Copyright 2015 Xamarin Inc.

using System;

using StoreKit;

namespace StoreKit {

	public partial class SKPayment {

#if !XAMCORE_3_0

		[Obsolete ("Use FromProduct (SKProduct) instead.")]
		public static SKPayment PaymentWithProduct (SKProduct product)
		{
			return CreateFrom (product);
		}

#if !MONOMAC

		[Obsolete ("Use FromProductIdentifier (string) instead.")]
		public static SKPayment PaymentWithProduct (string identifier)
		{
			return CreateFrom (identifier);
		}
#endif

#endif
	}
}
