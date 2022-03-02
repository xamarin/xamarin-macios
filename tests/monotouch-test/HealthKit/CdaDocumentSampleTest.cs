//
// Unit tests for HKCdaDocumentSample
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//
#if HAS_HEALTHKIT

using System;

using Foundation;
using ObjCRuntime;

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
					// Objective-C exception thrown.  Name: _HKObjectValidationFailureException Reason: startDate (0001-01-01 00:00:00 +0000) and endDate (4001-01-01 00:00:00 +0000) exceed the maximum allowed duration for this sample type. Maximum duration for type HKDocumentTypeIdentifierCDA is 345600.000000
					var startDate = NSDate.DistantPast;
					var endDate = startDate.AddSeconds (345600);
					using (var s = HKCdaDocumentSample.Create (d, startDate, endDate, (NSDictionary)null, out error)) {
						Assert.NotNull (error, "error");
						var details = new HKDetailedCdaErrors (error.UserInfo);
						Assert.That (details.ValidationError.Length, Is.EqualTo ((nint) 0), "Length");
					}
				};
#if __MACCATALYST__
				var throwsException = false;
#else
				var throwsException = TestRuntime.CheckXcodeVersion (11, 0);
#endif

				if (throwsException) {
#if NET
					var ex = Assert.Throws<ObjCException> (action, "Exception");
#else
					var ex = Assert.Throws<MonoTouchException> (action, "Exception");
#endif
					Assert.That (ex.Message, Does.Match ("startDate.*and endDate.*exceed the maximum allowed duration for this sample type"), "Exception Message");
				} else {
					action ();
				}
			}
		}
	}
}

#endif // HAS_HEALTHKIT
