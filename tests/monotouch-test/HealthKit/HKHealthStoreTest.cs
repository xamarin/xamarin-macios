//
// Unit tests for HKHealthStore
//
// Authors:
//	TJ Lambert  <TJ.Lambert@microsoft.com>
//
// Copyright 2022 Xamarin Inc. All rights reserved.
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
	public class HKHealthStoreTest {

		[SetUp]
		public void SetUp ()
		{
#if MONOMAC
			TestRuntime.AssertXcodeVersion (14, 0);
#endif
		}

		[Test]
		public void GetBiologicalSexNullReturnTest ()
		{
			var store = new HKHealthStore ();
			var ret = store.GetBiologicalSex (out _);
			Assert.IsNull (ret, "GetBiologicalSex should return a null value if biological sex is not set.");
		}

		[Test]
		public void GetBloodTypeNullReturnTest ()
		{
			var store = new HKHealthStore ();
			var ret = store.GetBloodType (out _);
			Assert.IsNull (ret, "GetBloodType should return a null value if blood type is not set.");
		}
	}
}
#endif // HAS_HEALTHKIT
