#if !__TVOS__ && !MONOMAC

using System;
using Foundation;
using UIKit;
using PassKit;
using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PKPaymentRequestTest {

		[Test]
		public void RequiredBillingContactFields ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			using (var pr = new PKPaymentRequest ()) {
				Assert.That (pr.RequiredBillingContactFields, Is.EqualTo (PKContactFields.None), "None");

				pr.RequiredBillingContactFields |= PKContactFields.PostalAddress;
				Assert.That (pr.RequiredBillingContactFields, Is.EqualTo (PKContactFields.PostalAddress), "PostalAddress");

				pr.RequiredBillingContactFields |= PKContactFields.EmailAddress;
				pr.RequiredBillingContactFields |= PKContactFields.PhoneNumber;
				pr.RequiredBillingContactFields |= PKContactFields.Name;
				pr.RequiredBillingContactFields |= PKContactFields.PhoneticName;
				Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 5), "Count-5");

				pr.RequiredBillingContactFields = PKContactFields.PhoneticName;
				Assert.That (pr.RequiredBillingContactFields, Is.EqualTo (PKContactFields.PhoneticName), "PhoneticName");
			}
		}

		[Test]
		public void WeakRequiredBillingContactFields ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			using (var pr = new PKPaymentRequest ()) {
				Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 0), "Count");

				using (var set = new NSMutableSet ()) {
					pr.WeakRequiredBillingContactFields = set;
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 0), "Count-0");
					set.Add (PKContactFields.PostalAddress.GetConstant ());
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 1), "Count-1");
					set.Add (PKContactFields.EmailAddress.GetConstant ());
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 2), "Count-2");
					set.Add (PKContactFields.PhoneNumber.GetConstant ());
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 3), "Count-3");
					set.Add (PKContactFields.Name.GetConstant ());
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 4), "Count-5");
					set.Add (PKContactFields.PhoneticName.GetConstant ());
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 5), "Count-5");
					set.Add (PKContactFields.PhoneticName.GetConstant ());
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 5), "Count-5b");
					set.Remove (PKContactFields.PhoneticName.GetConstant ());
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 4), "Count-4b");
					set.RemoveAll ();
					Assert.That (pr.WeakRequiredBillingContactFields.Count, Is.EqualTo ((nuint) 0), "Count-0b");
				}
			}
		}

		[Test]
		public void CheckDefaultNulls ()
		{
			using var pr = new PKPaymentRequest ();
			Assert.IsNull (pr.CountryCode, "'PKPaymentRequest.CountryCode' is not returning null by default.");
			Assert.IsNull (pr.CurrencyCode, "'PKPaymentRequest.CurrencyCode' is not returning null by default.");
			Assert.IsNull (pr.MerchantIdentifier, "'PKPaymentRequest.MerchantIdentifier' is not returning null by default.");
			Assert.IsNull (pr.PaymentSummaryItems, "'PKPaymentRequest.PaymentSummaryItems' is not returning null by default.");
			Assert.IsNull (pr.SupportedNetworks, "'PKPaymentRequest.SupportedNetworks' is not returning null by default.");

			Assert.DoesNotThrow (delegate { pr.CountryCode = null; },
				"'PKPaymentRequest.CountryCode' cannot be set to null.");
			Assert.DoesNotThrow (delegate { pr.CurrencyCode = null; },
				"'PKPaymentRequest.CurrencyCode' cannot be set to null.");
			Assert.DoesNotThrow (delegate { pr.MerchantIdentifier = null; },
				"'PKPaymentRequest.MerchantIdentifier' cannot be set to null.");
			Assert.DoesNotThrow (delegate { pr.PaymentSummaryItems = null; },
				"'PKPaymentRequest.PaymentSummaryItems' cannot be set to null.");
			Assert.DoesNotThrow (delegate { pr.SupportedNetworks = null; },
				"'PKPaymentRequest.SupportedNetworks' cannot be set to null.");
		}
	}
}

#endif
