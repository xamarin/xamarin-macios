#if !__TVOS__ && !MONOMAC

using System;
using Foundation;
using UIKit;
using PassKit;
using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PKPaymentSummaryItemTest {

		[Test]
		public void CheckDefaultNulls ()
		{
			using var ps = new PKPaymentSummaryItem ();
			Assert.IsNull (ps.Amount, "'PKPaymentSummaryItem.Amount' is not returning null by default.");
			Assert.IsNull (ps.Label, "'PKPaymentSummaryItem.Label' is not returning null by default.");

			Assert.DoesNotThrow (delegate { ps.Amount = null; },
				"'PKPaymentSummaryItem.Amount' cannot be set to null.");
			Assert.DoesNotThrow (delegate { ps.Label = null; },
				"'PKPaymentSummaryItem.Label' cannot be set to null.");
		}
	}
}

#endif
