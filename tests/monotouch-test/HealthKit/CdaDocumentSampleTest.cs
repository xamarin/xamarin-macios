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
using System.Linq;
using System.Text.RegularExpressions;

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

					var possibleMessages = new string [] {
						"startDate.*and endDate.*exceed the maximum allowed duration for this sample type",
						"Objective-C exception thrown.  Name: _HKObjectValidationFailureException Reason: Type HKSample can not have endDate of NSDate.distantFuture",
					};
					var success = possibleMessages.Any (v => Regex.IsMatch (ex.Message, v, RegexOptions.IgnoreCase));
					Assert.IsTrue (success, $"The exception message:\n{ex.Message}\nDoes not match any of the expected messages:\n\t{string.Join ("\n\t", possibleMessages)}");
				} else {
					action ();
				}
			}
		}
	}
}

#endif // HAS_HEALTHKIT
