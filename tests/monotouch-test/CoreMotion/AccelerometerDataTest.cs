//
// Unit tests for CMAccelerometerData
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using CoreMotion;
using ObjCRuntime;
#else
using MonoTouch.CoreMotion;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMotion {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AccelerometerDataTest {

#if !XAMCORE_2_0 // the default ctor has been removed.
		[Test]
		public void Default ()
		{
			// this would (and did) crash if we called 'init'
			using (var ad = new CMAccelerometerData ()) {
				Assert.That (ad.Acceleration.X, Is.EqualTo (0.0d), "X");
				Assert.That (ad.Acceleration.Y, Is.EqualTo (0.0d), "Y");
				Assert.That (ad.Acceleration.Z, Is.EqualTo (0.0d), "Z");
				// from CMLogItem
				// NaN on the simulator, 0.0d on devices - but we just want to ensure we do not crash
				Assert.That (ad.Timestamp, Is.Not.EqualTo (1.0d), "Timestamp");
			}
		}
#endif
	}
}

#endif // !__TVOS__
