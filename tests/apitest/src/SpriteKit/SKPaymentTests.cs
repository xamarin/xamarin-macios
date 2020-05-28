using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;
using StoreKit;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class SKPaymentTests
	{
		[Test]
		public void SKPayment_PaymentWithProduct ()
		{
			SKProduct product = new SKProduct();
			SKPayment payment = SKPayment.PaymentWithProduct (product);
			Assert.IsNotNull (payment);
		}
	}
}

