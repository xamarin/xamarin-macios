// Copyright 2015-2016 Xamarin Inc.

#if !XAMCORE_3_0

using System;

using StoreKit;

namespace StoreKit {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class SKPaymentTransactionObserver {

		[Obsolete ("Use RestoreCompletedTransactionsFinished (SKPaymentQueue) instead.")]
		public virtual void PaymentQueueRestoreCompletedTransactionsFinished (SKPaymentQueue queue)
		{
			RestoreCompletedTransactionsFinished (queue);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static partial class SKPaymentTransactionObserver_Extensions {
		
		[Obsolete ("Use RestoreCompletedTransactionsFinished (SKPaymentQueue) instead.")]
		public static void PaymentQueueRestoreCompletedTransactionsFinished (ISKPaymentTransactionObserver This, SKPaymentQueue queue)
		{
			RestoreCompletedTransactionsFinished (This, queue);
		}
	}
}

#endif
