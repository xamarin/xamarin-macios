using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.StoreKit;
using nuint = System.UInt32;
#else
using AppKit;
using Foundation;
using StoreKit;
#endif

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

