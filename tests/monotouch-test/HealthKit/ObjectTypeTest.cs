//
// Unit tests for HKObjectType
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
	public class ObjectTypeTest {

		[Test]
		public void Workout ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);

			using (var t = HKObjectType.GetWorkoutType ()) {
				Assert.That (t.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}
	}
}
#endif // !__TVOS__
