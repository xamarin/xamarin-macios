//
// Unit tests for CMLogItem
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using ObjCRuntime;
using Foundation;
using CoreMotion;
#else
using MonoTouch.CoreMotion;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMotion {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LogItemTest {

#if !XAMCORE_2_0 // the default ctor has been removed.
		[Test]
		public void Default ()
		{
			// this would (and did) crash if we called 'init'
			using (var li = new CMLogItem ()) {
				// NaN on the simulator, 0.0d on devices - but we just want to ensure we do not crash
				Assert.That (li.Timestamp, Is.Not.EqualTo (1.0d), "Timestamp");
			}
		}
#endif
	}
}

#endif // !__TVOS__
