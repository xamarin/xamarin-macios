#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;
using StoreKit;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SKPaymentTests {
		[Test]
		public void SKPayment_PaymentWithProduct ()
		{
			SKProduct product = new SKProduct ();
			SKPayment payment = SKPayment.CreateFrom (product);
			Assert.IsNotNull (payment);
		}
	}
}

#endif // __MACOS__
