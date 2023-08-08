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
using NUnit.Framework;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CdaDocumentSampleTest {

		[Test]
		public void Error ()
		{
#if MONOMAC
			TestRuntime.AssertXcodeVersion (14, 0);
#else
			TestRuntime.AssertXcodeVersion (8, 0);
#endif

			NSError error;
			using (var d = new NSData ()) {
				TestDelegate action = () => {
					using (var s = HKCdaDocumentSample.Create (d, NSDate.DistantPast, NSDate.DistantFuture, (NSDictionary) null, out error)) {
						Assert.NotNull (error, "error");
						var details = new HKDetailedCdaErrors (error.UserInfo);
						Assert.That (details.ValidationError.Length, Is.EqualTo ((nint) 0), "Length");
					}
				};
#if __MACCATALYST__
				var throwsException = TestRuntime.CheckXcodeVersion (12, 0);
#else
				var throwsException = TestRuntime.CheckXcodeVersion (11, 0);
#endif

				if (throwsException) {
#if NET || MONOMAC
					var ex = Assert.Throws<ObjCException> (action, "Exception");
#else
					var ex = Assert.Throws<MonoTouchException> (action, "Exception");
#endif
					if (TestRuntime.CheckXcodeVersion (15, 0)) {
						Assert.That (ex.Message, Does.Match ("Objective-C exception thrown.  Name: _HKObjectValidationFailureException Reason: Type HKSample can not have endDate of NSDate.distantFuture"), "Exception Message");
					} else {
						Assert.That (ex.Message, Does.Match ("startDate.*and endDate.*exceed the maximum allowed duration for this sample type"), "Exception Message");
					}
				} else {
					action ();
				}
			}
		}
	}
}

#endif // HAS_HEALTHKIT
