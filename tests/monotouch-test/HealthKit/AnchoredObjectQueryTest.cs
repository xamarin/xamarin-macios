//
// Unit tests for HKAnchoredObjectQuery
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if HAS_HEALTHKIT

using System;

using Foundation;
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
	public class AnchoredObjectQueryTest {

		[Test]
		public void NoAnchor ()
		{
#if MONOMAC
			TestRuntime.AssertXcodeVersion (14, 0);
#else
			TestRuntime.AssertXcodeVersion (6, 0);
#endif

			using (var t = HKCategoryType.Create (HKCategoryTypeIdentifier.SleepAnalysis))
#if __WATCHOS__
			using (var aoq = new HKAnchoredObjectQuery (t, null, HKQueryAnchor.Create (HKAnchoredObjectQuery.NoAnchor), 0, delegate (HKAnchoredObjectQuery query, HKSample [] addedObjects, HKDeletedObject [] deletedObjects, HKQueryAnchor newAnchor, NSError error) {
#else
			using (var aoq = new HKAnchoredObjectQuery (t, null, HKAnchoredObjectQuery.NoAnchor, 0, delegate (HKAnchoredObjectQuery query, HKSample [] results, nuint newAnchor, NSError error)
			{
#endif
			})) {
				Assert.That (aoq.Handle, Is.Not.EqualTo (IntPtr.Zero), "handle");
			}
		}
	}
}
#endif // HAS_HEALTHKIT
