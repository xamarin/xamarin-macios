// Copyright 2015-2016 Xamarin Inc.

#nullable enable

#if !XAMCORE_3_0

using System;

using StoreKit;

namespace StoreKit {

	public partial class SKPaymentTransactionObserver {

		[Obsolete ("Use RestoreCompletedTransactionsFinished (SKPaymentQueue) instead.")]
		public virtual void PaymentQueueRestoreCompletedTransactionsFinished (SKPaymentQueue queue)
		{
			RestoreCompletedTransactionsFinished (queue);
		}
	}

	public static partial class SKPaymentTransactionObserver_Extensions {

		[Obsolete ("Use RestoreCompletedTransactionsFinished (SKPaymentQueue) instead.")]
		public static void PaymentQueueRestoreCompletedTransactionsFinished (ISKPaymentTransactionObserver This, SKPaymentQueue queue)
		{
			RestoreCompletedTransactionsFinished (This, queue);
		}
	}
}

#endif
