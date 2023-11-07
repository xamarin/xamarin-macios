//
// Unit tests for CAEmitterCell
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
using CoreAnimation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreAnimation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EmitterCellTest {

		[Test]
		public void XEmitterCellTest ()
		{
			using (var ec = new CAEmitterCell ()) {
				// ICAMediaTiming
				Assert.That (ec.BeginTime, Is.EqualTo (0.0d), "BeginTime");
				Assert.True (Double.IsInfinity (ec.Duration), "Duration");
				Assert.That (ec.Speed, Is.EqualTo (1.0f), "Speed");
				Assert.That (ec.TimeOffset, Is.EqualTo (0.0d), "TimeOffset");
				Assert.That (ec.RepeatCount, Is.EqualTo (0.0f), "RepeatCount");
				Assert.That (ec.RepeatDuration, Is.EqualTo (0.0d), "RepeatDuration");
				Assert.False (ec.AutoReverses, "AutoReverses");
				Assert.That (ec.FillMode, Is.EqualTo ("removed"), "FillMode");
			}
		}
	}
}

#endif // !__WATCHOS__
