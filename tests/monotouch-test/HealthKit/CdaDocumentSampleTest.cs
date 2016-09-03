//
// Unit tests for HKCdaDocumentSample
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//
#if !__TVOS__

using System;

#if XAMCORE_2_0
using Foundation;
using HealthKit;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.HealthKit;
using MonoTouch.UIKit;
#endif
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
			using (var d = new NSData ())
			using (var s = HKCdaDocumentSample.Create (d, NSDate.DistantPast, NSDate.DistantFuture, null, out error)) {
				Assert.NotNull (error, "error");
				var details = new HKDetailedCdaErrors (error.UserInfo);
			}
		}
	}
}

#endif // __TVOS__
