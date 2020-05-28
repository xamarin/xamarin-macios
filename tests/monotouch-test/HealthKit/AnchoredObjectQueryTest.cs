//
// Unit tests for HKAnchoredObjectQuery
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;

using Foundation;
using HealthKit;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AnchoredObjectQueryTest {

		[Test]
		public void NoAnchor ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);

			using (var t = HKCategoryType.Create (HKCategoryTypeIdentifier.SleepAnalysis))
#if __WATCHOS__
			using (var aoq = new HKAnchoredObjectQuery (t, null, HKQueryAnchor.Create (HKAnchoredObjectQuery.NoAnchor), 0, delegate (HKAnchoredObjectQuery query, HKSample [] addedObjects, HKDeletedObject [] deletedObjects, HKQueryAnchor newAnchor, NSError error) {
#else
			using (var aoq = new HKAnchoredObjectQuery (t, null, HKAnchoredObjectQuery.NoAnchor, 0, delegate (HKAnchoredObjectQuery query, HKSample[] results, nuint newAnchor, NSError error) {
#endif
			})) {
				Assert.That (aoq.Handle, Is.Not.EqualTo (IntPtr.Zero), "handle");
			}
		}
	}
}
#endif // !__TVOS__
