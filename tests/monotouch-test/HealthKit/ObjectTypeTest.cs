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
