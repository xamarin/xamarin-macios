// Can be uncommented when this issue is resolved: # https://github.com/xamarin/xamarin-macios/issues/19271

// #if __IOS__ && !__MACCATALYST__

// using System;
// using Foundation;
// using UIKit;
// using PassKit;
// using NUnit.Framework;

// namespace MonoTouchFixtures.PassKit {

// 	[TestFixture]
// 	[Preserve (AllMembers = true)]
// 	public class PKPayLaterViewTest {

// 		[Test]
// 		public void ValidateAmountTest ()
// 		{
// 			TestRuntime.AssertXcodeVersion (15, 0);

// 			for (int i = 0; i < 1000; i++){
// 				PKPayLaterView.ValidateAmount (new NSDecimalNumber (i), "USD", (eligible) => {
// 					Assert.False (eligible);
// 				});
// 			}
// 		}
// 	}
// }

// #endif
