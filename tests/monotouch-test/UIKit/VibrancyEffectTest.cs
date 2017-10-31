//
// Unit tests for UIVibrancyEffect
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#if !__TVOS__
using NotificationCenter;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.NotificationCenter;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VibrancyEffectTest {

#if !XAMCORE_2_0
		[Test]
		public void NotificationCenterVibrancyEffect_Old ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (8,0))
				Assert.Inconclusive ("Requires 8.0+");

			(null as UIVibrancyEffect).NotificationCenterVibrancyEffect ();
		}
#endif
		[Test]
		public void NotificationCenterVibrancyEffect_New ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (8,0))
				Assert.Inconclusive ("Requires 8.0+");

			UIVibrancyEffect.CreateForNotificationCenter ();
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__