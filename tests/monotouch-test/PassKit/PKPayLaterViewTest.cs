#if __IOS__ && !__MACCATALYST__

using System;
using System.Threading;

using Foundation;
using UIKit;

using PassKit;

using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PKPayLaterViewTest {

		[Test]
		public void ValidateAmountTest_NSDecimal ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var counter = 100;
			var cnt = 0;
			for (int i = 0; i < counter; i++) {
				PKPayLaterView.ValidateAmount (new NSDecimalNumber (i), "USD", (eligible) => {
					Interlocked.Increment (ref cnt);
				});
			}
			// The callback is rarely called, so just assert that we don't get more callbacks than
			// actual validation requests.
			Assert.That (cnt, Is.Not.LessThan (0).And.Not.GreaterThan (counter), $"NSDecimalNumber overload");
		}

		[Test]
		public void ValidateAmountTest_Decimal ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var counter = 100;
			var cnt = 0;
			for (int i = 0; i < counter; i++) {
				PKPayLaterView.ValidateAmount (i, "USD", (eligible) => {
					Interlocked.Increment (ref cnt);
				});
			}
			// The callback is rarely called, so just assert that we don't get more callbacks than
			// actual validation requests.
			Assert.That (cnt, Is.Not.LessThan (0).And.Not.GreaterThan (counter), $"decimal overload");
		}
	}
}

#endif
