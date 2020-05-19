//
// Unit tests for HKCdaDocumentSample
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//
#if __IOS__

using System;

using Foundation;
using HealthKit;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CdaDocumentSampleTest {

		[Test]
		public void Error ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			NSError error;
			using (var d = new NSData ()) {
				TestDelegate action = () => {
					using (var s = HKCdaDocumentSample.Create (d, NSDate.DistantPast, NSDate.DistantFuture, (NSDictionary)null, out error)) {
						Assert.NotNull (error, "error");
						var details = new HKDetailedCdaErrors (error.UserInfo);
						Assert.That (details.ValidationError.Length, Is.EqualTo (0), "Length");
					}
				};
				if (TestRuntime.CheckXcodeVersion (11, 0)) {
					var ex = Assert.Throws<MonoTouchException> (action, "Exception");
					Assert.That (ex.Message, Is.StringMatching ("startDate.*and endDate.*exceed the maximum allowed duration for this sample type"), "Exception Message");
				} else {
					action ();
				}
			}
		}
	}
}

#endif // __IOS__
