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

#if !XAMCORE_2_0
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

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
